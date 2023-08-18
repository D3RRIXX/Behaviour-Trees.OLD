using Derrixx.BehaviourTrees.Nodes;
using UnityEditor;
using UnityEngine;

namespace Derrixx.BehaviourTrees.Editor
{
	[CustomEditor(typeof(Node), true, isFallback = true)]
	public class NodeEditor : UnityEditor.Editor
	{
		private SerializedProperty _nameProperty;
		private SerializedProperty _scriptProperty;

		protected virtual void OnEnable()
		{
			_nameProperty = serializedObject.FindProperty("nodeName");
			_scriptProperty = serializedObject.FindProperty("m_Script");
		}

		public sealed override void OnInspectorGUI()
		{
			serializedObject.Update();
			
			GUI.enabled = false;
			EditorGUILayout.PropertyField(_scriptProperty, true);
			GUI.enabled = true;
			EditorGUILayout.PropertyField(_nameProperty);
			
			OnInspectorGUI_Implementation();

			serializedObject.ApplyModifiedProperties();
		}

		protected virtual void OnInspectorGUI_Implementation()
		{
			DrawPropertiesExcluding(serializedObject, "m_Script", "nodeName");
		}
	}
}
