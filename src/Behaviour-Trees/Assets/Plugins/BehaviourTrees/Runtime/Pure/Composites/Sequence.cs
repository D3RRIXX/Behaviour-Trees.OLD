using System;
using System.Collections.Generic;

namespace Derrixx.BehaviourTrees.Runtime.Pure.Composites
{
    public sealed class Sequence : Composite
    {
        public Sequence(string name, IEnumerable<INode> children) : base($"{name} (Sequence)", children)
        {
        }
        
        public Sequence(IEnumerable<INode> children) : base("Sequence", children)
        {
        }

        public override NodeState Execute()
        {
            foreach (INode node in Children)
            {
                state = node.Execute();
                
                switch (state)
                {
                    case NodeState.Failure:
                    case NodeState.Running:
                        return state;
                    case NodeState.Success:
                        continue;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            state = NodeState.Success;
            return state;
        }
    }
}