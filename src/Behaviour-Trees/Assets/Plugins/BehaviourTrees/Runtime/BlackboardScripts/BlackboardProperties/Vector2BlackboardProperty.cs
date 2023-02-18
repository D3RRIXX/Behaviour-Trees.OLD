using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.BlackboardScripts.BlackboardProperties
{
	public class Vector2BlackboardProperty : BlackboardProperty<Vector2>
	{
		public override ValueType GetValueType => ValueType.Vector2;
	}
}