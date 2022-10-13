using System.Collections.Generic;
using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.Pure.Composites
{
    public abstract class Composite : Node
    {
        private readonly List<INode> _children;
        private readonly bool _dynamic;

        private int _currentChildIndex;
        private INode _cachedChild;

        private protected Composite(string name, IEnumerable<INode> children, bool dynamic) : base(name)
        {
            _dynamic = dynamic;
            _children = new List<INode>();
            
            foreach (INode childNode in children)
            {
                AddChild(childNode);
            }

            _cachedChild = _children[0];
            _cachedChild.OnNodeEnter();
        }

        public override void InjectBlackboard(IBlackboard blackboard)
        {
	        foreach (INode node in _children)
	        {
		        node.InjectBlackboard(blackboard);
	        }
        }

        public sealed override NodeState Execute()
        {
            if (_dynamic)
                _currentChildIndex = 0;
            
            do
            {
                INode currentChild = Children[_currentChildIndex];
                if (currentChild != _cachedChild)
                {
                    _cachedChild.OnNodeExit();
                    _cachedChild = currentChild;
                    _cachedChild.OnNodeEnter();
                }
                
                State = currentChild.Execute();

                Debug.Log(currentChild);
                if (ShouldBreak(State))
                    return State;

                _currentChildIndex++;
            }
            while (_currentChildIndex < Children.Count);

            _currentChildIndex %= Children.Count;
            
            State = NodeState.Success;
            return State;
        }

        protected internal override string Color => "cyan";

        protected IReadOnlyList<INode> Children => _children;

        protected abstract bool ShouldBreak(NodeState state);

        private void AddChild(INode node)
        {
            node.Parent = this;
            _children.Add(node);
        }
    }
}