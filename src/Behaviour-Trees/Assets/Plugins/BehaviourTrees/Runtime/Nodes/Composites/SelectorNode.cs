namespace Derrixx.BehaviourTrees.Runtime.Nodes.Composites
{
    public class SelectorNode : CompositeNode
    {
	    protected override bool ShouldBreak(State state) => state != State.Failure;
    }
}
