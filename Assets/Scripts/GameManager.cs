using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int Lives { get; set; }
    public int Score { get; set; }

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        RestartGame();
        DontDestroyOnLoad(gameObject);
    }

    private void Update() {
        if(Lives <= 0) {
            GoToMenu();
        }
    }

    private void RestartGame() {
        Lives = 3;
        Score = 0;
    }

    public void GoToMenu() {
        SceneManager.LoadScene(0);
        RestartGame();
    }

    public void StartGame() {
        SceneManager.LoadScene(1);
        RestartGame();
    }
}
