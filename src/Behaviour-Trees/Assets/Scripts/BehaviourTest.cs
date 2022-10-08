using Derrixx.BehaviourTrees.Runtime;
using Derrixx.BehaviourTrees.Runtime.Behaviours;
using UnityEngine;

public class BehaviourTest : MonoBehaviour
{
    [SerializeField] private NodeBehaviour rootNode;

    private void Awake()
    {
        rootNode.InjectBlackboard(new TestBlackboard());        
    }

    private void Update()
    {
        rootNode.Execute();
    }
}

public class TestBlackboard : IBlackboard
{
    
}