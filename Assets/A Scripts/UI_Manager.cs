using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] GameObject winUI, loseUI;

    private void Start()
    {
        Guard.OnGuardHasSpottedPlayer += OnLose;
        Player.OnReachedEndOfLevel += OnWin;
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    private void OnLose()
    {
        OnGameOver(loseUI);
    }
    private void OnWin()
    {
        OnGameOver(winUI);
    }
    private void OnGameOver(GameObject GameOverUI)
    {
        GameOverUI.SetActive(true);
        Guard.OnGuardHasSpottedPlayer -= OnLose;
        Player.OnReachedEndOfLevel -= OnWin;
    }
}
