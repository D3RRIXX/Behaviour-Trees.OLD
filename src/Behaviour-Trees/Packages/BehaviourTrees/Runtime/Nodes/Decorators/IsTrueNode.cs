using Derrixx.BehaviourTrees.Runtime.BlackboardScripts.BlackboardProperties;
using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
	public class IsTrueNode : ConditionalNode
	{
		[SerializeField] private BoolBlackboardProperty _condition;

		public override string GetDescription() => $"Blackboard: {(_condition ? _condition.Key : "Condition" )} is Set";

		protected override bool ConditionValue()
		{
			return _condition.Value;
		}
	}
}