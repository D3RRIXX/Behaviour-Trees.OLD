using System.Collections.Generic;

namespace Derrixx.BehaviourTrees.Composites
{
    public abstract class Composite : Node
    {
        private readonly List<Node> _children;

        private protected Composite(string name, IEnumerable<Node> children) : base(name)
        {
            _children = new List<Node>();
            
            foreach (Node childNode in children)
            {
                AddChild(childNode);
            }
        }

        protected internal override string Color => "cyan";

        protected IReadOnlyCollection<Node> Children => _children;

        public override NodeState Evaluate(IBlackboard blackboard)
        {
            return NodeState.Failure;
        }

        private void AddChild(Node node)
        {
            node.parent = this;
            _children.Add(node);
        }

        protected internal override string PrintNode(int nodeLevel)
        {
            string output = base.PrintNode(nodeLevel);
            _children.ForEach(child => output += child.PrintNode(nodeLevel + 1));
            return output;
        }
    }
}