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
            chase
        }

		private EnemyMotor motor;

        [SerializeField] private EnemyState currentState = EnemyState.patrol;

        [SerializeField] private Transform[] wayPoints;
        public int targetWayPointIndex;

        [SerializeField] private float waitTime = 1;
        private float startWaitTime;
        private bool isWaiting = false;

        private void Start()
        {
			motor = GetComponent<EnemyMotor>();
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
            }
        }

        private void Chase()
        {
            throw new NotImplementedException();
        }

        private void Patrol()
        {
            //Debug.Log(Vector3.Distance(transform.position, wayPoints[targetWayPointIndex].position));
            if (Vector3.Distance(transform.position, wayPoints[targetWayPointIndex].position) < 0.5f)
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
        }
    }
}
