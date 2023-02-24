using Derrixx.BehaviourTrees.Runtime;
using Derrixx.BehaviourTrees.Runtime.BlackboardScripts.BlackboardProperties;
using Derrixx.BehaviourTrees.Runtime.Nodes;
using UnityEngine;

namespace DefaultNamespace
{
	public class SawEnemyNode : ActionNode
	{
		[SerializeField] private ObjectBlackboardProperty enemy;
		
		protected override State OnEvaluate(BehaviourTreeRunner runner)
		{
			TestAI ai = (TestAI)runner;
			if (!ai.CanSee(out TestAI other))
				return State.Failure;

			if (other.Team == ai.Team)
				return State.Failure;

			enemy.Value = other;
			return State.Success;
		}
	}
}