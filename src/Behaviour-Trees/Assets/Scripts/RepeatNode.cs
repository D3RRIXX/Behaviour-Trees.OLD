using Derrixx.BehaviourTrees.Runtime.Nodes;

namespace DefaultNamespace
{
	public class RepeatNode : DecoratorNode
	{
		protected override void OnStart()
		{
			
		}

		protected override void OnFinish()
		{
			
		}
		
		protected override State OnUpdate()
		{
			Child.Update();
			return State.Running;
		}
	}
}