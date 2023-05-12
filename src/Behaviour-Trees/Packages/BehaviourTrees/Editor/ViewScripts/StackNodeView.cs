using System;
using System.Collections.Generic;
using System.Linq;
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

		public StackNodeView(List<NodeView> nodeViews)
		{
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

		protected override bool AcceptsElement(GraphElement element, ref int proposedIndex, int maxIndex)
		{
			var nodeView = (NodeView)element;
			if (nodeView.Node is ActionNode)
				return childCount == 0;
			
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