using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AsteroidsSpawner : MonoBehaviour
{
    [Header("Asteroid")]
    public GameObject asteroidPrefab;
    public GameObject movAsteroidPrefab;
    public GameObject EnemyDronePrefab;
    public List<GameObject> startingAsteroids = new List<GameObject>();


    [Header("SpawnTimes")]
    // These variables control how long the game waits before starting each wave.
    public float spawnTime1 = 140;
    public float spawnTime2 = 100;
    public float spawnTime3 = 60;

    [Header("Starting Asteroid Amount")]
    // I use these two values to choose a random amount of starting asteroids.
    public int minAsteroids = 10;
    public int maxAsteroids = 25;

    [Header("SpawningAmounts")]
    // These variables control how many asteroids and drones spawn in each wave.
    public int asteroidAmount = 10;
    public int SpawnAmount1 = 1;
    public int SpawnAmount2 = 2;
    public int SpawnAmount3 = 3;

    [Header("Wave Checks")]
    // I use these booleans to keep track of which waves have spawned.
    public bool wave1Spawned = false;
    public bool wave2Spawned = false;
    public bool wave3Spawned = false;

    // These booleans stop the same wave warning from starting more than once.
    private bool wave1WarningStarted = false;
    private bool wave2WarningStarted = false;
    private bool wave3WarningStarted = false;


    [Header("Wave Warning")]
    // This is the UI text that displays the countdown before a wave starts.
    public TextMeshProUGUI WaveWarningDisplay;
    public float warningTime = 10f;

    [Header("Spawn Area")]
    // These values control the size of the area where asteroids and drones can spawn.
    public float spawnAreaX = 30f;
    public float spawnAreaY = 10f;
    public float spawnAreaZ = 30f;



    [Header("GameManager")]
    public GameManager GM;


    private void Start()
    {
        // I check if the GameManager has been assigned in the Inspector.
        if (GM == null)
        {
            // If it has not been assigned, I search for a GameManager in the scene.
            GM = FindFirstObjectByType<GameManager>();
        }

        // I display an error if the GameManager still cannot be found.
        if (GM == null)
        {
            Debug.LogError("The GameManager was not found");
        }

        // I call this function to spawn the starting asteroids when the game begins.
        SpawnAsteroids();


    }

    private void Update()
    {
        // I stop the Update function from continuing if there is no GameManager.
        if (GM == null) return;

        // I check if it is time to start the warning for wave 1.
        if (!wave1WarningStarted && GM.time <= spawnTime1 + warningTime)
        {
            // I set this to true so the warning does not start again.
            wave1WarningStarted = true;

            // I start the countdown coroutine for the first wave.
            StartCoroutine(WaveCountdown(1));
        }

        // I check if it is time to start the warning for wave 2.
        if (!wave2WarningStarted && GM.time <= spawnTime2 + warningTime)
        {
            wave2WarningStarted = true;
            StartCoroutine(WaveCountdown(2));
        }

        // I check if it is time to start the warning for wave 3.
        if (!wave3WarningStarted && GM.time <= spawnTime3 + warningTime)
        {
            wave3WarningStarted = true;
            StartCoroutine(WaveCountdown(3));
        }
    }

    public void SpawnAsteroids()
    {
        // I use Random.Range to choose a random amount of starting asteroids.
        // I add 1 to maxAsteroids because the integer version of Random.Range excludes the maximum value.
        asteroidAmount = Random.Range(minAsteroids, maxAsteroids + 1);

        Debug.Log("Starting asteroid amount: " + asteroidAmount);

        // I use a for loop to repeat the spawning process based on the random asteroid amount.
        for (int i = 0; i < asteroidAmount; i++)
        {
            // I call GetRandomSpawnPosition to get a random position for each asteroid.
            Vector3 spawnPosition = GetRandomSpawnPosition();

            // I use Instantiate to create an asteroid at the selected position with a random rotation.
            GameObject asteroid = Instantiate(
                asteroidPrefab,
                spawnPosition,
                Random.rotation
            );

            // I add each spawned asteroid to the list so I can keep track of the starting asteroids.
            startingAsteroids.Add(asteroid);

            Debug.Log("Starting asteroids remaining: " + startingAsteroids.Count);

            // I register each asteroid with the GameManager if it exists.
            if (GM != null)
            {
                GM.RegisterAsteroids();
            }
        }
    }

    public void Wave1()
    {
        // I use a for loop to spawn the amount of asteroids set for the first wave.
        for (int i = 0; i < SpawnAmount1; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();

            // I create a moving asteroid and an enemy drone at the random spawn position.
            Instantiate(
                movAsteroidPrefab,
                spawnPosition,
                Random.rotation
            );
            Instantiate(
                EnemyDronePrefab,
                spawnPosition,
                Random.rotation
            );

            Debug.Log("The first Wave asteroid has spawned in.");
        }
    }

    public void Wave2()
    {
        // I use this loop to spawn the second wave using SpawnAmount2.
        for (int i = 0; i < SpawnAmount2; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();

            // I spawn a moving asteroid and an enemy drone for each loop.
            Instantiate(
                movAsteroidPrefab,
                spawnPosition,
                Random.rotation
            );
            Instantiate(
                EnemyDronePrefab,
                spawnPosition,
                Random.rotation
            );

            Debug.Log("The second wave asteroids have spawned in.");
        }
    }

    public void Wave3()
    {
        // I use this loop to spawn the third wave using SpawnAmount3.
        for (int i = 0; i < SpawnAmount3; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();

            // I create the moving asteroid and enemy drone at the chosen position.
            Instantiate(
                movAsteroidPrefab,
                spawnPosition,
                Random.rotation
            );
            Instantiate(
                EnemyDronePrefab,
                spawnPosition,
                Random.rotation
            );

            Debug.Log("The Third wave asteroids have spawned in");
        }
    }

    private IEnumerator WaveCountdown(int waveNumber)
    {
        // I convert the warning time into a whole number for the countdown.
        int countdown = Mathf.CeilToInt(warningTime);

        // I keep repeating this loop until the countdown reaches zero.
        while (countdown > 0)
        {
            // I check if the warning text has been assigned before displaying the countdown.
            if (WaveWarningDisplay != null)
            {
                WaveWarningDisplay.text =
                    "WAVE " + waveNumber + " STARTS IN: " + countdown;
            }

            // I make the coroutine wait for one second before continuing.
            yield return new WaitForSeconds(1f);

            // I decrease the countdown by one every second.
            countdown--;
        }

        // I hide the warning text after the countdown has finished.
        if (WaveWarningDisplay != null)
        {
            WaveWarningDisplay.text = "";
        }

        // I use a switch statement to check which wave needs to spawn.
        switch (waveNumber)
        {
            case 1:
                // If the wave number is 1, I spawn the first wave and update its status.
                Wave1();
                wave1Spawned = true;
                break;

            case 2:
                // If the wave number is 2, I spawn the second wave and update its status.
                Wave2();
                wave2Spawned = true;
                break;

            case 3:
                // If the wave number is 3, I spawn the third wave and update its status.
                Wave3();
                wave3Spawned = true;
                break;
        }
    }

    public void StartingAsteroidDestroyed(GameObject asteroid)
    {
        // I remove the destroyed asteroid from the starting asteroid list.
        if (startingAsteroids.Remove(asteroid))
        {
            // I display how many starting asteroids are still left.
            Debug.Log("Starting asteroid destroyed. Remaining: " + startingAsteroids.Count);
        }


        // I check if all the starting asteroids have been destroyed.
        if (startingAsteroids.Count == 0)
        {
            // If the list is empty, I call WinGame in the GameManager.
            GM.WinGame();
        }


    }


    public void RegisterStartingAsteroid(GameObject asteroid)
    {
        // I add the asteroid to the list so that it can be tracked as a starting asteroid.
        startingAsteroids.Add(asteroid);

        Debug.Log("Starting asteroid registered. Total starting asteroids: " + startingAsteroids.Count);

    }

    private Vector3 GetRandomSpawnPosition()
    {
        // I generate random X, Y and Z positions inside the spawn area.
        float x = Random.Range(-spawnAreaX, spawnAreaX);
        float y = Random.Range(-spawnAreaY, spawnAreaY);
        float z = Random.Range(-spawnAreaZ, spawnAreaZ);

        // I return the random position added to the spawner's current position.
        return transform.position + new Vector3(x, y, z);
    }
}