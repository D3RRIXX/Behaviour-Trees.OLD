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
        
        /// <summary>
        /// The Object that runs this node
        /// </summary>
        protected BehaviourTreeRunner Runner { get; private set; }

        public int ExecutionOrder => executionOrder;
        public virtual string GetDescription() => GetNodeName(GetType());

        public State Update()
        {
            if (!Started)
            {
                OnActivate();
                Started = true;
            }

            CurrentState = OnUpdate();

            if (CurrentState != State.Running)
            {
                OnDeactivate();
                Started = false;
            }

            return CurrentState;
        }

        public virtual Node Clone(BehaviourTreeRunner runner)
        {
            Node clone = Instantiate(this);
            Runner = runner;
            
            clone.OnCreate();
            
            return clone;
        }

        private void Awake()
        {
            nodeName = name;
        }

        protected internal virtual void ResetState()
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
        /// Gets called immediately after this node is instantiated
        /// </summary>
        protected virtual void OnCreate() { }

        /// <summary>
        /// Gets called when this node starts execution
        /// </summary>
        protected virtual void OnActivate() { }

        /// <summary>
        /// Gets called when this node finishes execution
        /// </summary>
        protected virtual void OnDeactivate() { }

        protected abstract State OnUpdate();

        public static string GetNodeName(Type nodeType)
        {
            string name = nodeType.Name;
            if (name.EndsWith("Node"))
                name = name.Substring(0, name.Length - 4);

            return Regex.Replace(name, "(\\B[A-Z])", " $1");
        }
    }
}
