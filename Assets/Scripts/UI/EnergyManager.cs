using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace ns
{
	/// <summary>
	///
	/// </summary>
	public class EnergyManager : MonoSingleton<EnergyManager>
	{
        private List<EnergyBar> energyBars = new List<EnergyBar>();

        [SerializeField] private float everyBarMaxEnergy = 50;

        public float currentEnergy;
        public float maxEnergy;

        [Tooltip("静止状态时能量增加的速度")]
        [SerializeField] private float energyIncreaseSpeed = 5;
        [Tooltip("移动状态时能量增加速度的倍率")]
        [SerializeField] private float energyIncreasAcclerateMultiplier = 3;

        private void Start()
        {
            foreach (var energyBar in GetComponentsInChildren<EnergyBar>())
            {
                energyBars.Add(energyBar);
            }
            maxEnergy = energyBars.Count * everyBarMaxEnergy;
        }

        private void Update()
        {
            if(PlayerInputController.Instance.isMoving)
                SetCurrentEnergy(energyIncreaseSpeed * Time.deltaTime * energyIncreasAcclerateMultiplier);
            else
                SetCurrentEnergy(energyIncreaseSpeed * Time.deltaTime);
        }

        public void SetCurrentEnergy(float value)
        {
            currentEnergy += value;
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
            int energyBarIndex = Mathf.FloorToInt(currentEnergy / (everyBarMaxEnergy + 0.001f));
            Debug.Log(energyBarIndex);
            energyBars[energyBarIndex].SetEnergy((currentEnergy - everyBarMaxEnergy * energyBarIndex) / everyBarMaxEnergy);

            //if(currentEnergy >= everyBarMaxEnergy)
            //{
            //    PlayerInputController.Instance.SetSpeedToAcceleratedSpeed();
            //}
            //else
            //{
            //    PlayerInputController.Instance.ResetSpeed();
            //}
        }

    }
}
