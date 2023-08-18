using System;
using UnityEngine;

#if BTREE_ADD_ZENJECT
using Zenject;
#endif

namespace Derrixx.BehaviourTrees
{
	public class RunTreeNode : ActionNode
	{
		[SerializeField] private BehaviourTree _behaviourTree;
		
#if BTREE_ADD_ZENJECT
		private DiContainer _container;

		[Inject]
		private void Construct(DiContainer container)
		{
			_container = container;
		}
#endif

		public override string GetDescription() => _behaviourTree == null ? base.GetDescription() : $"Run Tree '{_behaviourTree.name}'";

		public override void OnCreate()
		{
			_behaviourTree = _behaviourTree.Clone(Runner)
			                               .WithCustomBlackboard(BehaviourTree.Blackboard)
			                               .WithTraverseNodeAction(OnTraverseNodes)
			                               .Build();

			// _behaviourTree = _behaviourTree.Clone(Runner, null);
		}

		private void OnTraverseNodes(Node node)
		{
#if BTREE_ADD_ZENJECT
			_container?.Inject(node);
#endif
		}

		protected override State OnUpdate() => _behaviourTree.Update();
	}
}
