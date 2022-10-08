using Derrixx.BehaviourTrees.Runtime.Pure;

namespace Derrixx.BehaviourTrees.Runtime
{
	public interface INode
	{
		INode Parent { get; set; }
		
		NodeState Execute();

		void InjectBlackboard(IBlackboard blackboard);
	}
}