using Derrixx.BehaviourTrees.Runtime.Nodes;
using UnityEngine;

namespace DefaultNamespace
{
	public class LogNode : ActionNode
	{
		[SerializeField] private string message;
		
		public string Message
		{
			get => message;
			set => message = value;
		}

		protected override void OnStart()
		{
			Debug.Log($"OnStart {message}");
		}

		protected override void OnFinish()
		{
			Debug.Log($"OnFinish {message}");
		}
		protected override State OnUpdate()
		{
			Debug.Log($"OnUpdate {message}");
			return State.Success;
		}

		public override string GetDescription()
		{
			return $"Print \"{message}\"";
		}
	}
}