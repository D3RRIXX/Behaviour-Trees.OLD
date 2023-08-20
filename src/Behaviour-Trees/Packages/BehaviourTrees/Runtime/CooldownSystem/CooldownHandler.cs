using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Derrixx.BehaviourTrees.TimerSystem
{
	internal class CooldownHandler
	{
		private readonly BehaviourTreeRunner _runner;
		private readonly Dictionary<int, Cooldown> _cooldownMap = new();

		public CooldownHandler(BehaviourTreeRunner runner)
		{
			_runner = runner;
		}
		
		public void SetCooldown(string cooldownTag, SetCooldownParams cooldownParams)
		{
			SetCooldown(Animator.StringToHash(cooldownTag), cooldownParams);
		}

		public bool IsCooldownActive(string cooldownTag) => IsCooldownActive(Animator.StringToHash(cooldownTag));
		public bool IsCooldownActive(int cooldownHash) => _cooldownMap.ContainsKey(cooldownHash);

		public void SetCooldown(int timerHash, SetCooldownParams cooldownParams)
		{
			if (_cooldownMap.TryGetValue(timerHash, out Cooldown cooldown))
			{
				_runner.StopCoroutine(cooldown.Coroutine);
				
				if (cooldownParams.AddToExistingDuration)
					cooldown.RemainingDuration += cooldownParams.Duration;
				else
					cooldown.RemainingDuration = cooldownParams.Duration;
			}
			else
			{
				cooldown = new Cooldown { RemainingDuration = cooldownParams.Duration };
				cooldown.Coroutine = _runner.StartCoroutine(CooldownRoutine(timerHash, cooldown));
				
				_cooldownMap.Add(timerHash, cooldown);
			}
		}

		private IEnumerator CooldownRoutine(int key, Cooldown cooldown)
		{
			while (cooldown.RemainingDuration > 0)
			{
				cooldown.RemainingDuration -= Time.deltaTime;
				yield return null;
			}

			_cooldownMap.Remove(key);
		}
	}
}
