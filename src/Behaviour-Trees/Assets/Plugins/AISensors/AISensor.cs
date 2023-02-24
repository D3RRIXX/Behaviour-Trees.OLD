using System;
using UnityEngine;

namespace Plugins.AISensors
{
	public abstract class AISensor
	{
		protected Transform AIController { get; }

		public AISensor(Transform aiController)
		{
			AIController = aiController;
		}
	}

	[Flags]
	public enum SensorType
	{
		Vision = 1,
		Hearing = 2
	}
}