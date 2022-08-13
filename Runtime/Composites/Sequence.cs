using System;
using System.Collections.Generic;

namespace Derrixx.BehaviourTrees.Composites
{
    public sealed class Sequence : Composite
    {
        public Sequence(string name, IEnumerable<Node> children) : base($"{name} (Sequence)", children)
        {
        }
        
        public Sequence(IEnumerable<Node> children) : base("Sequence", children)
        {
        }

        public override NodeState Evaluate(IBlackboard blackboard)
        {
            foreach (Node node in Children)
            {
                state = node.Evaluate(blackboard);
                
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