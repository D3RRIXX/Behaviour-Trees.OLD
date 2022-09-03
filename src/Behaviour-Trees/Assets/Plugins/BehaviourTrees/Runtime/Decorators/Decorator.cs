namespace Derrixx.BehaviourTrees.Runtime.Decorators
{
    public abstract class Decorator : Node
    {
        protected Node Child { get; }

        private protected Decorator(string name, Node child) : base(name)
        {
            Child = child;
        }

        protected internal override string Color => "lime";
    }
}