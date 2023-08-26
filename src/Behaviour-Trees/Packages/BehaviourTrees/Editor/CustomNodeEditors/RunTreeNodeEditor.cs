using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Derrixx.BehaviourTrees.Editor
{
	[CustomEditor(typeof(RunTreeNode), true)]
	public class RunTreeNodeEditor : NodeEditor
	{
		private HelpBox _helpBox;
		private RunTreeNode _runTreeNode;

		protected override void OnEnable()
		{
			_runTreeNode = target as RunTreeNode;
		}

		protected override VisualElement CreateInspectorGUI_Implementation()
		{
			var root = new VisualElement();
			
			var subTreeField = new ObjectField("Sub Tree") { bindingPath = "_behaviourTree", objectType = typeof(BehaviourTree) };
			subTreeField.RegisterValueChangedCallback(OnSubtreeChanged);
			
			_helpBox = new HelpBox(string.Empty, HelpBoxMessageType.Error);
			
			root.Add(subTreeField);
			root.Add(_helpBox);
			
			return root;
		}

		private void OnSubtreeChanged(ChangeEvent<Object> evt)
		{
			if (evt.newValue is not BehaviourTree subTree)
			{
				_helpBox.SetActive(false);
				return;
			}

			if (subTree == _runTreeNode.BehaviourTree)
			{
				_helpBox.text = $"You've assigned '{subTree.name}' as a subtree of self. This will lead to stack overflow!";
				_helpBox.SetActive(true);
			}
			else if (subTree.Blackboard != _runTreeNode.BehaviourTree.Blackboard)
			{
				_helpBox.text = "This node will not run because it uses a different Blackboard!";
				_helpBox.SetActive(true);
			}
			else
			{
				_helpBox.SetActive(false);
			}
		}
	}
}
