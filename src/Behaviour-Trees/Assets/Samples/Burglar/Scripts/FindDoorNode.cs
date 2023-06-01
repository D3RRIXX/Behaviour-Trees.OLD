using Derrixx.BehaviourTrees;
using Derrixx.BehaviourTrees.Nodes;
using UnityEngine;

namespace Samples.Burglar.Scripts
{
	public class FindDoorNode : ActionNode
	{
		[SerializeField] private ObjectBlackboardProperty doorProperty;
		
		protected override State OnUpdate()
		{
			TestAI testAI = Runner as TestAI;
			bool canSee = testAI!.CanSee(out Door door);
			
			if (!canSee)
				return State.Failure;

			doorProperty.Value = door;
			return State.Success;
		}
	}
}