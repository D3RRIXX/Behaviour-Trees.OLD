using UnityEngine;

namespace Derrixx.BehaviourTrees
{
	public class Vector2IntBlackboardProperty : BlackboardProperty<Vector2Int>
	{
		public override ValueType GetValueType => ValueType.Vector2Int;
	}
}
