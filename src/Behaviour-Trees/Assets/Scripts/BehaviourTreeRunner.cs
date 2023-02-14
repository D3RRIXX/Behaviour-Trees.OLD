using DefaultNamespace;
using Derrixx.BehaviourTrees.Runtime.Nodes;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    [SerializeField] private BehaviourTree _tree;

    private void Awake()
    {
        _tree = _tree.Clone();
    }

    private void Update()
    {
        _tree.Update();
    }
}