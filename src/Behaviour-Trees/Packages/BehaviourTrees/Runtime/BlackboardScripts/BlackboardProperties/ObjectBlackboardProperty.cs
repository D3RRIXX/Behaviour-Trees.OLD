using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime
{
	public class ObjectBlackboardProperty : BlackboardProperty<Object>
	{
		public override ValueType GetValueType => ValueType.Object;
	}
}