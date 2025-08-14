using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonControls : MonoBehaviour
{
    //load game
    public void PlayGame() 
    {
         SceneManager.LoadScene("GamePlay");
    }

    //quit application
    public void Quit() 
    {
        Application.Quit();
    }

    //back to main menu
    public void MainMenu() 
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PuaseGame()
    {
        Time.timeScale = 0f;
    }    

    public void unPauseGame()
    {
        Time.timeScale = 1.0f;
    }
}
