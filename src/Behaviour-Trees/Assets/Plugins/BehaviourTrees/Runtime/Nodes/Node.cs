using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
    public abstract partial class Node : ScriptableObject
    {
        public enum State
        {
            Running,
            Failure,
            Success
        }

        public State CurrentState { get; set; } = State.Running;
        public bool Started { get; set; }

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

        public virtual Node Clone() => Instantiate(this);
        public virtual bool IsConnectedWith(Node other) => this == other;
        
        protected virtual void OnStart() { }
        protected virtual void OnFinish() { }

        protected abstract State OnUpdate();
    }
}
