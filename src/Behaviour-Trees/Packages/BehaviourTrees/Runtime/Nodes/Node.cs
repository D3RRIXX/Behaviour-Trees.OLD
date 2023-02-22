using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
    public abstract partial class Node : ScriptableObject
    {
        [SerializeField] private string nodeName;
        [SerializeField, HideInInspector] private int executionOrder;
        
        public enum State
        {
            Running,
            Failure,
            Success
        }

        [SerializeField, HideInInspector] private string _cachedTypeName;
        
        public new string name
        {
            get
            {
                if (string.IsNullOrEmpty(nodeName) || string.IsNullOrWhiteSpace(nodeName))
                {
                    if (string.IsNullOrEmpty(_cachedTypeName))
                        _cachedTypeName = GetNodeName(GetType());

                    return _cachedTypeName;
                }

                return nodeName;
            }
        }

        public State CurrentState { get; private set; } = State.Running;
        public bool Started { get; private set; }

        public int ExecutionOrder => executionOrder;
        public virtual string GetDescription() => GetNodeName(GetType());

        public State Evaluate(BehaviourTreeRunner runner)
        {
            if (!Started)
            {
                OnStart(runner);
                Started = true;
            }

            CurrentState = OnEvaluate(runner);

            if (CurrentState != State.Running)
            {
                OnFinish(runner);
                Started = false;
            }

            return CurrentState;
        }

        public virtual Node Clone()
        {
            Node clone = Instantiate(this);
            // clone.name = name;
            
            return clone;
        }

        private void Awake()
        {
            nodeName = name;
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
        /// <param name="runner"></param>
        protected virtual void OnStart(BehaviourTreeRunner runner) { }

        /// <summary>
        /// Gets called when this node finishes execution
        /// </summary>
        /// <param name="behaviourTreeRunner"></param>
        protected virtual void OnFinish(BehaviourTreeRunner behaviourTreeRunner) { }

        protected abstract State OnEvaluate(BehaviourTreeRunner runner);

        public static string GetNodeName(Type nodeType)
        {
            const string pattern = @"([A-Z])\w+(?=Node)";
            var name = Regex.Match(nodeType.Name, pattern).Value;

            return Regex.Replace(name, "(\\B[A-Z])", " $1");
        }
    }
}
