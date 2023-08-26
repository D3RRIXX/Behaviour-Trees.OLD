using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Derrixx.BehaviourTrees.Editor
{
	[CustomEditor(typeof(RepeatNode), true)]
	public class RepeatNodeEditor : NodeEditor
	{
		// protected override void OnInspectorGUI_Implementation()
		// {
		// 	var repeatInfinitelyProperty = serializedObject.FindProperty("repeatInfinitely");
		// 	EditorGUILayout.PropertyField(repeatInfinitelyProperty);
		// 	
		// 	if (repeatInfinitelyProperty.boolValue)
		// 		return;
		//
		// 	EditorGUILayout.PropertyField(serializedObject.FindProperty("timesToRepeat"));
		// 	EditorGUILayout.PropertyField(serializedObject.FindProperty("_resetOnDeactivate"));
		// }

		protected override VisualElement CreateInspectorGUI_Implementation()
		{
			var root = new VisualElement();

			var repeatInfinitelyField = new PropertyField(serializedObject.FindProperty("repeatInfinitely"));
			
			var subContainer = new VisualElement();
			subContainer.Add(new PropertyField(serializedObject.FindProperty("timesToRepeat")));
			subContainer.Add(new PropertyField(serializedObject.FindProperty("_resetOnDeactivate")));
			
			repeatInfinitelyField.RegisterValueChangeCallback(evt =>
			{
				subContainer.SetActive(evt.changedProperty.boolValue);
			});

			root.Add(repeatInfinitelyField);
			root.Add(subContainer);

			return root;
		}
	}
}
