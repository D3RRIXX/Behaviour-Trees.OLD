using Derrixx.BehaviourTrees.Runtime.Nodes;
using UnityEditor.Experimental.GraphView;
using Node = Derrixx.BehaviourTrees.Runtime.Nodes.Node;

namespace Derrixx.BehaviourTrees.Editor.ViewScripts
{
	public class StackNodeView : StackNode
	{
		public StackNodeView(Node node)
		{
			Node = node;
			viewDataKey = node.Guid;
			
			SetupCapabilities(node);
			
			AddElement(new NodeView(node));
		}
		
		private void SetupCapabilities(Node node)
		{
			if (node is RootNode)
				capabilities &= ~(Capabilities.Deletable | Capabilities.Copiable);

			capabilities |= Capabilities.Stackable;
		}

		public sealed override string title
		{
			get => base.title;
			set => base.title = value;
		}

		public Node Node { get; }
	}
}