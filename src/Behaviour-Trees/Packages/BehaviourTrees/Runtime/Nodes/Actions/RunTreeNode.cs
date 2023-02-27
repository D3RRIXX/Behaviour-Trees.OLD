using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
	public class RunTreeNode : ActionNode
	{
		[SerializeField] private BehaviourTree _behaviourTree;

		protected override void OnCreate()
		{
			_behaviourTree = _behaviourTree.Clone(Runner, null);
		}

		protected override State OnUpdate() => _behaviourTree.Update();
	}
}