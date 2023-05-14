using System;
using System.Collections.Generic;
using System.Linq;
using Derrixx.BehaviourTrees.Editor.ViewScripts;
using Derrixx.BehaviourTrees.Runtime;
using Derrixx.BehaviourTrees.Runtime.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
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

			var styleSheet = Resources.Load<StyleSheet>("BehaviourTreeEditorStyle");
			styleSheets.Add(styleSheet);

			nodeCreationRequest = ctx => SearchWindow.Open(new SearchWindowContext(ctx.screenMousePosition), SetupSearchWindow());

			Undo.undoRedoPerformed += OnUndoRedo;
		}

		private NodeSearchWindow SetupSearchWindow()
		{
			var searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
			searchWindow.Initialize(null, this);
			return searchWindow;
		}

		private void OnUndoRedo()
		{
			PopulateView(_tree);
			AssetDatabase.Refresh();
		}

		public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
		{
			if (evt.target is StackNodeView)
				return;
			
			evt.menu.AppendAction("Create Node", action =>
			{
				nodeCreationRequest.Invoke(new NodeCreationContext { target = this, screenMousePosition = action.eventInfo.mousePosition });
			});
		}

		public void UpdateNodeStates()
		{
			foreach (StackNodeView nodeView in _tree.Select(FindNodeStack))
			{
				// nodeView.UpdateState();
			}
		}

		internal void PopulateView(BehaviourTree tree)
		{
			_tree = tree;
			
			graphViewChanged -= OnGraphViewChanged;
			DeleteElements(graphElements.ToList());
			graphViewChanged += OnGraphViewChanged;

			if (tree.RootNode == null)
			{
				tree.RootNode = tree.CreateNode<RootNode>();
				EditorUtility.SetDirty(tree);
				AssetDatabase.SaveAssets();
			}

			var stackNodeViews = new HashSet<StackNodeView>();
			foreach (IEnumerable<Node> nodeGroup in GetNodeGroups())
			{
				StackNodeView nodeStack = CreateNodeStack(nodeGroup);
				stackNodeViews.Add(nodeStack);
			}

			foreach (StackNodeView parentStack in stackNodeViews.Where(x => x.Output != null))
			{
				List<Node> children = parentStack.LastNode.GetChildren();
				IEnumerable<StackNodeView> childStacks = children.Select(x => stackNodeViews.First(stack => stack.FirstNode == x));

				foreach (StackNodeView childStack in childStacks)
				{
					Edge edge = parentStack.Output.ConnectTo(childStack.Input);
					AddElement(edge);
				}
			}

			// foreach (Node node in _tree)
			// foreach (Node child in FindNodeStack(node).LastNode.GetChildren())
			// {
			// 	StackNodeView parentStack = FindNodeStack(node);
			// 	StackNodeView childStack = FindNodeStack(child);
			//
			// 	if (parentStack.Output == null)
			// 		continue;
			// 	
			// 	Edge edge = parentStack.Output.ConnectTo(childStack.Input);
			// 	AddElement(edge);
			// }

			UpdateNodesActiveState();
		}

		private IEnumerable<IEnumerable<Node>> GetNodeGroups()
		{
			void CreateStacksFromChildren(Node parent, List<IEnumerable<Node>> list)
			{
				foreach (Node child in parent.GetChildren())
				{
					var subGroup = new List<Node>();
					FillGroup(child);

					list.Add(subGroup);

					void FillGroup(Node node)
					{
						subGroup.Add(node);
						switch (node)
						{
							case DecoratorNode decoratorNode:
								FillGroup(decoratorNode.Child);
								break;
							case CompositeNode compositeNode:
								CreateStacksFromChildren(compositeNode, list);
								break;
						}
					}
				}
			}

			var nodeGroups = new List<IEnumerable<Node>> { new[] { _tree.RootNode } };
			CreateStacksFromChildren(_tree.RootNode, nodeGroups);

			return nodeGroups;
		}

		private NodeView FindNodeView(Node node) => (NodeView)GetNodeByGuid(node.Guid);
		private StackNodeView FindNodeStack(Node node) => FindNodeView(node).Stack;

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

			if (graphViewChange.movedElements != null)
				SortChildNodesByXPos();

			UpdateNodesActiveState();

			return graphViewChange;
		}

		private void SortChildNodesByXPos()
		{
			foreach (NodeView nodeView in _tree.Select(FindNodeView))
			{
				nodeView.SortChildren();
			}
		}

		private void CreateEdges(GraphViewChange graphViewChange)
		{
			if (graphViewChange.edgesToCreate == null)
				return;

			foreach (Edge edge in graphViewChange.edgesToCreate)
			{
				var parentView = (StackNodeView)edge.output.node;
				var childView = (StackNodeView)edge.input.node;
				parentView.LastNode.AddChild(childView.FirstNode);
			}

			SortChildNodesByXPos();
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
						var parentView = (StackNodeView)edge.output.node;
						var childView = (StackNodeView)edge.input.node;
						parentView.LastNode.RemoveChild(childView.FirstNode);
						break;
				}
			}
		}

		public void UpdateNodesActiveState()
		{
			// var activeViews = new List<NodeView>();
			foreach (var stackNodeView in nodes.OfType<StackNodeView>())
			{
				bool isConnected = _tree.RootNode.IsConnectedWith(stackNodeView.FirstNode);
				stackNodeView.SetIsConnectedToRoot(isConnected);
			}

			_tree.UpdateExecutionOrder();
			// foreach (NodeView nodeView in activeViews)
			// {
			// 	nodeView.UpdateExecutionOrderLabel(nodeView.Node.ExecutionOrder);
			// }
		}

		public void CreateNode(Type type, Vector2 mousePosition)
		{
			Node node = _tree.CreateNode(type);
			
			var nodeView = CreateNodeStack(node);
			
			nodeView.SetPosition(new Rect(mousePosition, Vector2.zero));
			nodeView.UpdatePresenterPosition();
			nodeView.Select(this, false);

			UpdateNodesActiveState();
		}

		private StackNodeView CreateNodeStack(Node node) => CreateNodeStack(new[] { node });

		private StackNodeView CreateNodeStack(IEnumerable<Node> nodes)
		{
			NodeView CreateNodeView(Node node)
			{
				var nodeView = new NodeView(node) { OnNodeSelected = OnNodeSelected };
				nodeView.AddToClassList(StyleClassNames.INACTIVE_NODE);
				return nodeView;
			}

			var stackNodeView = new StackNodeView(nodes.Select(CreateNodeView).ToList(), this);
			AddElement(stackNodeView);
			
			return stackNodeView;
		}
	}
}