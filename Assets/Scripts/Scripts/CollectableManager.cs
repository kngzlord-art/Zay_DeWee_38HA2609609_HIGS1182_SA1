using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    [Header("Collectible")]
    //These are the prefabs variables that i use to store the different collectibles that gets spawned in.
    public GameObject moneyPrefab;
    public GameObject scrapPrefab;
    public GameObject healthPrefab;
    public GameObject powerUpPrefab;

    [Header("Spawning")]
    //THis is the variables that control how many collectibles will spawn in.
    public int MoneycollectibleAmount = 5;
    public int HealthcollectibleAmount = 5;
    public int PowerUpcollectibleAmount = 5;

    [Header("Scrap Amount")]
    //These variables i use to set the minimum ad maximum amount of scrap that canm spawn.
    public int ScrapcollectibleAmount = 10;
    public int minScrap = 10;
    public int maxScrap = 50;


    [Header("Spawn Area")]
    //These variables control the size of the area where the collectibles will spawn in.
    public float spawnAreaX = 30f;
    public float spawnAreaY = 10f;
    public float spawnAreaZ = 30f;

    private void Start()
    {
        //These functions wil run at the start of the game and spawn in the different types of collectibles 
        SpawnMoneyCollectibles();
        SpawnScrapCollectibles();
        SpawnHealthCollectibles();
        SpawnPowerUPCollectibles();
    }

    //This fuctions spawns in the money collectibles.
    public void SpawnMoneyCollectibles()
    {
        //I use a for loop to repeat the spawning a specified amount of times 
        for (int i = 0; i < MoneycollectibleAmount; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            
            //I use Instantiate to create the money collectible in the game at a random position.
            Instantiate(
                moneyPrefab,
                spawnPosition,
                Quaternion.identity
            );

            Debug.Log("Collectible Spawned");
        }
    }

    //This fuctions spawns in the Scrap collectibles.
    public void SpawnScrapCollectibles()
    {
        ScrapcollectibleAmount = Random.Range(minScrap, maxScrap + 1);

        //Here i also use a for loop to repeat the spawning a specified amount of times 
        for (int i = 0; i < ScrapcollectibleAmount; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();

            //I use Instantiate to create the Scrap collectible in the game at a random position.
            Instantiate(
                scrapPrefab,
                spawnPosition,
                Quaternion.identity
            );

            Debug.Log("Collectible Spawned");
        }
    }

    //This fuctions spawns in the Health collectibles.
    public void SpawnHealthCollectibles()
    {
        //Here i also use a for loop to repeat the spawning a specified amount of times 
        for (int i = 0; i < HealthcollectibleAmount; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();

            //I use Instantiate to create the Health collectible in the game at a random position.
            Instantiate(
                healthPrefab,
                spawnPosition,
                Quaternion.identity
            );

            Debug.Log("Collectible Spawned");
        }
    }

    //This fuctions spawns in the PowerUp collectibles.
    public void SpawnPowerUPCollectibles()
    {
        //Here i also use a for loop to repeat the spawning a specified amount of times 
        for (int i = 0; i < PowerUpcollectibleAmount; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();

            //I use Instantiate to create the PowerUp collectible in the game at a random position.
            Instantiate(
                powerUpPrefab,
                spawnPosition,
                Quaternion.identity
            );

            Debug.Log("Collectible Spawned");
        }
    }

    //This fuction uses Random.Range to generate random positions 
    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(-spawnAreaX, spawnAreaX);
        float y = Random.Range(-spawnAreaY, spawnAreaY);
        float z = Random.Range(-spawnAreaZ, spawnAreaZ);

        //Here i return the random position and add  it to the position of the gameobject the script is attached to.
        return transform.position + new Vector3(x, y, z);
    }

}