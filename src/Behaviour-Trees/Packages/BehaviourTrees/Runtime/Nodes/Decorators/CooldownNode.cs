using System.Collections;
using System.Globalization;
using UnityEngine;

namespace Derrixx.BehaviourTrees.Nodes
{
	public class CooldownNode : DecoratorNode
	{
		[SerializeField] private float _cooldownDuration;
		
		private bool _isCooldownActive;

		public override string GetDescription() => $"Cooldown: lock for {_cooldownDuration.ToString(CultureInfo.CurrentCulture)}s after execution";

		protected override State OnUpdate()
		{
			if (_isCooldownActive)
				return State.Failure;
			
			var childState = Child.Update();
			if (childState == State.Running)
				return State.Running;

			Runner.StartCoroutine(SetCooldown());
			return childState;
		}

		private IEnumerator SetCooldown()
		{
			_isCooldownActive = true;
			yield return new WaitForSeconds(_cooldownDuration);
			_isCooldownActive = false;
		}
	}
}