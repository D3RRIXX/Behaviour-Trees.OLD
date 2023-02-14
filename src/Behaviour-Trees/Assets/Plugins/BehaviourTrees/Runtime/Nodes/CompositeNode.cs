using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
	public abstract class CompositeNode : Node
	{
		[SerializeField, HideInInspector] public List<Node> Children = new List<Node>();

		public sealed override Node Clone()
		{
			CompositeNode clone = (CompositeNode)base.Clone();
			clone.Children = Children.Select(x => x.Clone()).ToList();
			
			return clone;
		}
	}
}