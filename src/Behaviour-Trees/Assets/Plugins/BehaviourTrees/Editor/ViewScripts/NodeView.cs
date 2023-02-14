using System;
using Derrixx.BehaviourTrees.Runtime.Nodes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Node = Derrixx.BehaviourTrees.Runtime.Nodes.Node;

namespace Derrixx.BehaviourTrees.Editor
{
	public class NodeView : UnityEditor.Experimental.GraphView.Node
	{
		public NodeView(Node node)
		{
			Node = node;
			title = node.name;
			viewDataKey = node.Guid;

			style.left = node.Position.x;
			style.top = node.Position.x;

			CreateInputPorts();
			CreateOutputPorts();
		}

		public Node Node { get; }
		public Port Input { get; private set; }
		public Port Output { get; private set; }

		public override void SetPosition(Rect newPos)
		{
			base.SetPosition(newPos);

			Node.Position.x = newPos.xMin;
			Node.Position.y = newPos.yMin;
		}

		private void CreateInputPorts()
		{
			Input = InstantiatePort(Direction.Input, Port.Capacity.Single);

			if (Input == null)
				return;
			
			Input.portName = string.Empty;
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

			Output = InstantiatePort(Direction.Output, capacity);
			
			if (Output == null)
				return;

			Output.portName = string.Empty;
			outputContainer.Add(Output);
		}

		private Port InstantiatePort(Direction direction, Port.Capacity capacity)
			=> InstantiatePort(Orientation.Horizontal, direction, capacity, typeof(Node));
	}
}