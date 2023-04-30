using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime
{
	[CreateAssetMenu(fileName = "New Blackboard", menuName = Utils.SO_CREATION_PATH + "Blackboard", order = 0)]
	public class Blackboard : ScriptableObject
	{
		[SerializeField] private Blackboard _parent;
		[SerializeField] private List<BlackboardProperty> _properties = new List<BlackboardProperty>();

		public IReadOnlyList<BlackboardProperty> Properties => _parent != null ? _properties.Concat(_parent.Properties).ToList() : _properties;

		public BlackboardProperty FindProperty(string key) => Properties.FirstOrDefault(x => x.Key == key);
		public T FindProperty<T>(string key) where T : BlackboardProperty
			=> Properties.FirstOrDefault(x => x.Key == key && x.GetType() == typeof(T)) as T;

		internal Blackboard Clone()
		{
			Blackboard clone = Instantiate(this);
			clone.name = name;
			clone._properties = _properties.Select(x => x.Clone()).ToList();
			if (_parent)
				clone._parent = _parent.Clone();

			return clone;
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			if (_parent== null)
				return;

			if (_parent == this)
			{
				Debug.LogError("Can't assign self as a parent!", this);
				_parent = null;
				return;
			}
			
			_parent = CheckRecursiveParenting(this);
		}

		private static Blackboard CheckRecursiveParenting(Blackboard target)
		{
			StringBuilder recursionGraph = new();
			
			Blackboard original = target._parent;
			Blackboard parent = original;
			do
			{
				parent = parent._parent;
				if (parent != target)
					continue;

				Debug.LogError($"Detected recursive parenting! {recursionGraph}", target);
				return null;
				
			} while (parent != null);

			return original;
		}

		/// <summary>
		/// INTERNAL EDITOR USE ONLY
		/// </summary>
		/// <param name="valueType">Value type that the BlackboardProperty should have</param>
		public void AddProperty(BlackboardProperty.ValueType valueType)
		{
			if (Application.isPlaying)
			{
				Debug.LogException(new InvalidOperationException("Can't modify blackboard while in Play Mode!"));
				return;
			}
			
			BlackboardProperty property = BlackboardProperty.Create(string.Empty, valueType);
			property.hideFlags = HideFlags.HideInHierarchy;
			
			Undo.RecordObject(this, "Blackboard (Add Property)");
			
			_properties.Add(property);
			AssetDatabase.AddObjectToAsset(property, this);
			
			Undo.RegisterCreatedObjectUndo(property, "Blackboard (Add Property)");
			
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
		}

		/// <summary>
		/// INTERNAL EDITOR USE ONLY
		/// </summary>
		/// <param name="property">Property that should be removed</param>
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
#endif
	}
}