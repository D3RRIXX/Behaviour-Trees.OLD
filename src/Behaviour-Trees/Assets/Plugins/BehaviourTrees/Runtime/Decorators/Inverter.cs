namespace Derrixx.BehaviourTrees.Runtime.Decorators
{
	public class Inverter : Decorator
	{
		internal Inverter(Node child) : base("Inverter", child)
		{
		}

		public override NodeState Execute(IBlackboard blackboard)
		{
			NodeState output = Child.Execute(blackboard) switch
			{
				NodeState.Failure => NodeState.Success,
				NodeState.Success => NodeState.Failure,
				_ => NodeState.Running
			};
			
			return output;
		}
	}
}