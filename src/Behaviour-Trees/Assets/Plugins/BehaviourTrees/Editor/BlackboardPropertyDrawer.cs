using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Derrixx.BehaviourTrees.Runtime.BlackboardScripts;
using Derrixx.BehaviourTrees.Runtime.BlackboardScripts.BlackboardProperties;
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
			
			if (!(targetObject is Node node))
			{
				EditorGUI.PropertyField(position, property, label);
				return;
			}

			BlackboardProperty targetProperty = (BlackboardProperty)property.objectReferenceValue;
			Blackboard blackboard = node.BehaviourTree.Blackboard;

			if (!blackboard || blackboard.Properties.Count == 0)
			{
				EditorGUI.LabelField(position, label, new GUIContent("Blackboard is unassigned or empty!"));
				return;
			}

			BlackboardProperty[] options = GetPropertyOptions(blackboard);
			int indexOf = Array.IndexOf(options, targetProperty);
			bool valueIsUnassigned = indexOf < 0;

			if (options.Length == 0)
			{
				var style = new GUIStyle
				{
					wordWrap = true,
					normal =
					{
						textColor = GUI.skin.label.normal.textColor
					}
				};

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

		private static BlackboardProperty GetPopupValue(Rect position, GUIContent label, IReadOnlyList<BlackboardProperty> options, int index)
		{
			string[] keys = (from option in options select option.Key).ToArray();
			int i = EditorGUI.Popup(position, label.text, index, keys);
			
			return options[i];
		}

		private BlackboardProperty[] GetPropertyOptions(Blackboard blackboard)
		{
			return (from property in blackboard.Properties
				where fieldInfo.FieldType.IsInstanceOfType(property) select property).ToArray();
		}
	}
}