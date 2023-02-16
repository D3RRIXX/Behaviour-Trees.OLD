using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.BlackboardScripts
{
	[CreateAssetMenu(fileName = "New Blackboard", menuName = "Derrixx/Behaviour Trees/Blackboard", order = 0)]
	public class Blackboard : ScriptableObject
	{
		[SerializeField] private List<BlackboardProperty> _properties = new List<BlackboardProperty>();

		private void Reset()
		{
			BlackboardProperty<int> intProperty = CreateInstance<IntBlackboardProperty>();
			intProperty.Key = "Amogus";
			intProperty.Value = 25;
			
			_properties.Add(intProperty);
			AssetDatabase.AddObjectToAsset(intProperty, this);
			AssetDatabase.SaveAssets();
		}
	}
}