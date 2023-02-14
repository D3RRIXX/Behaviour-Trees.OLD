using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
#if UNITY_EDITOR
	public abstract partial class Node
	{
        [HideInInspector] public string Guid;
        [HideInInspector] public Vector2 Position;
	}
#endif
}