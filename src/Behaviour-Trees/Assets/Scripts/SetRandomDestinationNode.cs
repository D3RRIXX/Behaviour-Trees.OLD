using Derrixx.BehaviourTrees.Runtime.BlackboardScripts.BlackboardProperties;
using Derrixx.BehaviourTrees.Runtime.Nodes;
using UnityEngine;
using UnityEngine.AI;

namespace DefaultNamespace
{
	public class SetRandomDestinationNode : ActionNode
	{
		[SerializeField] private float _radius;
		[SerializeField] private Vector3BlackboardProperty _destination;
		[SerializeField] private ObjectBlackboardProperty _navMeshAgent;
		
		protected override State OnUpdate()
		{
			NavMeshAgent agent = (NavMeshAgent)_navMeshAgent.Value;
			Vector3 randomDirection = Random.insideUnitSphere * _radius;
			randomDirection.y = 0f;
			
			if (NavMesh.SamplePosition(agent.transform.position + randomDirection, out NavMeshHit hit, _radius, NavMesh.AllAreas))
			{
				_destination.Value = hit.position;
				return State.Success;
			}
			
			return State.Running;
		}
	}
}