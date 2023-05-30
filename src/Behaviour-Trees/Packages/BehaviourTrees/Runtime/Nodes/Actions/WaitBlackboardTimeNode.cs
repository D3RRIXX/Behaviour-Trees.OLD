using UnityEngine;

namespace Derrixx.BehaviourTrees.Nodes.Actions
{
	public class WaitBlackboardTimeNode : ActionNode
	{
		[SerializeField] private FloatBlackboardProperty _waitTime;

		private float _timeOnStart;
		
		protected override void OnActivate()
		{
			_timeOnStart = Time.time;
		}

		protected override State OnUpdate() 
			=> Time.time - _timeOnStart > _waitTime.Value ? State.Success : State.Running;
	}
}
