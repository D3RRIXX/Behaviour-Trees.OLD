namespace Derrixx.BehaviourTrees.Runtime.Decorators
{
    public sealed class ExecuteNTimes : Decorator
    {
        private readonly int _limit;
        
        public int Counter { get; private set; }
        
        public ExecuteNTimes(int n, Node child) : base($"Execute {n} times", child)
        {
            _limit = n;
        }

        public override NodeState Execute(IBlackboard blackboard)
        {
            if (Counter >= _limit)
                return NodeState.Success;
                
            Counter++;
            return Child.Execute(blackboard);
        }

        public void Reset() => Counter = 0;

        protected internal override string PrintNode(int nodeLevel)
        {
            string output = base.PrintNode(nodeLevel);
            output += Child.PrintNode(nodeLevel + 1);
            
            return output;
        }
    }
}