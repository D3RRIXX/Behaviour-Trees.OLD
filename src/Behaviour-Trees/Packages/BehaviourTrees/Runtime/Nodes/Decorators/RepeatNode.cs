using UnityEngine;

namespace Derrixx.BehaviourTrees.Nodes
{
	public class RepeatNode : DecoratorNode
	{
		[SerializeField, Min(0)] private int timesToRepeat = 1;
		[SerializeField] private bool repeatInfinitely;

		private int _repeatsPassed;
		
		public override string GetDescription()
		{
			if (repeatInfinitely)
				return "Repeat infinitely";
			
			string times = timesToRepeat != 1 ? "times" : "time";
			return $"Repeat {timesToRepeat} {times}";
		}

		protected override State OnUpdate()
		{
			if (repeatInfinitely || _repeatsPassed++ < timesToRepeat)
			{
				Child.Update();
				return State.Running;
			}

			return State.Success;
		}
	}
}
