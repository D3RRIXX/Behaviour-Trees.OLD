using UnityEngine;

namespace Derrixx.BehaviourTrees.TimerSystem
{
	public struct Cooldown
	{
		public float RemainingDuration { get; set; }

		public void Update()
		{
			RemainingDuration -= Time.deltaTime;
		}
	}
}
