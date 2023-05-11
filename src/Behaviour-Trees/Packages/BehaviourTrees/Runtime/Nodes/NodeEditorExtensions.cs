using System;
using System.Collections.Generic;
using UnityEditor;

namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
	public static class NodeEditorExtensions
	{
#if UNITY_EDITOR
		public static void AddChild(this Node parent, Node child)
		{
			bool changeHappened = !(parent is ActionNode);
			if (!changeHappened)
				return;
			
			Undo.RecordObject(parent, "Behaviour Tree (Add Child)");
			switch (parent)
			{
				case DecoratorNode decorator:
					decorator.Child = child;
					break;
				case CompositeNode composite:
					composite.Children.Add(child);
					break;
			}
			
			EditorUtility.SetDirty(parent);
		}

		public static void RemoveChild(this Node parent, Node child)
		{
			bool changeHappened = !(parent is ActionNode);
			if (!changeHappened)
				return;
			
			Undo.RecordObject(parent, "Behaviour Tree (Remove Child)");
			switch (parent)
			{
				case DecoratorNode decorator:
					decorator.Child = null;
					break;
				case CompositeNode composite:
					composite.Children.Remove(child);
					break;
			}
			
			EditorUtility.SetDirty(parent);
		}

		public static void Traverse(this Node node, Action<Node> visitor)
		{
			if (node is null)
				return;
			
			visitor.Invoke(node);
			foreach (Node child in node.GetChildren())
			{
				child.Traverse(visitor);
			}
		}
#endif

		public static List<Node> GetChildren(this Node node) => node switch
		{
			null => new List<Node>(),
			DecoratorNode decorator when decorator.Child != null => new List<Node> { decorator.Child },
			CompositeNode composite => composite.Children,
			_ => new List<Node>()
		};
	}
}