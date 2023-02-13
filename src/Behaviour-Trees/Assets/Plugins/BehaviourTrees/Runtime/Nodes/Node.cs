using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
    public abstract class Node : ScriptableObject
    {
        public enum State
        {
            Running,
            Failure,
            Success
        }

        public State CurrentState { get; set; } = State.Running;
        public bool Started { get; set; }

#if UNITY_EDITOR
        [SerializeField, HideInInspector] private string guid;
        [SerializeField, HideInInspector] private Vector2 position;

        internal string Guid
        {
            get => guid;
            set => guid = value;
        }
        
        public Vector2 Position
        {
            get => position;
            set => position = value;
        }
#endif

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
        
        protected virtual void OnStart() { }
        protected virtual void OnFinish() { }
        protected abstract State OnUpdate();
    }
}
