namespace Derrixx.BehaviourTrees.Nodes
{
	public class UntilFailureNode : DecoratorNode
	{
		protected override State OnUpdate()
		{
			return Child.Update() == State.Failure ? State.Success : State.Running;
		}
	}
}
