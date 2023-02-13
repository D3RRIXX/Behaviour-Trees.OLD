using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

		public BehaviourTreeView()
		{
			Insert(0, new GridBackground());

			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new ContentZoomer());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());

			StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/BehaviourTrees/Editor/BehaviourTreeEditor.uss");
			styleSheets.Add(styleSheet);
		}

		public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
		{
			SetupCreateNodeActions(evt);
		}

		private  void SetupCreateNodeActions(ContextualMenuPopulateEvent evt)
		{
			void AppendCreateSubclassNodesActions(Type baseType)
			{
				const string pattern = @"([A-Z])\w+(?=Node)";
				const string actionName = "Create Node/{0}/{1}";

				Regex regex = new Regex(pattern);
				
				TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom(baseType);
				foreach (Type type in types)
				{
					evt.menu.AppendAction(string.Format(actionName, regex.Match(baseType.Name).Value, regex.Match(type.Name).Value), _ => CreateNode(type));
				}
			}
			
			AppendCreateSubclassNodesActions(typeof(ActionNode));
			AppendCreateSubclassNodesActions(typeof(DecoratorNode));
			AppendCreateSubclassNodesActions(typeof(CompositeNode));
		}

		internal void PopulateView(BehaviourTree tree)
		{
			_tree = tree;

			graphViewChanged -= OnGraphViewChanged;
			DeleteElements(graphElements);
			graphViewChanged += OnGraphViewChanged;

			foreach (Node node in _tree.Nodes)
			{
				CreateNodeView(node);
			}
		}

		public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
		{
			return ports.ToList()
				.Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node)
				.ToList();
		}

		private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
		{
			if (graphViewChange.elementsToRemove != null)
			{
				foreach (GraphElement graphElement in graphViewChange.elementsToRemove)
				{
					if (graphElement is NodeView nodeView)
						_tree.DeleteNode(nodeView.Node);
				}
			}
			
			return graphViewChange;
		}

		private void CreateNode(Type type)
		{
			Node node = _tree.CreateNode(type);
			CreateNodeView(node);
		}

		private void CreateNodeView(Node node)
		{
			NodeView nodeView = new NodeView(node);
			AddElement(nodeView);
		}
	}
}