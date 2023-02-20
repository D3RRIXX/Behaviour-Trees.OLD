using Derrixx.BehaviourTrees.Editor.ViewScripts;
using UnityEngine;
using UnityEngine.UIElements;

namespace Derrixx.BehaviourTrees.Editor
{
	public class InspectorView : VisualElement
	{
		private UnityEditor.Editor _editor;

		public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }

		public void UpdateSelection(NodeView nodeView)
		{
			Clear();

			Object.DestroyImmediate(_editor);

			_editor = UnityEditor.Editor.CreateEditor(nodeView.Node);
			var container = new IMGUIContainer(() =>
			{
				if (!_editor.target)
					return;

				_editor.OnInspectorGUI();
				nodeView.UpdateDescription();
			});
			Add(container);
		}
	}
}