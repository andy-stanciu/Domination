using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private string GameScene;

    [SerializeField]
    private string FakeGame;

    public void PlayGameGame()
    {
    SceneManager.LoadScene(FakeGame);
    }

    public void PlayGame()
    {
    SceneManager.LoadScene(GameScene);
    }

    public void QuitGame()
    {
    Debug.Log("QUIT!");
    Application.Quit();
    }
}
