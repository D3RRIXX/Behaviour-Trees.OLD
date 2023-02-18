using System;

namespace Derrixx.BehaviourTrees.Runtime.Nodes
{
	public class SequenceNode : CompositeNode
	{
		private int _current;

		protected override void OnStart()
		{
			_current = 0;
		}
		protected override void OnFinish() { }

		protected override State OnUpdate()
		{
			if (Children.Count == 0)
				return State.Success;
			
			Node currentChild = Children[_current];

			State updateResult = currentChild.Update();
			switch (updateResult)
			{
				case State.Running:
				case State.Failure:
					return updateResult;
				case State.Success:
					_current++;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return _current == Children.Count ? State.Success : State.Running;
		}
	}
}