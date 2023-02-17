using System;
using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.BlackboardScripts.BlackboardProperties
{
	public abstract class BlackboardProperty : ScriptableObject
	{
		public enum ValueType
		{
			Boolean,
			Int,
			Float,
			String,
			Vector2,
			Vector2Int,
			Vector3,
			Object
		}
		
		[SerializeField] private string _key;
		
		public string Key => _key;
		
		public abstract ValueType GetValueType { get; }

		private void OnValidate()
		{
			name = Key;
		}

		public static BlackboardProperty Create(string key, ValueType valueType)
		{
			Type propertyType = valueType switch
			{
				ValueType.Boolean => typeof(BoolBlackboardProperty),
				ValueType.Int => typeof(IntBlackboardProperty),
				ValueType.Float => typeof(FloatBlackboardProperty),
				ValueType.String => typeof(StringBlackboardProperty),
				ValueType.Vector2 => typeof(Vector2BlackboardProperty),
				ValueType.Vector2Int => typeof(Vector2IntBlackboardProperty),
				ValueType.Vector3 => typeof(Vector3BlackboardProperty),
				ValueType.Object => typeof(ObjectBlackboardProperty),
				_ => throw new ArgumentOutOfRangeException(nameof(valueType), valueType, null)
			};

			BlackboardProperty property = (BlackboardProperty)CreateInstance(propertyType);
			property._key = key;
			
			return property;
		}
	}

	public abstract class BlackboardProperty<T> : BlackboardProperty
	{
		[SerializeField] private T _value;
		
		public T Value
		{
			get => _value;
			set => _value = value;
		}
	}
}