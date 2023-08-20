#if UNITY_EDITOR
using Derrixx.BehaviourTrees.Nodes;
using UnityEditor;

namespace Derrixx.BehaviourTrees.Editor
{
	[CustomEditor(typeof(RunTreeNode), true)]
	public class RunTreeNodeEditor : NodeEditor
	{
		private RunTreeNode _runTreeNode;
		private SerializedProperty _behaviourTreeProperty;

		protected override void OnEnable()
		{
			base.OnEnable();
			_runTreeNode = target as RunTreeNode;
			_behaviourTreeProperty = serializedObject.FindProperty("_behaviourTree");
		}

		protected override void OnInspectorGUI_Implementation()
		{
			EditorGUILayout.PropertyField(_behaviourTreeProperty);
			
			if (_behaviourTreeProperty.objectReferenceValue is not BehaviourTree subTree)
				return;
			
			if (subTree.Blackboard != _runTreeNode.BehaviourTree.Blackboard)
				EditorGUILayout.HelpBox("This node will not run because it uses a different Blackboard!", MessageType.Error);
            
			if (subTree == _runTreeNode.BehaviourTree)
				EditorGUILayout.HelpBox($"You've assigned '{subTree.name}' as a subtree of self. This will lead to stack overflow!", MessageType.Error);
		}
	}
}
#endif
