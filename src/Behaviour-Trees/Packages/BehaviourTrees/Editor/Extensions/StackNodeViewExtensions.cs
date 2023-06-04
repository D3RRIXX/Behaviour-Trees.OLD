using Derrixx.BehaviourTrees.Editor.ViewScripts;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Derrixx.BehaviourTrees.Editor.Extensions
{
	public static class StackNodeViewExtensions
	{
		public static void SetIsConnectedToRoot(this StackNodeView stackNodeView, bool value)
		{
			const string className = StyleClassNames.INACTIVE_NODE;

			void SetNodeInactive(VisualElement nodeView)
			{
				if (!nodeView.ClassListContains(className))
					nodeView.AddToClassList(className);
			}

			void SetNodeActive(VisualElement nodeView)
			{
				if (nodeView.ClassListContains(className))
					nodeView.RemoveFromClassList(className);
			}
			
			foreach (NodeView nodeView in stackNodeView.NodeViews)
			{
				if (value)
					SetNodeActive(nodeView);
				else
					SetNodeInactive(nodeView);
			}
		}

		public static Port InstantiatePort(this StackNodeView stackNodeView, Direction direction, Port.Capacity capacity, FlexDirection flexDirection)
		{
			Port port = stackNodeView.InstantiatePort(Orientation.Vertical, direction, capacity, typeof(Node));
			
			port.style.flexDirection = flexDirection;
			port.portName = string.Empty;

			return port;
		}

		public static void AddNodeView(this StackNodeView stackNodeView, NodeView nodeView)
		{
			stackNodeView.InsertNodeView(nodeView, index: 0);
		}
	}
}