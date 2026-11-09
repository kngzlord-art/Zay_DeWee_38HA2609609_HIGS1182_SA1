using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// This script manages the player's name and keeps the player data available between scenes.
public class PlayerDataManager : MonoBehaviour
{
    // I use this static variable so other scripts can access this PlayerDataManager.
    public static PlayerDataManager Instance;

    [Header("Name")]
    // I use this input field to allow the player to enter their name.
    public TMP_InputField nameInput;

    // I use this object to enable or disable the continue button.
    public GameObject continuebutton;

    [Header("scene")]
    // This stores the name of the gameplay scene that will be loaded.
    public string gameplaySceneName = "GamePlay";

    // I use this variable to store the player's name.
    public string playerName;

    // Awake runs before Start and is used here to set up the PlayerDataManager.
    private void Awake()
    {
        // I check if there is already a PlayerDataManager instance.
        if
            (Instance == null)
        {
            // If there is no instance, I make this object the main instance.
            Instance = this;

            // I use DontDestroyOnLoad so this object stays when changing scenes.
            DontDestroyOnLoad(gameObject);

            // I load the player's previously saved name from PlayerPrefs.
            playerName = PlayerPrefs.GetString("PlayerName");
        }
        else
        {
            // If another instance already exists, I destroy this duplicate.
            Destroy(gameObject);
        }



    }

    // This function runs when the object starts.
    private void Start()
    {
        // I display the saved player name inside the input field.
        nameInput.text = playerName;
    }


    // This function checks the name input every frame.
    private void Update()
    {

        // I check if the input field is missing before trying to use it.
        if (nameInput == null)
        {
            return;
        }

        // If the input field exists, I call CheckName to check what the player entered.
        if (nameInput != null)
        {
            CheckName();
        }



    }

    // This function checks if the player has entered a valid name.
    public void CheckName()
    {
        // I check if the input field is empty or only contains spaces.
        if (string.IsNullOrWhiteSpace(nameInput.text))
        {
            // If no valid name was entered, I hide the continue button.
            continuebutton.SetActive(false);
        }
        else
        {
            // If the player has entered a name, I show the continue button.
            continuebutton.SetActive(true);
        }

    }


    // This function saves the player's name and loads the gameplay scene.
    public void ContinueGame()
    {
        // I check again if the input field is empty before continuing.
        if (string.IsNullOrWhiteSpace(nameInput.text))
        {
            return;
        }

        // I store the name entered by the player in the playerName variable.
        playerName = nameInput.text;

        // I save the player's name using PlayerPrefs so it can be loaded later.
        PlayerPrefs.SetString("PlayerName", playerName);

        // I make sure the saved PlayerPrefs data is written.
        PlayerPrefs.Save();

        // I load the gameplay scene using the scene name assigned in the Inspector.
        SceneManager.LoadScene(gameplaySceneName);
    }

}