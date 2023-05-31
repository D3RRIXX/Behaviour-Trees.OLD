using Derrixx.BehaviourTrees;
using Derrixx.BehaviourTrees.Nodes.Actions;
using UnityEngine;
using UnityEngine.AI;

namespace DefaultNamespace
{
	public class MoveToNode : ActionNode
	{
		[SerializeField] private Vector3BlackboardProperty _destination;
		[SerializeField] private ObjectBlackboardProperty _agentProperty;
		[SerializeField] private float _stoppingDistance = 2f;
		
		private NavMeshAgent _agent;

		public override void OnCreate()
		{
			_agent = _agentProperty.Value as NavMeshAgent;
		}

		protected override void OnActivate()
		{
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