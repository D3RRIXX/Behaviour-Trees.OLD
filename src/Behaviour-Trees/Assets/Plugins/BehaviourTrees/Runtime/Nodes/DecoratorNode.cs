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

		public sealed override bool IsConnectedWith(Node other) => base.IsConnectedWith(other) || _child.IsConnectedWith(other);
	}
}