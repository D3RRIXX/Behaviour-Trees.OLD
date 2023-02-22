using System;

namespace Derrixx.BehaviourTrees.Runtime.Nodes.Decorators
{
	public class InverterNode : DecoratorNode
	{
		public override string GetDescription()
		{
			string childName = Child == null ? "Child" : Child.name;
			return $"Inverts {childName}'s Output";
		}

		protected override State OnEvaluate(BehaviourTreeRunner runner) => Child.Evaluate(runner) switch
		{
			State.Running => State.Running,
			State.Failure => State.Success,
			State.Success => State.Failure,
			_ => throw new ArgumentOutOfRangeException()
		};
	}
}