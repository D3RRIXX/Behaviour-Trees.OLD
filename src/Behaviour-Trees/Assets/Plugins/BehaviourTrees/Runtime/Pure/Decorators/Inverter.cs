namespace Derrixx.BehaviourTrees.Runtime.Pure.Decorators
{
	public class Inverter : Decorator
	{
		internal Inverter(INode child) : base("Inverter", child)
		{
		}

		public override NodeState Execute()
		{
			NodeState output = Child.Execute() switch
			{
				NodeState.Failure => NodeState.Success,
				NodeState.Success => NodeState.Failure,
				_ => NodeState.Running
			};
			
			return output;
		}
	}
}