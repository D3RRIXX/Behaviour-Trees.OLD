using Derrixx.BehaviourTrees.Runtime;
using Derrixx.BehaviourTrees.Runtime.Behaviours;
using Derrixx.BehaviourTrees.Runtime.Pure;
using UnityEngine;

namespace DefaultNamespace
{
	public class TestNode : NodeBehaviour
	{
		[SerializeField] private NodeState returnValue;
		[SerializeField] private bool printSomethingOnNodeEnter;
		
		private TestBlackboard _blackboard;

		public override void OnNodeEnter()
		{
			if (printSomethingOnNodeEnter)
				Debug.Log($"Entered node {name}", this);
		}

		public override void InjectBlackboard(IBlackboard blackboard)
		{
			_blackboard = (TestBlackboard)blackboard;
		}

		public override NodeState Execute()
		{
			return returnValue;
		}
	}
}