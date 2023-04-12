using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ns
{
	/// <summary>
	///
	/// </summary>
	public class PlayerInputController : MonoBehaviour
	{
        private float xInput, yInput;

        private PlayerMotor motor;

        private void Start()
        {
            motor = GetComponent<PlayerMotor>();
        }

        private void Update()
        {
            xInput = Input.GetAxisRaw("Horizontal");
            yInput = Input.GetAxisRaw("Vertical");

            motor.Movement(new Vector3(xInput, 0, yInput));
        }
    }
}
