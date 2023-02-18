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

		public sealed override bool IsConnectedWith(Node other)
		{
			return base.IsConnectedWith(other) || Children.Any(x => x.IsConnectedWith(other));
		}

		internal sealed override void SetExecutionOrder(ref int order)
		{
			base.SetExecutionOrder(ref order);
			foreach (Node child in Children)
			{
				child.SetExecutionOrder(ref order);
			}
		}
	}
}