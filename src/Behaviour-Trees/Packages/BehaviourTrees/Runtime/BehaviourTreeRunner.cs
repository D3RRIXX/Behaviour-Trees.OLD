using System.Collections.Generic;
using System.Linq;
using Derrixx.BehaviourTrees.Runtime.PropertyReferences;
using UnityEngine;

#if BTREE_ADD_ZENJECT
using Zenject;
#endif

namespace Derrixx.BehaviourTrees.Runtime
{
	/// <summary>
	/// Class used to assign and run <see cref="Runtime.BehaviourTree"/>
	/// </summary>
	public abstract partial class BehaviourTreeRunner : MonoBehaviour
	{
		[SerializeField] private BehaviourTree _behaviourTree;
		[SerializeField, SerializeReference] private List<BlackboardPropertyReference> _propertyReferences = new();

		[SerializeField, HideInInspector] private Blackboard _cachedBlackboard;
		
		private bool _createdClone;

		/// <summary>
		/// Returns this runner's Behaviour Tree
		/// </summary>
		private BehaviourTree BehaviourTree
		{
			get
			{
				if (!_createdClone)
					_behaviourTree = CloneTree();

				return _behaviourTree;
			}
		}

		public Blackboard Blackboard => BehaviourTree.Blackboard;

#if BTREE_ADD_ZENJECT
		[Inject]
		private void ConstructTreeRunner(DiContainer container)
		{
			BehaviourTree.TraverseNodes(BehaviourTree.RootNode, container.Inject);
		}
#endif

		public void RunBehaviourTree()
		{
			BehaviourTree.Update();
		}

		private BehaviourTree CloneTree()
		{
			_createdClone = true;

			BehaviourTree cloneTree = _behaviourTree.Clone(this, blackboard =>
			{
				foreach (var propertyReference in _propertyReferences)
				{
					propertyReference.AssignPropertyValue(blackboard);
				}
			});
			
			return cloneTree;
		}

		private void SyncBlackboards()
		{
			if (_behaviourTree == null)
			{
				_propertyReferences.Clear();
				return;
			}

			var blackboard = _behaviourTree.Blackboard;
			if (blackboard == null)
			{
				_propertyReferences.Clear();
				return;
			}

			static bool ShouldReferenceProperty(BlackboardProperty blackboardProperty) => !blackboardProperty.InstanceSynced && blackboardProperty.IsExposed;
			
			List<BlackboardProperty> instancedProperties = blackboard.Properties
				.Where(ShouldReferenceProperty)
				.ToList();

			if (_cachedBlackboard != blackboard)
			{
				_cachedBlackboard = blackboard;
				_propertyReferences = instancedProperties
					.Select(BlackboardPropertyReference.Create)
					.ToList();

				return;
			}

			foreach (BlackboardProperty property in instancedProperties.Where(property => _propertyReferences.All(reference => reference.Property != property)))
			{
				_propertyReferences.Add(BlackboardPropertyReference.Create(property));
			}

			var propertiesToRemove = _propertyReferences.Where(reference => reference.Property == null || !ShouldReferenceProperty(reference.Property)).ToList();
			foreach (BlackboardPropertyReference propertyReference in propertiesToRemove)
			{
				_propertyReferences.Remove(propertyReference);
			}
		}
	}
}