namespace Derrixx.BehaviourTrees.Runtime.Decorators
{
    public sealed class ExecuteNTimes : Decorator
    {
        private readonly int _limit;
        private readonly Node _child;
        
        public int Counter { get; private set; }
        
        public ExecuteNTimes(int n, Node child) : base($"Execute {n} times")
        {
            _limit = n;
            _child = child;
        }

        public override NodeState Execute(IBlackboard blackboard)
        {
            if (Counter >= _limit)
                return NodeState.Success;
                
            Counter++;
            return _child.Execute(blackboard);
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