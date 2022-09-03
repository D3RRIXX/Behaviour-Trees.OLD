using System;
using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.Decorators
{
    public sealed class Conditional : Leaf
    {
        private readonly Func<bool> _condition;

        public Conditional(string name, Func<bool> condition) : base(name)
        {
            if (!name.Contains("?"))
                Debug.LogWarning($"Please add \'?\' to \"{name}\" node for better code readability");
            
            _condition = condition ?? throw new ArgumentNullException(nameof(condition), ErrorLog(name));
        }

        public override NodeState Execute(IBlackboard blackboard)
        {
            return _condition() ? NodeState.Success : NodeState.Failure;
        }
        
        private static string ErrorLog(string name) => $"Conditional \"{name}\" 's condition method is null";
    }
}