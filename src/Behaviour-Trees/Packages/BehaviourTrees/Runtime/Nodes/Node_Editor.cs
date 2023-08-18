using UnityEngine;

namespace Derrixx.BehaviourTrees
{
	public abstract partial class Node
	{
#if UNITY_EDITOR
        [HideInInspector] public string Guid;
        [HideInInspector] public Vector2 Position;
#endif
	}
}
