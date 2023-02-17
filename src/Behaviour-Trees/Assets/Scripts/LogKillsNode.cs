using Derrixx.BehaviourTrees.Runtime.BlackboardScripts.BlackboardProperties;
using Derrixx.BehaviourTrees.Runtime.Nodes;
using UnityEngine;

namespace DefaultNamespace
{
	public class LogKillsNode : ActionNode
	{
		[SerializeField] private IntBlackboardProperty kills;
		
		protected override State OnUpdate()
		{
			Debug.Log(kills.Value);
			return State.Success;
		}
	}
}