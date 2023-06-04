using System;
using System.Linq;
using Derrixx.BehaviourTrees.Nodes;
using UnityEditor;

namespace Derrixx.BehaviourTrees.Editor.Extensions
{
	public static class NodeEditorExtensions
	{
		public static void AddChild(this Node parent, Node child)
		{
			bool changeHappened = parent is not ActionNode;
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

		public static Node GetParent(this Node node, BehaviourTree behaviourTree)
		{
			return behaviourTree.Nodes.FirstOrDefault(n => n.GetChildren().Contains(node));
		}

		public static void InsertNodeBeforeChild(this Node parent, Node child, Node toInsert)
		{
			if (parent is ActionNode)
				throw new ArgumentException($"'{parent.name}' is an Action Node, and thus has no children", nameof(parent));

			if (parent == child)
				throw new ArgumentException("Passed parent itself as a child", nameof(child));

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

		public static void ReplaceChild(this Node parent, Node currentChild, Node newChild)
		{
			if (parent is ActionNode)
				throw new ArgumentException($"'{parent.name}' is an Action Node, and thus has no children", nameof(parent));

			if (!parent.GetChildren().Contains(currentChild))
				throw new ArgumentException($"'{currentChild.name}' isn't a direct child of '{parent.name}'", nameof(currentChild));

			switch (parent)
			{
				case DecoratorNode decoratorNode:
				{
					decoratorNode.Child = newChild;
					break;
				}
				case CompositeNode compositeNode:
				{
					int index = compositeNode.Children.IndexOf(currentChild);
					compositeNode.Children.Remove(currentChild);
					compositeNode.Children.Insert(index, newChild);
					break;
				}
			}
		}
	}
}
