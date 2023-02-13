using System.Collections.Generic;
using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
	public abstract class CompositeNode : Node
	{
		[SerializeField] protected List<Node> Children = new List<Node>();
	}
}