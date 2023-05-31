using UnityEngine;

namespace Derrixx.BehaviourTrees
{
	public class ObjectBlackboardProperty : BlackboardProperty<Object>
	{
		public override ValueType GetValueType => ValueType.Object;
	}
}
