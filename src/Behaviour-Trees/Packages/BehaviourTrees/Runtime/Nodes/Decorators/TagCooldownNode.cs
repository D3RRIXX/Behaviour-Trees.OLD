using UnityEngine;

namespace Derrixx.BehaviourTrees.Nodes.Decorators
{
	public class TagCooldownNode : ConditionalNode
	{
		[SerializeField] private string _cooldownTag;
		
		private int _cooldownHash;

		public override string GetDescription()
		{
			return $"Run if cooldown {_cooldownTag} isn't active";
		}

		public override void OnCreate()
		{
			_cooldownHash = Animator.StringToHash(_cooldownTag);
		}

		protected override bool ConditionValue() => !Runner.CooldownHandler.IsCooldownActive(_cooldownHash);
	}
}