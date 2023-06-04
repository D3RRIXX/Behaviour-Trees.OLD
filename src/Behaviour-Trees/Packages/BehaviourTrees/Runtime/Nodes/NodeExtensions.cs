using System.Collections.Generic;

namespace Derrixx.BehaviourTrees.Nodes
{
	public static class NodeExtensions
	{
		public static List<Node> GetChildren(this Node node) => node switch
		{
			null => new List<Node>(),
			DecoratorNode decorator when decorator.Child != null => new List<Node> { decorator.Child },
			CompositeNode composite => composite.Children,
			_ => new List<Node>()
		};
	}
}