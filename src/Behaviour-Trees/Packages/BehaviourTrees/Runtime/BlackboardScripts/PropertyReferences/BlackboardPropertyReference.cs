using System;
using UnityEngine;

namespace Derrixx.BehaviourTrees.PropertyReferences
{
    [Serializable]
    public abstract class BlackboardPropertyReference
    {
        [SerializeField, HideInInspector] private BlackboardProperty _blackboardProperty;
        
        public string Key => _blackboardProperty.Key;

        public BlackboardProperty Property
        {
	        get => _blackboardProperty;
	        set => _blackboardProperty = value;
        }

        public abstract void AssignPropertyValue(Blackboard blackboard);

        public static BlackboardPropertyReference Create(BlackboardProperty property)
        {
	        BlackboardPropertyReference propertyReference = property switch
	        {
		        BoolBlackboardProperty => new BoolPropertyReference(),
		        FloatBlackboardProperty => new FloatPropertyReference(),
		        IntBlackboardProperty => new IntPropertyReference(),
		        ObjectBlackboardProperty => new ObjectPropertyReference(),
		        StringBlackboardProperty => new StringPropertyReference(),
		        Vector2BlackboardProperty => new Vector2PropertyReference(),
		        Vector2IntBlackboardProperty => new Vector2IntPropertyReference(),
		        Vector3BlackboardProperty => new Vector3PropertyReference(),
		        _ => throw new ArgumentOutOfRangeException(nameof(property))
	        };

	        propertyReference.Property = property;
	        return propertyReference;
        }
    }

    [Serializable]
    public abstract class BlackboardPropertyReference<T> : BlackboardPropertyReference
    {
        [SerializeField] private T _value;
        
        public T Value
        {
            get => _value;
            set => _value = value;
        }
    }
}
