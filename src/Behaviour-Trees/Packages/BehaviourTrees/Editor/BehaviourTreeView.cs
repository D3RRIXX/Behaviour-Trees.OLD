using System;
using System.Collections.Generic;
using System.Linq;
using Derrixx.BehaviourTrees.Editor.ViewScripts;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

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

			StyleSheet styleSheet = Resources.Load<StyleSheet>("BehaviourTreeEditorStyle");
			styleSheets.Add(styleSheet);
			
			nodeCreationRequest = ctx => SearchWindow.Open(new SearchWindowContext(ctx.screenMousePosition), SetupSearchWindow());

			Undo.undoRedoPerformed += OnUndoRedo;
		}

		private void OnUndoRedo()
		{
			PopulateView(_tree);
			AssetDatabase.Refresh();
		}
		
		private NodeSearchWindow SetupSearchWindow()
		{
			var searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
			searchWindow.Initialize(null, this);
			return searchWindow;
		}

		public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
		{
			evt.menu.AppendAction("Create Node",
				action => nodeCreationRequest.Invoke(new NodeCreationContext { target = this, screenMousePosition = action.eventInfo.mousePosition }));
		}

		public void UpdateNodeStates()
		{
			foreach (NodeView nodeView in _tree.Select(FindNodeView))
			{
				nodeView.UpdateState();
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

			foreach (Node node in _tree)
			{
				CreateNodeView(node);
			}

			foreach (Node node in _tree)
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
				NodeView parentView = (NodeView)edge.output.node;
				NodeView childView = (NodeView)edge.input.node;
				parentView.Node.AddChild(childView.Node);
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

			List<NodeView> activeViews = new List<NodeView>();
			foreach (NodeView nodeView in _tree.Select(FindNodeView))
			{
				bool isConnected = _tree.RootNode.IsConnectedWith(nodeView.Node);
				if (isConnected)
				{
					activeViews.Add(nodeView);
					SetNodeActive(nodeView);
				}
				else
				{
					SetNodeInactive(nodeView);
				}
			}

			_tree.UpdateExecutionOrder();
			foreach (NodeView nodeView in activeViews)
			{
				nodeView.UpdateExecutionOrderLabel(nodeView.Node.ExecutionOrder);
			}
		}

		internal void CreateNode(Type type, Vector2 mousePosition)
		{
			Node node = _tree.CreateNode(type);
			
			NodeView nodeView = CreateNodeView(node);
			nodeView.SetPosition(new Rect(mousePosition, Vector2.zero));
			nodeView.UpdatePresenterPosition();
			nodeView.Select(this, false);

			UpdateNodesActiveState();
		}

		private NodeView CreateNodeView(Node node)
		{
			NodeView nodeView = new NodeView(node)
			{
				OnNodeSelected = OnNodeSelected,
			};

			nodeView.AddToClassList(StyleClassNames.INACTIVE_NODE);
			AddElement(nodeView);

			return nodeView;
		}
	}
}
