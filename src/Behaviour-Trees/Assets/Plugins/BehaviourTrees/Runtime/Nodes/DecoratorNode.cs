using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
	public abstract class DecoratorNode : Node
	{
		[SerializeField] private Node _child;
		
		public Node Child
		{
			get => _child;
			set => _child = value;
		}
	}
}