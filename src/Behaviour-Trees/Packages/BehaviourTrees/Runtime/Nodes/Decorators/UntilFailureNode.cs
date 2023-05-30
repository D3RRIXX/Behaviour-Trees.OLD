namespace Derrixx.BehaviourTrees.Nodes.Decorators
{
	public class UntilFailureNode : DecoratorNode
	{
		protected override State OnUpdate()
		{
			return Child.Update() == State.Failure ? State.Success : State.Running;
		}
	}
}
