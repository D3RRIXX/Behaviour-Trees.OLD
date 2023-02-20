using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.BlackboardScripts.BlackboardProperties
{
	public sealed class BoolBlackboardProperty : BlackboardProperty<bool>
	{
		public override ValueType GetValueType => ValueType.Boolean;
	}
}