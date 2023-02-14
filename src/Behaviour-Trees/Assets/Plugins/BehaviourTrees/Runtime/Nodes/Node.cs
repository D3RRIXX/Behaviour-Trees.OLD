using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
    public abstract partial class Node : ScriptableObject
    {
        // [SerializeField] private string customName;
        [SerializeField, HideInInspector] private int executionOrder;
        
        public enum State
        {
            Running,
            Failure,
            Success
        }

        // public new string name => string.IsNullOrEmpty(customName) ? GetNodeName(GetType()) : customName;
        
        public State CurrentState { get; set; } = State.Running;
        public bool Started { get; set; }

        public int ExecutionOrder => executionOrder;

        public State Update()
        {
            if (!Started)
            {
                OnStart();
                Started = true;
            }

            CurrentState = OnUpdate();

            if (CurrentState != State.Running)
            {
                OnFinish();
                Started = false;
            }

            return CurrentState;
        }

        public virtual Node Clone()
        {
            Node clone = Instantiate(this);
            clone.name = GetNodeName(clone.GetType());
            
            return clone;
        }

        internal virtual void SetExecutionOrder(ref int order)
        {
	        executionOrder = order++;
        }

        public virtual bool IsConnectedWith(Node other) => this == other;
        
        protected virtual void OnStart() { }
        protected virtual void OnFinish() { }

        protected abstract State OnUpdate();

        private static Regex _nameRegex;
        
        public static string GetNodeName(Type nodeType)
        {
            const string pattern = @"([A-Z])\w+(?=Node)";
            _nameRegex ??= new Regex(pattern);

            return _nameRegex.Match(nodeType.Name).Value;
        }
    }
}
