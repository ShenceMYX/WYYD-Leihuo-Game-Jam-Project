using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace ns
{
	/// <summary>
	///
	/// </summary>
    public enum SoulColor
    {
        red,
        yellow,
        blue,
        purple
    }

    public class EnemyAI : MonoBehaviour
	{
        public enum EnemyState
        {
            patrol,
            chase,
            paralysis,
            waitForChase
        }


		protected EnemyMotor motor;

        [SerializeField] private SoulColor enemySoulColor;

        private Soul enemySoul;
        public Soul EnemySoul { get { return enemySoul; } }
        [SerializeField] private EnemyState currentState = EnemyState.patrol;
        public EnemyState CurrentState { get { return currentState; } }

        [SerializeField] private Transform[] wayPoints;
        public int targetWayPointIndex;

        [SerializeField] private float patrolWaitTime = 1;
        private float startWaitTime;
        private bool isWaiting = false;

        public Transform chaseTarget;

        private float currentStealValue = 0;
        private float stealSpeed = 50;
        private float maxStealValue = 100;
        private MeshRenderer meshRenderer;
        private bool withinTriggerRange = false;

        public event Action<float> OnStealValueChanged;
        public event Action OnStealValueFull;

        [SerializeField] private float paralysisRecoverTime = 2f;
        private float paralysisTimer;

        [SerializeField] private float soulStolenRange = 4;
        [SerializeField] private LayerMask playerLayer;
        private Collider[] playerCollider = new Collider[1];

        public CombinedSoul chasingCombinedSoul;

        private void Start()
        {
			motor = GetComponent<EnemyMotor>();
            meshRenderer = GetComponentInChildren<MeshRenderer>();
            //wayPoints = GetComponentInChildren<WayLine>().WayPoints;
            motor.MoveToTarget(wayPoints[targetWayPointIndex].position);

            enemySoul = new Soul(enemySoulColor, this);
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
                case EnemyState.paralysis:
                    Paralyze();
                    break;
                case EnemyState.waitForChase:
                    WaitForChase();
                    break;
            }

            withinTriggerRange = Convert.ToBoolean(Physics.OverlapSphereNonAlloc(transform.position, soulStolenRange, playerCollider, playerLayer));

        }

        private void WaitForChase()
        {
            Invoke("StartChasing", 1);
        }

        private void StartChasing()
        {
            currentState = EnemyState.chase;
        }

        private void Paralyze()
        {
            motor.MoveToTarget(transform.position);

            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && withinTriggerRange)
            {
                paralysisTimer = 0;

                float stealRatio = currentStealValue / maxStealValue;
                if (stealRatio < 1)
                {
                    currentStealValue += stealSpeed * Time.deltaTime;

                    OnStealValueChanged?.Invoke(stealRatio);
                    meshRenderer.material.SetFloat("_GradientIntensity", 0.5f + 0.5f * stealRatio);
                    PlayerInputController.Instance.canNotMove = true;
                }
                else
                {
                    if(currentStealValue > 0)
                    {
                        PlayerInputController.Instance.GrowBody(enemySoul);
                        //chaseTarget = enemySoul.soulGO.transform; 在GrowBody中直接设置chaseTarget

                        currentState = EnemyState.waitForChase;
                        currentStealValue = 0;
                        OnStealValueFull?.Invoke();
                    }
                    PlayerInputController.Instance.canNotMove = false;
                }
            }
            else
            {
                paralysisTimer += Time.deltaTime;
                if (paralysisTimer > paralysisRecoverTime)
                {
                    currentState = EnemyState.patrol;
                }
                PlayerInputController.Instance.canNotMove = false;
            }
        }

        private void Chase()
        {
            if (chaseTarget != null)
                ChaseAI(chaseTarget);
            else
                currentState = EnemyState.patrol;
        }

        protected virtual void ChaseAI(Transform chaseTarget)
        {
            motor.MoveToTarget(chaseTarget.position);
        }
        

        private void Patrol()
        {
            if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && withinTriggerRange && (Mathf.Abs(PlayerInputController.Instance.transform.position.y - transform.position.y) < 0.5f))
            {
                currentState = EnemyState.paralysis;
            }

            if (Vector3.Distance(transform.position, wayPoints[targetWayPointIndex].position) < 1f)
            {
                if (!isWaiting)
                {
                    isWaiting = true;
                    startWaitTime = patrolWaitTime;
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

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && currentState == EnemyState.chase)
            {
                //追普通的灵魂
                if (chasingCombinedSoul == null)
                {
                    chaseTarget = null;
                    PlayerInputController.Instance.ReduceBody(enemySoul);
                    TurnToOriginalColor();
                }
                //追三合一的灵魂
                else
                    PlayerInputController.Instance.SplitCombinedSoul(chasingCombinedSoul);

                
                HealthManager.Instance.DecreaseHealth();
            }
        }

        public void TurnToOriginalColor()
        {
            DOTween.To((float pNewValue) =>
                    meshRenderer.material.SetFloat("_GradientIntensity", pNewValue)
                    , 1, 0, 1);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, soulStolenRange);
        }
    }
}
