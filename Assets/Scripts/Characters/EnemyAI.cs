using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ns
{
	/// <summary>
	///
	/// </summary>
	public class EnemyAI : MonoBehaviour
	{
        enum EnemyState
        {
            patrol,
            chase,
            idle
        }

		private EnemyMotor motor;

        [SerializeField] private EnemyState currentState = EnemyState.patrol;

        [SerializeField] private Transform[] wayPoints;
        public int targetWayPointIndex;

        [SerializeField] private float waitTime = 1;
        private float startWaitTime;
        private bool isWaiting = false;

        private Transform chaseTarget;

        private float currentStealValue = 50;
        private float stealSpeed = 25;
        private float maxStealValue = 100;
        private MeshRenderer meshRenderer;
        private bool startStealing = false;

        private void Start()
        {
			motor = GetComponent<EnemyMotor>();
            meshRenderer = GetComponentInChildren<MeshRenderer>();
            //wayPoints = GetComponentInChildren<WayLine>().WayPoints;
            motor.MoveToTarget(wayPoints[targetWayPointIndex].position);
        }
        
        private void Update()
        {
            switch (currentState)
            {
                case EnemyState.patrol:
                    Patrol();
                    break;
                case EnemyState.chase:
                    Chase();
                    break;
                case EnemyState.idle:
                    Idle();
                    break;
            }

            if (startStealing)
            {
                currentState = EnemyState.idle;
                currentStealValue += stealSpeed * Time.deltaTime;
                float stealRatio = currentStealValue / maxStealValue;
                meshRenderer.material.SetFloat("_GradientIntensity", stealRatio);
                if(stealRatio >= 1)
                {
                    PlayerInputController.Instance.GrowBody();
                    startStealing = false;
                    chaseTarget = PlayerInputController.Instance.transform;
                    Invoke("StartChasing", 1);
                }
            }
        }

        private void StartChasing()
        {
            currentState = EnemyState.chase;
        }

        private void Idle()
        {
            motor.MoveToTarget(transform.position);
        }

        private void Chase()
        {
            motor.MoveToTarget(chaseTarget.position);
        }

        private void Patrol()
        {
            if (Vector3.Distance(transform.position, wayPoints[targetWayPointIndex].position) < 1f)
            {
                if (!isWaiting)
                {
                    isWaiting = true;
                    startWaitTime = waitTime;
                }
                else
                {
                    startWaitTime -= Time.deltaTime;

                    if (startWaitTime <= 0)
                    {
                        targetWayPointIndex = (targetWayPointIndex + 1) % wayPoints.Length;
                        motor.MoveToTarget(wayPoints[targetWayPointIndex].position);
                        isWaiting = false;
                    }
                }
            }
            else
            {
                motor.MoveToTarget(wayPoints[targetWayPointIndex].position);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
                {
                    //target = other.transform;
                    //currentState = EnemyState.chase;
                    startStealing = true;
                }

                if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
                {
                    startStealing = false;
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Player") && currentState == EnemyState.chase)
            {
                PlayerInputController.Instance.ReduceBody();
                currentState = EnemyState.patrol;
            }
        }
    }
}
