using Derrixx.BehaviourTrees.Runtime;
using Derrixx.BehaviourTrees.Runtime.Behaviours;
using Derrixx.BehaviourTrees.Runtime.Pure;
using UnityEngine;

namespace DefaultNamespace
{
	public class TestNode : NodeBehaviour
	{
		private TestBlackboard _blackboard;

		public override void InjectBlackboard(IBlackboard blackboard)
		{
			_blackboard = (TestBlackboard)blackboard;
		}

		public override NodeState Execute()
		{
			Debug.Log(name + _blackboard);
			return NodeState.Success;
		}
	}
}