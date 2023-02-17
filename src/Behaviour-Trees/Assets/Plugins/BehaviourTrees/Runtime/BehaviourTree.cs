using System;
using System.Collections;
using System.Collections.Generic;
using Derrixx.BehaviourTrees.Runtime.BlackboardScripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
	[CreateAssetMenu(fileName = "New Behaviour Tree", menuName = "Derrixx/Behaviour Trees/Behaviour Tree")]
	public sealed class BehaviourTree : ScriptableObject, IEnumerable<Node>
	{
		[SerializeField, HideInInspector] private List<Node> nodes = new List<Node>();
		[SerializeField, HideInInspector] private RootNode rootNode;
		[SerializeField] private Blackboard blackboard;
		
		public Node.State TreeState = Node.State.Running;

		public RootNode RootNode
		{
			get => rootNode;
			set => rootNode = value;
		}
		
		public Blackboard Blackboard => blackboard;

		public Node.State Update()
		{
			if (RootNode.CurrentState == Node.State.Running)
				TreeState = RootNode.Update();
				
			return TreeState;
		}

		public BehaviourTree Clone()
		{
			BehaviourTree tree = Instantiate(this);
			tree.name = $"{name} (Runtime)";
			
			tree.RootNode = (RootNode)tree.RootNode.Clone();
			tree.nodes = new List<Node>();
			TraverseNodes(tree.RootNode, node =>
			{
				tree.nodes.Add(node);
				node.BehaviourTree = tree;
			});
			
			return tree;
		}
		
		public T CreateNode<T>() where T : Node => CreateNode(typeof(T)) as T;

		public void UpdateExecutionOrder()
		{
			int executionOrder = 0;
			RootNode.SetExecutionOrder(ref executionOrder);
		}

		public Node CreateNode(Type type)
		{
			Assert.IsTrue(type.IsSubclassOf(typeof(Node)));
			
			Node node = CreateInstance(type) as Node;
			node.hideFlags = HideFlags.HideInHierarchy;
			node.name = Node.GetNodeName(node.GetType());
			node.Guid = GUID.Generate().ToString();
			node.BehaviourTree = this;
			
			Undo.RecordObject(this, "Behaviour Tree (Create Node)");
			nodes.Add(node);

			if (!Application.isPlaying)
			{
				AssetDatabase.AddObjectToAsset(node, this);
			}
			
			Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (Create Node)");
			
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();

			return node;
		}

		public void DeleteNode(Node node)
		{
			Undo.RecordObject(this, "Behaviour Tree (Delete Node)");
			nodes.Remove(node);
			
			Undo.DestroyObjectImmediate(node);
			AssetDatabase.SaveAssets();
		}

		private void TraverseNodes(Node node, Action<Node> visitor)
		{
			if (!node)
				return;
			
			visitor.Invoke(node);
			foreach (Node child in node.GetChildren())
			{
				TraverseNodes(child, visitor);
			}
		}

		public IEnumerator<Node> GetEnumerator() => nodes.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}