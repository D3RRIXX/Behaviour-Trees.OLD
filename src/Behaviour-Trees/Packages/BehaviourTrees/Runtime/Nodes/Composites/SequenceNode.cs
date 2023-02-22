namespace Derrixx.BehaviourTrees.Runtime.Nodes.Composites
{
	public class SequenceNode : CompositeNode
	{
		protected override State FinalState => State.Success;
	}
}