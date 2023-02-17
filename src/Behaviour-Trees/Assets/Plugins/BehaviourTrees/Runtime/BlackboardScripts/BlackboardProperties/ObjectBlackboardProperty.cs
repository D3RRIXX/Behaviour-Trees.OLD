using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.BlackboardScripts.BlackboardProperties
{
	public class ObjectBlackboardProperty : BlackboardProperty<Object>
	{
		public override ValueType GetValueType => ValueType.Object;
	}
}