using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ns
{
	/// <summary>
	///
	/// </summary>
	public class Box : MonoBehaviour
	{
		private void OnTriggerStay(Collider other)
		{
            if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.Z))
            {
				PlayerInputController.Instance.ReduceBodyNotHealth();
                
            }
		}


	}
}
