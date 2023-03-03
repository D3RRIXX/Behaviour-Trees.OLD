using Derrixx.BehaviourTrees.Runtime;
using Derrixx.BehaviourTrees.Runtime.Nodes;
using UnityEngine;
using UnityEngine.AI;

namespace DefaultNamespace
{
	public class SetRandomDestinationNode : ActionNode
	{
		[SerializeField] private float _radius;
		[SerializeField] private Vector3BlackboardProperty _destination;
		[SerializeField] private ObjectBlackboardProperty _agentProperty;

		protected override State OnUpdate()
		{
			Vector3 randomDirection = Random.insideUnitSphere * _radius;
			randomDirection.y = 0f;
			
			if (NavMesh.SamplePosition((_agentProperty.Value as NavMeshAgent).transform.position + randomDirection, out NavMeshHit hit, _radius, NavMesh.AllAreas))
			{
				_destination.Value = hit.position;
				return State.Success;
			}
			
			return State.Failure;
		}
	}
}