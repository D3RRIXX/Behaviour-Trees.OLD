using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
	[CreateAssetMenu(fileName = "New Behaviour Tree", menuName = "Derrixx/Behaviour Trees/Behaviour Tree", order = -100)]
	public class BehaviourTree : ScriptableObject
	{
		[SerializeField, HideInInspector] private List<Node> nodes = new List<Node>();
		
		public Node RootNode;
		public Node.State TreeState = Node.State.Running;

		public IReadOnlyList<Node> Nodes => nodes;

		public Node.State Update()
		{
			if (RootNode.CurrentState == Node.State.Running)
				TreeState = RootNode.Update();
				
			return TreeState;
		}

		public Node CreateNode<T>() where T : Node => CreateNode(typeof(T));

		public Node CreateNode(Type type)
		{
			Assert.IsTrue(type.IsSubclassOf(typeof(Node)));
			
			Node node = CreateInstance(type) as Node;
			node.hideFlags = HideFlags.HideInHierarchy;
			node.name = type.Name;
			node.Guid = GUID.Generate().ToString();
			
			nodes.Add(node);
			
			AssetDatabase.AddObjectToAsset(node, this);
			AssetDatabase.SaveAssets();

			return node;
		}

		public void DeleteNode(Node node)
		{
			nodes.Remove(node);
			
			AssetDatabase.RemoveObjectFromAsset(node);
			AssetDatabase.SaveAssets();
		}
	}
}