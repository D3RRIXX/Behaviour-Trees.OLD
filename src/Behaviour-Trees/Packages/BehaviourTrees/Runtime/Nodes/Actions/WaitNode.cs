using System.Globalization;
using UnityEngine;

namespace Derrixx.BehaviourTrees
{
	public class WaitNode : ActionNode
	{
		[SerializeField] private float _waitTime = 1f;
		[SerializeField, Min(0f)] private float _randomOffset;

		private float _timeOnStart;
		private float _timeToWait;
		
		public override string GetDescription()
		{
			string waitTime = _waitTime.ToString(CultureInfo.CurrentCulture);
			if (_randomOffset > 0)
				waitTime += $"+-{_randomOffset.ToString(CultureInfo.CurrentCulture)}";
			
			return $"Wait: {waitTime}s";
		}

		protected override void OnActivate()
		{
			_timeToWait = _waitTime + Random.Range(-_randomOffset, _randomOffset);
			_timeOnStart = Time.time;
		}

		protected override State OnUpdate() 
			=> Time.time - _timeOnStart >= _timeToWait ? State.Success : State.Running;
	}
}
