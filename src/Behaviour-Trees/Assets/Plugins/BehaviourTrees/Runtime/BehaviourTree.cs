using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
	[CreateAssetMenu(fileName = "New Behaviour Tree", menuName = "Derrixx/Behaviour Trees/Behaviour Tree", order = -100)]
	public sealed class BehaviourTree : ScriptableObject
	{
		[SerializeField, HideInInspector] private List<Node> nodes = new List<Node>();
		[SerializeField, HideInInspector] private RootNode rootNode;
		
		public Node.State TreeState = Node.State.Running;

		public IEnumerable<Node> Nodes => nodes;
		
		public RootNode RootNode
		{
			get => rootNode;
			set => rootNode = value;
		}

		public Node.State Update()
		{
			if (RootNode.CurrentState == Node.State.Running)
				TreeState = RootNode.Update();
				
			return TreeState;
		}

		public BehaviourTree Clone()
		{
			BehaviourTree tree = Instantiate(this);
			tree.RootNode = (RootNode)RootNode.Clone();
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
			
			Undo.RecordObject(this, "Behaviour Tree (Create Node)");
			nodes.Add(node);
			
			AssetDatabase.AddObjectToAsset(node, this);
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
	}
}