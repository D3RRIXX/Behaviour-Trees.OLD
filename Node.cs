using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Derrixx.BehaviourTrees
{
    public enum NodeState
    {
        Failure,
        Running,
        Success,
    }

    public abstract class Node
    {
        private readonly string _name;

        protected NodeState state;
        protected internal Node parent;
    
        private protected Node(string name)
        {
            _name = name;
        }

        protected internal virtual string Color => "silver";

        public void PrintTree()
        {
            Node root = this;
            while (root.parent != null)
            {
                root = root.parent;
            }
            
            Debug.Log(root.PrintNode(0));
        }

        public abstract NodeState Evaluate(IBlackboard blackboard);

        protected internal virtual string PrintNode(int nodeLevel)
        {
            IEnumerable<string> levelDashes = Enumerable.Repeat("-", nodeLevel);
            string output = $"{string.Join(string.Empty, levelDashes)}<color={Color}>{_name}</color>\n";

            return output;
        }
    }
}