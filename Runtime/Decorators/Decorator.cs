namespace Derrixx.BehaviourTrees.Runtime.Decorators
{
    public abstract class Decorator : Node
    {
        private protected Decorator(string name) : base(name)
        {
        }
        
        protected internal override string Color => "lime";
    }
}