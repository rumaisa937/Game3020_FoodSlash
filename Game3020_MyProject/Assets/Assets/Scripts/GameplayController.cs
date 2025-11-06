using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameplayController : MonoBehaviour {

	public static GameplayController instance;

	public Text countDownText, scoreText, livesText;
	public float countDownTimer = 3.0f;
	public bool hasLevelStarted;

	public int playerScore;
	private int playerLives = 5;

	void Awake () {
		CreateInstance ();
	}

	void Start () {
		InitializeGameplayController ();
		hasLevelStarted = true;
	}

	void Update () {
		UpdateGameplayController ();
	}

	void CreateInstance () {
		if (instance == null) {
			instance = this;
		}
	}

	void InitializeGameplayController () {
		Time.timeScale = 0;
		countDownText.text = countDownTimer.ToString ("F0");

		playerScore = 0;
		scoreText.text = "" + playerScore;

		// Initialize lives
		playerLives = 5;
		if (livesText != null) {
			livesText.text = playerLives.ToString ();
		}
	}

	void UpdateGameplayController () {
		scoreText.text = "" + playerScore;
		if (hasLevelStarted) {
			CountDownAndBeginLevel ();
		}
	}

	void CountDownAndBeginLevel () {
		countDownTimer -= (0.19f * 0.15f);
		countDownText.text = countDownTimer.ToString ("F0");

		if (countDownTimer <= 0) {
			Time.timeScale = 1;
			hasLevelStarted = false;
			countDownText.gameObject.SetActive (false);
		}
	}

	// Decrease player lives by one, update UI, and go to GameOver when no lives left
	public void DecreaseLife () {
		playerLives = Mathf.Max(0, playerLives - 1);
		if (livesText != null) {
			livesText.text = playerLives.ToString ();
		}

		if (playerLives <= 0) {
			// Load GameOver scene (scene must be added to build settings with this exact name)
			SceneManager.LoadScene("GameOver");
		}
	}

} // GameplayController