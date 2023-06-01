using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Derrixx.BehaviourTrees.Nodes
{
	public abstract class CompositeNode : Node
	{
		[SerializeField, HideInInspector] public List<Node> Children = new List<Node>();

		public sealed override Node Clone(BehaviourTreeRunner runner)
		{
			CompositeNode clone = (CompositeNode)base.Clone(runner);
			clone.Children = Children.Select(x => x.Clone(runner)).ToList();
			
			return clone;
		}

		public sealed override bool IsConnectedWith(Node other)
		{
			return base.IsConnectedWith(other) || Children.Any(x => x.IsConnectedWith(other));
		}

		protected internal sealed override void ResetState()
		{
			base.ResetState();
			foreach (Node child in Children)
			{
				child.ResetState();
			}
		}

		protected override void OnActivate()
		{
			foreach (Node child in Children)
			{
				child.ResetState();
			}
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
