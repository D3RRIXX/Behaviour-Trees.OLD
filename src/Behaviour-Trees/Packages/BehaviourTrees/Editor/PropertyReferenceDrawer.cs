using Derrixx.BehaviourTrees;
using Derrixx.BehaviourTrees.PropertyReferences;
using UnityEditor;
using UnityEngine;

namespace Derrixx.BehaviourTrees.Editor.RunnerEditors
{
	[CustomPropertyDrawer(typeof(BlackboardPropertyReference))]
	public class PropertyReferenceDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{
			float labelWidth = EditorGUIUtility.labelWidth;
			float lineHeight = EditorGUIUtility.singleLineHeight;

			var propertyReference = property.managedReferenceValue as BlackboardPropertyReference;
			
			EditorGUI.LabelField(new Rect(rect.x, rect.y, labelWidth, lineHeight), propertyReference.Key);
			Rect position = new Rect(rect.x + labelWidth + 10, rect.y, rect.width - labelWidth - 10, lineHeight);
			GUIContent content = GUIContent.none;

			switch (propertyReference)
			{
				case BoolPropertyReference boolReference:
					boolReference.Value = EditorGUI.Toggle(position, content, boolReference.Value);
					break;
				case IntPropertyReference intReference:
					intReference.Value = EditorGUI.IntField(position, content, intReference.Value);
					break;
				case FloatPropertyReference floatReference:
					floatReference.Value = EditorGUI.FloatField(position, content, floatReference.Value);
					break;
				case Vector2PropertyReference vector2Reference:
					vector2Reference.Value = EditorGUI.Vector2Field(position, content, vector2Reference.Value);
					break;
				case Vector2IntPropertyReference vector2IntReference:
					vector2IntReference.Value = EditorGUI.Vector2IntField(position, content, vector2IntReference.Value);
					break;
				case Vector3PropertyReference vector3Reference:
					vector3Reference.Value = EditorGUI.Vector3Field(position, content, vector3Reference.Value);
					break;
				case ObjectPropertyReference objectReference:
					objectReference.Value = EditorGUI.ObjectField(position, content, objectReference.Value, typeof(Object), true);
					break;
				case StringPropertyReference objectReference:
					objectReference.Value = EditorGUI.TextField(position, content, objectReference.Value);
					break;
			}
		}
	}
}
