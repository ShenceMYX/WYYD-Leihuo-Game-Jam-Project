using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

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

        public GameObject blueSoulPrefab;
        public GameObject redSoulPrefab;
        public GameObject yellowSoulPrefab;
        public GameObject combinedPurpleSoulPrefab;

        public int currentBodypartIndex;
        private List<GameObject> bodyParts = new List<GameObject>();
        [SerializeField] private Transform[] pivotTransArr;
        private Dictionary<SoulColor, List<Soul>> soulDIC = new Dictionary<SoulColor, List<Soul>>();

        private ExplosionController explosionController;

        public bool canNotMove { get; set; } = false;

        private void Start()
        {
            motor = GetComponent<PlayerMotor>();
			groundChecker = GetComponent<GroundChecker>();
            explosionController = GetComponent<ExplosionController>();

            pivotTransArr = new Transform[8];
            pivotTransArr[0] = transform.FindChildByName("pivot1");
            for (int i = 1; i < pivotTransArr.Length; i++)
            {
                pivotTransArr[i] = pivotTransArr[i - 1].GetChild(0);
            }

            soulDIC.Add(SoulColor.blue, new List<Soul>());
            soulDIC.Add(SoulColor.red, new List<Soul>());
            soulDIC.Add(SoulColor.yellow, new List<Soul>());
            soulDIC.Add(SoulColor.purple, new List<Soul>());

            currentBodypartIndex++;
        }

        private void Update()
        {
            xInput = Input.GetAxisRaw("Horizontal");
            yInput = Input.GetAxisRaw("Vertical");

            Vector3 inputVec = new Vector3(xInput, 0, yInput).normalized;

            if (canNotMove) return;

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

        private void CombineSouls()
        {
            Soul redSoul = soulDIC[SoulColor.red].Last();
            Soul blueSoul = soulDIC[SoulColor.blue].Last();
            Soul yellowSoul = soulDIC[SoulColor.yellow].Last();

            CombinedSoul combinedSoul = new CombinedSoul(redSoul, blueSoul, yellowSoul);

            ReduceBody(redSoul);
            ReduceBody(blueSoul);
            ReduceBody(yellowSoul);

            GrowBody(combinedSoul);

            //原来的分别追逐的三个不同灵魂都三个敌人现在统一都追逐一个三合一灵魂
            redSoul.chasedEnemyAI.chaseTarget = combinedSoul.soulGO.transform;
            redSoul.chasedEnemyAI.chasingCombinedSoul = combinedSoul;
            blueSoul.chasedEnemyAI.chaseTarget = combinedSoul.soulGO.transform;
            blueSoul.chasedEnemyAI.chasingCombinedSoul = combinedSoul;
            yellowSoul.chasedEnemyAI.chaseTarget = combinedSoul.soulGO.transform;
            yellowSoul.chasedEnemyAI.chasingCombinedSoul = combinedSoul;
        }

        private bool CheckCombineSoul()
        {
            //如果红色、蓝色、黄色灵魂都至少有一个
            return soulDIC[SoulColor.red].Count != 0 && soulDIC[SoulColor.blue].Count != 0 && soulDIC[SoulColor.yellow].Count != 0;
        }

        [Button]
        public void GrowBody(Soul enemySoul)
        {
            GameManager.Instance.UpdateCurrentSoul(1);

            //if (currentBodypartIndex >= 9) return;
            GameObject prefabToBeSpawned = null;
            switch (enemySoul.soulColor)
            {
                case SoulColor.blue:
                    prefabToBeSpawned = blueSoulPrefab;
                    break;
                case SoulColor.red:
                    prefabToBeSpawned = redSoulPrefab;
                    break;
                case SoulColor.yellow:
                    prefabToBeSpawned = yellowSoulPrefab;
                    break;
                case SoulColor.purple:
                    prefabToBeSpawned = combinedPurpleSoulPrefab;
                    break;
            }

            GameObject soulGO = Instantiate(prefabToBeSpawned, pivotTransArr[bodyParts.Count]);
            enemySoul.soulGO = soulGO;

            if (enemySoul.soulColor != SoulColor.purple)
                enemySoul.chasedEnemyAI.chaseTarget = soulGO.transform;

            bodyParts.Add(soulGO);
            soulDIC[enemySoul.soulColor].Add(enemySoul);

            soulGO.transform.localPosition = new Vector3(0, 0, 0);
            soulGO.transform.localScale = Vector3.one * 0.5f;


            if (CheckCombineSoul())
                CombineSouls();
        }

        public void ReduceBody(Soul enemySoul)
        {
            GameManager.Instance.UpdateCurrentSoul(-1);

            GameObject soulToBeRemoved = enemySoul.soulGO;
            soulDIC[enemySoul.soulColor].Remove(enemySoul);

            Destroy(soulToBeRemoved);
            enemySoul.soulGO = null;

            string log = "remove before: ";
            foreach (var item in bodyParts)
            {
                log += item;
                log += " , ";
            }
            Debug.Log(log);
            bodyParts.Remove(soulToBeRemoved);
            string log2 = "remove before: ";
            foreach (var item in bodyParts)
            {
                log2 += item;
                log2 += " , ";
            }
            Debug.Log(log2);

            for (int i = 0; i < bodyParts.Count; i++)
            {
                bodyParts[i].transform.parent = pivotTransArr[i];
            }
        }

        public void SplitCombinedSoul(CombinedSoul combinedSoul)
        {
            GameManager.Instance.UpdateCurrentSoul(-1);

            Soul redSoul = combinedSoul.redSoul;
            Soul blueSoul = combinedSoul.blueSoul;
            Soul yellowSoul = combinedSoul.yellowSoul;

            ReduceBody(combinedSoul);

            GameObject redSoulGO = explosionController.Explode(redSoulPrefab);
            GameObject blueSoulGO = explosionController.Explode(blueSoulPrefab);
            GameObject yellowSoulGO = explosionController.Explode(yellowSoulPrefab);

            redSoul.chasedEnemyAI.chaseTarget = redSoulGO.transform;
            redSoul.chasedEnemyAI.chasingCombinedSoul = null;
            blueSoul.chasedEnemyAI.chaseTarget = blueSoulGO.transform;
            blueSoul.chasedEnemyAI.chasingCombinedSoul = null;
            yellowSoul.chasedEnemyAI.chaseTarget = yellowSoulGO.transform;
            yellowSoul.chasedEnemyAI.chasingCombinedSoul = null;

            SoulCollectible redSoulCollectible = redSoulGO.AddComponent<SoulCollectible>();
            redSoulCollectible.soul = redSoul;
            SoulCollectible blueSoulCollectible = blueSoulGO.AddComponent<SoulCollectible>();
            blueSoulCollectible.soul = blueSoul;
            SoulCollectible yellowSoulCollectible = yellowSoulGO.AddComponent<SoulCollectible>();
            yellowSoulCollectible.soul = yellowSoul;
        }
        


    }

    public class Soul
    {
        public SoulColor soulColor;
        public EnemyAI chasedEnemyAI { get; private set; }
        public GameObject soulGO;

        public Soul() { }

        public Soul(SoulColor soulColor, EnemyAI chasedEnemy)
        {
            this.soulColor = soulColor;
            this.chasedEnemyAI = chasedEnemy;
        }

        public Soul(SoulColor soulColor, EnemyAI chasedEnemy, GameObject soulGO)
        {
            this.soulColor = soulColor;
            this.chasedEnemyAI = chasedEnemy;
            this.soulGO = soulGO;
        }
    }

    public class CombinedSoul : Soul
    {
        public Soul redSoul { get; private set; }
        public Soul blueSoul { get; private set; }
        public Soul yellowSoul { get; private set; }

        public CombinedSoul(Soul redSoul, Soul blueSoul, Soul yellowSoul)
        {
            this.redSoul = redSoul;
            this.blueSoul = blueSoul;
            this.yellowSoul = yellowSoul;

            this.soulColor = SoulColor.purple;
        }

    }

}
