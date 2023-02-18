using Derrixx.BehaviourTrees.Runtime.BlackboardScripts.BlackboardProperties;
using Derrixx.BehaviourTrees.Runtime.Nodes;
using UnityEngine;

namespace DefaultNamespace
{
	public class IncrementKillsNode : ActionNode
	{
		[SerializeField] private IntBlackboardProperty kills;
		
		protected override State OnUpdate()
		{
			kills.Value++;
			return State.Success;
		}
	}
}