using Derrixx.BehaviourTrees.Runtime.Nodes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Derrixx.BehaviourTrees.Editor
{
    public class BehaviourTreeEditor : EditorWindow
    {
        private BehaviourTreeView _behaviourTreeView;
        private InspectorView _inspectorView;
        private Label _namelabel;

        [MenuItem("Window/Derrixx/Behaviour Trees/Behaviour Tree Editor")]
        public static void OpenWindow()
        {
            BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
            wnd.titleContent = new GUIContent("BehaviourTreeEditor");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Plugins/BehaviourTrees/Editor/BehaviourTreeEditor.uxml");
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/BehaviourTrees/Editor/BehaviourTreeEditor.uss");
            root.styleSheets.Add(styleSheet);

            _behaviourTreeView = root.Q<BehaviourTreeView>();
            _behaviourTreeView.OnNodeSelected = OnNodeSelectionChange;
            
            _inspectorView = root.Q<InspectorView>();

            _namelabel = root.Q<Label>("tree-name");

            OnSelectionChange();
        }

        private void OnNodeSelectionChange(NodeView nodeView)
        {
            _inspectorView.UpdateSelection(nodeView);
        }

        private void OnSelectionChange()
        {
            BehaviourTree tree = Selection.activeObject as BehaviourTree;
            if (!tree)
                return;

            _behaviourTreeView.PopulateView(tree);
            _namelabel.text = tree.name;
        }
    }
}