using System.Collections.Generic;

namespace Derrixx.BehaviourTrees.Runtime.Pure.Composites
{
    public abstract class Composite : Node
    {
        private readonly List<INode> _children;

        private protected Composite(string name, IEnumerable<INode> children) : base(name)
        {
            _children = new List<INode>();
            
            foreach (INode childNode in children)
            {
                AddChild(childNode);
            }
        }

        public override void InjectBlackboard(IBlackboard blackboard)
        {
	        foreach (INode node in _children)
	        {
		        node.InjectBlackboard(blackboard);
	        }
        }

        protected internal override string Color => "cyan";

        protected IReadOnlyCollection<INode> Children => _children;

        private void AddChild(INode node)
        {
            node.Parent = this;
            _children.Add(node);
        }

        /*protected internal override string PrintNode(int nodeLevel)
        {
            string output = base.PrintNode(nodeLevel);
            _children.ForEach(child => output += child.PrintNode(nodeLevel + 1));
            return output;
        }*/
    }
}