using System.Collections.Generic;
using JetBrains.Annotations;

namespace Derrixx.BehaviourTrees.Runtime.Pure.Composites
{
    public sealed class Selector : Composite
    {
        public Selector(string name, IEnumerable<INode> children, bool dynamic)
            : base($"{name} (Selector)", children, dynamic)
        {
        }
        
        public Selector(IEnumerable<INode> children, bool dynamic)
            : base("Sequence", children, dynamic)
        {
        }

        protected override bool ShouldBreak(NodeState state)
        {
            switch (state)
            {
                case NodeState.Running:
                case NodeState.Success:
                    return true;
                default:
                    return false;
            }
        }
    }
}