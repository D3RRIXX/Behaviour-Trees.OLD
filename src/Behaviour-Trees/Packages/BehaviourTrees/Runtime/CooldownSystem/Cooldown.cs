using UnityEngine;

namespace Derrixx.BehaviourTrees.TimerSystem
{
	public class Cooldown
	{
		public float RemainingDuration { get; set; }
		public Coroutine Coroutine { get; set; }
	}
}
