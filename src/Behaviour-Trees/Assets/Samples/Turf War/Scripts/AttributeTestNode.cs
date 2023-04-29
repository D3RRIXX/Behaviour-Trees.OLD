using Derrixx.BehaviourTrees.Runtime.Attributes;
using Derrixx.BehaviourTrees.Runtime.Nodes;
using UnityEngine;

namespace DefaultNamespace
{
	[NodeCreationPath("Custom/Prikol/Attribute Test (or some funny name)")]
	public class AttributeTestNode : ActionNode
	{
		protected override State OnUpdate() => State.Running;
	}
}