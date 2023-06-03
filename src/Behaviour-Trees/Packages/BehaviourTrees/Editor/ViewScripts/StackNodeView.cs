using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Derrixx.BehaviourTrees.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Node = Derrixx.BehaviourTrees.Nodes.Node;

namespace Derrixx.BehaviourTrees.Editor.ViewScripts
{
	public class StackNodeView : StackNode
	{
		private readonly BehaviourTreeView _behaviourTreeView;

		public StackNodeView(IReadOnlyList<NodeView> nodeViews, BehaviourTreeView behaviourTreeView)
		{
			_behaviourTreeView = behaviourTreeView;

			style.paddingLeft = style.paddingRight = 7;

			for (int i = nodeViews.Count - 1; i >= 0; i--)
			{
				NodeView nodeView = nodeViews[i];
				InsertNodeView(nodeView, nodeViews.Count - 1 - i);
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

		public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
		{
			base.BuildContextualMenu(evt);
			evt.menu.AppendAction("Add Decorator", action => _behaviourTreeView.nodeCreationRequest(new NodeCreationContext
			{
				target = this,
				index = 0,
				screenMousePosition = action.eventInfo.mousePosition
			}));
		}

		public override bool DragPerform(DragPerformEvent evt, IEnumerable<ISelectable> selection, IDropTarget dropTarget, ISelection dragSource)
		{
			Node FindParentNode(int nodeIndex, out StackNodeView parentStack)
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

			var selectables = selection.ToList();

			var first = (NodeView)selectables[0];
			var draggedDecorator = (DecoratorNode)first.Node;

			base.DragPerform(evt, selectables, dropTarget, dragSource);

			int newIndex = IndexOf(first);
			first.CachedIndex = newIndex;

			Node parentNode = FindParentNode(newIndex, out StackNodeView parentStack);
			parentNode.InsertNodeBeforeChild(NodeViews[newIndex + 1].Node, draggedDecorator);

			parentStack.UpdateNodeViews();
			UpdateNodeViews();

			_behaviourTreeView.UpdateNodesActiveState();
			
			Debug.Log("Drag performed");

			return true;
		}
		
		public override void OnStartDragging(GraphElement ge)
		{
			Node FindParentNode(int i)
			{
				if (i != 0)
				{
					return NodeViews[i - 1].Node;
				}

				var stackNodeViews = _behaviourTreeView.nodes.OfType<StackNodeView>().ToList();
				var parentStack = stackNodeViews.First(x => x.LastNode.GetChildren().Contains(NodeViews[i].Node));

				return parentStack.LastNode;
			}

			var nodeView = (NodeView)ge;
			
			int index = IndexOf(nodeView);

			Node parentNode = FindParentNode(index);

			parentNode.RemoveChild(nodeView.Node);
			parentNode.AddChild(NodeViews[index + 1].Node);
			
			base.OnStartDragging(nodeView);
		}

		public void InsertNodeView(NodeView nodeView, int index, bool updateHierarchy = false)
		{
			nodeView.CachedIndex = index;
			nodeView.Stack = this;
			InsertElement(index, nodeView);
			
			UpdateNodeViews();
		}

		public void AddNodeView(NodeView nodeView, bool updateHierarchy = false)
		{
			InsertNodeView(nodeView, index: 0, updateHierarchy);
		}

		private void UpdateNodeViews()
		{
			foreach (NodeView nodeView in NodeViews)
			{
				nodeView.Update();
				nodeView.Stack = this;
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

			Input = this.InstantiatePort(Direction.Input, Port.Capacity.Single, FlexDirection.ColumnReverse);
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

			Output = this.InstantiatePort(Direction.Output, capacity, FlexDirection.Column);
			outputContainer.Add(Output);
		}

		private void SetupCapabilities(Node node)
		{
			if (node is RootNode)
				capabilities &= ~(Capabilities.Deletable | Capabilities.Copiable);
		}
	}
}