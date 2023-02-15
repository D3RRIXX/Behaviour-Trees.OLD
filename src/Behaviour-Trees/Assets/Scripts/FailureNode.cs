namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
	public class FailureNode : ActionNode
	{
		protected override State OnUpdate() => State.Failure;
	}
}