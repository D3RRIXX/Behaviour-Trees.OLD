using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Derrixx.BehaviourTrees.Runtime.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Node = Derrixx.BehaviourTrees.Runtime.Nodes.Node;

namespace Derrixx.BehaviourTrees.Editor.ViewScripts
{
	public class StackNodeView : StackNode
	{
		private readonly List<Node> _nodes;
		private readonly List<NodeView> _nodeViews;
		private readonly BehaviourTreeView _behaviourTreeView;

		public StackNodeView(List<NodeView> nodeViews, BehaviourTreeView behaviourTreeView)
		{
			_behaviourTreeView = behaviourTreeView;
			_nodeViews = nodeViews;
			_nodes = nodeViews.Select(x => x.Node).ToList();

			style.paddingLeft = style.paddingRight = 7;

			Node lastNode = LastNode;
			style.left = lastNode.Position.x;
			style.top = lastNode.Position.y;

			CreateInputPorts(FirstNode);
			CreateOutputPorts(LastNode);
			SetupCapabilities(LastNode);

			for (int index = nodeViews.Count - 1; index >= 0; index--)
			{
				NodeView nodeView = nodeViews[index];
				nodeView.Stack = this;
				InsertElement(0, nodeView);
			}

			title = lastNode.name;
		}

		public Node FirstNode => _nodes[0];
		public Node LastNode => _nodes[^1];
		
		public sealed override string title
		{
			get => base.title;
			set => base.title = value;
		}

		public Port Input { get; private set; }
		public Port Output { get; private set; }

		public override string ToString()
		{
			var stringBuilder = new StringBuilder("StackNodeView (");
			stringBuilder.AppendJoin(", ", _nodes.Select(x => x.name));
			stringBuilder.Append(")");

			return stringBuilder.ToString();
		}

		public override void SetPosition(Rect newPos)
		{
			base.SetPosition(newPos);

			Undo.RecordObject(LastNode, "Behaviour Tree (Set Position)");

			LastNode.Position.x = newPos.xMin;
			LastNode.Position.y = newPos.yMin;
			
			EditorUtility.SetDirty(LastNode);
		}

		public void SetIsConnectedToRoot(bool value)
		{
			const string className = StyleClassNames.INACTIVE_NODE;

			void SetNodeInactive(VisualElement nodeView)
			{
				if (!nodeView.ClassListContains(className))
					nodeView.AddToClassList(className);
			}

			void SetNodeActive(VisualElement nodeView)
			{
				if (nodeView.ClassListContains(className))
					nodeView.RemoveFromClassList(className);
			}
			
			foreach (NodeView nodeView in _nodeViews)
			{
				if (value)
					SetNodeActive(nodeView);
				else
					SetNodeInactive(nodeView);
			}
		}

		public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
		{
			base.BuildContextualMenu(evt);
			evt.menu.AppendAction("Add Decorator", action => {});
		}

		public override bool DragPerform(DragPerformEvent evt, IEnumerable<ISelectable> selection, IDropTarget dropTarget, ISelection dragSource)
		{
			var selectables = selection.ToList();
			
			var first = (NodeView)selectables[0];
			var draggedDecorator = (DecoratorNode)first.Node;

			var targetStack = (StackNodeView)dropTarget;
			
			base.DragPerform(evt, selectables, targetStack, dragSource);

			int newIndex = targetStack.IndexOf(first);
			Node parentNode = FindParentNode(newIndex);
			
			Debug.Log($"Decorator {draggedDecorator.name} now is at {newIndex}");
			parentNode.InsertNodeBeforeChild(_nodes[newIndex], draggedDecorator);
			
			_behaviourTreeView.UpdateNodesActiveState();

			return true;
		}

		public override void OnStartDragging(GraphElement ge)
		{
			var nodeView = (NodeView)ge;
			
			int index = IndexOf(nodeView);
			Node parentNode = FindParentNode(index);
			
			parentNode.RemoveChild(nodeView.Node);
			parentNode.AddChild(_nodes[index + 1]);
			
			base.OnStartDragging(nodeView);
		}

		private Node FindParentNode(int nodeIndex)
		{
			if (nodeIndex == 0)
			{
				IEnumerable<StackNodeView> stackNodeViews = _behaviourTreeView.nodes.OfType<StackNodeView>();
				
				return stackNodeViews
					.First(x => x.LastNode.GetChildren().Contains(_nodes[nodeIndex])).LastNode;
			}

			return _nodes[nodeIndex - 1];
		}

		protected override bool AcceptsElement(GraphElement element, ref int proposedIndex, int maxIndex)
		{
			var nodeView = (NodeView)element;
			if (nodeView.Node is not DecoratorNode or RootNode)
				return childCount == 0;

			proposedIndex = Mathf.Clamp(proposedIndex, 0, maxIndex - 1);
			return base.AcceptsElement(element, ref proposedIndex, maxIndex);
		}

		private void CreateInputPorts(Node firstNode)
		{
			if (firstNode is RootNode)
				return;

			if (!TryInstantiatePort(Direction.Input, Port.Capacity.Single, FlexDirection.ColumnReverse, out Port input))
				return;

			Input = input;
			inputContainer.Add(Input);
		}

		private void CreateOutputPorts(Node lastNode)
		{
			if (lastNode is ActionNode)
				return;

			Port.Capacity capacity = lastNode switch
			{
				CompositeNode => Port.Capacity.Multi,
				DecoratorNode => Port.Capacity.Single,
				_ => throw new ArgumentOutOfRangeException(nameof(Node))
			};

			if (!TryInstantiatePort(Direction.Output, capacity, FlexDirection.Column, out Port output))
				return;

			Output = output;
			outputContainer.Add(Output);
		}
		
		private bool TryInstantiatePort(Direction direction, Port.Capacity capacity, FlexDirection flexDirection, out Port port)
		{
			port = InstantiatePort(Orientation.Vertical, direction, capacity, typeof(Node));
			if (port == null)
				return false;

			port.style.flexDirection = flexDirection;
			port.portName = string.Empty;

			return true;
		}

		private void SetupCapabilities(Node node)
		{
			if (node is RootNode)
				capabilities &= ~(Capabilities.Deletable | Capabilities.Copiable);
		}
	}
}