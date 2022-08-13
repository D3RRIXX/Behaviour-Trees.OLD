using System;

namespace Derrixx.BehaviourTrees
{
    public sealed class Conditional : Node
    {
        private readonly Func<bool> _condition;

        public Conditional(string name, Func<bool> condition) : base(ProcessName(name))
        {
            _condition = condition ?? throw new ArgumentNullException(nameof(condition), ErrorLog(name));
        }

        protected internal override string Color => "lime";

        public override NodeState Evaluate(IBlackboard blackboard)
        {
            return _condition() ? NodeState.Success : NodeState.Failure;
        }

        private static string ErrorLog(string name) => $"Conditional \"{name}\" 's condition method is null";

        private static string ProcessName(string name) => name.Contains('?') ? name : name + '?';
    }
}