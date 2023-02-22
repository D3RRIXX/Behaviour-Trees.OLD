namespace Derrixx.BehaviourTrees.Runtime.Nodes.Composites
{
    public class SelectorNode : CompositeNode
    {
	    protected override State FinalState => State.Failure;
    }
}
