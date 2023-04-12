using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ns
{
	/// <summary>
	///
	/// </summary>
	public class PlayerMotor : CharacterMotor
	{
		private Rigidbody rb;

		private void Start()
		{
			rb = GetComponent<Rigidbody>();
		}

		public void Movement(Vector3 direction)
		{
			rb.velocity = direction * moveSpeed;
		}
	}
}
