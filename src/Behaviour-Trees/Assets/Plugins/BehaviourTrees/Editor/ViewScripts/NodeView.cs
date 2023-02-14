using System;
using Derrixx.BehaviourTrees.Runtime.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Node = Derrixx.BehaviourTrees.Runtime.Nodes.Node;

namespace Derrixx.BehaviourTrees.Editor.ViewScripts
{
	public class NodeView : UnityEditor.Experimental.GraphView.Node
	{
		private readonly Label _executionOrderLabel;
		
		public Action<NodeView> OnNodeSelected;

		public NodeView(Node node) : base("Assets/Plugins/BehaviourTrees/Editor/UIBuilder/NodeView.uxml")
		{
			Node = node;
			title = node.name;
			viewDataKey = node.Guid;

			_executionOrderLabel = this.Q<Label>("execution-order");
			
			style.left = node.Position.x;
			style.top = node.Position.y;

			SetupClass();
			CreateInputPorts();
			CreateOutputPorts();
		}

		public Node Node { get; }
		public Port Input { get; private set; }
		public Port Output { get; private set; }

		public override void SetPosition(Rect newPos)
		{
			base.SetPosition(newPos);
			
			Undo.RecordObject(Node, "Behaviour Tree (Set Position)");
			
			Node.Position.x = newPos.xMin;
			Node.Position.y = newPos.yMin;
			
			EditorUtility.SetDirty(Node);
		}

		public override void OnSelected()
		{
			base.OnSelected();
			OnNodeSelected?.Invoke(this);
		}

		public void UpdateExecutionOrderLabel(int order)
		{
			_executionOrderLabel.text = order.ToString();
		}

		private void SetupClass()
		{
			string className = Node switch
			{
				ActionNode _ => "action",
				CompositeNode _ => "composite",
				RootNode _ => "root",
				DecoratorNode _ => "decorator",
				_ => null
			};
			
			AddToClassList(className);
		}

		private void CreateInputPorts()
		{
			if (Node is RootNode)
				return;
			
			if (!TryInstantiatePort(Direction.Input, Port.Capacity.Single, FlexDirection.Column, out Port input))
				return;

			Input = input;
			inputContainer.Add(Input);
		}

		private void CreateOutputPorts()
		{
			if (Node is ActionNode)
				return;

			Port.Capacity capacity = Node switch
			{
				CompositeNode _ => Port.Capacity.Multi,
				DecoratorNode _ => Port.Capacity.Single,
				_ => throw new ArgumentOutOfRangeException(nameof(Node))
			};

			if (!TryInstantiatePort(Direction.Output, capacity, FlexDirection.ColumnReverse, out Port output))
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
	}
}