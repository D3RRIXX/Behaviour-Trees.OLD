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
        
        public State CurrentState { get; private set; } = State.Running;
        public bool Started { get; private set; }

        public int ExecutionOrder => executionOrder;
        public virtual string GetDescription() => GetNodeName(GetType());

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
            clone.name = name;
            
            return clone;
        }

        protected internal void ResetState()
        {
            Started = false;
            CurrentState = State.Running;
        }

        internal virtual void SetExecutionOrder(ref int order)
        {
	        executionOrder = order++;
        }

        public virtual bool IsConnectedWith(Node other) => this == other;
        
        /// <summary>
        /// Gets called when this node starts execution
        /// </summary>
        protected virtual void OnStart() { }
        
        /// <summary>
        /// Gets called when this node finishes execution
        /// </summary>
        protected virtual void OnFinish() { }

        protected abstract State OnUpdate();

        public static string GetNodeName(Type nodeType)
        {
            const string pattern = @"([A-Z])\w+(?=Node)";
            var name = Regex.Match(nodeType.Name, pattern).Value;

            return Regex.Replace(name, "(\\B[A-Z])", " $1");
        }
    }
}
