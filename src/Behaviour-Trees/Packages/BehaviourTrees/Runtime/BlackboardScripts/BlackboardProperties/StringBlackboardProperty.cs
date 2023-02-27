namespace Derrixx.BehaviourTrees.Runtime
{
	public class StringBlackboardProperty : BlackboardProperty<string>
	{
		public override ValueType GetValueType => ValueType.String;
	}
}