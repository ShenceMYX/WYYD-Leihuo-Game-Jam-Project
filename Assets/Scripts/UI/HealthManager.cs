using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace ns
{
	/// <summary>
	///
	/// </summary>
	public class HealthManager : MonoSingleton<HealthManager>
	{
		public int currentHealth;
		public int maxHealth;

		public List<Transform> healthUIGOs = new List<Transform>();

        private void Start()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
				healthUIGOs.Add(transform.GetChild(i));
            }
			maxHealth = healthUIGOs.Count;
			currentHealth = maxHealth;
        }

		public void DecreaseHealth()
        {
			if (currentHealth == 0) return;

			Destroy(healthUIGOs[currentHealth - 1].gameObject);
			currentHealth--;

			if (currentHealth == 0)
				GameManager.Instance.GameOver();
        }
    }
}
