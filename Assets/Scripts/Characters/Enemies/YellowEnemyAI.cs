using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ns
{
    /// <summary>
    ///
    /// </summary>
    public class YellowEnemyAI : EnemyAI
    {
        [SerializeField] private float chaseSoulMinDistance = 4;
        [SerializeField] private float chaseWaitTime = 4;
        private float startWaitTimer;


        protected override void ChaseAI(Transform chaseTarget)
        {
            startWaitTimer += Time.deltaTime;

            if (startWaitTimer < chaseWaitTime)
            {
                Chase(chaseTarget);
            }
            else if(startWaitTimer < chaseWaitTime * 2)
            {
                Wait();
            }
            else
            {
                startWaitTimer = 0;
            }
            
        }

        private void Wait()
        {
            motor.MoveToTarget(transform.position);
        }

        private void Chase(Transform chaseTarget)
        {
            Transform playerTrans = PlayerInputController.Instance.transform;
            if (Vector3.Distance(playerTrans.position, transform.position) > chaseSoulMinDistance)
            {
                Vector3 targetPos = playerTrans.position + playerTrans.forward * chaseSoulMinDistance;
                motor.MoveToTarget(targetPos);
            }
            else
            {
                motor.MoveToTarget(chaseTarget.position);
            }
        }
    }
}
