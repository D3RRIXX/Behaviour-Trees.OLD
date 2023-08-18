using UnityEngine;

namespace Derrixx.BehaviourTrees.TimerSystem
{
	public struct Cooldown
	{
		public float RemainingDuration { get; set; }
		public Coroutine Coroutine { get; set; }
	}
}
