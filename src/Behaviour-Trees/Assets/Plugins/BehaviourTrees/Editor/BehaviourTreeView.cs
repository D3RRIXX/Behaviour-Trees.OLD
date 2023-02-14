using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Derrixx.BehaviourTrees.Editor.EditorClassExtensions;
using Derrixx.BehaviourTrees.Editor.ViewScripts;
using Derrixx.BehaviourTrees.Runtime.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using Node = Derrixx.BehaviourTrees.Runtime.Nodes.Node;

namespace Derrixx.BehaviourTrees.Editor
{
	public class BehaviourTreeView : GraphView
	{
		public new class UxmlFactory : UxmlFactory<BehaviourTreeView, UxmlTraits> { }

		private BehaviourTree _tree;

		public Action<NodeView> OnNodeSelected; 

		public BehaviourTreeView()
		{
			Insert(0, new GridBackground());

			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new ContentZoomer());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());

			StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/BehaviourTrees/Editor/BehaviourTreeEditor.uss");
			styleSheets.Add(styleSheet);

			Undo.undoRedoPerformed += OnUndoRedo;
		}

		private void OnUndoRedo()
		{
			PopulateView(_tree);
			AssetDatabase.Refresh();
		}

		public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
		{
			SetupCreateNodeActions(evt);
		}

		private void SetupCreateNodeActions(ContextualMenuPopulateEvent evt)
		{
			void AppendCreateSubclassNodesActions(Type baseType, Func<Type, bool> additionalCondition = null)
			{
				const string pattern = @"([A-Z])\w+(?=Node)";
				const string actionName = "Create Node/{0}/{1}";

				Regex regex = new Regex(pattern);
				
				TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom(baseType);
				foreach (Type type in types.Where(type => !type.IsAbstract))
				{
					if (additionalCondition?.Invoke(type) == false)
						continue;
					
					evt.menu.AppendAction(string.Format(actionName, regex.Match(baseType.Name).Value, regex.Match(type.Name).Value), _ => CreateNode(type));
				}
			}
			
			AppendCreateSubclassNodesActions(typeof(ActionNode));
			AppendCreateSubclassNodesActions(typeof(DecoratorNode), x => x != typeof(RootNode));
			AppendCreateSubclassNodesActions(typeof(CompositeNode));
		}

		internal void PopulateView(BehaviourTree tree)
		{
			_tree = tree;

			graphViewChanged -= OnGraphViewChanged;
			DeleteElements(graphElements);
			graphViewChanged += OnGraphViewChanged;

			if (tree.RootNode == null)
			{
				tree.RootNode = tree.CreateNode<RootNode>();
				EditorUtility.SetDirty(tree);
				AssetDatabase.SaveAssets();
			}

			foreach (Node node in _tree.Nodes)
			{
				CreateNodeView(node);
			}

			foreach (Node node in _tree.Nodes)
			foreach (Node child in node.GetChildren())
			{
				NodeView parentView = FindNodeView(node);
				NodeView childView = FindNodeView(child);

				Edge edge = parentView.Output.ConnectTo(childView.Input);
				AddElement(edge);
			}
			
			UpdateNodesActiveState();
		}

		private NodeView FindNodeView(Node node) => GetNodeByGuid(node.Guid) as NodeView;

		public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
		{
			return ports.ToList()
				.Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node && endPort.portType == startPort.portType)
				.ToList();
		}

		private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
		{
			RemoveElementsFromGraph(graphViewChange);
			CreateEdges(graphViewChange);
			UpdateNodesActiveState();
			
			return graphViewChange;
		}

		private static void CreateEdges(GraphViewChange graphViewChange)
		{
			if (graphViewChange.edgesToCreate == null)
				return;
			
			foreach (Edge edge in graphViewChange.edgesToCreate)
			{
				NodeView parentView = (NodeView)edge.output.node;
				NodeView childView = (NodeView)edge.input.node;
				parentView.Node.AddChild(childView.Node);
			}
		}

		private void RemoveElementsFromGraph(GraphViewChange graphViewChange)
		{
			if (graphViewChange.elementsToRemove == null)
				return;
			
			foreach (GraphElement graphElement in graphViewChange.elementsToRemove)
			{
				switch (graphElement)
				{
					case NodeView nodeView:
						_tree.DeleteNode(nodeView.Node);
						break;
					case Edge edge:
						NodeView parentView = (NodeView)edge.output.node;
						NodeView childView = (NodeView)edge.input.node;
						parentView.Node.RemoveChild(childView.Node);
						break;
				}
			}
		}

		private void UpdateNodesActiveState()
		{
			const string className = StyleClassNames.INACTIVE_NODE;

			void SetNodeInactive(NodeView nodeView)
			{
				if (!nodeView.ClassListContains(className))
					nodeView.AddToClassList(className);
			}

			void SetNodeActive(NodeView nodeView)
			{
				if (nodeView.ClassListContains(className))
					nodeView.RemoveFromClassList(className);
			}

			foreach (NodeView nodeView in _tree.Nodes.Select(FindNodeView))
			{
				bool isConnected = _tree.RootNode.IsConnectedWith(nodeView.Node);
				if (isConnected)
				{
					SetNodeActive(nodeView);
				}
				else
				{
					SetNodeInactive(nodeView);
				}
			}
		}

		private void CreateNode(Type type)
		{
			Node node = _tree.CreateNode(type);
			CreateNodeView(node);
			UpdateNodesActiveState();
		}

		private void CreateNodeView(Node node)
		{
			NodeView nodeView = new NodeView(node)
			{
				OnNodeSelected = OnNodeSelected,
			};
			
			nodeView.AddToClassList(StyleClassNames.INACTIVE_NODE);
			AddElement(nodeView);
		}
	}
}