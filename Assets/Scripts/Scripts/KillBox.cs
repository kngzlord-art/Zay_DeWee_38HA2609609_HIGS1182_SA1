using UnityEngine;

public class KillBox : MonoBehaviour
{
public PlayerStats PS;
    private bool insideKillBox = false;
    public float killboxDamage;

    void Update()
    {
        if (insideKillBox && PS != null) 
            {
            PS.hurt(killboxDamage);
            }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {            
            PS = other.GetComponent<PlayerStats>();
            if (PS != null)
            {
                insideKillBox = true;
            }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            insideKillBox = false;
            PS = null;

        }
    }
}
