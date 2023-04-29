using System;
using System.Collections.Generic;
using System.Linq;
using Derrixx.BehaviourTrees.Runtime.Attributes;
using Derrixx.BehaviourTrees.Runtime.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Node = Derrixx.BehaviourTrees.Runtime.Nodes.Node;

namespace Derrixx.BehaviourTrees.Editor
{
    public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private BehaviourTreeView _treeView;
        private EditorWindow _window;

        public void Initialize(EditorWindow window, BehaviourTreeView treeView)
        {
            _window = window;
            _treeView = treeView;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
	        var tree = new List<SearchTreeEntry> { new SearchTreeGroupEntry(new GUIContent("Nodes")) };

	        Dictionary<Type, NodeCreationPathAttribute> customPathNodes = GetNodesWithCustomCreationPath();

	        void AddGroup<T>(Func<Type, bool> additionalCondition = null)
	        {
		        tree.Add(new SearchTreeGroupEntry(new GUIContent(Node.GetNodeName(typeof(T))), 1));

		        tree.AddRange(
			        from type in TypeCache.GetTypesDerivedFrom<T>().Where(t => !t.IsAbstract && !t.IsInterface && !customPathNodes.ContainsKey(t))
			        where additionalCondition == null || additionalCondition(type)
			        let name = Node.GetNodeName(type)
			        select new SearchTreeEntry(new GUIContent(name)) { userData = type, level = 2 });
	        }

	        AddGroup<ActionNode>();
	        AddGroup<CompositeNode>();
	        AddGroup<DecoratorNode>(x => x != typeof(RootNode));

	        AddNodesWithCustomPath(customPathNodes, tree);

	        return tree;
        }

        private static void AddNodesWithCustomPath(Dictionary<Type, NodeCreationPathAttribute> customPathNodes, List<SearchTreeEntry> tree)
        {
	        foreach ((Type type, NodeCreationPathAttribute attribute) in customPathNodes)
	        {
		        string[] levels = attribute.Path.Split('/');
		        for (int i = 0; i < levels.Length; i++)
		        {
			        SearchTreeEntry entry;
			        var label = new GUIContent(levels[i]);
			        int level = i + 1;

			        if (i != levels.Length - 1)
			        {
				        entry = new SearchTreeGroupEntry(label, level);
			        }
			        else
			        {
				        entry = new SearchTreeEntry(label)
				        {
					        level = level,
					        userData = type
				        };
			        }

			        tree.Add(entry);
		        }
	        }
        }

        private static Dictionary<Type, NodeCreationPathAttribute> GetNodesWithCustomCreationPath()
	        => TypeCache.GetTypesDerivedFrom<Node>()
		        .Select(t => new { Type = t, Attributes = t.GetCustomAttributes(typeof(NodeCreationPathAttribute), false) })
		        .Where(x => x.Attributes.Length > 0)
		        .ToDictionary(x => x.Type, x => (NodeCreationPathAttribute)x.Attributes[0]);

        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            // Vector2 position = RuntimePanelUtils.ScreenToPanel(_treeView.panel, context.screenMousePosition);
            Vector2 position = Vector2.zero;
            _treeView.CreateNode((Type)entry.userData, position);

            return true;
        }
    }
}