using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
	public abstract partial class Node
	{
#if UNITY_EDITOR
        [HideInInspector] public string Guid;
        [HideInInspector] public Vector2 Position;
        [SerializeField, HideInInspector] private BehaviourTree behaviourTree;
        
        public BehaviourTree BehaviourTree
        {
	        get => behaviourTree;
	        internal set => behaviourTree = value;
        }
#endif
	}
}