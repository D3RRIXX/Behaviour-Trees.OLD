using System.Collections.Generic;
using System.Linq;
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
			if (!(property.serializedObject.targetObject is Node node))
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
			
			int index = Mathf.Max(0, ((List<BlackboardProperty>)blackboard.Properties).IndexOf(targetProperty));
			index = EditorGUI.Popup(position, label.text, index, GetPropertyOptions(blackboard));

			property.objectReferenceValue = blackboard.Properties[index];
		}

		private string[] GetPropertyOptions(Blackboard blackboard)
		{
			return (from property in blackboard.Properties where property.GetType() == fieldInfo.FieldType select property.Key).ToArray();
		}
	}
}