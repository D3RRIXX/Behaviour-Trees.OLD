using Derrixx.BehaviourTrees.Nodes;
using UnityEditor;

namespace Derrixx.BehaviourTrees.Editor
{
	[CustomEditor(typeof(TagCooldownNode))]
	public class TagCooldownNodeEditor : NodeEditor
	{
		private SerializedProperty _setCooldownOnDeactivateProperty;
		private SerializedProperty _cooldownDurationProperty;
		private SerializedProperty _addToDurationProperty;
		private SerializedProperty _cooldownTagProperty;

		protected override void OnEnable()
		{
			base.OnEnable();

			_cooldownTagProperty = serializedObject.FindProperty("_cooldownTag");
			_setCooldownOnDeactivateProperty = serializedObject.FindProperty("_setCooldownOnDeactivate");
			_cooldownDurationProperty = serializedObject.FindProperty("_cooldownDuration");
			_addToDurationProperty = serializedObject.FindProperty("_addToExistingDuration");
		}

		protected override void OnInspectorGUI_Implementation()
		{
			EditorGUILayout.PropertyField(_cooldownTagProperty);
			EditorGUILayout.PropertyField(_setCooldownOnDeactivateProperty);

			if (_setCooldownOnDeactivateProperty.boolValue)
			{
				EditorGUILayout.PropertyField(_cooldownDurationProperty);
				EditorGUILayout.PropertyField(_addToDurationProperty);
			}
		}
	}
}