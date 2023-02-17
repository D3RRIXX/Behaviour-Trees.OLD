using System.Collections.Generic;
using System.Reflection;
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
		private List<BlackboardProperty> _blackboardProperties;

		private void OnEnable()
		{
			_propertyList = new ReorderableList(serializedObject, serializedObject.FindProperty("_properties"), false, true, true, true)
			{
				drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Properties"),
				drawElementCallback = DrawElements
			};

			_blackboardProperties = FindBlackboardProperties();
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			return;
			
			serializedObject.Update();

			_propertyList.displayAdd = _propertyList.displayRemove = !Application.isPlaying;
			_propertyList.DoLayoutList();

			serializedObject.ApplyModifiedProperties();
		}

		private void DrawElements(Rect rect, int index, bool isActive, bool isFocused)
		{
			BlackboardProperty property = _blackboardProperties[index];
			if (property == null)
				return;
			
			SerializedObject propertyObject = new SerializedObject(property);
			propertyObject.Update();
			// object value = targetObject.GetType().GetField("Key").GetValue(targetObject);

			rect.y += 2;
			// EditorGUI.PropertyField(new Rect(rect.x + 60, rect.y, rect.width - 60 - 30, EditorGUIUtility.singleLineHeight), key, GUIContent.none);
			// EditorGUI.PropertyField(new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight), propertyObject.FindProperty("Key"), GUIContent.none);
			// EditorGUI.PropertyField(new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight), propertyObject.FindProperty("Value"), GUIContent.none);
			EditorGUI.PropertyField(rect, propertyObject.FindProperty("Key"));
			EditorGUILayout.PropertyField(propertyObject.FindProperty("Value"));
			
			propertyObject.ApplyModifiedProperties();
		}

		private List<BlackboardProperty> FindBlackboardProperties()
		{
			const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

			Blackboard blackboard = serializedObject.targetObject as Blackboard;
			FieldInfo fieldInfo = typeof(Blackboard).GetField("_properties", bindingFlags)!;
			return fieldInfo.GetValue(blackboard) as List<BlackboardProperty>;
		}
	}
}