using System.Reflection;
using Derrixx.BehaviourTrees.Editor.ViewScripts;
using Derrixx.BehaviourTrees.Runtime;
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
		private IMGUIContainer _blackboardContainer;
		private Label _nameLabel;

		private SerializedObject _treeObject;

		private SerializedProperty _blackboardProperty;

		private bool _isViewingRuntimeTree;
		private BlackboardEditor _blackboardEditor;

		[MenuItem("Window/AI/Behaviour Tree Editor")]
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

		private void OnBecameVisible()
		{
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
		}

		private void OnBecameInvisible()
		{
			EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
		}

		private void OnInspectorUpdate()
		{
			if (_isViewingRuntimeTree && Application.isPlaying)
				_behaviourTreeView?.UpdateNodeStates();
		}

		private void OnPlayModeStateChanged(PlayModeStateChange stateChange)
		{
			switch (stateChange)
			{
				case PlayModeStateChange.EnteredEditMode:
					OnSelectionChange();
					break;
				case PlayModeStateChange.EnteredPlayMode:
					OnSelectionChange();
					break;
			}
		}

		public void CreateGUI()
		{
			// Each editor window contains a root VisualElement object
			VisualElement root = rootVisualElement;

			// Import UXML
			var visualTree = Resources.Load<VisualTreeAsset>("BehaviourTreeEditor");
			visualTree.CloneTree(root);

			// A stylesheet can be added to a VisualElement.
			// The style will be applied to the VisualElement and all of its children.
			var styleSheet = Resources.Load<StyleSheet>("BehaviourTreeEditorStyle");
			root.styleSheets.Add(styleSheet);

			_behaviourTreeView = root.Q<BehaviourTreeView>();
			_behaviourTreeView.OnNodeSelected = OnNodeSelectionChange;

			_inspectorView = root.Q<InspectorView>();
			_blackboardContainer = root.Q<IMGUIContainer>();
			_blackboardContainer.onGUIHandler = DrawBlackboardContainer;

			_nameLabel = root.Q<Label>("tree-name");

			OnSelectionChange();
		}

		private void DrawBlackboardContainer()
		{
			if (_blackboardProperty == null || _treeObject.targetObject == null)
				return;

			_treeObject.Update();
			EditorGUILayout.PropertyField(_blackboardProperty, new GUIContent("Selected Blackboard"));
			_treeObject.ApplyModifiedProperties();

			Object targetObject = _blackboardProperty.objectReferenceValue;
			if (!targetObject)
				return;

			bool editorIsNull = _blackboardEditor == null;
			if (editorIsNull || _blackboardEditor.target != targetObject)
			{
				if (editorIsNull)
					DestroyImmediate(_blackboardEditor);

				_blackboardEditor = (BlackboardEditor)UnityEditor.Editor.CreateEditor(targetObject, typeof(BlackboardEditor));
			}

			EditorGUILayout.Space(15);
			_blackboardEditor.OnInspectorGUI();
		}

		private void OnNodeSelectionChange(NodeView nodeView)
		{
			_inspectorView.UpdateSelection(nodeView);
		}

		private void OnSelectionChange()
		{
			_isViewingRuntimeTree = false;
			if (!TryGetBehaviourTreeTarget(out BehaviourTree tree, out bool treeIsAttachedToObject))
				return;

			if (!Application.isPlaying && (!AssetDatabase.CanOpenForEdit(tree) || !EditorUtility.IsPersistent(tree)))
				return;

			_blackboardProperty?.Dispose();
			_treeObject?.Dispose();

			_treeObject = new SerializedObject(tree);
			_blackboardProperty = _treeObject.FindProperty("blackboard");

			_isViewingRuntimeTree = treeIsAttachedToObject;
			_behaviourTreeView.PopulateView(tree);
			_nameLabel.text = tree.name;
		}

		private static bool TryGetBehaviourTreeTarget(out BehaviourTree tree, out bool treeIsAttachedToObject)
		{
			tree = Selection.activeObject as BehaviourTree;
			treeIsAttachedToObject = false;

			if (tree != null)
				return true;

			GameObject target = Selection.activeGameObject;
			if (target == null)
				return false;

			treeIsAttachedToObject = true;
			tree = GetBehaviourTreeField(target);
			return tree != null;
		}

		private static BehaviourTree GetBehaviourTreeField(GameObject target)
		{
			if (!target.TryGetComponent(out BehaviourTreeRunner runner))
				return null;

			return (BehaviourTree)typeof(BehaviourTreeRunner)
				.GetField("_behaviourTree",  BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)!
				.GetValue(runner);
		}
	}
}