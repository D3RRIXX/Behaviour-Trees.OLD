# Behaviour Trees
A small project of mine that I started to get a better grasp at the Behaviour Tree system and that will hopefully be of use to someone.
My main goal was to replicate the Behaviour Tree workflow from Unreal Engine in the Unity engine.

Key features:
* Free for commercial use
* Visual editor for clarity
* Smart blackboard implementation

## Table of contents:
- [Introduction](#introduction)
- [Installation](#installation)
- [Quickstart Guide](#quickstart-guide)
- [Blackboards & Blackboard Properties](#blackboards--blackboard-properties)
- [Zenject Integration](#zenject-integration)

## Introduction
### What are Behaviour Trees?

If you are reading this, you are most likely familiar with [Finite-State Machines (FSM)](https://gamedevelopment.tutsplus.com/tutorials/finite-state-machines-theory-and-implementation--gamedev-11867). 
They are a great way to design a simple game AI, but quickly get cumbersome to handle when you need something more complex than 2-3 states. This is where Behaviour Trees kick in.

![An example Behaviour Tree showing a simple enemy AI](Documentation/Images/introduction_tree.png "An image of a Behaviour Tree")
*An example Behaviour Tree showing a simple enemy AI*

### Behaviour Trees 101
Behaviour Trees are executed from top-to-bottom and from left-to-right, starting from the **Root** node. Every node has its execution order displayed in the upper-right corner.

![](Documentation/Images/execution_order.png)

There are 3 node types (excluding the Root node). The purple node on the top is a [Decorator](Documentation/Decorators.md) (or a [Conditional](Documentation/Decorators.md#conditionals), to be precise). It checks whether the enemy can see player (for how this is implemented, see [Blackboards & Blackboard Properties](#blackboards--blackboard-properties)) and if the condition is met, it allows the [Composite](Documentation/Composites.md) node underneath to execute. A composite node executes its child nodes one by one until it reaches the end or is forced to stop. The red child nodes of the Composite node are [Action](Documentation/Actions.md) nodes, which don't have any child nodes and simply perform an action and return a result.

## Installation
There are several options on how you can install Behaviour Trees.

### Install via Package Manager 
Copy `https://github.com/D3RRIXX/Behaviour-Trees.git?path=src/Behaviour-Trees/Packages/BehaviourTrees` and paste it into Unity Package Manager.

![Open Package Manager and select *Add package from Git URL*](Documentation/Images/package_manager.png)

### Download a .unitypackage file
Go to [Releases](https://github.com/D3RRIXX/Behaviour-Trees/releases) page, download the last stable version and import it into your Unity project.

## Quickstart Guide

### Creating a new Behaviour Tree
To start using Behaviour Trees, first create one by by going **Assets/Create/Derrixx/Behaviour Trees/Blackboard**. After you've create a Behaviour Tree asset, double-click on it and the **Behaviour Tree Editor** window will show up.

![](Documentation/Images/empty_tree.png)
*What the Behaviour Tree Editor looks like for a newly created tree*

To add new nodes, right-click on the grid to open a context menu with all available node types.

### Assigning Behaviour Tree to a GameObject
After you've created and set up your Behaviour Tree, you'll most likely want to assign it to some GameObject and try it out. To do this, select a GameObject and add a `BehaviourTreeRunnerBase` component, then assign your Behaviour Tree. If you need to customise your logic, you can create your own class that inherits from `BehaviourTreeRunner`, but don't forget to add a `RunBehaviourTree()` call in `Update()`!

![Final result](Documentation/Images/runner.png "Final result")

## Blackboards & Blackboard Properties
A Behaviour Tree uses a **Blackboard** to store and share data across all of its nodes, while a Blackboard by itself is a **ScriptableObject** that contains a list of **Blackboard Properties**.

### Creating a Blackboard
To use Blackboards, first you need to create one by going **Assets/Create/Derrixx/Behaviour Trees/Blackboard** and then assign it to your Behaviour Tree in the Editor.

![](Documentation/Images/blackboard.png)

After a Blackboard is assigned you can start adding new BlackboardProperties by clicking the **+** button and selecting the value type you want your property to hold.

BlackboardProperty supports all most commonly used data types:
* Boolean
* Int
* Float
* String
* Vector2
* Vector2Int
* Vector3
* UnityEngine.Object

### Using Blackboard Properties in your custom nodes
To reference Blackboard Properties in your nodes, add a serialized field to your node class. If you need a property with a specific value, use the according property type (e.g. `BoolBlackboardProperty`) or simply `BlackboardProperty` otherwise.

```cs
[SerializeField] private BoolBlackboardProperty _condition;
```

After a field is created, open your select your node in the Editor and you will see a dropdown menu with all Blackboard Properties of the type you've specified.

![](Documentation/Images/dropdown.png)

### Exposing Blackboard Properties & overriding default values
If you need to set a default value for every instance of your Runner GameObject (e.g. to assign a component reference or override a parameter), you have to check the *Is Exposed* toggle on your Blackboard Property (note: works only with not instanced synced Properties).

![](Documentation/Images/exposed_properties.png)

After that, go to your BehaviourTreeRunner object and you'll see a field appear, waiting for you to assign something.

![](Documentation/Images/assigned_exposed.png)

### Instance Syncing
While using Blackboard Properties you've propbably noticed 'Instance Synced' checkbox that is available for every property type. It basically marks this property as `static` so its value is shared between all runtime instances of that Blackboard.

### Blackboard Inheritance
You can set one Blackboard as a parent of another, thus making the child inherit all properties from its parent.

## Zenject Integration
If you'd like to use [Zenject](https://github.com/modesttree/Zenject)'s `[Inject]` attribute inside your custom nodes, simply add `BTREE_ADD_ZENJECT` to your define symbols.