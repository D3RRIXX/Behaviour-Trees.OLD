using Derrixx.BehaviourTrees;
using Derrixx.BehaviourTrees.Nodes;
using UnityEngine;

namespace DefaultNamespace
{
	public class RotateTowardsNode : ActionNode
	{
		[SerializeField] private ObjectBlackboardProperty _target;

		public override string GetDescription() => $"Rotate towards {(_target != null ? _target.Key : "Target")}";

		protected override State OnUpdate() => State.Success;
	}
}