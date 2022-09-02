using UnityEngine;
using UnityEngine.AI;

namespace Derrixx.BehaviourTrees.Runtime.PreMadeLeaves
{
    public sealed class GoToPosition : Leaf
    {
        private readonly NavMeshAgent _agent;
        private readonly Vector3 _destination;
        private readonly float _stoppingDistance;
        
        public GoToPosition(NavMeshAgent agent, Vector3 destination, float stoppingDistance) : base($"Go to {destination}")
        {
            _agent = agent;
            _destination = destination;
            _stoppingDistance = stoppingDistance;
        }
        
        public GoToPosition(NavMeshAgent agent, Transform destination) : base($"Go to {destination.name}")
        {
            _agent = agent;
            _destination = destination.position;
        }

        public override NodeState Execute(IBlackboard blackboard)
        {
	        if (!_agent.SetDestination(_destination))
		        return NodeState.Failure;

	        float distance = Vector3.Distance(_agent.transform.position, _destination);
	        return distance <= _stoppingDistance ? NodeState.Success : NodeState.Running;
        }
    }
}