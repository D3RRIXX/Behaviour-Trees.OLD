using System;
using System.Collections.Generic;
using System.Linq;
using Derrixx.BehaviourTrees.Editor;
using Derrixx.BehaviourTrees.Editor.ViewScripts;
using Derrixx.BehaviourTrees.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Node = Derrixx.BehaviourTrees.Nodes.Node;

namespace Derrixx.BehaviourTrees.Editor
{
    public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private BehaviourTreeView _treeView;
        private VisualElement _creationTarget;
        private bool _createDecorator;

        public VisualElement CreationTarget
        {
	        get => _creationTarget;
	        set
	        {
		        _createDecorator = value is StackNodeView;
		        _creationTarget = value;
	        }
        }

        public void Initialize(BehaviourTreeView treeView)
        {
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

	        Debug.Log($"Create decorator: {_createDecorator}");
	        if (_createDecorator)
	        {
		        AddGroup<DecoratorNode>(x => x != typeof(RootNode));
	        }
	        else
	        {
		        AddGroup<ActionNode>();
		        AddGroup<CompositeNode>();
	        }

			tree.AddRange(GetCustomCreationPaths(customPathNodes));

	        return tree;
        }

        private static IEnumerable<SearchTreeEntry> GetCustomCreationPaths(Dictionary<Type, NodeCreationPathAttribute> customPathNodes)
        {
	        var usedTypes = new Dictionary<int, HashSet<string>>();
	        var output = new List<SearchTreeEntry>();

	        foreach (KeyValuePair<Type, NodeCreationPathAttribute> _ in customPathNodes)
	        {
		        const int pathDepth = 0;
		        AddSomething(customPathNodes, pathDepth, output, usedTypes);
	        }

	        return output;
        }

        private static void AddSomething(Dictionary<Type, NodeCreationPathAttribute> customPathNodes, int pathDepth, List<SearchTreeEntry> output, Dictionary<int, HashSet<string>> usedTypes)
        {
	        var groupings = customPathNodes.Select(pair => new KeyValuePair<Type, string[]>(pair.Key, GetSplitPath(pair.Value)))
		        .GroupBy(pair => pair.Value[0]);
	        
	        foreach (var grouping in groupings)
	        foreach (KeyValuePair<Type, string[]> pair in grouping)
	        {
		        string title = pair.Value[pathDepth];
		        
		        if (!usedTypes.ContainsKey(pathDepth))
			        usedTypes.Add(pathDepth, new HashSet<string>());
		        
		        if (usedTypes[pathDepth].Contains(title))
			        continue;

		        usedTypes[pathDepth].Add(title);
		        
		        int level = pathDepth + 1;
		        var guiContent = new GUIContent(title);

		        Debug.Log($"Path length: {pair.Value.Length}; Depth: {pathDepth}; Title: {title}");

		        if (pathDepth == pair.Value.Length - 1)
		        {
			        output.Add(new SearchTreeEntry(guiContent) { level = level, userData = pair.Key });
		        }
		        else
		        {
			        output.Add(new SearchTreeGroupEntry(guiContent, level));
			        AddSomething(customPathNodes, pathDepth + 1, output, usedTypes);
		        }
	        }
        }

        private static string[] GetSplitPath(NodeCreationPathAttribute creationPathAttribute) => creationPathAttribute.Path.Split('/');

        private static Dictionary<Type, NodeCreationPathAttribute> GetNodesWithCustomCreationPath()
	        => TypeCache.GetTypesDerivedFrom<Node>()
		        .Select(t => new { Type = t, Attributes = t.GetCustomAttributes(typeof(NodeCreationPathAttribute), false) })
		        .Where(x => x.Attributes.Length > 0)
		        .ToDictionary(x => x.Type, x => (NodeCreationPathAttribute)x.Attributes[0]);

        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            Vector2 position = Vector2.zero;
            var nodeType = (Type)entry.userData;
            
            if (_createDecorator)
            {
	            var stackNodeView = (StackNodeView)CreationTarget;
				_treeView.AddDecoratorTo(nodeType, stackNodeView);
            }
            else
            {
	            NodeView nodeView = _treeView.CreateNodeView(nodeType);
	            _treeView.CreateStackNode(nodeView, true);
            }

            return true;
        }
    }
}
