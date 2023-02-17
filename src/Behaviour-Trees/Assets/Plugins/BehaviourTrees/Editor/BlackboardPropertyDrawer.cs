using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Derrixx.BehaviourTrees.Runtime.BlackboardScripts;
using Derrixx.BehaviourTrees.Runtime.BlackboardScripts.BlackboardProperties;
using Derrixx.BehaviourTrees.Runtime.Nodes;
using UnityEditor;
using UnityEngine;

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

			int indexOf = ((List<BlackboardProperty>)blackboard.Properties).IndexOf(targetProperty);
			bool valueIsUnassigned = indexOf < 0;
			
			int index = Mathf.Max(0, indexOf);
			int newIndex = EditorGUI.Popup(position, label.text, index, GetPropertyOptions(blackboard));
			
			if (valueIsUnassigned || newIndex != index)
			{
				FieldInfo field = targetObject.GetType().GetField(property.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				field!.SetValue(targetObject, blackboard.Properties[newIndex]);

				EditorUtility.SetDirty(targetObject);
			}
		}

		private string[] GetPropertyOptions(Blackboard blackboard)
		{
			return (from property in blackboard.Properties where property.GetType() == fieldInfo.FieldType select property.Key).ToArray();
		}
	}
}