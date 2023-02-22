namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
    public class SelectorNode : CompositeNode
    {
	    protected override State FinalState => State.Failure;
    }
}
