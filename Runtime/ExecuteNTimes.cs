namespace Derrixx.BehaviourTrees
{
    public sealed class ExecuteNTimes : Node
    {
        private readonly int _limit;
        private readonly Node _child;
        
        public int Counter { get; private set; }
        
        public ExecuteNTimes(int n, Node child) : base($"Execute {n} times")
        {
            _limit = n;
            _child = child;
        }

        public override NodeState Evaluate(IBlackboard blackboard)
        {
            if (Counter >= _limit)
                return NodeState.Success;
                
            Counter++;
            
            return _child.Evaluate(blackboard);
        }

        public void Reset() => Counter = 0;

        protected internal override string PrintNode(int nodeLevel)
        {
            string output = base.PrintNode(nodeLevel);
            output += _child.PrintNode(nodeLevel + 1);
            
            return output;
        }
    }
}