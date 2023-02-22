using Derrixx.BehaviourTrees.Runtime;
using Derrixx.BehaviourTrees.Runtime.Nodes;
using UnityEngine;

namespace DefaultNamespace
{
	public class WaitNode : ActionNode
	{
		[SerializeField] private float _waitTime;

		private float _timeOnStart;
		
		public override string GetDescription() => $"Wait: {_waitTime}s";

		protected override void OnStart(BehaviourTreeRunner runner)
		{
			_timeOnStart = Time.time;
		}

		protected override State OnEvaluate(BehaviourTreeRunner runner) => Time.time - _timeOnStart >= _waitTime ? State.Success : State.Running;
	}
}