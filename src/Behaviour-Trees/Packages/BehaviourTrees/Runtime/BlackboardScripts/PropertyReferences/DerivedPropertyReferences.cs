using UnityEngine;

namespace Derrixx.BehaviourTrees.PropertyReferences
{
	public class BoolPropertyReference : BlackboardPropertyReference<bool>
	{
		public override void AssignPropertyValue(Blackboard blackboard)
		{
			blackboard.FindProperty<BoolBlackboardProperty>(Key).Value = Value;
		}

		public BoolPropertyReference(BlackboardProperty<bool> blackboardProperty) : base(blackboardProperty) { }
	}
	
	public class IntPropertyReference : BlackboardPropertyReference<int>
	{
		public override void AssignPropertyValue(Blackboard blackboard)
		{
			blackboard.FindProperty<IntBlackboardProperty>(Key).Value = Value;
		}

		public IntPropertyReference(BlackboardProperty<int> blackboardProperty) : base(blackboardProperty) { }
	}
	
	public class FloatPropertyReference : BlackboardPropertyReference<float>
	{
		public override void AssignPropertyValue(Blackboard blackboard)
		{
			blackboard.FindProperty<FloatBlackboardProperty>(Key).Value = Value;
		}

		public FloatPropertyReference(BlackboardProperty<float> blackboardProperty) : base(blackboardProperty) { }
	}
	
	public class StringPropertyReference : BlackboardPropertyReference<string>
	{
		public override void AssignPropertyValue(Blackboard blackboard)
		{
			blackboard.FindProperty<StringBlackboardProperty>(Key).Value = Value;
		}

		public StringPropertyReference(BlackboardProperty<string> blackboardProperty) : base(blackboardProperty) { }
	}
	
	public class Vector2PropertyReference : BlackboardPropertyReference<Vector2>
	{
		public override void AssignPropertyValue(Blackboard blackboard)
		{
			blackboard.FindProperty<Vector2BlackboardProperty>(Key).Value = Value;
		}

		public Vector2PropertyReference(BlackboardProperty<Vector2> blackboardProperty) : base(blackboardProperty) { }
	}
	
	public class Vector2IntPropertyReference : BlackboardPropertyReference<Vector2Int>
	{
		public override void AssignPropertyValue(Blackboard blackboard)
		{
			blackboard.FindProperty<Vector2IntBlackboardProperty>(Key).Value = Value;
		}

		public Vector2IntPropertyReference(BlackboardProperty<Vector2Int> blackboardProperty) : base(blackboardProperty) { }
	}
	
	public class Vector3PropertyReference : BlackboardPropertyReference<Vector3>
	{
		public override void AssignPropertyValue(Blackboard blackboard)
		{
			blackboard.FindProperty<Vector3BlackboardProperty>(Key).Value = Value;
		}

		public Vector3PropertyReference(BlackboardProperty<Vector3> blackboardProperty) : base(blackboardProperty) { }
	}
	
	public class ObjectPropertyReference : BlackboardPropertyReference<Object>
	{
		public override void AssignPropertyValue(Blackboard blackboard)
		{
			blackboard.FindProperty<ObjectBlackboardProperty>(Key).Value = Value;
		}

		public ObjectPropertyReference(BlackboardProperty<Object> blackboardProperty) : base(blackboardProperty) { }
	}
}
