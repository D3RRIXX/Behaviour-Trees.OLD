using System.Globalization;
using Derrixx.BehaviourTrees.TimerSystem;
using UnityEngine;

namespace Derrixx.BehaviourTrees.Nodes.Actions
{
	public class SetTagCooldownNode : ActionNode
	{
		[SerializeField] private string _cooldownTag;
		[SerializeField] private float _cooldownDuration;
		[SerializeField] private bool _addToExistingDuration;
		
		private int _cooldownHash;

		public override string GetDescription()
		{
			var duration = $"{_cooldownDuration.ToString(CultureInfo.CurrentCulture)}s";
			return _addToExistingDuration ? $"Add {duration} to {_cooldownTag}" : $"Set {_cooldownTag} to {duration}";
		}

		//TODO: Add runtime editing support
		public override void OnCreate()
		{
			_cooldownHash = Animator.StringToHash(_cooldownTag);
		}

		protected override State OnUpdate()
		{
			Runner.CooldownHandler.SetCooldown(_cooldownHash, new SetCooldownParams { Duration = _cooldownDuration, AddToExistingDuration = _addToExistingDuration });
			return State.Success;
		}
	}
}
