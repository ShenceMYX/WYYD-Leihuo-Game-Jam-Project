using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ns
{
	/// <summary>
	///
	/// </summary>
	public class EnergyBar : MonoBehaviour
	{
        private Image energyBarImg;

        private void Start()
        {
            energyBarImg = GetComponent<Image>();    
        }

        public void SetEnergy(float ratio)
        {
            energyBarImg.fillAmount = ratio;
        }
    }
}
