using System;
using System.Collections.Generic;
using System.Linq;

namespace Derrixx.BehaviourTrees
{
	[AttributeUsage(AttributeTargets.Field)]
	public class AllowedPropertyTypesAttribute : Attribute
	{
		private readonly BlackboardProperty.ValueType[] _allowedTypes;

		public AllowedPropertyTypesAttribute(params BlackboardProperty.ValueType[] allowedTypes)
		{
			_allowedTypes = allowedTypes;
		}

		public IEnumerable<Type> GetBlackboardPropertyTypes() => new HashSet<Type>(_allowedTypes.Select(GetTypeFromPropertyType));

		private static Type GetTypeFromPropertyType(BlackboardProperty.ValueType valueType)
		{
			return valueType switch
			{
				BlackboardProperty.ValueType.Boolean => typeof(BoolBlackboardProperty),
				BlackboardProperty.ValueType.Int => typeof(IntBlackboardProperty),
				BlackboardProperty.ValueType.Float => typeof(FloatBlackboardProperty),
				BlackboardProperty.ValueType.String => typeof(StringBlackboardProperty),
				BlackboardProperty.ValueType.Vector2 => typeof(Vector2BlackboardProperty),
				BlackboardProperty.ValueType.Vector2Int => typeof(Vector2IntBlackboardProperty),
				BlackboardProperty.ValueType.Vector3 => typeof(Vector3BlackboardProperty),
				BlackboardProperty.ValueType.Object => typeof(ObjectBlackboardProperty),
				_ => throw new ArgumentOutOfRangeException(nameof(valueType), valueType, null)
			};
		}
	}
}
