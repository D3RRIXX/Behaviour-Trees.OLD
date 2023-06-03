using System;
using System.Collections.Generic;
using System.Linq;
using Derrixx.BehaviourTrees.Editor.ViewScripts;
using Derrixx.BehaviourTrees;
using Derrixx.BehaviourTrees.Editor;
using Derrixx.BehaviourTrees.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Node = Derrixx.BehaviourTrees.Nodes.Node;

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
			this.AddManipulator(new SelectionDropper());
			this.AddManipulator(new RectangleSelector());

			StyleSheet styleSheet = Resources.Load<StyleSheet>("BehaviourTreeEditorStyle");
			styleSheets.Add(styleSheet);

			nodeCreationRequest = ctx =>
			{
				NodeSearchWindow searchWindow = SetupSearchWindow();
				searchWindow.CreationTarget = ctx.target;
				
				SearchWindow.Open(new SearchWindowContext(ctx.screenMousePosition), searchWindow);
			};
			
			Undo.undoRedoPerformed += OnUndoRedo;
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

		private NodeSearchWindow SetupSearchWindow()
		{
			var searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
			searchWindow.Initialize(this);
			return searchWindow;
		}

		private void OnUndoRedo()
		{
			PopulateView(_tree);
			AssetDatabase.Refresh();
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

			HashSet<StackNodeView> stackNodeViews = SetupStackNodes();

			ConnectStackNodes(stackNodeViews);
			UpdateNodesActiveState();
		}

		private void ConnectStackNodes(HashSet<StackNodeView> stackNodeViews)
		{
			foreach (StackNodeView parentStack in stackNodeViews.Where(x => x.Output != null))
			{
				List<Node> children = parentStack.LastNode.GetChildren();
				IEnumerable<StackNodeView> childStacks = children.Select(x => stackNodeViews.FirstOrDefault(stack => stack.FirstNode == x));

				foreach (StackNodeView childStack in childStacks)
				{
					if (childStack == null)
						continue;
					
					Edge edge = parentStack.Output.ConnectTo(childStack.Input);
					AddElement(edge);
				}
			}
		}

		private HashSet<StackNodeView> SetupStackNodes()
		{
			var stackNodeViews = new HashSet<StackNodeView>();
			foreach (IEnumerable<Node> nodeGroup in GetNodeGroups())
			{
				StackNodeView nodeStack = CreateStackNode(nodeGroup.Select(x => new NodeView(x)).ToArray(), false);
				stackNodeViews.Add(nodeStack);
			}

			return stackNodeViews;
		}

		private IEnumerable<IEnumerable<Node>> GetNodeGroups()
		{
			var output = new List<HashSet<Node>>();

			foreach (Node node in _tree.Nodes)
			{
				if (output.Any(set => set.Contains(node)))
					continue;

				var hashSet = new HashSet<Node> { node };
				if (node is DecoratorNode decoratorNode)
				{
					if (node is RootNode)
					{
						AddToOutput();
						continue;
					}
					
					var current = decoratorNode;
					while (current.Child is not (null or not DecoratorNode))
					{
						current = (DecoratorNode)current.Child;
						
					}
				}

				void AddToOutput() => output.Add(hashSet);
			}
			

			return output;
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
			{
				SortChildNodesByXPos();
			}

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
			foreach (var stackNodeView in nodes.OfType<StackNodeView>())
			{
				bool isConnected = _tree.RootNode.IsConnectedWith(stackNodeView.FirstNode);
				stackNodeView.SetIsConnectedToRoot(isConnected);
			}

			_tree.UpdateExecutionOrder();
		}
		
		public NodeView CreateNode(Type type, Vector2 mousePosition)
		{
			Node node = _tree.CreateNode(type);
			Debug.Log($"Created node of type {type}");
			
			NodeView CreateNodeView()
			{
				var nodeView = new NodeView(node) { OnNodeSelected = OnNodeSelected };
				nodeView.AddToClassList(StyleClassNames.INACTIVE_NODE);
				return nodeView;
			}

			NodeView nodeView = CreateNodeView();
			nodeView.SetPosition(new Rect(mousePosition, Vector2.zero));
			nodeView.UpdatePresenterPosition();

			UpdateNodesActiveState();

			return nodeView;
		}

		public StackNodeView CreateStackNode(NodeView nodeView, bool select) => CreateStackNode(new[] { nodeView }, select);
		
		public StackNodeView CreateStackNode(IEnumerable<NodeView> nodeViews, bool select)
		{
			Debug.Log("Creating node stack");
			
			var stackNodeView = new StackNodeView(nodeViews.ToList(), this);

			if (select)
				stackNodeView.Select(this, false);
			
			AddElement(stackNodeView);
			
			return stackNodeView;
		}
	}
}
