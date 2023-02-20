using Derrixx.BehaviourTrees.Runtime.BlackboardScripts.BlackboardProperties;
using Derrixx.BehaviourTrees.Runtime.Nodes;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(BehaviourTreeRunner))]
public class TestAI : MonoBehaviour
{
	private void Awake()
	{
		BehaviourTreeRunner runner = GetComponent<BehaviourTreeRunner>();
		runner.BehaviourTree.Blackboard.FindProperty<ObjectBlackboardProperty>("Agent").Value = GetComponent<NavMeshAgent>();
	}
}
