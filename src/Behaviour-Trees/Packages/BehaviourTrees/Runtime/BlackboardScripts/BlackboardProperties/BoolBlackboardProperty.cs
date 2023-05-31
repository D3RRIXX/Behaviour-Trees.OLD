namespace Derrixx.BehaviourTrees
{
	public sealed class BoolBlackboardProperty : BlackboardProperty<bool>
	{
		public override ValueType GetValueType => ValueType.Boolean;
	}
}
