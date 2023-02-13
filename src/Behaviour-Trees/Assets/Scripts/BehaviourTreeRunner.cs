using DefaultNamespace;
using Derrixx.BehaviourTrees.Runtime.Nodes;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    private BehaviourTree _tree;

    private void Awake()
    {
        _tree = ScriptableObject.CreateInstance<BehaviourTree>();
        var log = ScriptableObject.CreateInstance<LogNode>();
        log.Message = "POW! HAHA";

        var repeat = ScriptableObject.CreateInstance<RepeatNode>();
        repeat.Child = log;
        
        _tree.RootNode = repeat;
    }

    private void Update()
    {
        _tree.Update();
    }
}