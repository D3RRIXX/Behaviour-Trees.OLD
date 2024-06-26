using System;

namespace Derrixx.BehaviourTrees
{
	public class InverterNode : DecoratorNode
	{
		public override string GetDescription()
		{
			string childName = Child == null ? "Child" : Child.name;
			return $"Invert {childName}'s Output";
		}

		protected override State OnUpdate() => Child.Update() switch
		{
			State.Running => State.Running,
			State.Failure => State.Success,
			State.Success => State.Failure,
			_ => throw new ArgumentOutOfRangeException()
		};
	}
}
