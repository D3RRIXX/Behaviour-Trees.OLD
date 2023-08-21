using Derrixx.BehaviourTrees;
using UnityEngine;
using UnityEngine.AI;

public class SetRandomDestinationNode : ActionNode
{
	[SerializeField] private float _radius;
	[SerializeField] private Vector3BlackboardProperty _destination;

	protected override State OnUpdate()
	{
		Vector3 randomDirection = Random.insideUnitSphere * _radius;
		randomDirection.y = 0f;

		if (NavMesh.SamplePosition(Runner.transform.position + randomDirection, out NavMeshHit hit, _radius, NavMesh.AllAreas))
		{
			_destination.Value = hit.position;
			return State.Success;
		}

		return State.Failure;
	}
}