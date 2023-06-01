using UnityEngine;

namespace Derrixx.BehaviourTrees.Nodes
{
	public class SequenceNode : CompositeNode
	{
		[Tooltip("Re-evaluate all children nodes every update?")]
		[SerializeField] private bool _dynamic;
		
		private int _currentChildIndex;
		protected virtual State FinalState => State.Success;

		public override string GetDescription()
		{
			string description = base.GetDescription();
			if (_dynamic)
				description += " (Dynamic)";
			
			return description;
		}

		protected override State OnUpdate()
		{
			for (_currentChildIndex = _dynamic ? 0 : _currentChildIndex % Children.Count; _currentChildIndex < Children.Count; _currentChildIndex++)
			{
				Node currentChild = Children[_currentChildIndex];
				State updateResult = currentChild.Update();

				if (updateResult != State.Success)
					return updateResult;
			}

			return State.Success;
		}
	}
}
