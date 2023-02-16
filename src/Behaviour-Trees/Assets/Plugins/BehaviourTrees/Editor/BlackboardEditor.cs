using Derrixx.BehaviourTrees.Runtime.BlackboardScripts;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Derrixx.BehaviourTrees.Editor
{
	[CustomEditor(typeof(Blackboard))]
	public class BlackboardEditor : UnityEditor.Editor
	{
		private ReorderableList _propertyList;

		private void OnEnable()
		{
			_propertyList = new ReorderableList(serializedObject, serializedObject.FindProperty("_properties"), false, true, true, true);
			_propertyList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Properties");
			_propertyList.drawElementCallback = DrawElements;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			
			_propertyList.displayAdd = _propertyList.displayRemove = !Application.isPlaying;
			_propertyList.DoLayoutList();
			
			serializedObject.ApplyModifiedProperties();
		}

		private void DrawElements(Rect rect, int index, bool isActive, bool isFocused)
		{
			var element = _propertyList.serializedProperty.GetArrayElementAtIndex(index);
			EditorGUILayout.PropertyField(element);
		}
	}
}