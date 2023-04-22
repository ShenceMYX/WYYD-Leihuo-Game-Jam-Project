using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ns
{
	/// <summary>
	///
	/// </summary>
	public class StealProgressBar : MonoBehaviour
	{
        public Image progressBarImg;
        public GameObject progressBarGO;

        private void Awake()
        {
            GetComponentInParent<EnemyAI>().OnStealValueChanged += StealValueChangedHandler;
            GetComponentInParent<EnemyAI>().OnStealValueFull += StealValueFullHandler;
            progressBarGO.SetActive(false);
        }

        private void StealValueFullHandler()
        {
            progressBarGO.SetActive(false);
        }

        private void StealValueChangedHandler(float stealValueRatio)
        {
            if(progressBarGO.activeInHierarchy == false) progressBarGO.SetActive(true);

            progressBarImg.fillAmount = stealValueRatio;
        }

    }
}
