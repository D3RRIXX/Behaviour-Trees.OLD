using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
	public abstract class DecoratorNode : Node
	{
		[SerializeField, HideInInspector] private Node _child;
		
		public Node Child
		{
			get => _child;
			set => _child = value;
		}

		public sealed override Node Clone()
		{
			DecoratorNode clone = (DecoratorNode)base.Clone();
			clone.Child = Child.Clone();
			
			return clone;
		}

		internal sealed override void SetExecutionOrder(ref int order)
		{
			base.SetExecutionOrder(ref order);

			if (_child)
				_child.SetExecutionOrder(ref order);
		}

		public sealed override bool IsConnectedWith(Node other)
		{
			if (_child != null && _child.IsConnectedWith(other))
				return true;
			
			return base.IsConnectedWith(other);
		}
	}
}