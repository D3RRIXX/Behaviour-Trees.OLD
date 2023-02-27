using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime
{
	public partial class BehaviourTreeRunner
	{
#if UNITY_EDITOR
		[CustomEditor(typeof(BehaviourTreeRunner), true)]
		public class BehaviourTreeRunnerEditor : Editor
		{
			private const string PROPERTY_LIST_NAME = "_propertyReferences";
			private const string TREE_PROPERTY_NAME = "_behaviourTree";

			private ReorderableList _propertyList;
			private SerializedProperty _treeSerializedProperty;
			private SerializedProperty _serializedListProperty;
			
			private BehaviourTreeRunner _behaviourTreeRunner;

			private void OnEnable()
			{
				_behaviourTreeRunner = serializedObject.targetObject as BehaviourTreeRunner;
				_treeSerializedProperty = serializedObject.FindProperty(TREE_PROPERTY_NAME);
				_serializedListProperty = serializedObject.FindProperty(PROPERTY_LIST_NAME);
				_propertyList = SetupPropertyList();
			}

			private ReorderableList SetupPropertyList()
			{
				return new ReorderableList(serializedObject, _serializedListProperty, false, true, false, false)
				{
					drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Property References"),
					drawElementCallback = DrawElement
				};
			}

			private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
			{
				rect.y += EditorGUIUtility.singleLineHeight / 6f;
				EditorGUI.PropertyField(rect, _serializedListProperty.GetArrayElementAtIndex(index));
			}

			public override void OnInspectorGUI()
			{
				EditorGUI.BeginChangeCheck();
				_behaviourTreeRunner.SyncBlackboards();
				serializedObject.Update();

				EditorGUILayout.PropertyField(_treeSerializedProperty);

				if (!Application.isPlaying)
					_propertyList.DoLayoutList();
				
				DrawPropertiesExcluding(serializedObject, "m_Script", TREE_PROPERTY_NAME, PROPERTY_LIST_NAME);
				
				if (EditorGUI.EndChangeCheck())
				{
					EditorUtility.SetDirty(serializedObject.targetObject);
				}
				
				serializedObject.ApplyModifiedProperties();
			}
		}
#endif
	}
}