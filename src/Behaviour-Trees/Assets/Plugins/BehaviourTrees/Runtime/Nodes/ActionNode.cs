namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
	public abstract class ActionNode : Node
	{
		public sealed override bool IsConnectedWith(Node other) => base.IsConnectedWith(other);
		internal sealed override void SetExecutionOrder(ref int order) => base.SetExecutionOrder(ref order);
	}
}