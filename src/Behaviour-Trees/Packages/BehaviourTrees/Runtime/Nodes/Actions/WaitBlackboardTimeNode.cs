using Derrixx.BehaviourTrees.Runtime.BlackboardScripts.BlackboardProperties;
using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
	public class WaitBlackboardTimeNode : ActionNode
	{
		[SerializeField] private FloatBlackboardProperty _waitTime;

		private float _timeOnStart;
		
		protected override void OnStart(BehaviourTreeRunner runner)
		{
			_timeOnStart = Time.time;
		}

		protected override State OnEvaluate(BehaviourTreeRunner runner) 
			=> Time.time - _timeOnStart > _waitTime.Value ? State.Success : State.Running;
	}
}