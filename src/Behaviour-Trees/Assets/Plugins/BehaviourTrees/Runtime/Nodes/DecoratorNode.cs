namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
	public abstract class DecoratorNode : Node
	{
        public Node Child { get; set; }
	}
}