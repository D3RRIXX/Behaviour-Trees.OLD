using System;
using System.Collections.Generic;

namespace Derrixx.BehaviourTrees.Runtime.Pure.Composites
{
    public sealed class Selector : Composite
    {
        public Selector(string name, IEnumerable<INode> children) : base($"{name} (Selector)", children)
        {
        }
        
        public Selector(IEnumerable<INode> children) : base("Sequence", children)
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
                        continue;
                    case NodeState.Running:
                    case NodeState.Success:
                        return state;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            state = NodeState.Failure;
            return state;
        }
    }
}