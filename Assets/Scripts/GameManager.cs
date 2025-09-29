using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Character playerCharacter;
    public GameUI_Manager gameUIManager;
    private bool _isGameOver;
    
    private void Awake()
    {
        playerCharacter = GameObject.FindWithTag("Player").GetComponent<Character>();
    }

    private void GameOver()
    {
        gameUIManager.ShowGameOverUI();
    }

    public void GameIsFinished()
    {
        gameUIManager.ShowGameIsFinishedUI();
    }
    void Update()
    {
        if (_isGameOver)
        {
            return;
        }
        
        if (playerCharacter.currentState == Character.CharacterState.Dead)
        {
            _isGameOver = true;
            GameOver();
        }
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
