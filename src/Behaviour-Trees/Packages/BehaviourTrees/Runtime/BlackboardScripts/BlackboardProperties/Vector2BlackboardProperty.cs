using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime
{
	public class Vector2BlackboardProperty : BlackboardProperty<Vector2>
	{
		public override ValueType GetValueType => ValueType.Vector2;
	}
}