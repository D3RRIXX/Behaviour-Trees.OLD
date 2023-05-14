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
		private readonly BehaviourTreeView _behaviourTreeView;

		public StackNodeView(IReadOnlyList<NodeView> nodeViews, BehaviourTreeView behaviourTreeView)
		{
			_behaviourTreeView = behaviourTreeView;

			style.paddingLeft = style.paddingRight = 7;

			for (int index = nodeViews.Count - 1; index >= 0; index--)
			{
				NodeView nodeView = nodeViews[index];
				nodeView.Stack = this;
				InsertElement(0, nodeView);
			}
			
			Node lastNode = LastNode;
			style.left = lastNode.Position.x;
			style.top = lastNode.Position.y;

			CreateInputPorts(FirstNode);
			CreateOutputPorts(LastNode);
			SetupCapabilities(LastNode);

			title = lastNode.name;
		}

		public IReadOnlyList<NodeView> NodeViews => Children().OfType<NodeView>().ToList();

		public Node FirstNode => NodeViews[0].Node;
		public Node LastNode => NodeViews[^1].Node;

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
			stringBuilder.AppendJoin(", ", NodeViews.Select(x => x.Node.name));
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
			
			foreach (NodeView nodeView in NodeViews)
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

			base.DragPerform(evt, selectables, dropTarget, dragSource);

			int newIndex = IndexOf(first);
			
			Node parentNode = FindParentNode(newIndex, out StackNodeView parentStack);
			parentNode.InsertNodeBeforeChild(NodeViews[newIndex + 1].Node, draggedDecorator);

			UpdateNodeViews(parentStack);
			UpdateNodeViews(this);
			
			_behaviourTreeView.UpdateNodesActiveState();

			return true;
		}

		public override void OnStartDragging(GraphElement ge)
		{
			var nodeView = (NodeView)ge;
			
			int index = IndexOf(nodeView);
			Node parentNode = FindParentNode(index, out _);
			
			parentNode.RemoveChild(nodeView.Node);
			parentNode.AddChild(NodeViews[index + 1].Node);
			
			base.OnStartDragging(nodeView);
		}

		private Node FindParentNode(int nodeIndex, out StackNodeView parentStack)
		{
			if (nodeIndex != 0)
			{
				parentStack = this;
				return NodeViews[nodeIndex - 1].Node;
			}

			var stackNodeViews = _behaviourTreeView.nodes.OfType<StackNodeView>().ToList();
			parentStack = stackNodeViews.First(x => x.LastNode.GetChildren().Contains(NodeViews[nodeIndex + 1].Node));
			
			return parentStack.LastNode;
		}

		private static void UpdateNodeViews(StackNodeView stackNodeView)
		{
			foreach (NodeView nodeView in stackNodeView.NodeViews)
			{
				nodeView.Update();
			}
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