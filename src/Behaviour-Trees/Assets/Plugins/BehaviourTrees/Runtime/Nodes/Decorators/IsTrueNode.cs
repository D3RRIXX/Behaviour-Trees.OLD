using Derrixx.BehaviourTrees.Runtime.BlackboardScripts.BlackboardProperties;
using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.Nodes.Decorators
{
	public class IsTrueNode : DecoratorNode
	{
		[SerializeField] private BoolBlackboardProperty _condition;

		public override string GetDescription() => _condition == null ? base.GetDescription() : $"Blackboard: {_condition.Key} is Set";

		protected override State OnUpdate() => _condition.Value ? Child.Update() : State.Failure;
	}
}