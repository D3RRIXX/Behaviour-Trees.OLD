using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
	/// <summary>
	/// Node type that operates on a single child. Inherit from this class to create your own decorator nodes.
	/// <seealso cref="ConditionalNode"/>
	/// </summary>
	public abstract class DecoratorNode : Node
	{
		[SerializeField, HideInInspector] private Node _child;
		
		public Node Child
		{
			get => _child;
			set => _child = value;
		}

		public sealed override Node Clone(BehaviourTreeRunner runner)
		{
			DecoratorNode clone = (DecoratorNode)base.Clone(runner);
			clone.Child = Child.Clone(runner);
			
			return clone;
		}

		protected internal sealed override void ResetState()
		{
			base.ResetState();
			
			if (Child != null)
				Child.ResetState();
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