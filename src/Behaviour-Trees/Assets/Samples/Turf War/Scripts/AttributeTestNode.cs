﻿using Derrixx.BehaviourTrees;
using Derrixx.BehaviourTrees.Nodes;

namespace DefaultNamespace
{
	[NodeCreationPath("Custom/Prikol/Attribute Test (or some funny name)")]
	public class AttributeTestNode : ActionNode
	{
		protected override State OnUpdate() => State.Running;
	}
}