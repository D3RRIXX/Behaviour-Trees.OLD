namespace Derrixx.BehaviourTrees.Nodes.Decorators
{
	public sealed class RootNode : DecoratorNode
	{
		protected override State OnUpdate() => Child.Update();
	}
}
