using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Derrixx.BehaviourTrees.Runtime;
using Plugins.AISensors;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class TestAI : BehaviourTreeRunner
{
	[SerializeField] private Team team;
	[SerializeField, Min(0f)] private float fovDistance;
	[SerializeField, Range(0f, 360f)] private float fovAngle;
	[SerializeField] private LayerMask visionMask;

	private int _botMask;

	private readonly Collider[] _overlapResults = new Collider[10];

	public Team Team => team;

	private void Awake()
	{
		_botMask = LayerMask.GetMask("Bot");
	}

	public bool CanSee<T>(out T other) where T : Component
	{
		int overlapCount = Physics.OverlapSphereNonAlloc(transform.position, fovDistance, _overlapResults, _botMask);

		var targets = _overlapResults.Take(overlapCount)
			.Select(x => x.transform)
			.Where(t => t != transform)
			.OrderBy(t => Vector3.Distance(transform.position, t.position));

		foreach (Transform target in targets)
		{
			Vector3 direction = target.position - transform.position;
			
			if (Vector3.Angle(direction, transform.forward) > fovAngle)
				continue;

			if (Physics.Raycast(transform.position, direction, out RaycastHit hit, fovDistance, visionMask) && hit.transform == target)
			{
				other = target.GetComponent<T>();
				return true;
			}
		}
		
		other = null;
		return false;
	}
	
	private void Update()
	{
		RunBehaviourTree();
	}
}

public enum Team
{
	Blue,
	Red
}
