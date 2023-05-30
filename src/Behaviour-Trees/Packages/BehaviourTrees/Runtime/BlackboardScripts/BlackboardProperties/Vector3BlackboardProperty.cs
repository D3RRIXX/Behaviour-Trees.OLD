using UnityEngine;

namespace Derrixx.BehaviourTrees
{
	public class Vector3BlackboardProperty : BlackboardProperty<Vector3>
	{
		public override ValueType GetValueType => ValueType.Vector3;
	}
}
