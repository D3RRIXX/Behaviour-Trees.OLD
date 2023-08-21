using UnityEngine;

namespace Derrixx.BehaviourTrees
{
	public class RunTreeNode : ActionNode
	{
		[SerializeField] private BehaviourTree _behaviourTree;

		public override void OnCreate()
		{
			_behaviourTree = _behaviourTree.Clone(Runner, null);
		}

		protected override State OnUpdate() => _behaviourTree.Update();
	}
}
