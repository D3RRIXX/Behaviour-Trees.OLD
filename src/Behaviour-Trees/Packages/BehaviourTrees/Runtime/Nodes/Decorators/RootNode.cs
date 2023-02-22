namespace Derrixx.BehaviourTrees.Runtime.Nodes.Decorators
{
	public sealed class RootNode : DecoratorNode
	{
		protected override State OnEvaluate(BehaviourTreeRunner runner) => Child.Evaluate(runner);
	}
}