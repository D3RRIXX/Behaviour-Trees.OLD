using System;
using UnityEngine;

namespace Derrixx.BehaviourTrees.PropertyReferences
{
    [Serializable]
    public abstract class BlackboardPropertyReference
    {
        [SerializeField, HideInInspector] private BlackboardProperty _blackboardProperty;

        protected BlackboardPropertyReference() { }

        protected BlackboardPropertyReference(BlackboardProperty property)
        {
	        _blackboardProperty = property;
        }
        
        public string Key => _blackboardProperty.Key;

        public BlackboardProperty Property => _blackboardProperty;

        public abstract void AssignPropertyValue(Blackboard blackboard);

        public static BlackboardPropertyReference Create(BlackboardProperty property)
        {
	        BlackboardPropertyReference propertyReference = property switch
	        {
		        BoolBlackboardProperty x => new BoolPropertyReference(x),
		        FloatBlackboardProperty x => new FloatPropertyReference(x),
		        IntBlackboardProperty x => new IntPropertyReference(x),
		        ObjectBlackboardProperty x => new ObjectPropertyReference(x),
		        StringBlackboardProperty x => new StringPropertyReference(x),
		        Vector2BlackboardProperty x => new Vector2PropertyReference(x),
		        Vector2IntBlackboardProperty x => new Vector2IntPropertyReference(x),
		        Vector3BlackboardProperty x => new Vector3PropertyReference(x),
		        _ => throw new ArgumentOutOfRangeException(nameof(property))
	        };

	        return propertyReference;
        }
    }

    [Serializable]
    public abstract class BlackboardPropertyReference<T> : BlackboardPropertyReference
    {
        [SerializeField] private T _value;

        protected BlackboardPropertyReference(BlackboardProperty<T> blackboardProperty) : base(blackboardProperty)
        {
	        _value = blackboardProperty.Value;
        }
        
        public T Value
        {
            get => _value;
            set => _value = value;
        }
    }
}
