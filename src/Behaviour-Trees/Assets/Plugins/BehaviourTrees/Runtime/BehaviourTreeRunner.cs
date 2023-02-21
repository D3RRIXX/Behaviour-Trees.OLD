using Derrixx.BehaviourTrees.Runtime.BlackboardScripts;
using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime
{
	/// <summary>
	/// Class used to assign and run <see cref="BehaviourTrees.Runtime.BehaviourTree"/>
	/// </summary>
	public sealed class BehaviourTreeRunner : MonoBehaviour
	{
		[SerializeField] private BehaviourTree _behaviourTree;

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

		private void Update()
		{
			BehaviourTree.Update();
		}

		private BehaviourTree CloneTree()
		{
			_createdClone = true;
			return _behaviourTree.Clone();
		}
	}
}