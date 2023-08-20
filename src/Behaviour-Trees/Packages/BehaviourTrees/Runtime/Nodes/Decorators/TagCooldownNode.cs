using Derrixx.BehaviourTrees.TimerSystem;
using UnityEngine;

namespace Derrixx.BehaviourTrees.Nodes.Decorators
{
	public class TagCooldownNode : ConditionalNode
	{
		[SerializeField] private string _cooldownTag;
		[SerializeField] private bool _setCooldownOnDeactivate;
		[SerializeField] private float _cooldownDuration;
		[SerializeField] private bool _addToExistingDuration;
		
		private int _cooldownHash;

		public override string GetDescription()
		{
			return $"Run if {_cooldownTag} isn't active";
		}

		//TODO: Add runtime editing support
		public override void OnCreate()
		{
			_cooldownHash = Animator.StringToHash(_cooldownTag);
		}

		protected override void OnDeactivate()
		{
			if (_setCooldownOnDeactivate)
				Runner.CooldownHandler.SetCooldown(_cooldownHash, new SetCooldownParams { AddToExistingDuration = _addToExistingDuration, Duration = _cooldownDuration });
		}

		protected override bool ConditionValue() => !Runner.CooldownHandler.IsCooldownActive(_cooldownHash);
	}
}