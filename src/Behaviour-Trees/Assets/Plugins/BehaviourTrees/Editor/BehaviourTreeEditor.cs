using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Derrixx.BehaviourTrees.Editor.ViewScripts;
using Derrixx.BehaviourTrees.Runtime.Nodes;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Derrixx.BehaviourTrees.Editor
{
    public class BehaviourTreeEditor : EditorWindow
    {
        private BehaviourTreeView _behaviourTreeView;
        private InspectorView _inspectorView;
        private Label _nameLabel;

        [MenuItem("Window/Derrixx/Behaviour Trees/Behaviour Tree Editor")]
        public static void OpenWindow()
        {
            BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
            wnd.titleContent = new GUIContent("Behaviour Tree Editor");
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (Selection.activeObject is BehaviourTree)
            {
                OpenWindow();
                return true;
            }

            return false;
        }

        public void CreateGUI()
        {
            const string path = "Assets/Plugins/BehaviourTrees/Editor/";
            
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path + "UIBuilder/BehaviourTreeEditor.uxml");
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path + "BehaviourTreeEditor.uss");
            root.styleSheets.Add(styleSheet);

            _behaviourTreeView = root.Q<BehaviourTreeView>();
            _behaviourTreeView.OnNodeSelected = OnNodeSelectionChange;
            
            _inspectorView = root.Q<InspectorView>();

            _nameLabel = root.Q<Label>("tree-name");

            OnSelectionChange();
        }

        private void OnNodeSelectionChange(NodeView nodeView)
        {
            _inspectorView.UpdateSelection(nodeView);
        }

        private void OnSelectionChange()
        {
            BehaviourTree tree = Selection.activeObject as BehaviourTree;

            if (!tree && Selection.activeGameObject != null)
            {
                TryFindBehaviourTreeField(Selection.activeGameObject, out tree);
            }

            if (tree == null)
                return;

            if (!Application.isPlaying && !AssetDatabase.CanOpenForEdit(tree))
                return;
            
            _behaviourTreeView.PopulateView(tree);
            _nameLabel.text = tree.name;
        }

        private static bool TryFindBehaviourTreeField(GameObject target, out BehaviourTree tree)
        {
            var fields = target.GetComponents<MonoBehaviour>()
                .Select(x => x.GetType())
                .SelectMany(x => x.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));

            foreach (MonoBehaviour component in target.GetComponents<MonoBehaviour>())
            foreach (FieldInfo field in component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (field.FieldType == typeof(BehaviourTree))
                {
                    tree = (BehaviourTree)field.GetValue(component);
                    return true;
                }
            }
            
            // foreach (FieldInfo field in fields)
            // {
            //     if (field.FieldType == typeof(BehaviourTree))
            //     {
            //         tree = (BehaviourTree)field.GetValue(target);
            //         return true;
            //     }
            // }

            tree = null;
            return false;
        }
    }
}