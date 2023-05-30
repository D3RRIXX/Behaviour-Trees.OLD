namespace Derrixx.BehaviourTrees
{
	public class StringBlackboardProperty : BlackboardProperty<string>
	{
		public override ValueType GetValueType => ValueType.String;
	}
}
