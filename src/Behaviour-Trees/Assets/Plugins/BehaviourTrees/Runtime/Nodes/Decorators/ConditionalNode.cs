namespace Derrixx.BehaviourTrees.Runtime.Nodes.Decorators
{
	public abstract class ConditionalNode : DecoratorNode
	{
		protected sealed override State OnUpdate() => ConditionValue() ? Child.Update() : State.Failure;
		protected abstract bool ConditionValue();
	}
}