using UnityEngine;

namespace Derrixx.BehaviourTrees.Runtime.PropertyReferences
{
	public class BoolPropertyReference : BlackboardPropertyReference<bool>
	{
		public override void AssignPropertyValue(Blackboard blackboard)
		{
			blackboard.FindProperty<BoolBlackboardProperty>(Key).Value = Value;
		}
	}
	
	public class IntPropertyReference : BlackboardPropertyReference<int>
	{
		public override void AssignPropertyValue(Blackboard blackboard)
		{
			blackboard.FindProperty<IntBlackboardProperty>(Key).Value = Value;
		}
	}
	
	public class FloatPropertyReference : BlackboardPropertyReference<float>
	{
		public override void AssignPropertyValue(Blackboard blackboard)
		{
			blackboard.FindProperty<FloatBlackboardProperty>(Key).Value = Value;
		}
	}
	
	public class StringPropertyReference : BlackboardPropertyReference<string>
	{
		public override void AssignPropertyValue(Blackboard blackboard)
		{
			blackboard.FindProperty<StringBlackboardProperty>(Key).Value = Value;
		}
	}
	
	public class Vector2PropertyReference : BlackboardPropertyReference<Vector2>
	{
		public override void AssignPropertyValue(Blackboard blackboard)
		{
			blackboard.FindProperty<Vector2BlackboardProperty>(Key).Value = Value;
		}
	}
	
	public class Vector2IntPropertyReference : BlackboardPropertyReference<Vector2Int>
	{
		public override void AssignPropertyValue(Blackboard blackboard)
		{
			blackboard.FindProperty<Vector2IntBlackboardProperty>(Key).Value = Value;
		}
	}
	
	public class Vector3PropertyReference : BlackboardPropertyReference<Vector3>
	{
		public override void AssignPropertyValue(Blackboard blackboard)
		{
			blackboard.FindProperty<Vector3BlackboardProperty>(Key).Value = Value;
		}
	}
	
	public class ObjectPropertyReference : BlackboardPropertyReference<Object>
	{
		public override void AssignPropertyValue(Blackboard blackboard)
		{
			blackboard.FindProperty<ObjectBlackboardProperty>(Key).Value = Value;
		}
	}
}