namespace Derrixx.BehaviourTrees.Runtime.Pure
{
    public enum NodeState
    {
        Failure,
        Running,
        Success,
    }

    public abstract class Node : INode
    {
        private readonly string _name;

        protected NodeState State;

        public INode Parent { get; set; }

        private protected Node(string name)
        {
            _name = name;
        }

        protected internal virtual string Color => "silver";

        public virtual void OnNodeEnter() { }
        public virtual void OnNodeExit() { }

        public abstract NodeState Execute();

        public abstract void InjectBlackboard(IBlackboard blackboard);

        public void PrintTree()
        {
            INode root = this;
            while (root.Parent != null)
            {
                root = root.Parent;
            }
            
            //TODO Return functionality
            // Debug.Log(root.PrintNode(0));
        }
    }
}