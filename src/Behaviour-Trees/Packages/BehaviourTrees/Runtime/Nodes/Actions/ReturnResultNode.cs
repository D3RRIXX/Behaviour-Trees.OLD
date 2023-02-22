using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
	public class ReturnResultNode : ActionNode
	{
		[SerializeField] private State _output;
		
		protected override State OnEvaluate(BehaviourTreeRunner runner) => _output;
	}
}