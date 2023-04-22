using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Common;

namespace ns
{
	/// <summary>
	///
	/// </summary>
	public class DoorController : MonoSingleton<DoorController>
	{
		[SerializeField] private float targetY = 1.75f;
		[SerializeField] private float moveUpDuration = 3f;

		public void RaiseUp()
        {
			transform.DOMoveY(targetY, moveUpDuration);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && GameManager.Instance.CheckWinCondition())
            {
                GameManager.Instance.GameWin();
            }
        }
    }
}
