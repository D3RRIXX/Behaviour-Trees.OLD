namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
	public sealed class RootNode : DecoratorNode
	{
		protected override State OnEvaluate(BehaviourTreeRunner runner) => Child.Evaluate(runner);
	}
}