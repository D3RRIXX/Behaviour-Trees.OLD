using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
	public abstract class CompositeNode : Node
	{
		[SerializeField, HideInInspector] public List<Node> Children = new List<Node>();
		
		[Tooltip("Re-evaluate all children nodes every update?")]
		[SerializeField] private bool _dynamic;

		private int _currentChildIndex;
		
		public override string GetDescription()
		{
			string description = base.GetDescription();
			if (_dynamic)
				description += " (Dynamic)";
			
			return description;
		}

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

		protected abstract State FinalState { get; }

		protected sealed override State OnUpdate()
		{
			if (_dynamic)
				_currentChildIndex = 0;
			else
				_currentChildIndex %= Children.Count;
			
			do
			{
				Node currentChild = Children[_currentChildIndex];
				State updateResult = currentChild.Update();

				if (updateResult != FinalState)
					return updateResult;

				_currentChildIndex++;
			} while (_currentChildIndex < Children.Count);

			return FinalState;
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