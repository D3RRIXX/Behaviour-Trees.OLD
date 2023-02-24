using Derrixx.BehaviourTrees.Runtime;
using Derrixx.BehaviourTrees.Runtime.BlackboardScripts.BlackboardProperties;
using Derrixx.BehaviourTrees.Runtime.Nodes;
using UnityEngine;

namespace Samples.Burglar.Scripts
{
	public class FindDoorNode : ActionNode
	{
		[SerializeField] private ObjectBlackboardProperty doorProperty;
		
		protected override State OnEvaluate(BehaviourTreeRunner runner)
		{
			TestAI testAI = runner as TestAI;
			bool canSee = testAI!.CanSee(out Door door);
			
			if (!canSee)
				return State.Failure;

			doorProperty.Value = door;
			return State.Success;
		}
	}
}