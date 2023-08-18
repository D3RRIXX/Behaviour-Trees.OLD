using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Derrixx.BehaviourTrees
{
	/// <summary>
	/// A Behaviour Tree asset. To actually use Behaviour Trees, use <see cref="BehaviourTreeRunner"/>
	/// </summary>
	[CreateAssetMenu(fileName = "New Behaviour Tree", menuName = Utils.SO_CREATION_PATH + "Behaviour Tree")]
	public sealed class BehaviourTree : ScriptableObject
	{
		[SerializeField, HideInInspector] private List<Node> nodes = new();
		[SerializeField, HideInInspector] private RootNode rootNode;
		[SerializeField] private Blackboard blackboard;

		public List<Node> Nodes
		{
			get => nodes;
			internal set => nodes = value;
		}
		
		public RootNode RootNode
		{
			get => rootNode;
			set => rootNode = value;
		}
		
		public Blackboard Blackboard
		{
			get => blackboard;
			internal set => blackboard = value;
		}

		internal Node.State Update()
		{
			return RootNode.Update();
		}

		internal BehaviourTreeCloneBuilder Clone(BehaviourTreeRunner runner)
		{
			return new BehaviourTreeCloneBuilder(this, runner);
		}
		
#if UNITY_EDITOR
		public T CreateNode<T>() where T : Node => CreateNode(typeof(T)) as T;

		public void UpdateExecutionOrder()
		{
			int executionOrder = 0;
			RootNode.SetExecutionOrder(ref executionOrder);
		}

		public Node CreateNode(Type type)
		{
			Assert.IsTrue(type.IsSubclassOf(typeof(Node)));
			
			var node = (Node)CreateInstance(type);
			node.hideFlags = HideFlags.HideInHierarchy;
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

			return node;
		}

		public void DeleteNode(Node node)
		{
			Undo.RecordObject(this, "Behaviour Tree (Delete Node)");
			nodes.Remove(node);
			
			Undo.DestroyObjectImmediate(node);
			EditorUtility.SetDirty(this);
		}
#endif
	}
}
