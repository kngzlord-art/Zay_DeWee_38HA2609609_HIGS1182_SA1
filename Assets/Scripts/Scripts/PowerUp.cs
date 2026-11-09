using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType
    {
        speed,
        Shield,
        health,
        Score,
    }

    public PowerUpType type = PowerUpType.speed;
    public float duration = 5f;
    public float value = 2f;

    [Header("Randomizer Setting")]
    public bool randomizePowerUp = true;

    public float minValue = 1f;
    public float maxValue;

    public float minDuration = 1f;
    public float maxDuration = 8f;

    private void Start()
    {
       if(randomizePowerUp)
        {
            type  = (PowerUpType)Random.Range(0, System.Enum.GetValues(typeof(PowerUpType)).Length);

            value = Random.Range(minValue, maxValue);

            if(type == PowerUpType.Shield)
            {
                duration = Random.Range(minDuration, maxDuration);
            }
            else
            {
                duration = 0f;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        PlayerControls player = other.GetComponent<PlayerControls>();
        PlayerStats playerStats = other.GetComponent<PlayerStats>();

        if (playerStats != null && player == null) return;

        if (player != null)
        {
            player.ApplyPowerUp(type, value, duration);
            Destroy(gameObject);
        }

            switch (type)
            {
                case PowerUpType.speed:
                    player?.ApplyPowerUp(type, value, duration);
                    break;
                case PowerUpType.Shield:
                    player?.ApplyPowerUp(type, value, duration);
                    break;
                case PowerUpType.health:
                    playerStats?.heal((int)value);
                    break;
                case PowerUpType.Score:
                    playerStats?.addScore((int)value);
                    break;
            }

          Destroy(gameObject);
    }



}