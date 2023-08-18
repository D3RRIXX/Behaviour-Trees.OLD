using Derrixx.BehaviourTrees.Nodes.Actions;
using UnityEngine;

namespace Tests.Trees
{
	public class LogNode : ActionNode
	{
		[SerializeField] private string _message;

		protected override State OnUpdate()
		{
			Debug.Log(_message, Runner);
			return State.Success;
		}
	}
}