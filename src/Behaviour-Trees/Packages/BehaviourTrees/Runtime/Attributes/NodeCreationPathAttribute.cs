using System;

namespace Derrixx.BehaviourTrees.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class NodeCreationPathAttribute : Attribute
	{
		public NodeCreationPathAttribute(string path)
		{
			Path = path;
		}

		public string Path { get; }
	}
}
