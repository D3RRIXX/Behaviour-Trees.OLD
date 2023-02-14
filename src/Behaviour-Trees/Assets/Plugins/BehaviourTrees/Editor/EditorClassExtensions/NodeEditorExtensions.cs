using System.Collections.Generic;
using Derrixx.BehaviourTrees.Runtime.Nodes;

namespace Derrixx.BehaviourTrees.Editor
{
	internal static class NodeEditorExtensions
	{
		public static void AddChild(this Node parent, Node child)
		{
			switch (parent)
			{
				case DecoratorNode decorator:
					decorator.Child = child;
					break;
				case CompositeNode composite:
					composite.Children.Add(child);
					break;
			}
		}

		public static void RemoveChild(this Node parent, Node child)
		{
			switch (parent)
			{
				case DecoratorNode decorator:
					decorator.Child = null;
					break;
				case CompositeNode composite:
					composite.Children.Remove(child);
					break;
			}
		}

		public static List<Node> GetChildren(this Node node) => node switch
		{
			DecoratorNode decorator when decorator.Child != null => new List<Node> { decorator.Child },
			CompositeNode composite => composite.Children,
			_ => new List<Node>()
		};
	}
}