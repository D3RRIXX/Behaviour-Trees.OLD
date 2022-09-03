Behaviour Trees
===
A small project of mine that I started to get a better grasp at the Behaviour Tree system and that will hopefully be of use to someone.

Key features:
* Free for commercial use
* No weird visual editors and custom scriptable objects
* You're can propose your own "pre made" nodes

## Table of contents:
- [Installing](#installing)
- [How to use](#how-to-use)

Installing
---
Simply copy `https://github.com/D3RRIXX/Behaviour-Trees.git?path=src` into Unity Package Manager. **Images TBA**

How to use
---
Create a behaviour class, for example `EnemyBehaviour`. Inside of it you'll nest nodes inside each other to create your very own behaviour tree!

Each node implements `Execute(IBlackboard)` method, which returns either of the three evaluation states: `Running, Failure` or `Success`.

There are 3 types of nodes in a behaviour tree:
* [Leaves](#leaves)
* [Decorators](#decorators)
* [Composites](#composites)

### Leaves
Leaves are the simplest node type: they can't have any child nodes, but they're the ones that execute actions. For example, take a look at the `GoToPosition` node:

```csharp
public sealed class GoToPosition : Leaf
{
    private readonly NavMeshAgent _agent;
    private readonly Vector3 _destination;
    
    public GoToPosition(NavMeshAgent agent, Vector3 destination) : base($"Go to {destination}")
    {
        _agent = agent;
        _destination = destination;
    }

    public override NodeState Execute(IBlackboard blackboard)
    {
        _agent.SetDestination(_destination);
    
        return NodeState.Success;
    }
}
```

It takes `NavMeshAgent` and `Vector3` (or `Transform`) in its constructor and when it comes to evaluation it sets the agent's destination to what was assigned and returns `NodeState.Success`. You can *easily* create **your own** leaf nodes by creating a new class and making it inherit from `Leaf`.

### Decorators
A decorator is a node that doesn't have any functionality of its own, but instead it can cancel or prevent execution of other nodes and can have only one child node.
Right now there are only two types of decorators: `Inverter` and `ExecuteNTimes`.

`Inverter` (obviously) inverts the result of its child node's execution.

```csharp
public override NodeState Execute(IBlackboard blackboard)
{
    NodeState output = Child.Execute(blackboard) switch
	{
		NodeState.Failure => NodeState.Success,
		NodeState.Success => NodeState.Failure,
		_ => NodeState.Running
	};

	return output;
}
```

`ExecuteNTimes` takes a single child node and the amount of times it will execute.

```csharp
public override NodeState Execute(IBlackboard blackboard)
{
    if (Counter >= _limit)
        return NodeState.Success;

    Counter++;
    return _child.Execute(blackboard);
}
```

You can also call `Reset()` to set the `Counter` value to 0.

### Composites
A composite is a node that doesn't have any functionality of its own, but instead can have multiple child nodes and process them in its own way. The two main types of composites (and the only ones implemented for the moment) are `Sequence` and `Selector`.

`Sequence` executes every of its child nodes **from left to right**. If the nodes that's being executed returns either `Failure` or `Running`, the Sequence won't execute further until that node returns `Success`.

`Selector` works similarly to `Sequence` with the difference that it keeps executing its children until either of them returns `Success`.