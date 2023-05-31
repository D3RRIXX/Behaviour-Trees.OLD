namespace Derrixx.BehaviourTrees.Nodes.Decorators
{
	/// <summary>
	/// A decorator node that returns either <see cref="Node.State.Success"/> or <see cref="Node.State.Failure"/> based on a condition.
	/// Inherit from this class to create your own conditional nodes.
	/// <seealso cref="IsTrueNode"/><seealso cref="AreEqualNode"/>
	/// </summary>
	public abstract class ConditionalNode : DecoratorNode
	{
		protected sealed override State OnUpdate()
		{
			if (ConditionValue() == false)
				return State.Failure;

			return Child == null ? State.Success : Child.Update();
		}

		protected abstract bool ConditionValue();
	}
}
