using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.BlackboardScripts
{
	public abstract class BlackboardProperty : ScriptableObject
	{
		public string Key;

		private void OnValidate()
		{
			name = Key;
		}
	}

	public abstract class BlackboardProperty<T> : BlackboardProperty
	{
		public T Value;
	}
}