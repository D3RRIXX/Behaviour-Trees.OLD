namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
	public class SequenceNode : CompositeNode
	{
		protected override State FinalState => State.Success;
	}
}