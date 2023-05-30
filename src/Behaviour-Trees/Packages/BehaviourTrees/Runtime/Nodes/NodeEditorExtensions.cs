using System.Collections.Generic;
using Derrixx.BehaviourTrees.Nodes.Actions;
using Derrixx.BehaviourTrees.Nodes.Composites;
using Derrixx.BehaviourTrees.Nodes.Decorators;
using UnityEditor;

namespace Derrixx.BehaviourTrees.Nodes
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
