using UnityEngine;
using UnityEngine.AI;

namespace Derrixx.BehaviourTrees.PreMadeLeaves
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

        public override NodeState Evaluate(IBlackboard blackboard)
        {
            _agent.SetDestination(_destination);
            
            return NodeState.Success;
        }
    }
}