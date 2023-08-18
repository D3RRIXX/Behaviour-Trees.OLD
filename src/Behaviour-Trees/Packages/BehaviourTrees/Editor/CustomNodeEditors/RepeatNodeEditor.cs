using Derrixx.BehaviourTrees.Nodes.Decorators;
using UnityEditor;

namespace Derrixx.BehaviourTrees.Editor
{
	[CustomEditor(typeof(RepeatNode), true)]
	public class RepeatNodeEditor : NodeEditor
	{
		private SerializedProperty _timesProperty;
		private SerializedProperty _repeatInfinitelyProperty;
		private SerializedProperty _resetProperty;

		protected override void OnEnable()
		{
			base.OnEnable();

			_timesProperty = serializedObject.FindProperty("timesToRepeat");
			_repeatInfinitelyProperty = serializedObject.FindProperty("repeatInfinitely");
			_resetProperty = serializedObject.FindProperty("_resetOnDeactivate");
		}

		protected override void OnInspectorGUI_Implementation()
		{
			EditorGUILayout.PropertyField(_repeatInfinitelyProperty);
			
			if (_repeatInfinitelyProperty.boolValue)
				return;

			EditorGUILayout.PropertyField(_timesProperty);
			EditorGUILayout.PropertyField(_resetProperty);
		}
	}
}
