using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Derrixx.BehaviourTrees.Runtime;
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

		private Dictionary<BlackboardProperty, bool> _foldouts = new();

		private void OnEnable()
		{
			try
			{
				SerializedProperty serializedProperty = serializedObject.FindProperty("_properties");
				_propertyList = new ReorderableList(serializedObject, serializedProperty, true, true, true, true)
				{
					drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Properties"),
					drawElementCallback = DrawElements,
					onAddDropdownCallback = CreateAddDropdown,
					onRemoveCallback = OnRemove,
					elementHeightCallback = x =>
					{
						if (!_foldouts[_blackboardProperties[x]])
							return EditorGUIUtility.singleLineHeight;
						
						float propertyHeight = EditorGUI.GetPropertyHeight(serializedProperty.GetArrayElementAtIndex(x));

						if (x < serializedProperty.arraySize - 1)
							propertyHeight += 10;
						
						return propertyHeight;
					}
				};

				_blackboard = serializedObject.targetObject as Blackboard;
				_blackboardProperties = FindBlackboardProperties();
				_parent = serializedObject.FindProperty("_parent");
				_foldouts = _blackboardProperties.ToDictionary(x => x, _ => false);
			}
			catch (Exception)
			{
				// ignored
			}
		}

		private void OnRemove(ReorderableList list)
		{
			BlackboardProperty property = _blackboardProperties[list.index];
			_blackboard.RemoveProperty(property);
			_foldouts.Remove(property);
			
			ReorderableList.defaultBehaviours.DoRemoveButton(list);
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(_parent);
			
			CheckDuplicateKeys(_blackboard.Properties);
			
			_propertyList.displayAdd = _propertyList.displayRemove = !Application.isPlaying;
			_propertyList.DoLayoutList();

			serializedObject.ApplyModifiedProperties();
		}

		private static void CheckDuplicateKeys(IReadOnlyList<BlackboardProperty> blackboardProperties)
		{
			foreach (BlackboardProperty property in blackboardProperties)
			{
				string key = property.Key;
				if (blackboardProperties.Count(x => x != property && x.Key == key) <= 0)
					continue;

				EditorGUILayout.HelpBox($"Property key '{key}' is defined multiple times!", MessageType.Error);
				break;
			}
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
			_foldouts.Add(_blackboard.Properties[_propertyList.index], true);
		}

		private void DrawElements(Rect rect, int index, bool isActive, bool isFocused)
		{
			float height = EditorGUIUtility.singleLineHeight;

			void MoveDrawerToNextLine() => rect.y += height;
			
			BlackboardProperty property = _blackboardProperties[index];
			if (property == null)
				return;

			var propertyObject = new SerializedObject(property);
			if (!IsFoldoutActive(rect, property, GetFoldoutLabel(propertyObject, property)))
				return;

			const float foldoutOffset = 20;
			rect.x += foldoutOffset;
			rect.width -= foldoutOffset;
			
			MoveDrawerToNextLine();
			propertyObject.Update();

			DrawPropertyField(rect, propertyObject.FindProperty("_key"));
			MoveDrawerToNextLine();

			GUI.enabled = !Application.isPlaying;
			
			SerializedProperty exposedProperty = propertyObject.FindProperty("_isExposed");
			SerializedProperty syncProperty = propertyObject.FindProperty("_sync");
			
			if (!syncProperty.boolValue)
			{
				DrawPropertyField(rect, exposedProperty, labelWidth: 100);
				MoveDrawerToNextLine();
			}
			else
			{
				exposedProperty.boolValue = false;
			}

			DrawPropertyField(rect, syncProperty, label: "Instance Synced", labelWidth: 100);
			
			GUI.enabled = true;
			
			MoveDrawerToNextLine();

			const int offset = 70 + 10;
			EditorGUI.LabelField(new Rect(rect.x, rect.y, 70, height), "Value Type");

			var valueType = (BlackboardProperty.ValueType)EditorGUI.EnumPopup(new Rect(rect.x + offset, rect.y, rect.width - offset, height), property.GetValueType);
			if (property.GetValueType != valueType)
			{
				ChangePropertyTypeAtIndex(property, index, valueType);
				return;
			}

			MoveDrawerToNextLine();

			DrawPropertyField(rect, propertyObject.FindProperty("_value"), 40);

			propertyObject.ApplyModifiedProperties();
		}

		private bool IsFoldoutActive(Rect position, BlackboardProperty property, string label)
		{
			position.size = new Vector2(position.width - 6, 15);

			bool initialValue = _foldouts[property];

			// string label = $"{property.Key} ({property.GetValueType})";
			bool foldout = EditorGUI.Foldout(position, initialValue, label);
			_foldouts[property] = foldout;
			return foldout;
		}
		
		private string GetFoldoutLabel(SerializedObject propertyObject, BlackboardProperty blackboardProperty)
		{
			string GetStringWithComma(string propertyPath, string text) => propertyObject.FindProperty(propertyPath).boolValue ? $", {text}" : string.Empty;

			return string.Format("{0} ({1}{2}{3})", blackboardProperty.Key, blackboardProperty.GetValueType,
				GetStringWithComma("_isExposed", "Exposed"),
				GetStringWithComma("_sync", "Instance Synced"));
		}

		private void ChangePropertyTypeAtIndex(BlackboardProperty originalProperty, int index, BlackboardProperty.ValueType valueType)
		{
			var newProperty = BlackboardProperty.Create(originalProperty.Key, valueType);
			_blackboardProperties[index] = newProperty;
			_foldouts.Add(newProperty, true);
			
			_foldouts.Remove(originalProperty);
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