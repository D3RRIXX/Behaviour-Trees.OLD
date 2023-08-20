using System.Globalization;
using System.Text;
using Derrixx.BehaviourTrees.TimerSystem;
using UnityEngine;

namespace Derrixx.BehaviourTrees.Nodes
{
	public class TagCooldownNode : DecoratorNode
	{
		[SerializeField] private string _cooldownTag;
		[SerializeField] private bool _setCooldownOnDeactivate;
		[SerializeField] private float _cooldownDuration;
		[SerializeField] private bool _addToExistingDuration;
		
		private int _cooldownHash;

		public override string GetDescription()
		{
			var builder = new StringBuilder($"Run if {_cooldownTag} isn't active");
			if (_setCooldownOnDeactivate)
				builder.Append($" and lock for {_cooldownDuration.ToString(CultureInfo.CurrentCulture)}s");
			
			return builder.ToString();
		}

		//TODO: Add runtime editing support
		public override void OnCreate()
		{
			_cooldownHash = Animator.StringToHash(_cooldownTag);
		}

		protected override State OnUpdate()
		{
			if (Runner.CooldownHandler.IsCooldownActive(_cooldownHash))
				return State.Failure;

			var childState = Child.Update();
			if (childState == State.Running)
				return State.Running;
			
			if (_setCooldownOnDeactivate)
				SetCooldown();

			return childState;
		}

		private void SetCooldown()
		{
			if (_setCooldownOnDeactivate)
				Runner.CooldownHandler.SetCooldown(_cooldownHash, new SetCooldownParams { AddToExistingDuration = _addToExistingDuration, Duration = _cooldownDuration });
		}
	}
}