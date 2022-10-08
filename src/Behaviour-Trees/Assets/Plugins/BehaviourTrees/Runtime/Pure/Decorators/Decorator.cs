namespace Derrixx.BehaviourTrees.Runtime.Pure.Decorators
{
    public abstract class Decorator : Node
    {
        protected INode Child { get; }

        private protected Decorator(string name, INode child) : base(name)
        {
            Child = child;
        }

        public override void InjectBlackboard(IBlackboard blackboard)
        {
	        Child.InjectBlackboard(blackboard);
        }

        protected internal override string Color => "lime";
    }
}