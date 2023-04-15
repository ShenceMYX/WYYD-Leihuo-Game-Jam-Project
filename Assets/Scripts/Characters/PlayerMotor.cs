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

		[SerializeField] private float jumpSpeed = 5;
		[SerializeField] private float turnSpeed = 50;

		[SerializeField] private Vector3 moveUpGravity = new Vector3(0, -9.81f, 0);
		[SerializeField] private Vector3 moveDownGravity = new Vector3(0, -19.62f, 0);

		private Vector3 initialVelocity;

		private void Start()
		{
			rb = GetComponent<Rigidbody>();
		}

        private void FixedUpdate()
        {
			if (rb.velocity.y > 0)
				Physics.gravity = moveUpGravity;
			else if(rb.velocity.y < 0)
				Physics.gravity = moveDownGravity;
		}

		public void ResetVelocityVec()
        {
			initialVelocity = rb.velocity;
		}

		//public void Movement(Vector3 direction)
		//{
		//	rb.velocity = direction * moveSpeed;
		//}

		public void MoveForward(float forwardInput)
        {
			Vector3 moveForwardVec = transform.forward * moveSpeed * forwardInput;
			initialVelocity = new Vector3(moveForwardVec.x, rb.velocity.y, moveForwardVec.z);
        }

		public void RotateToTarget(Vector3 target)
        {
			Quaternion targetRot = Quaternion.LookRotation(target - transform.position, Vector3.up);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        }

		//public void Turn(float horizontalInput)
        //{
		//	rb.angularVelocity = new Vector3(0, horizontalInput * turnSpeed, 0);
        //}

		public void Jump()
        {
			initialVelocity += new Vector3(0, jumpSpeed, 0);
		}

		public void SetVelocity()
        {
			rb.velocity = initialVelocity;
        }
	}
}
