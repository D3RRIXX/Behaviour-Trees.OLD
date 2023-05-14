using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime
{
	[AddComponentMenu("Behaviour Trees/Behaviour Tree Runner Base")]
	public class BehaviourTreeRunnerBase : BehaviourTreeRunner
	{
		private void Update()
		{
			RunBehaviourTree();
		}
	}
}