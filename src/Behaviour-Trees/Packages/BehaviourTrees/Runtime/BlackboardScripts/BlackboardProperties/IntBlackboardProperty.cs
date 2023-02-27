namespace Derrixx.BehaviourTrees.Runtime
{
	public sealed class IntBlackboardProperty : BlackboardProperty<int>
	{
		public override ValueType GetValueType => ValueType.Int;
	}
}