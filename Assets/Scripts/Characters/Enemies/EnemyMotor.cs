using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ns
{
	/// <summary>
	///
	/// </summary>
	public class EnemyMotor : CharacterMotor
	{
		private NavMeshAgent agent;

        private void Awake()
        {
			agent = GetComponent<NavMeshAgent>();
			agent.speed = moveSpeed;
        }

		public void MoveToTarget(Vector3 targetPos)
        {
			agent.SetDestination(targetPos);
        }
    }
}
