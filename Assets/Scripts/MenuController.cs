using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MenuScene");
        }
    }

    //loads the main game
    public void LoadGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    //quits the game
    public void QuitGame()
    {
        Application.Quit();
    }

    //load main menu
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    //load hints menu
    public void LoadHints()
    {
        SceneManager.LoadScene("HintsScene");
    }

    //load hints p2
    public void LoadHintsP2()
    {
        SceneManager.LoadScene("HintsSceneP2");
    }

    //load contols scene
    public void LoadControls()
    {
        SceneManager.LoadScene("ControlsScene");
    }
}
