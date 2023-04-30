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
			var clone = (DecoratorNode)base.Clone(runner);

			if (Child != null)
				clone.Child = Child.Clone(runner);
			else
				Debug.LogWarning($"Decorator '{name}' doesn't have a child assigned! Are you sure you're using it correctly?", runner);
			
			return clone;
		}

		internal sealed override void SetExecutionOrder(ref int order)
		{
			base.SetExecutionOrder(ref order);

			if (_child)
				_child.SetExecutionOrder(ref order);
		}

		internal sealed override void CallOnCreate()
		{
			base.CallOnCreate();
			_child.CallOnCreate();
		}

		protected internal sealed override void ResetState()
		{
			base.ResetState();
			
			if (Child != null)
				Child.ResetState();
		}

		public sealed override bool IsConnectedWith(Node other)
		{
			if (_child != null && _child.IsConnectedWith(other))
				return true;
			
			return base.IsConnectedWith(other);
		}
	}
}