using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{

    // I use an enum to keep teas of the different state that my game can be in.
    public enum GameState
    {
        Start,
        Playing,
        GameOver,
        Win
    }

    //I use this static variable to make the GameManager accessible from other script 
    public static GameManager Instance;

    //These variables are used to control game timer and display the remaining time on the UI
    [Header("Time Settings")]
    public TextMeshProUGUI timerDisplay;
    public float roundTime;
    public float time;

    //This variable keep; track of the cuurent state of the game.
    [Header("Game State")]
    public GameState currentState;

    //These variables are used to keep track of the player's score and the total amount of items.
    [Header("Score")]
    public int score = 0;
    public int totalItems = 5;

    //I use these variables to keep track of how many asteroids have spawned and how many have been destroyed.
    [Header("Win Conditon")]
    public int totalAsteroids = 0;
    public int destroyedAsteroids = 0;

    //These variables are used to connect the different Ui panels and text displays in the game.
    [Header("UI")]
    public GameObject losePanel;
    public GameObject winPanel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI FuelPurchaseDisplay;
    public TextMeshProUGUI resultText;
    private TextMeshProUGUI highScoreText;


    //I use this reference to access the player's health, fuel, money, and score from the playerStats script
    public PlayerStats PS;



    /*In my Awake
     * I check if an instance of the GameManager already exist.
     * If there is no instance, i assign this GameManager as the main instance.
     * If another instance exist, I destroy the duplicate GameManager.
     */
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(gameObject);
        }

    }


    /*In my start
     * I set the game Timer to the starting round time when the game begins
     * I call the starting game state and update the score display.
     * I lock the cursor so the player can control the game properly.
     */
    private void Start()
    {
        time = roundTime;

        SetStartState();
        UpdateScoreUI();

        LockCursor();
    }


    /*In my Update
     * I check if the playerStats script has been assigned before accessing the player's fuel and money.
     * I make the purchase display available while the checking the player's fuel.
     * i check if the player run out of fuel and has at least 20 money.
     * If the player presses B, i take 20 money and give the player 100 fuel
     * If the player has fuel i hide the purchase display.
     * I check it the game is still being played 
     * I check if the cursor has become unlocked while the player is playing and if the cursor is locked it agian so the player can continue the game.
     * I update the Display every frame
     */
    private void Update()
    {
        if (PS != null)
        {
            FuelPurchaseDisplay.enabled = true;
            if (PS.Fuel == 0 && PS.Money >= 20)
            {
                if (Input.GetKeyDown(KeyCode.B))
                {
                    PS.loseMoney(20);
                    PS.addFuel(100);
                }
            }
            else if (PS.Fuel != 0)
            {
                FuelPurchaseDisplay.enabled = false;
            }

        }

        time -= Time.deltaTime;
        if (currentState == GameState.Playing)
        {
            if (time <= 0f)
            {
                GameOver();
            }
        }

        if(currentState == GameState.Playing)
        {
            if(Cursor.lockState != CursorLockMode.Locked)
            {
                LockCursor();
            }

        }

        UpdateUI();

    }

    //This fuction is called when the player starts the game and it changes the current state of the game to the playing state 
    //I check if the lose panel exist before hiding it and i do the same for the winpanel
    //I hide the fuel purchase display when the game starts 
    //I check if the playerStats script is available and use the that script to add to my score and update the score display
    public void StartGame()
    {
        Time.timeScale = 1f;

        currentState = GameState.Playing;

        LockCursor();

        if (losePanel != null)
        {
            losePanel.SetActive(false);
        }

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        Debug.Log("Game Started");

        FuelPurchaseDisplay.enabled = false;
    }

    //This fuctions adds to my Score amd update the ScoreUi
    public void AddScore()
    {
        if (PS != null)
        {
            PS.addScore(1);
            score = PS.Score;
        }
        else
        {
            score++;
        }

        UpdateScoreUI();

        Debug.Log("Scrap collected. Score: " + score);
    }

    //Here i Update the UI, i check for the timerDisplay and update it accordingly 
    public void UpdateUI()
    {
        if (!timerDisplay)
        {
            return;
        }

        timerDisplay.text = Mathf.CeilToInt(time).ToString();
    }


    public void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Scrap: " + score;
        }
    }
   

    //Here i set the condition for the game over and set the state to the game over state 
    //I active the losepanel and i unlock the cursor 
    public void GameOver()
    {
        

        if (currentState == GameState.GameOver)
        {
            return;
        }

        currentState = GameState.GameOver;
        
        float finishedTime = time;

        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }

        ShowLose(finishedTime, PS.Score);

        Savehighscore();

        Debug.Log("Game Over");

        //time = 0f;
        UnlockCursor();
    }

    //Here i set the condition for the game win and set the state to the game win state 
    //I active the Winpanel and i unlock the cursor
    public void WinGame()
    {
       

        if (currentState == GameState.Win || currentState == GameState.GameOver)
        {
            return;
        }
        
        currentState = GameState.Win;

        Time.timeScale = 0f;

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        if(losePanel != null)
        {
            losePanel.SetActive(false);
        }
        

        float finishedTime = time;

        ShowWin(finishedTime, PS.Score);

        Savehighscore();

        UnlockCursor();

        Debug.Log("Player collected all scrap. You Win!");
    }

   //This function restart the game and unlock the cursor and loads the active scene
    public void RestartGame()
    {
        Debug.Log("Restarting Game");
        UnlockCursor();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    //Here i load the menu scene and unlock the cursor 
    public void LoadMenu()
    {
        Time.timeScale = 1f;

        Debug.Log("Loading Menu");

        SceneManager.LoadScene("Main Menu");

        UnlockCursor();
    }


    //this sets the start state it enables the lose and win panel 
    private void SetStartState()
    {
        currentState = GameState.Start;


        if (losePanel != null)
        {
            losePanel.SetActive(false);
        }

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
    }

   //Here i refresh the Ui sliders and displays 
    public void ForceRefresh()
    {
        if (PS == null)
        {
            return;
        }

        score = PS.Score;

        UpdateScoreUI();

        if (PS.HealthSlider != null)
        {
            PS.HealthSlider.value = PS.Health;
        }

        if (PS.FuelSlider != null)
        {
            PS.FuelSlider.value = PS.Fuel;
        }

        if (PS.moneyDisplay != null)
        {
            PS.moneyDisplay.text = "Money: " + PS.Money;
        }

        if (PS.scoreDisplay != null)
        {
            PS.scoreDisplay.text = "Score: " + PS.Score;
        }
    }

    //THis gets displayed on the losepanel
    public void ShowLose(float time, int score)
    {
        string playerName = GetPlayerName();

        resultText.text = "The player " + playerName + " has died at " + Mathf.CeilToInt(time) + " seconds out and has obtained a score of " + score;
    }

    //THis gets displayed on the Winpanel
    public void ShowWin(float time, int score)
    {
        string playerName = GetPlayerName();

        resultText.text = "The player " + playerName + " has won at " + Mathf.CeilToInt(time) + " seconds out and has obtained a score of " + score;
    }

    //This registers the amount of asteroids that spawns at the start of the game.
    public void RegisterAsteroids()
    {
        totalAsteroids++;

        Debug.Log("The Asteroids have Spawned. The Total asteroids are " + totalAsteroids   );
    }

    //This gets called when starting asteroid has been destroyed and check if all the asteroids have been destroed and calls the winning condintions
    public void AsteroidsDestroyed()
    {

        if(currentState == GameState.Win || currentState == GameState.GameOver)
        {
            return;
        }

        destroyedAsteroids++;

        Debug.Log("A spawned in Asteroid have been destroyed " + destroyedAsteroids + "/" + totalAsteroids);

        if(totalAsteroids > 0 && destroyedAsteroids >= totalAsteroids)
        {
            WinGame();
        }

    }

    //Here i save the highscore using playerPrefs
    public void Savehighscore()
    {
        int oldHighScore = PlayerPrefs.GetInt("HighScore", 0);

        int currentScore = PS.Score;

        if (currentScore > oldHighScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);

            PlayerPrefs.Save();

            Debug.Log("New HighScore saved: " + currentScore);
        }
        else
        {
            Debug.Log("HighScore remains: " + oldHighScore);
        }
    }



    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GamePlay")
        {
            
            if(!timerDisplay)
            {
                var tagged = GameObject.FindGameObjectWithTag("Timer");
                if (tagged) timerDisplay = tagged.GetComponent<TextMeshProUGUI>();

                if(timerDisplay == null)
                {
                    GameObject byName = GameObject.Find("timerDisplay");
                    
                    if(!byName) timerDisplay = byName.GetComponent<TextMeshProUGUI>();
                }
            }

            UpdateUI();
            
        }
    }

    
    private string GetPlayerName()
        {
            if (PlayerDataManager.Instance != null)
            {
                if (!string.IsNullOrWhiteSpace(
                    PlayerDataManager.Instance.playerName))
                {
                    return PlayerDataManager.Instance.playerName;
                }
            }

            string savedName = PlayerPrefs.GetString("PlayerName", "");

            if (!string.IsNullOrWhiteSpace(savedName))
            {
                return savedName;
            }

            return "Unknown Player";
        }

    //This fuction locks the cursor and make the cursor invisible
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //This fuction locks the cursor and make the cursor visible
    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}