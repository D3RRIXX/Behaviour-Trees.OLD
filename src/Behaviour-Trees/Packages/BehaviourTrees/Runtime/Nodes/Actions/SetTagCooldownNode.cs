using UnityEngine;

namespace Derrixx.BehaviourTrees.Nodes.Actions
{
	public class SetTagCooldownNode : ActionNode
	{
		[SerializeField] private string _timerName;
		
		protected override State OnUpdate() => State.Running;
	}
}
