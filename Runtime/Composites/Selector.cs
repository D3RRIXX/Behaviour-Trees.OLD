using System;
using System.Collections.Generic;

namespace Derrixx.BehaviourTrees.Runtime.Composites
{
    public sealed class Selector : Composite
    {
        public Selector(string name, IEnumerable<Node> children) : base($"{name} (Selector)", children)
        {
        }
        
        public Selector(IEnumerable<Node> children) : base("Sequence", children)
        {
        }

        public override NodeState Execute(IBlackboard blackboard)
        {
            foreach (Node node in Children)
            {
                state = node.Execute(blackboard);
                
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