using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
	public sealed class RootNode : DecoratorNode
	{
		protected override State OnUpdate() => Child.Update();
	}
}