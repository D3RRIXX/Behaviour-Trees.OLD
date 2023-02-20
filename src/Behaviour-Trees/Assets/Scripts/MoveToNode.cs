using Derrixx.BehaviourTrees.Runtime.BlackboardScripts.BlackboardProperties;
using Derrixx.BehaviourTrees.Runtime.Nodes;
using UnityEngine;
using UnityEngine.AI;

namespace DefaultNamespace
{
	public class MoveToNode : ActionNode
	{
		[SerializeField] private ObjectBlackboardProperty _navMeshAgent;
		[SerializeField] private Vector3BlackboardProperty _destination;
		[SerializeField] private float _stoppingDistance = 2f;
		
		private NavMeshAgent _agent;
		
		protected override void OnStart()
		{
			if (_agent == null)
				_agent = (NavMeshAgent)_navMeshAgent.Value;
			
			_agent.SetDestination(_destination.Value);
		}

		protected override State OnUpdate()
		{
			float distance = Vector3.Distance(_agent.transform.position, _destination.Value);
			float stoppingDistance = _stoppingDistance;
			return distance <= stoppingDistance ? State.Success : State.Running;
		}
	}
}