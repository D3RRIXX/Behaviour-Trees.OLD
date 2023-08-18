namespace Derrixx.BehaviourTrees
{
    public class SelectorNode : CompositeNode
    {
	    private int _lastChildIndex;
	    
	    protected override State OnUpdate()
	    {
		    for (int i = 0; i < Children.Count; i++)
		    {
			    Node child = Children[i];
			    State childState = child.Update();

			    if (childState != State.Failure)
			    {
				    if (_lastChildIndex != i)
					    Children[_lastChildIndex].ResetState();
				    
				    _lastChildIndex = i;
				    return childState;
			    }
		    }

		    return State.Failure;
	    }
    }
}
