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
		public int maxCapacity = 2;
		public int currentCapacity = 0;

		private void OnTriggerStay(Collider other)
		{
            if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.Q))
            {
                
            }
		}


	}
}
