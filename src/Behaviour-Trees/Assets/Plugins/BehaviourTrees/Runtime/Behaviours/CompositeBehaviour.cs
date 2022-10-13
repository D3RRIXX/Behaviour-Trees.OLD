using System;
using System.Collections.Generic;
using Derrixx.BehaviourTrees.Runtime.Pure;
using Derrixx.BehaviourTrees.Runtime.Pure.Composites;
using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.Behaviours
{
	public abstract class CompositeBehaviour : NodeBehaviour
	{
	}

	public abstract class CompositeBehaviour<T> : CompositeBehaviour where T : Composite
	{
		[Tooltip("Will this composite re-run all previous nodes on each execution?")]
		[SerializeField] private bool dynamic = true;
		
		private Composite _composite;

		private void Start()
		{
			_composite ??= CreateCompositeNode(CollectChildren());
		}

		public override NodeState Execute()
		{
			return _composite.Execute();
		}

		public sealed override void InjectBlackboard(IBlackboard blackboard)
		{
			_composite ??= CreateCompositeNode(CollectChildren());
			_composite.InjectBlackboard(blackboard);
		}

		public CompositeBehaviour Setup(IEnumerable<INode> children)
		{
			_composite = CreateCompositeNode(children);
			return this;
		}

		private List<INode> CollectChildren()
		{
			var childNodes = new List<INode>();
			foreach (Transform child in transform)
			{
				if (child.TryGetComponent(out INode node))
					childNodes.Add(node);
			}

			return childNodes;
		}

		private Composite CreateCompositeNode(IEnumerable<INode> children)
		{
			return (Composite)Activator.CreateInstance(typeof(T), name, children, dynamic);
		}
	}
}