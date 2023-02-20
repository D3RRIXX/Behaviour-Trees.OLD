using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.BlackboardScripts.BlackboardProperties
{
	public class Vector3BlackboardProperty : BlackboardProperty<Vector3>
	{
		public override ValueType GetValueType => ValueType.Vector3;
	}
}