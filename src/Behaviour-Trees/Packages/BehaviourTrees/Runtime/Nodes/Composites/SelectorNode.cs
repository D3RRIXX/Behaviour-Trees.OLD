namespace Derrixx.BehaviourTrees
{
    public class SelectorNode : CompositeNode
    {
	    protected override State OnUpdate()
	    {
		    foreach (Node child in Children)
		    {
			    State childState = child.Update();
			    
			    if (childState != State.Failure)
				    return childState;
		    }

		    return State.Failure;
	    }

	    protected virtual State FinalState => State.Failure;
    }
}
