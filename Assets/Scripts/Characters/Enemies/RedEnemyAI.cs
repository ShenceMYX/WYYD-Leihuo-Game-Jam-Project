using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ns
{
    /// <summary>
    ///
    /// </summary>
    public class RedEnemyAI : EnemyAI
    {
        protected override void ChaseAI(Transform chaseTarget)
        {
            motor.MoveToTarget(chaseTarget.position);
        }
    }
}
