using System.Collections.Generic;
using JetBrains.Annotations;

namespace Derrixx.BehaviourTrees.Runtime.Pure.Composites
{
    public sealed class Sequence : Composite
    {
        public Sequence(string name, IEnumerable<INode> children, bool dynamic) 
            : base($"{name} (Sequence)", children, dynamic)
        {
        }

        public Sequence(IEnumerable<INode> children, bool dynamic) : base("Sequence", children, dynamic)
        {
        }

        protected override bool ShouldBreak(NodeState state)
        {
            switch (state)
            {
                case NodeState.Failure:
                case NodeState.Running:
                    return true;
                default:
                    return false;
            }
        }
    }
}