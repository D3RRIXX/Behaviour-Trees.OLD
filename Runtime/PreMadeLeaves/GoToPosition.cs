using UnityEngine;
using UnityEngine.AI;

namespace Derrixx.BehaviourTrees.Runtime.PreMadeLeaves
{
    public sealed class GoToPosition : Leaf
    {
        private readonly NavMeshAgent _agent;
        private readonly Vector3 _destination;
        
        public GoToPosition(NavMeshAgent agent, Vector3 destination) : base($"Go to {destination}")
        {
            _agent = agent;
            _destination = destination;
        }
        
        public GoToPosition(NavMeshAgent agent, Transform destination) : base($"Go to {destination.name}")
        {
            _agent = agent;
            _destination = destination.position;
        }

        public override NodeState Execute(IBlackboard blackboard)
        {
            _agent.SetDestination(_destination);
            
            return NodeState.Success;
        }
    }
}