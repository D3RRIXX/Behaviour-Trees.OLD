namespace Derrixx.BehaviourTrees.Runtime.Nodes.Decorators
{
	public sealed class RootNode : DecoratorNode
	{
		protected override State OnUpdate() => Child.Update();
	}
}