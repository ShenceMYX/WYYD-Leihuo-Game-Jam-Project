using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace ns
{
	/// <summary>
	///
	/// </summary>
	public class SoulCollectible : MonoBehaviour
	{
        public Soul soul { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                PlayerInputController.Instance.GrowBody(soul);
                Destroy(gameObject);
            }
            else if (other.CompareTag("Enemy"))
            {
                EnemyAI enemyAI = other.GetComponent<EnemyAI>();
                if (enemyAI.CurrentState == EnemyAI.EnemyState.chase)
                {
                    Destroy(gameObject);
                    enemyAI.chaseTarget = null;
                    enemyAI.TurnToOriginalColor();
                }
            }
        }
    }
}
