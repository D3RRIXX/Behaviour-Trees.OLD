using Derrixx.BehaviourTrees.Runtime.BlackboardScripts.BlackboardProperties;
using Derrixx.BehaviourTrees.Runtime.Nodes;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class TestAI : MonoBehaviour
{
	[SerializeField] private BehaviourTree _behaviourTree;

	private void Awake()
	{
		_behaviourTree = _behaviourTree.Clone();
		_behaviourTree.Blackboard.FindProperty<ObjectBlackboardProperty>("Agent").Value = GetComponent<NavMeshAgent>();
	}

	private void Update()
	{
		_behaviourTree.Update();
	}
}
