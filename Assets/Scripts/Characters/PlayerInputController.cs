using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace ns
{
	/// <summary>
	///
	/// </summary>
	public class PlayerInputController : MonoSingleton<PlayerInputController>
	{
        private float xInput, yInput;

        private PlayerMotor motor;
        private GroundChecker groundChecker;

        private void Start()
        {
            motor = GetComponent<PlayerMotor>();
			groundChecker = GetComponent<GroundChecker>();
        }

        private void Update()
        {
            xInput = Input.GetAxisRaw("Horizontal");
            yInput = Input.GetAxisRaw("Vertical");

            Vector3 inputVec = new Vector3(xInput, 0, yInput).normalized;

            motor.ResetVelocityVec();

            //motor.MoveForward(yInput);
            //motor.Turn(xInput);

            if (inputVec.magnitude != 0)
            {
                motor.MoveForward(1);

                Matrix4x4 matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
                var skewInput = matrix.MultiplyPoint3x4(inputVec);

                motor.RotateToTarget(transform.position + skewInput);
            }

            if (Input.GetKeyDown(KeyCode.Space) && groundChecker.OnGround)
                motor.Jump();

            motor.SetVelocity();
        }
    }
}
