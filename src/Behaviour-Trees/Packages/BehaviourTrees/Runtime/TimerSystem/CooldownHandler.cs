using System.Collections.Generic;
using UnityEngine;

namespace Derrixx.BehaviourTrees.TimerSystem
{
	internal class CooldownHandler
	{
		private readonly Dictionary<int, Cooldown> _cooldownMap = new();
		private readonly List<int> _toDispose = new();

		public void SetCooldown(string cooldownTag, SetCooldownParams cooldownParams)
		{
			SetCooldown(Animator.StringToHash(cooldownTag), cooldownParams);
		}

		public void Update()
		{
			foreach ((int key, Cooldown cooldown) in _cooldownMap)
			{
				cooldown.Update();
				if (cooldown.RemainingDuration <= 0f)
					_toDispose.Add(key);
			}

			DisposeExpiredCooldowns();
		}

		private void DisposeExpiredCooldowns()
		{
			foreach (int key in _toDispose)
			{
				_cooldownMap.Remove(key);
			}

			_cooldownMap.Clear();
		}

		public bool IsCooldownActive(string cooldownTag) => !_cooldownMap.TryGetValue(Animator.StringToHash(cooldownTag), out Cooldown cooldown) || cooldown.RemainingDuration <= 0;

		private void SetCooldown(int timerHash, SetCooldownParams cooldownParams)
		{
			if (_cooldownMap.TryGetValue(timerHash, out Cooldown cooldown))
			{
				if (cooldownParams.AddToExistingDuration)
					cooldown.RemainingDuration += cooldownParams.Duration;
				else
					cooldown.RemainingDuration = cooldownParams.Duration;
			}
			else
			{
				cooldown = new Cooldown { RemainingDuration = cooldownParams.Duration };
			}

			_cooldownMap[timerHash] = cooldown;
		}
	}
}
