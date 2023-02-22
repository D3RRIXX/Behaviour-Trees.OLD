using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
	public class RunTreeNode : ActionNode
	{
		[SerializeField] private BehaviourTree _behaviourTree;

		protected override void OnClone()
		{
			_behaviourTree = _behaviourTree.Clone();
		}

		protected override State OnEvaluate(BehaviourTreeRunner runner) => _behaviourTree.Evaluate(runner);
	}
}