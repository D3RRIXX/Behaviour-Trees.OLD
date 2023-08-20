using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Derrixx.BehaviourTrees.Editor
{
	[CustomPropertyDrawer(typeof(BlackboardProperty), true)]
	public class BlackboardPropertyDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			Object targetObject = property.serializedObject.targetObject;

			if (targetObject is not Node node)
				return new PropertyField(property);
			
			var targetProperty = (BlackboardProperty)property.objectReferenceValue;
			var blackboard = node.BehaviourTree.Blackboard;

			if (!blackboard || blackboard.Properties.Count == 0)
				return new Label("Blackboard is unassigned or empty!");

			BlackboardProperty[] options = GetPropertyOptions(blackboard);

			if (options.Length == 0)
				return new Label($"Found no properties of type {fieldInfo.FieldType.Name}!");

			return SetupDropdownField(property, options, targetProperty, targetObject);
		}

		private DropdownField SetupDropdownField(SerializedProperty property, BlackboardProperty[] options, BlackboardProperty targetProperty, Object targetObject)
		{
			int defaultIndex = Mathf.Max(0, Array.IndexOf(options, targetProperty));
			var keys = (from option in options select option.Key).ToList();

			var dropdownField = new DropdownField(property.displayName, keys, defaultIndex)
			{
				name = "blackboard-property__dropdown",
				style =
				{
					height = EditorGUIUtility.singleLineHeight
				}
			};
			dropdownField.RegisterValueChangedCallback(evt => SetBlackboardProperty(targetObject, options[dropdownField.index]));
			
			return dropdownField;
		}

		private void SetBlackboardProperty(Object targetObject, BlackboardProperty propertyFromPopup)
		{
			// var field = targetObject.GetType().GetField(property.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			fieldInfo!.SetValue(targetObject, propertyFromPopup);
			
			EditorUtility.SetDirty(targetObject);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (property.serializedObject.targetObject is not Blackboard)
				// return EditorGUIUtility.singleLineHeight;
				return base.GetPropertyHeight(property, label);

			const int baseFieldCount = 6;
			int fieldCount = baseFieldCount;
			
			var serializedObject = new SerializedObject(property.objectReferenceValue);
			bool isSynced = serializedObject.FindProperty("_sync").boolValue;
			
			if (isSynced)
				fieldCount--;
			
			serializedObject.Dispose();

			return EditorGUIUtility.singleLineHeight * fieldCount;
		}

		private BlackboardProperty[] GetPropertyOptions(Blackboard blackboard)
		{
			var propertyTypesAttributes = fieldInfo.GetCustomAttributes<AllowedPropertyTypesAttribute>().ToList();
			if (propertyTypesAttributes.Count == 0)
			{
				return (from property in blackboard.Properties
				        where fieldInfo.FieldType.IsInstanceOfType(property)
				        select property).ToArray();
			}

			var allowedTypes = new HashSet<Type>(propertyTypesAttributes.SelectMany(x => x.GetBlackboardPropertyTypes()));
			return (from property in blackboard.Properties
			        where allowedTypes.Contains(property.GetType())
			        select property).ToArray();
		}
	}
}
