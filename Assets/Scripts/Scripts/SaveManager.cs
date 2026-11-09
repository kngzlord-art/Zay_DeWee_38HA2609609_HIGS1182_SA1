using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveManager : MonoBehaviour
{
    public GameObject savePanel;
    public GameObject LoadButton;

    public GameObject saveOpenButton;
    public GameObject saveCloseButton;


    public PlayerStats PS;
    private string path;


    private void Awake()
    {
        savePanel.SetActive(false);
        LoadButton.SetActive(false);
        saveCloseButton.SetActive(false);

        path = Application.persistentDataPath + "/save.json";

    }


    private void Start()
    {

        if(PS == null)
        {
            Debug.LogError("SaveManager: PlayerStats has not been assigned");
        }
       
        //PS = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (savePanel.activeSelf)
            {
                CloseSavePanel();
            }
            else
            {
                OpenSavePanel();
            }
        }

        if (File.Exists(path))
        {
            LoadButton.SetActive(true);
        }
        else
        {
            LoadButton.SetActive(false);
        }
    }


    public class SaveData
    {
        public int score;
        public float health;
        public int money;
        public float fuel;
    }

    public void saveGame()
    {
        if(PS == null)
        {
            Debug.LogError("Save has failed: PlayerStats is missing");
            return;
        }

        SaveData data = new SaveData()
        {
            score = PS.Score,
            health = PS.Health,
            money = PS.Money,
            fuel = PS.Fuel

        };

        File.WriteAllText(path, JsonUtility.ToJson(data));

        Debug.Log("File save" + path);  
    }


    public void loadGame()
    {
        if(PS == null)
        {
            Debug.LogError("Load Has Failed: PLayerStats is missing");
            return;
        }    

        if(File.Exists(path))
        {
            var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));

            PS.Health = data.health;
            PS.Money = data.money;
            PS.Fuel = data.fuel;
            PS.Score = data.score;


            PS.RefreshUI();

            Debug.Log("Game has Loaded succesfully.");


        }
    
    
    
    }

    public void OpenSavePanel()
    {
        if (savePanel != null)
        {
            savePanel.SetActive(true);
            saveOpenButton.SetActive(false);
            saveCloseButton.SetActive(true);

            Time.timeScale = 0f;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }

    }

    public void CloseSavePanel()
    {
        if (savePanel != null)
        {
            savePanel.SetActive(false);
            saveOpenButton.SetActive(true);
            saveCloseButton.SetActive(false);

            Time.timeScale = 1f;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

        }
    }




}
