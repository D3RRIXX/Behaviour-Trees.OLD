using System;
using System.Collections;
using System.Collections.Generic;
using Derrixx.BehaviourTrees.Runtime.BlackboardScripts.BlackboardProperties;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Derrixx.BehaviourTrees.Runtime.BlackboardScripts
{
	[CreateAssetMenu(fileName = "New Blackboard", menuName = "Derrixx/Behaviour Trees/Blackboard", order = 0)]
	public class Blackboard : ScriptableObject
	{
		[SerializeField] private List<BlackboardProperty> _properties = new List<BlackboardProperty>();

		public IReadOnlyList<BlackboardProperty> Properties => _properties;

		private void Awake()
		{
			Debug.Log("Called <color=\"red\">Awake()</color>");
			AddProperty(typeof(IntBlackboardProperty));
		}

		public void AddProperty<T>() where T : BlackboardProperty => AddProperty(typeof(T));

		public void AddProperty(Type propertyType)
		{
			if (Application.isPlaying)
			{
				Debug.LogException(new InvalidOperationException("Can't modify blackboard while in Play Mode!"));
				return;
			}
			
			Assert.IsTrue(propertyType.IsSubclassOf(typeof(BlackboardProperty)));
			
			BlackboardProperty property = (BlackboardProperty)CreateInstance(propertyType);
			// property.hideFlags = HideFlags.HideInHierarchy;
			
			Undo.RecordObject(this, "Blackboard (Add Property)");
			
			_properties.Add(property);
			AssetDatabase.AddObjectToAsset(property, this);
			
			Undo.RegisterCreatedObjectUndo(property, "Blackboard (Add Property)");
			
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
		}

		public void RemoveProperty(BlackboardProperty property)
		{
			if (Application.isPlaying)
			{
				Debug.LogException(new InvalidOperationException("Can't modify blackboard while in Play Mode!"));
				return;
			}
			
			Undo.RecordObject(this, "Blackboard (Remove Property)");
			_properties.Remove(property);
			
			Undo.DestroyObjectImmediate(property);
			AssetDatabase.SaveAssets();
		}
	}
}