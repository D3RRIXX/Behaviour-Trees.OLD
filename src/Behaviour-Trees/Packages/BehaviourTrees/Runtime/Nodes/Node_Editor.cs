using UnityEngine;

namespace Derrixx.BehaviourTrees
{
	public abstract partial class Node
	{
#if UNITY_EDITOR
        [HideInInspector] public string Guid;
        [HideInInspector] public Vector2 Position;
        [SerializeField, HideInInspector] private BehaviourTree behaviourTree;
        
        /// <summary>
        /// EDITOR USE ONLY
        /// </summary>
        public BehaviourTree BehaviourTree
        {
	        get => behaviourTree;
	        internal set => behaviourTree = value;
        }
#endif
	}
}
