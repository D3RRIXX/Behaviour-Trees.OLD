using Derrixx.BehaviourTrees.Runtime.BlackboardScripts.BlackboardProperties;
using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.Nodes.Decorators
{
	public class IsTrueNode : ConditionalNode
	{
		[SerializeField] private BoolBlackboardProperty _condition;

		public override string GetDescription() => _condition == null ? base.GetDescription() : $"Blackboard: {_condition.Key} is Set";

		protected override bool ConditionValue() => _condition.Value;
	}
}