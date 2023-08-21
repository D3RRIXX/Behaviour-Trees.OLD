using System;
using UnityEngine;

namespace Derrixx.BehaviourTrees
{
	public class AreEqualNode : ConditionalNode
	{
		private enum Operator
		{
			Equal,
			NotEqual
		}
		
		[SerializeField] private BlackboardProperty _propertyA;
		[SerializeField] private BlackboardProperty _propertyB;
		[SerializeField] private Operator _operator;

		public override string GetDescription()
		{
			string nameA = !_propertyA ? "Property A" : _propertyA.Key;
			string nameB = !_propertyB ? "Property B" : _propertyB.Key;
			return $"Is {nameA}'s Value {GetOperatorName} to {nameB}'s?";
		}

		private string GetOperatorName => _operator switch
		{
			Operator.Equal => "EQUAL",
			Operator.NotEqual => "NOT EQUAL",
			_ => throw new ArgumentOutOfRangeException()
		};

		protected override bool ConditionValue()
		{
			bool equals = _propertyA.Equals(_propertyB);
			return _operator switch
			{
				Operator.Equal => equals,
				Operator.NotEqual => !equals,
				_ => throw new ArgumentOutOfRangeException()
			};
		}
	}
}
