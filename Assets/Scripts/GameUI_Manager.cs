using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameUI_Manager : MonoBehaviour
{
    [SerializeField] private GameManager gm;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private GameObject uiPause;
    [SerializeField] private GameObject uiGameOver;
    [SerializeField] private GameObject uiGameIsFinished;
    private enum GameUIState
    {
        GamePlay, Pause, GameOver, GameIsFinished
    }

    private GameUIState _currentState;

    public BackGroundMusic backGroundMusic;

    private void Awake()
    {
        backGroundMusic = GameObject.FindGameObjectWithTag("BackGroundMusic").GetComponent<BackGroundMusic>();
    }

    void Update()
    {
        healthSlider.value = gm.playerCharacter.GetComponent<Health>().GetCurrentHealthPercentage();
        coinText.text = gm.playerCharacter.totalCoin.ToString();
    }

    private void SwitchUIState(GameUIState state)
    {
        uiPause.SetActive(false);
        uiGameOver.SetActive(false);
        uiGameIsFinished.SetActive(false);
        Time.timeScale = 1;
        switch (state)
        {
            case GameUIState.GamePlay:
                break;
            case GameUIState.Pause:
                Time.timeScale = 0;
                uiPause.SetActive(true);
                break;
            case GameUIState.GameOver:
                uiGameOver.SetActive(true);
                break;
            case GameUIState.GameIsFinished:
                uiGameIsFinished.SetActive(true);
                break;
        }
        _currentState = state;
    }

    public void TogglePauseUI()
    {
        backGroundMusic.PlaySound(backGroundMusic.button);
        if (_currentState == GameUIState.GamePlay)
        {
            SwitchUIState(GameUIState.Pause);
        } else if (_currentState == GameUIState.Pause)
        {
            SwitchUIState(GameUIState.GamePlay);
        }
    }

    public void ButtonMenu()
    {
        backGroundMusic.PlaySound(backGroundMusic.button);
        gm.ReturnToMenu();
    }

    public void ButtonRestart()
    {
        backGroundMusic.PlaySound(backGroundMusic.button);
        Time.timeScale = 1;
        gm.Restart();
    }

    public void ShowGameOverUI()
    {
        SwitchUIState(GameUIState.GameOver);
    }

    public void ShowGameIsFinishedUI()
    {
        SwitchUIState(GameUIState.GameIsFinished);
    }
}
