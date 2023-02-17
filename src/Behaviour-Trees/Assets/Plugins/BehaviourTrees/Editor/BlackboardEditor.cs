using System;
using System.Collections.Generic;
using System.Reflection;
using Derrixx.BehaviourTrees.Runtime.BlackboardScripts;
using Derrixx.BehaviourTrees.Runtime.BlackboardScripts.BlackboardProperties;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Derrixx.BehaviourTrees.Editor
{
	[CustomEditor(typeof(Blackboard))]
	public class BlackboardEditor : UnityEditor.Editor
	{
		private Blackboard _blackboard;
		private ReorderableList _propertyList;
		private List<BlackboardProperty> _blackboardProperties;

		private void OnEnable()
		{
			try
			{
				_propertyList = new ReorderableList(serializedObject, serializedObject.FindProperty("_properties"), true, true, true, true)
				{
					drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Properties"),
					drawElementCallback = DrawElements,
					elementHeight = 70
				};
			}
			catch (Exception e)
			{
				Debug.Log(serializedObject);
				return;
			}

			_blackboardProperties = FindBlackboardProperties();
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			_propertyList.displayAdd = _propertyList.displayRemove = !Application.isPlaying;
			_propertyList.DoLayoutList();

			_blackboard = serializedObject.targetObject as Blackboard;
			serializedObject.ApplyModifiedProperties();
		}

		private void DrawElements(Rect rect, int index, bool isActive, bool isFocused)
		{
			float height = EditorGUIUtility.singleLineHeight;

			void MoveDrawerToNextLine()
			{
				rect.y += height + 5;
			}

			BlackboardProperty property = _blackboardProperties[index];
			if (property == null)
				return;
			
			SerializedObject propertyObject = new SerializedObject(property);
			propertyObject.Update();
			// object value = targetObject.GetType().GetField("Key").GetValue(targetObject);

			rect.y += 2;
			DrawPropertyField(rect, propertyObject.FindProperty("_key"));
			MoveDrawerToNextLine();

			const int offset = 70 + 10;
			EditorGUI.LabelField(new Rect(rect.x, rect.y, 70, height), "Value Type");
			
			BlackboardProperty.ValueType valueType = (BlackboardProperty.ValueType)EditorGUI.EnumPopup(new Rect(rect.x + offset, rect.y, rect.width - offset, height), property.GetValueType);
			if (property.GetValueType != valueType)
			{
				ChangePropertyTypeAtIndex(property, index, valueType);
				return;
			}

			// DrawPropertyField(rect, propertyObject.FindProperty("_valueType"), 70);
			MoveDrawerToNextLine();
			
			DrawPropertyField(rect, propertyObject.FindProperty("_value"), 40);
			
			propertyObject.ApplyModifiedProperties();
		}

		private void ChangePropertyTypeAtIndex(BlackboardProperty originalProperty, int index, BlackboardProperty.ValueType valueType)
		{
			BlackboardProperty newProperty = BlackboardProperty.Create(originalProperty.Key, valueType);
			_blackboardProperties[index] = newProperty;
			_blackboard.RemoveProperty(originalProperty);
		}

		private static void DrawPropertyField(Rect rect, SerializedProperty property, int labelWidth = 30, int offset = 10, string label = null)
		{
			float height = EditorGUIUtility.singleLineHeight;
			label ??= property.displayName;

			int posOffset = labelWidth + offset;

			EditorGUI.LabelField(new Rect(rect.x, rect.y, labelWidth, height), label);
			EditorGUI.PropertyField(new Rect(rect.x + posOffset, rect.y, rect.width - posOffset, height), property, GUIContent.none);
		}

		private List<BlackboardProperty> FindBlackboardProperties()
		{
			const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

			FieldInfo fieldInfo = typeof(Blackboard).GetField("_properties", bindingFlags)!;
			return fieldInfo.GetValue(_blackboard) as List<BlackboardProperty>;
		}
	}
}