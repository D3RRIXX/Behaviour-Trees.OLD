using Derrixx.BehaviourTrees.Nodes;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Derrixx.BehaviourTrees.Editor
{
	[CustomEditor(typeof(Node), true, isFallback = true)]
	public class NodeEditor : UnityEditor.Editor
	{
		private SerializedProperty _nameProperty;
		private SerializedProperty _scriptProperty;

		protected virtual void OnEnable()
		{
			_nameProperty = serializedObject.FindProperty("nodeName");
			_scriptProperty = serializedObject.FindProperty("m_Script");
		}

		public override VisualElement CreateInspectorGUI()
		{
			var root = new VisualElement();
			root.Add(new TextField("Node Name")
			{
				bindingPath = "nodeName"
			});
			
			CreateInspectorGUI_Implementation(root);
			
			root.Bind(serializedObject);
			
			return root;
		}

		public sealed override void OnInspectorGUI()
		{
			serializedObject.Update();
			
			GUI.enabled = false;
			EditorGUILayout.PropertyField(_scriptProperty, true);
			GUI.enabled = true;
			EditorGUILayout.PropertyField(_nameProperty);
			
			OnInspectorGUI_Implementation();

			serializedObject.ApplyModifiedProperties();
		}

		protected virtual void OnInspectorGUI_Implementation()
		{
			DrawPropertiesExcluding(serializedObject, "m_Script", "nodeName");
		}

		protected virtual void CreateInspectorGUI_Implementation(VisualElement root)
		{
			var currentProperty = _nameProperty;
			while (_nameProperty.NextVisible(false))
			{
				root.Add(new PropertyField(currentProperty)
				{
					bindingPath = currentProperty.propertyPath,
					style =
					{
						height = EditorGUIUtility.singleLineHeight
					}
				});
			}
		}
	}
}
