using UnityEngine;

namespace Derrixx.BehaviourTrees
{
	public class RepeatNode : DecoratorNode
	{
		[SerializeField, Min(0)] private int timesToRepeat = 1;
		[SerializeField] private bool repeatInfinitely;
		[SerializeField] private bool _resetOnDeactivate;

		private int _repeatsPassed;
		
		public override string GetDescription()
		{
			if (repeatInfinitely)
				return "Repeat infinitely";
			
			string times = timesToRepeat != 1 ? "times" : "time";
			return $"Repeat {timesToRepeat.ToString()} {times}";
		}

		protected override void OnDeactivate()
		{
			if (_resetOnDeactivate)
				_repeatsPassed = 0;
		}

		protected override State OnUpdate()
		{
			if (repeatInfinitely || _repeatsPassed < timesToRepeat)
			{
				State childState = Child.Update();
				if (childState == State.Success)
				{
					_repeatsPassed++;
					return State.Running;
				}
                
				return childState;
			}

			return State.Success;
		}
	}
}
