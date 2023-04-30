using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Derrixx.BehaviourTrees.Runtime;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Derrixx.BehaviourTrees.Editor
{
	[CustomEditor(typeof(Blackboard))]
	public class BlackboardEditor : UnityEditor.Editor
	{
		private Blackboard _blackboard;
		private Blackboard _blackboardParent;
		
		private ReorderableList _propertyList;
		private ReorderableList _inheritedPropertyList;
		
		private List<BlackboardProperty> _blackboardProperties;
		private SerializedProperty _parent;

		private Dictionary<BlackboardProperty, bool> _foldoutMap = new();

		private void OnEnable()
		{
			try
			{
				SerializedProperty serializedProperty = serializedObject.FindProperty("_properties");
				_propertyList = new ReorderableList(serializedObject, serializedProperty, true, true, true, true)
				{
					drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Properties"),
					drawElementCallback = (rect, index, _, _) => DrawElements(ModifyListItemRect(rect), index, _blackboardProperties),
					onAddDropdownCallback = CreateAddDropdown,
					onRemoveCallback = OnRemove,
					elementHeightCallback = x => GetPropertyHeight(x, serializedProperty, _blackboardProperties)
				};

				_blackboard = serializedObject.targetObject as Blackboard;
				_blackboardProperties = FindBlackboardProperties();
				_parent = serializedObject.FindProperty("_parent");
				_foldoutMap = _blackboardProperties.ToDictionary(x => x, _ => false);
			}
			catch (Exception)
			{
				// ignored
			}
		}

		private float GetPropertyHeight(int x, SerializedProperty serializedProperty, IReadOnlyList<BlackboardProperty> blackboardProperties)
		{
			return !_foldoutMap[blackboardProperties[x]]
				? EditorGUIUtility.singleLineHeight
				: EditorGUI.GetPropertyHeight(serializedProperty.GetArrayElementAtIndex(x));
		}

		private void OnRemove(ReorderableList list)
		{
			BlackboardProperty property = _blackboardProperties[list.index];
			_blackboard.RemoveProperty(property);
			_foldoutMap.Remove(property);
			
			ReorderableList.defaultBehaviours.DoRemoveButton(list);
		}

		private static Rect ModifyListItemRect(Rect rect)
		{
			const int offset = 10;
			rect.x += offset;
			rect.width -= offset;

			return rect;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(_parent);
			
			CheckDuplicateKeys(_blackboard.Properties);
			
			_inheritedPropertyList?.DoLayoutList();
			
			_propertyList.displayAdd = _propertyList.displayRemove = !Application.isPlaying;
			_propertyList.DoLayoutList();

			serializedObject.ApplyModifiedProperties();

			var parentPropertyValue = (Blackboard)_parent.objectReferenceValue;
			if (parentPropertyValue == _blackboardParent)
				return;
			
			_inheritedPropertyList = RecreateInheritedPropertyList(_blackboardParent, parentPropertyValue, _foldoutMap);
			_blackboardParent = parentPropertyValue;
		}

		private ReorderableList RecreateInheritedPropertyList(Blackboard oldParent, Blackboard newParent, IDictionary<BlackboardProperty, bool> foldoutMap)
		{
			if (oldParent is not null)
			{
				foreach (BlackboardProperty property in oldParent.Properties)
				{
					foldoutMap.Remove(property);
				}
			}
			
			if (newParent is null)
				return null;

			IReadOnlyList<BlackboardProperty> inheritedProperties = newParent.Properties;
			var parentObj = new SerializedObject(newParent);
			
			foreach (BlackboardProperty property in newParent.Properties)
			{
				foldoutMap.Add(property, false);
			}

			SerializedProperty serializedProperty = parentObj.FindProperty("_properties");
			return new ReorderableList(parentObj, serializedProperty)
			{
				displayAdd = false, displayRemove = false, draggable = false,
				drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Inherited Properties"),
				drawElementCallback = (rect, index, _, _) =>
				{
					GUI.enabled = false;
					DrawElements(ModifyListItemRect(rect), index, inheritedProperties, true);
					GUI.enabled = true;
				},
				elementHeightCallback = x => GetPropertyHeight(x, serializedProperty, inheritedProperties)
			};
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
			_foldoutMap.Add(_blackboard.Properties[_propertyList.index], true);
		}

		private void DrawElements(Rect rect, int index, IReadOnlyList<BlackboardProperty> propertyList, bool forceGUIState = false)
		{
			float height = EditorGUIUtility.singleLineHeight;

			void MoveDrawerToNextLine() => rect.y += height;
			
			BlackboardProperty property = propertyList[index];
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

			if (!forceGUIState)
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

			if (!forceGUIState)
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

			bool initialValue = _foldoutMap[property];
			bool foldout = EditorGUI.Foldout(position, initialValue, label);

			_foldoutMap[property] = foldout;
			
			return foldout;
		}
		
		private static string GetFoldoutLabel(SerializedObject propertyObject, BlackboardProperty blackboardProperty)
		{
			string GetStringWithComma(string propertyPath, string text) => propertyObject.FindProperty(propertyPath).boolValue ? $", {text}" : string.Empty;

			// ReSharper disable once UseStringInterpolation
			return string.Format("{0} ({1}{2}{3})", blackboardProperty.Key, blackboardProperty.GetValueType,
				GetStringWithComma("_isExposed", "Exposed"),
				GetStringWithComma("_sync", "Instance Synced"));
		}

		private void ChangePropertyTypeAtIndex(BlackboardProperty originalProperty, int index, BlackboardProperty.ValueType valueType)
		{
			var newProperty = BlackboardProperty.Create(originalProperty.Key, valueType);
			_blackboardProperties[index] = newProperty;
			_foldoutMap.Add(newProperty, true);
			
			_foldoutMap.Remove(originalProperty);
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