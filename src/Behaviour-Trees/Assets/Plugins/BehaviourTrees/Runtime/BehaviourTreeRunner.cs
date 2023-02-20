using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
	public sealed class BehaviourTreeRunner : MonoBehaviour
	{
		[SerializeField] private BehaviourTree _behaviourTree;

		private bool _createdClone;

		public BehaviourTree BehaviourTree
		{
			get
			{
				if (!_createdClone)
					_behaviourTree = CloneTree();

				return _behaviourTree;
			}
		}

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