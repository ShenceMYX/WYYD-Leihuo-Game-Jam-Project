using System.Collections;
using System.Collections.Generic;
using Common;
using Sirenix.OdinInspector;
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

        public bool isMoving { get; private set; } = false;

        [SerializeField] private float jumpRequiredEnergy = 30;

        [SerializeField] private PhysicMaterial playerPhysicMat;
        private bool lastOnGround;

        public GameObject bodyPartPrefab;
        public int currentBodypartIndex;
        private List<GameObject> bodyParts = new List<GameObject>();
        public float dampSpeed = 2;
        public float dist = 1.5f;

        private void Start()
        {
            motor = GetComponent<PlayerMotor>();
			groundChecker = GetComponent<GroundChecker>();

            bodyParts.Add(transform.GetChild(0).gameObject);
            currentBodypartIndex++;
        }

        private void Update()
        {
            xInput = Input.GetAxisRaw("Horizontal");
            yInput = Input.GetAxisRaw("Vertical");

            Vector3 inputVec = new Vector3(xInput, 0, yInput).normalized;

            motor.ResetVelocityVec();

            //motor.MoveForward(yInput);
            //motor.Turn(xInput);

            isMoving = inputVec.magnitude != 0;

            if (inputVec.magnitude != 0)
            {
                motor.MoveForward(1);

                Matrix4x4 matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
                var skewInput = matrix.MultiplyPoint3x4(inputVec);

                motor.RotateToTarget(transform.position + skewInput);

                
            }

            if (Input.GetKeyDown(KeyCode.Space) && groundChecker.OnGround && jumpRequiredEnergy <= EnergyManager.Instance.currentEnergy)
            {
                playerPhysicMat.dynamicFriction = 0;
                motor.Jump();
                EnergyManager.Instance.SetCurrentEnergy(-jumpRequiredEnergy);
            }

            if(groundChecker.OnGround && !lastOnGround)
                playerPhysicMat.dynamicFriction = 0.6f;

            lastOnGround = groundChecker.OnGround;

            motor.SetVelocity();

        }

        public void ResetSpeed()
        {
            motor.ResetSpeed();
        }

        public void SetSpeedToAcceleratedSpeed()
        {
            motor.SetSpeedToAcceleratedSpeed();
        }

        [Button]
        public void GrowBody()
        {
            GameObject bodyPart = Instantiate(bodyPartPrefab, transform.FindChildByName("pivot" + currentBodypartIndex));
            currentBodypartIndex++;
            bodyParts.Add(bodyPart);

            bodyPart.transform.localPosition = new Vector3(0, 0, 0);
            bodyPart.transform.localScale = Vector3.one * 0.5f;
        }

        public void ReduceBody()
        {
            Destroy(bodyParts[currentBodypartIndex]);
            bodyParts.RemoveAt(currentBodypartIndex--);
            HealthManager.Instance.DecreaseHealth();
        }

        public void ReduceBodyNotHealth()
        {
            
        }

       
    }

}
