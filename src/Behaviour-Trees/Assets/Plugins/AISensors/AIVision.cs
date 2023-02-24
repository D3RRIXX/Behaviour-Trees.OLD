using System;
using System.Linq;
using UnityEngine;

namespace Plugins.AISensors
{
	[Serializable]
	public class AIVision : AISensor
	{
		[SerializeField, Min(0f)] private float _fovDistance;
		[SerializeField, Range(0f, 360f)] private float _fovAngle;
		[SerializeField] private LayerMask _initialTargetMask;
		[SerializeField] private LayerMask _visionMask;

		private readonly Collider[] _overlapResults = new Collider[10];

		public AIVision(Transform aiController) : base(aiController) { }

		public bool CanSee<T>(out T other) where T : Component
		{
			int overlapCount = Physics.OverlapSphereNonAlloc(AIController.position, _fovDistance, _overlapResults, _initialTargetMask);

			var targets = _overlapResults.Take(overlapCount)
				.Select(x => x.transform)
				.Where(t => t != AIController)
				.OrderBy(t => Vector3.Distance(AIController.position, t.position));

			foreach (Transform target in targets)
			{
				Vector3 direction = target.position - AIController.position;

				if (Vector3.Angle(direction, AIController.forward) > _fovAngle)
					continue;

				if (!Physics.Raycast(AIController.position, direction, out RaycastHit hit, _fovDistance, _visionMask))
					continue;

				if (hit.transform == target && hit.transform.TryGetComponent(out other))
					return true;
			}

			other = null;
			return false;
		}
	}
}