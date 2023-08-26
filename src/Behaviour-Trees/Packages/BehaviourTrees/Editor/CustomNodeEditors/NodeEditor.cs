using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Derrixx.BehaviourTrees.Editor
{
	[CustomEditor(typeof(Node), true, isFallback = true)]
	public class NodeEditor : UnityEditor.Editor
	{
		protected virtual void OnEnable() { }

		public sealed override VisualElement CreateInspectorGUI()
		{
			var root = new VisualElement();
			root.Add(new TextField("Node Name")
			{
				bindingPath = "nodeName"
			});

			var childGUI = CreateInspectorGUI_Implementation();
			root.Add(childGUI ?? new IMGUIContainer(() =>
			{
				serializedObject.Update();
				OnInspectorGUI_Implementation();
				serializedObject.ApplyModifiedProperties();
			}));
			
			root.Bind(serializedObject);
			
			return root;
		}

		protected virtual void OnInspectorGUI_Implementation()
		{
			DrawPropertiesExcluding(serializedObject, "m_Script", "nodeName");
		}

		protected virtual VisualElement CreateInspectorGUI_Implementation()
		{
			var childRoot = new VisualElement();
			var currentProperty = serializedObject.FindProperty("nodeName");
			while (currentProperty.NextVisible(false))
			{
				childRoot.Add(new PropertyField(currentProperty)
				{
					bindingPath = currentProperty.propertyPath,
					style =
					{
						height = EditorGUIUtility.singleLineHeight
					}
				});
			}
			
			return childRoot;
		}
	}
}
