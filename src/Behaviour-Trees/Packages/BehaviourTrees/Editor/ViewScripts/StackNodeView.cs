using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Derrixx.BehaviourTrees.Editor.Extensions;
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
		private readonly List<NodeView> _nodeViews;

		public event Action<NodeView> StartedDecoratorDrag; 

		public StackNodeView(IReadOnlyList<NodeView> nodeViews, BehaviourTreeView behaviourTreeView)
		{
			_behaviourTreeView = behaviourTreeView;
			_nodeViews = new List<NodeView>(nodeViews);
			
			// Temp fix? Reverse column view might fix something
			this.Q("stackNodeContentContainer").style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.ColumnReverse);

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

		public IReadOnlyList<NodeView> NodeViews => _nodeViews;

		public Node FirstNode => NodeViews[0].Node;
		public Node LastNode => NodeViews[^1].Node;
		
		public int Count { get; private set; }

		public sealed override string title
		{
			get => base.title;
			set => base.title = value;
		}

		public Port Input { get; private set; }
		public Port Output { get; private set; }

		public override string ToString()
		{
			var builder = new StringBuilder("StackNodeView (");
			builder.AppendJoin(", ", NodeViews.Select(x => x.Node.name));
			builder.Append(")");

			return builder.ToString();
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

		public void InsertNodeView(NodeView nodeView, int index)
		{
			InsertElement(index, nodeView);

			OnAddedNode(nodeView);
			UpdateNodeViews();
		}

		public override void OnStartDragging(GraphElement ge)
		{
			int index = IndexOf(ge);
			// Invoke event that we started dragging out NodeView at specific index
			StartedDecoratorDrag?.Invoke(NodeViews[index]);
			
			base.OnStartDragging(ge);
			
			// Decrease Node count when starting to drag child nodes
			Count--;
		}

		protected override bool AcceptsElement(GraphElement element, ref int proposedIndex, int maxIndex)
		{
			var nodeView = (NodeView)element;
			
			// There can be only one non-decorator node in the stack
			if (nodeView.Node is not DecoratorNode or RootNode)
				return childCount == 0;

			// Decorator nodes can't be first in the stack
			// proposedIndex = Mathf.Clamp(proposedIndex, 0, maxIndex);
			// proposedIndex = Count - proposedIndex;
			
			return base.AcceptsElement(element, ref proposedIndex, maxIndex);
		}

		public override bool DragPerform(DragPerformEvent evt, IEnumerable<ISelectable> selection, IDropTarget dropTarget, ISelection dragSource)
		{
			List<ISelectable> list = selection.ToList();
			
			foreach (NodeView nodeView in list.Cast<NodeView>())
			{
				OnAddedNode(nodeView);
			}

			UpdateNodeViews();
			
			return base.DragPerform(evt, list, dropTarget, dragSource);
		}

		private void OnAddedNode(NodeView nodeView)
		{
			nodeView.Stack = this;
			Count++;
		}

		private void UpdateNodeViews()
		{
			foreach (NodeView nodeView in NodeViews)
			{
				nodeView.Update();
				nodeView.Stack = this;
			}
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