using UnityEngine;

public class AsteroidTarget : MonoBehaviour
{

    private bool isDestroyed = false;


    public void DestroyAsteroid()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        Debug.Log("The asteroids has been destroyed");

        AsteroidsSpawner spawner = FindAnyObjectByType<AsteroidsSpawner>();

        if (spawner != null)
        {
            spawner.StartingAsteroidDestroyed(gameObject);
        }

        Destroy(gameObject);
    }
}