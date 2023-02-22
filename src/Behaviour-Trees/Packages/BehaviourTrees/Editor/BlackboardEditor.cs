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
		private SerializedProperty _parent;

		private void OnEnable()
		{
			try
			{
				_propertyList = new ReorderableList(serializedObject, serializedObject.FindProperty("_properties"), true, true, true, true)
				{
					drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Properties"),
					drawElementCallback = DrawElements,
					onAddDropdownCallback = CreateAddDropdown,
					onRemoveCallback = OnRemove,
					elementHeightCallback = _ => 93
				};

				_blackboard = serializedObject.targetObject as Blackboard;
				_blackboardProperties = FindBlackboardProperties();
				_parent = serializedObject.FindProperty("_parent");
			}
			catch (Exception)
			{
				// ignored
			}
		}

		private void OnRemove(ReorderableList list)
		{
			_blackboard.RemoveProperty(_blackboardProperties[list.index]);
			ReorderableList.defaultBehaviours.DoRemoveButton(list);
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(_parent);
			_propertyList.displayAdd = _propertyList.displayRemove = !Application.isPlaying;
			_propertyList.DoLayoutList();

			serializedObject.ApplyModifiedProperties();
		}

		private void CreateAddDropdown(Rect buttonRect, ReorderableList list)
		{
			var menu = new GenericMenu();

			int i = 0;
			foreach (string typeName in Enum.GetNames(typeof(BlackboardProperty.ValueType)))
			{
				menu.AddItem(new GUIContent(typeName), false, AddProperty, i);
				i++;
			}

			menu.ShowAsContext();
		}

		private void AddProperty(object userData)
		{
			_blackboard.AddProperty((BlackboardProperty.ValueType)userData);
			_propertyList.index++;
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

			if (index > 0)
				rect.y += 5;

			DrawPropertyField(rect, propertyObject.FindProperty("_key"));
			MoveDrawerToNextLine();

			GUI.enabled = !Application.isPlaying;
			DrawPropertyField(rect, propertyObject.FindProperty("_sync"), label: "Instance Synced", labelWidth: 100);
			GUI.enabled = true;
			
			MoveDrawerToNextLine();

			const int offset = 70 + 10;
			EditorGUI.LabelField(new Rect(rect.x, rect.y, 70, height), "Value Type");

			BlackboardProperty.ValueType valueType =
				(BlackboardProperty.ValueType)EditorGUI.EnumPopup(new Rect(rect.x + offset, rect.y, rect.width - offset, height), property.GetValueType);
			if (property.GetValueType != valueType)
			{
				ChangePropertyTypeAtIndex(property, index, valueType);
				return;
			}

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

		private static void DrawPropertyField(Rect rect, SerializedProperty property, int labelWidth = 30, int offset = 10, string label = null, GUIContent content = null)
		{
			float height = EditorGUIUtility.singleLineHeight;
			label ??= property.displayName;
			content ??= GUIContent.none;

			int posOffset = labelWidth + offset;

			EditorGUI.LabelField(new Rect(rect.x, rect.y, labelWidth, height), label);
			EditorGUI.PropertyField(new Rect(rect.x + posOffset, rect.y, rect.width - posOffset, height), property, content);
		}

		private List<BlackboardProperty> FindBlackboardProperties()
		{
			const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

			FieldInfo fieldInfo = typeof(Blackboard).GetField("_properties", bindingFlags)!;
			return fieldInfo.GetValue(_blackboard) as List<BlackboardProperty>;
		}
	}
}