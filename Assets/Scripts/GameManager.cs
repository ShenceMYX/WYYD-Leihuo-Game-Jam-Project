using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

namespace ns
{
	/// <summary>
	///
	/// </summary>
	public class GameManager : MonoSingleton<GameManager>
	{
		[SerializeField] private int goal = 1;
		[SerializeField] private int currentSoul = 0;

		[SerializeField] private TextMeshProUGUI goalText;

		[SerializeField] private GameObject gameOverUI;

        private void Start()
        {
			goalText.text = currentSoul + "/" + goal;
        }

		[Button]
		public void ChangeGoalTextToCurrentSetup()
        {
			goalText.text = currentSoul + "/" + goal;
		}

		public bool CheckWinCondition()
        {
			return currentSoul >= goal;
		}

		public void UpdateCurrentSoul(int value)
        {
			currentSoul += value;
			goalText.text = currentSoul + "/" + goal;
		}

		public void GameWin()
        {
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		}

		public void GameOver()
        {
			Time.timeScale = 0;
			gameOverUI.SetActive(true);
		}

		public void RestartGame()
        {
			Time.timeScale = 1;
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
    }
}
