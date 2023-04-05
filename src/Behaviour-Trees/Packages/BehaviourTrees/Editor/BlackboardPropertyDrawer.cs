using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Derrixx.BehaviourTrees.Runtime;
using Derrixx.BehaviourTrees.Runtime.Nodes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Derrixx.BehaviourTrees.Editor
{
	[CustomPropertyDrawer(typeof(BlackboardProperty), true)]
	public class BlackboardPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Object targetObject = property.serializedObject.targetObject;

			if (targetObject is not Node node)
			{
				EditorGUI.PropertyField(position, property, label);
				return;
			}

			var targetProperty = (BlackboardProperty)property.objectReferenceValue;
			Blackboard blackboard = node.BehaviourTree.Blackboard;

			var style = new GUIStyle(GUI.skin.label)
			{
				wordWrap = true
			};
			
			if (!blackboard || blackboard.Properties.Count == 0)
			{
				GUIContent content = new GUIContent("Blackboard is unassigned or empty!");

				Rect labelRect = new Rect(position);
				labelRect.height = style.CalcHeight(content, labelRect.width);
				EditorGUI.LabelField(labelRect, label, content, style);
				
				return;
			}

			BlackboardProperty[] options = GetPropertyOptions(blackboard);
			int indexOf = Array.IndexOf(options, targetProperty);
			bool valueIsUnassigned = indexOf < 0;

			if (options.Length == 0)
			{
				EditorGUI.LabelField(position, label, new GUIContent($"Found no properties of type {fieldInfo.FieldType.Name}!"), style);
				return;
			}

			int index = Mathf.Max(0, indexOf);
			BlackboardProperty value = GetPopupValue(position, label, options, index);

			if (!valueIsUnassigned && value == targetProperty)
				return;

			FieldInfo field = targetObject.GetType().GetField(property.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			field!.SetValue(targetObject, value);

			EditorUtility.SetDirty(targetObject);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (property.serializedObject.targetObject is not Blackboard)
				return EditorGUIUtility.singleLineHeight;

			const int baseFieldCount = 6;
			int fieldCount = baseFieldCount;
			
			var serializedObject = new SerializedObject(property.objectReferenceValue);
			bool isSynced = serializedObject.FindProperty("_sync").boolValue;
			
			if (isSynced)
				fieldCount--;
			
			serializedObject.Dispose();

			return EditorGUIUtility.singleLineHeight * fieldCount;
		}

		private static BlackboardProperty GetPopupValue(Rect position, GUIContent label, IReadOnlyList<BlackboardProperty> options, int index)
		{
			string[] keys = (from option in options select option.Key).ToArray();
			int i = EditorGUI.Popup(position, label.text, index, keys);

			return options[i];
		}

		private BlackboardProperty[] GetPropertyOptions(Blackboard blackboard)
		{
			return (from property in blackboard.Properties
				where fieldInfo.FieldType.IsInstanceOfType(property)
				select property).ToArray();
		}
	}
}