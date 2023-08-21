using Derrixx.BehaviourTrees;
using UnityEngine;

namespace DefaultNamespace
{
	public class SawEnemyNode : ActionNode
	{
		[SerializeField] private ObjectBlackboardProperty enemy;
		
		protected override State OnUpdate()
		{
			TestAI ai = (TestAI)Runner;
			if (!ai.CanSee(out TestAI other))
				return State.Failure;

			if (other.Team == ai.Team)
				return State.Failure;

			enemy.Value = other;
			return State.Success;
		}
	}
}