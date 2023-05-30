namespace Derrixx.BehaviourTrees.Nodes.Actions
{
	/// <summary>
	/// Node type that executes an action rather than operating on its children. Inherit from this class to create your own action nodes
	/// </summary>
	public abstract class ActionNode : Node
	{
		public sealed override bool IsConnectedWith(Node other) => base.IsConnectedWith(other);
		internal sealed override void SetExecutionOrder(ref int order) => base.SetExecutionOrder(ref order);
	}
}
