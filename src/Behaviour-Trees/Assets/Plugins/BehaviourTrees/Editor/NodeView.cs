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

			style.left = node.Position.x;
			style.top = node.Position.x;

			CreateInputPorts();
			CreateOutputPorts();
		}

		public Node Node { get; }
		public Port Input { get; private set; }
		public Port Output { get; set; }

		public override void SetPosition(Rect newPos)
		{
			base.SetPosition(newPos);
			
			Vector2 nodePosition = Node.Position;
			nodePosition.x = newPos.xMin;
			nodePosition.y = newPos.yMin;

			Node.Position = nodePosition;
		}

		private void CreateInputPorts()
		{
			Input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));

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
				CompositeNode compositeNode => Port.Capacity.Multi,
				DecoratorNode decoratorNode => Port.Capacity.Single,
				_ => throw new ArgumentOutOfRangeException(nameof(Node))
			};

			Output = InstantiatePort(Orientation.Horizontal, Direction.Output, capacity, typeof(bool));
			
			if (Output == null)
				return;

			Output.portName = string.Empty;
			outputContainer.Add(Output);
		}
	}
}