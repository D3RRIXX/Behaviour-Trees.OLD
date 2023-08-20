using Derrixx.BehaviourTrees.Nodes;
using UnityEditor;
using UnityEngine.UIElements;

namespace Derrixx.BehaviourTrees.Editor
{
	[CustomEditor(typeof(TagCooldownNode))]
	public class TagCooldownNodeEditor : NodeEditor
	{
		/*private SerializedProperty _setCooldownOnDeactivateProperty;
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
		}*/

		protected override VisualElement CreateInspectorGUI_Implementation()
		{
			var root = new VisualElement();
			var setCooldownToggle = new Toggle("Set Cooldown On Deactivate") { bindingPath = "_setCooldownOnDeactivate" };
			root.Add(setCooldownToggle);

			var cooldownContainer = new VisualElement { name = "cooldown-container" };
			cooldownContainer.Add(new FloatField("Cooldown Duration") { bindingPath = "_cooldownDuration" });
			cooldownContainer.Add(new Toggle("Add To Existing Duration") { bindingPath = "_addToExistingDuration" });
			
			root.Add(cooldownContainer);

			setCooldownToggle.RegisterValueChangedCallback(evt => cooldownContainer.visible = evt.newValue);
			
			return root;
		}
	}
}