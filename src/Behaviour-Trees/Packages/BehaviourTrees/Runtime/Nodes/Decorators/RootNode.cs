namespace Derrixx.BehaviourTrees
{
	public sealed class RootNode : DecoratorNode
	{
		protected override State OnUpdate() => Child.Update();
	}
}
