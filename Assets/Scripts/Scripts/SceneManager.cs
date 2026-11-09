using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public GameObject startPanel;
    public GameObject playerDataPanel;


    private void Start()
    {
        startPanel.SetActive(true);
        playerDataPanel.SetActive(false);
    }

    public void Play()
    {
        startPanel.SetActive(false);
        playerDataPanel.SetActive(true);
    }

    public void Continue()
    {
        SceneManager.LoadScene("GamePlay");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}