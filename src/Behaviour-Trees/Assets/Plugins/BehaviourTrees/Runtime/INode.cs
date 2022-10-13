using Derrixx.BehaviourTrees.Runtime.Pure;

namespace Derrixx.BehaviourTrees.Runtime
{
	public interface INode
	{
		INode Parent { get; set; }

		void OnNodeEnter();
		void OnNodeExit();
		
		NodeState Execute();

		void InjectBlackboard(IBlackboard blackboard);
	}
}