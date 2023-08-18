using System.Globalization;
using UnityEngine;

namespace Derrixx.BehaviourTrees.Nodes.Decorators
{
	public class TimeLimitNode : DecoratorNode
	{
		[SerializeField] private float _timeLimit;

		private float _timePassed;
		
		public override string GetDescription() => $"Return Failure if child doesn't finish in {_timeLimit.ToString(CultureInfo.CurrentCulture)}s";

		protected override void OnDeactivate()
		{
			_timePassed = 0f;
		}

		protected override State OnUpdate()
		{
			if (_timePassed >= _timeLimit)
				return State.Failure;
			
			State childState = Child.Update();
			if (childState != State.Running)
				return childState;
			
			_timePassed += Time.deltaTime;
			return State.Running;

		}
	}
}
