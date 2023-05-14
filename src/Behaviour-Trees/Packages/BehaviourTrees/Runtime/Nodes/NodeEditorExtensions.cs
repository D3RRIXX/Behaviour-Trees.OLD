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
			bool changeHappened = parent is not ActionNode;
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

		public static void InsertNodeBeforeChild(this Node parent, Node child, Node toInsert)
		{
			if (parent is ActionNode)
				throw new ArgumentException($"'{parent.name}' is an Action Node, and thus has no children", nameof(parent));

			if (!parent.GetChildren().Contains(child))
				throw new ArgumentException($"'{child.name}' isn't a direct child of '{parent.name}'", nameof(child));

			switch (parent)
			{
				case DecoratorNode decorator:
					decorator.Child = toInsert;
					toInsert.AddChild(child);
					break;
				case CompositeNode composite:
					int index = composite.Children.IndexOf(child);
					composite.Children.Remove(child);
					composite.Children.Insert(index, toInsert);
					
					toInsert.AddChild(child);
					break;
			}
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