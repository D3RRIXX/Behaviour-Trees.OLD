using UnityEngine;

namespace Samples.Burglar.Scripts
{
	public class Door : MonoBehaviour
	{
		[SerializeField] private bool isLocked;
		[SerializeField] private float timeToUnlock;
	}
}