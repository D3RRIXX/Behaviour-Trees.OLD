using Derrixx.BehaviourTrees.Runtime.BlackboardScripts;
using Derrixx.BehaviourTrees.Runtime.BlackboardScripts.BlackboardProperties;
using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
	public class FailureNode : ActionNode
	{
		[SerializeField] private IntBlackboardProperty kills;
		
		protected override State OnUpdate() => State.Failure;
	}
}