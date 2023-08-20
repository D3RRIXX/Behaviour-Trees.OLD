using Derrixx.BehaviourTrees.Nodes;
using UnityEditor;
using UnityEngine.UIElements;

namespace Derrixx.BehaviourTrees.Editor
{
	[CustomEditor(typeof(TagCooldownNode))]
	public class TagCooldownNodeEditor : NodeEditor
	{
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