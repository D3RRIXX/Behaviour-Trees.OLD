using System;

namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
	public class InverterNode : DecoratorNode
	{
		public override string GetDescription()
		{
			string childName = Child == null ? "Child" : Child.name;
			return $"Inverts {childName}'s Output";
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