using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState {
    PreGame,
    GameInProgress,
    PostGame,
    Paused
}

public class GameLoop : MonoBehaviour {
    public static GameLoop Instance;
    public GameState GameState;

    [SerializeField] private float _timeLimit = 600;
    private float _timeLeft = 0f;
    [SerializeField] private float _score = 0f;

    void Start() {
        Instance = this;
        GameState = GameState.GameInProgress;

        Time.timeScale = 1;
        _timeLeft = _timeLimit;
        _score = 0f;
    }

    void Update() {
        if (GameState != GameState.GameInProgress) {
            return;
        }

        _timeLeft -= Time.deltaTime;

        if (_timeLeft <= 0f) {
            _timeLeft = 0f;
            GameOver();
        }
    }

    public void Pause() {
        if (GameState == GameState.GameInProgress) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Time.timeScale = 0f;

            HUD.Instance.PauseUI.SetActive(true);
            GameState = GameState.Paused;
        }
        else {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Time.timeScale = 1f;

            HUD.Instance.PauseUI.SetActive(false);
            GameState = GameState.GameInProgress;

        }
    }

    void GameOver() {
        GameState = GameState.PostGame;
        print("Game Over! score is " + _score);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        HUD.Instance.GameOverUI.SetActive(true);
    }

    public void Died() {
        GameState = GameState.PostGame;
        Time.timeScale = 0.4f;
        HUD.Instance.GameOverUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void AddToScore(int pointsToAdd) {
        _score += pointsToAdd; //will do mroe complex score system
    }

    public void PlayAgain() {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}