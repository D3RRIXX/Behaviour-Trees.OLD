using UnityEngine;

namespace Derrixx.BehaviourTrees.Nodes
{
	public class BlackboardConditionNode : ConditionalNode
	{
		[SerializeField] private BoolBlackboardProperty _condition;
		[SerializeField] private bool _shouldBeTrue = true;

		public override string GetDescription() 
			=> $"Blackboard: {(_condition ? _condition.Key : "Condition")} is {(_shouldBeTrue ? "TRUE" : "FALSE")}";

		protected override bool ConditionValue()
		{
			return _condition.Value == _shouldBeTrue;
		}
	}
}
