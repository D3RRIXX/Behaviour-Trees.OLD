using System;
using UnityEngine;

namespace Derrixx.BehaviourTrees
{
	/// <summary>
	/// A property which is used to store data between the nodes.
	/// <seealso cref="Blackboard"/>
	/// </summary>
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
		[SerializeField] private bool _isExposed;
		
		[Tooltip("Synchronize this property between all blackboard instances?")]
		[SerializeField] private bool _sync;
		
		private BlackboardProperty _runtimeClone;

		public string Key => _key;
		
		public abstract ValueType GetValueType { get; }
		internal bool InstanceSynced => _sync;

		internal bool IsExposed => _isExposed;

		private void OnValidate()
		{
			name = Key;
		}

		internal BlackboardProperty Clone()
		{
			if (!_sync)
				return Instantiate(this);
			
			if (_runtimeClone == null)
				_runtimeClone = Instantiate(this);

			return _runtimeClone;
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

		public virtual bool Equals(BlackboardProperty other) => GetValueType == other.GetValueType;
	}

	public abstract class BlackboardProperty<T> : BlackboardProperty
	{
		[SerializeField] private T _value;
		
		public T Value
		{
			get => _value;
			set => _value = value;
		}

		public override bool Equals(BlackboardProperty other)
		{
			return base.Equals(other) && ((BlackboardProperty<T>)other).Value.Equals(Value);
		}
	}
}
