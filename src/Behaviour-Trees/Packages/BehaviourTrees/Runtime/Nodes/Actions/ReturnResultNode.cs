using UnityEngine;

namespace Derrixx.BehaviourTrees
{
	public class ReturnResultNode : ActionNode
	{
		[SerializeField] private State _output;

		public override string GetDescription() => $"Return {_output}";

		protected override State OnUpdate() => _output;
	}
}
