namespace Derrixx.BehaviourTrees
{
	public class UntilFailureNode : DecoratorNode
	{
		protected override State OnUpdate()
		{
			return Child.Update() == State.Failure ? State.Success : State.Running;
		}
	}
}
