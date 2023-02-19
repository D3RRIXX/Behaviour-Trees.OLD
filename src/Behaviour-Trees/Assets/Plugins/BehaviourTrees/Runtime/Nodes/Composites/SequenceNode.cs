namespace Derrixx.BehaviourTrees.Runtime.Nodes.Composites
{
	public class SequenceNode : CompositeNode
	{
		protected override bool ShouldBreak(State state) => state != State.Success;
	}
}