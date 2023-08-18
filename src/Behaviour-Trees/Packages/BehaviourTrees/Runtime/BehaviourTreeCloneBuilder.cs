using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Derrixx.BehaviourTrees.Nodes;
using Derrixx.BehaviourTrees.Nodes.Decorators;
using Object = UnityEngine.Object;

namespace Derrixx.BehaviourTrees
{
	internal class BehaviourTreeCloneBuilder
	{
		private readonly BehaviourTree _originalTree;
		private readonly BehaviourTreeRunner _runner;
		private Blackboard _blackboard;
		private Blackboard _customBlackboard;

		private bool _cloneBlackboard = true;
		
		private Action<Blackboard> _processBlackboardCloning;
		private Action<Node> _traverseNodeAction;

		public BehaviourTreeCloneBuilder(BehaviourTree originalTree, BehaviourTreeRunner runner)
		{
			_originalTree = originalTree;
			_runner = runner;
			_blackboard = originalTree.Blackboard;
		}

		public BehaviourTreeCloneBuilder WithCustomBlackboard(Blackboard blackboard)
		{
			_customBlackboard = blackboard;
			_cloneBlackboard = false;
			
			return this;
		}

		public BehaviourTreeCloneBuilder WithBlackboardCloningAction(Action<Blackboard> processBlackboardCloning)
		{
			_processBlackboardCloning = processBlackboardCloning;
			return this;
		}

		public BehaviourTreeCloneBuilder WithTraverseNodeAction(Action<Node> traverseNodeAction)
		{
			_traverseNodeAction = traverseNodeAction;
			return this;
		}

		public BehaviourTree Build()
		{
			BehaviourTree tree = Object.Instantiate(_originalTree);
			tree.name = $"{_originalTree.name} (Runtime)";
			
			tree.RootNode = (RootNode)tree.RootNode.Clone(_runner);
			tree.Nodes = new List<Node>();

			if (_blackboard != null)
			{
				tree.Blackboard = _cloneBlackboard ? _blackboard.Clone() : _customBlackboard;
				_processBlackboardCloning?.Invoke(tree.Blackboard);
			}
			
			TraverseNodes(tree.RootNode, node =>
			{
				tree.Nodes.Add(node);
#if UNITY_EDITOR
				node.BehaviourTree = tree;
#endif
				_traverseNodeAction?.Invoke(node);
				ReassignBlackboardPropertyReferences(node, tree.Blackboard);
				node.OnCreate();
			});
			
			return tree;
		}
		
		private static void TraverseNodes(Node node, Action<Node> visitor)
		{
			if (!node)
				return;
			
			visitor.Invoke(node);
			foreach (Node child in node.GetChildren())
			{
				TraverseNodes(child, visitor);
			}
		}
		
		private static void ReassignBlackboardPropertyReferences(Node node, Blackboard blackboard)
		{
			IEnumerable<FieldInfo> fieldInfos = node.GetType()
				.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(x => x.FieldType.IsSubclassOf(typeof(BlackboardProperty)));

			foreach (FieldInfo fieldInfo in fieldInfos)
			{
				var blackboardProperty = (BlackboardProperty)fieldInfo.GetValue(node);
				fieldInfo.SetValue(node, blackboard.FindProperty(blackboardProperty.Key));
			}
		}
	}
}
