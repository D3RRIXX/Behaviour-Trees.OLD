using Derrixx.BehaviourTrees.Runtime.Nodes;
using UnityEngine;

namespace DefaultNamespace
{
	public class RepeatNode : DecoratorNode
	{
		[SerializeField, Min(0)] private int timesToRepeat = 1;

		public override string GetDescription()
		{
			string times = timesToRepeat != 1 ? "times" : "time";
			return $"Repeat {timesToRepeat} {times}";
		}

		protected override State OnUpdate()
		{
			Child.Update();
			return State.Running;
		}
	}
}