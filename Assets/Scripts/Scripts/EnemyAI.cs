using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAi : MonoBehaviour
{
    //Here i create an enum to give the enemy three different states.
    public enum EnemyState { Idle, Patrol, Chase }
    public EnemyState state;

    //These variables control how many enemies moves between its patrol points 
    [Header("Patrol")]
    public Transform[] waypoints;
    public float waitAtPoint = 1.5f;
    public bool loop = true;

    //Here the variables are used to detect the player and determine when the enemy should chase the player.
    [Header("Player")]
    public Transform player;
    public float detectRange = 5f;
    public float loseRange = 8f;

    //these variables control the movement of the enemy during the patrol state and also during the chase state 
    [Header("Movement")]
    public float chaseSpeed = 3.5f;
    public float patrolSpeed = 1.5f;

    //These variable control the player health and the health display.
    [Header("Movement")]
    public int maxHealth = 100;
    public float Health = 100;
    public Slider HealthSlider;

    //Here the variables allow me to assign an AudioSource and the sound effects gor when the enemy takes damage or dies.
    [Header("Audio Settings")]
    public AudioSource Audio;
    public AudioClip damageClip, deathClip;

    //This animator variable i use to access an control the animations for the enemy
    private Animator animator;

    //The index keeps track of which waypoint the enemy is currently moving towards and the waitTimer is used to control how long the enemy waits at a waypoint.
    private int index = 0;
    private float waitTimer = 0f;


    private void Awake()
    {
        //I use MathF.Clamp to make sure the enemy's health does not go below its maximum health.
        Health = Mathf.Clamp(Health, 0, maxHealth);
    }

    private void Start()
    {
        //I call this function to set up the health slider when the game starts.
        RefreshUI();

        //I check i theplayer variable has been assigned in the Inspector.
        if (!player)
        {
            //Here, if no player has been asigned, I search for a gameObject with player tag.
            GameObject playerobj = GameObject.FindGameObjectWithTag("Player");


            if (playerobj)
            {
                //If the player is found, I assign its transform to the player variable.
                player = playerobj.transform;
            }
        }

        //Here i check if the enemy is supposed to start patrolling and if there are waypoints assigned.
        if (state == EnemyState.Patrol && waypoints != null && waypoints.Length > 0)
        {
            goTo(index);
        }

        //I use getComponent to get the animator component attached to the enemy.
        animator = GetComponent<Animator>();    
    }


    /*In my Update i update the health slier so that it displays the enemy's current health.
     * I check if the player has been found and check the distance between the enemy and player
     * I use Vector3.Distance to calculate the distance 
     * I check if the nemy is chasing the player, if not and the enemy detects the player i change the state to chase.
     * If the enemy does not detect the player the state gets changed to patrol.
     * If there are waypoints the enemy can patrol
     * if there are no waypoints the enemy state is set to idle.
     */
    private void Update()
    {
        UpdateUI();

        if (player)
        {
            float dist = Vector3.Distance(transform.position, player.position);

            
            if (state != EnemyState.Chase && dist <= detectRange)
            {
                state = EnemyState.Chase;
               
            }

            
            else if (state == EnemyState.Chase && dist > loseRange)
            {
                if (waypoints != null && waypoints.Length > 0)
                {
                    state = EnemyState.Patrol;
                    goTo(index);
                    
                }
                else
                {
                    state = EnemyState.Idle;
                   
                }
            }
        }



        
        switch (state)
        {
            case EnemyState.Idle:
                updateIdle(); 
                animator.SetBool("isIdle", true);
                break;

            case EnemyState.Patrol:
                animator.SetBool("isMoving", true);
                updatePatrol();
                break;

            case EnemyState.Chase:
                updateChase();
                animator.SetBool("isMoving", true);
                break;
        }
    }

 

    //In my UpdateUi i update the Health slider 
   
    private void UpdateUI()
    {
        HealthSlider.value = Health;
        
    } 


    //This function i use to set the health slider 's minimum and the maximum and starting values and i aslo check if the health slider has been assinged.
    private void RefreshUI()
    {
        if (HealthSlider != null)
        {
            HealthSlider.minValue = 0;
            HealthSlider.maxValue = maxHealth;
            HealthSlider.value = Health;
        }
    }

    /*My hurtEnemy fuction hurts the enemy and subtracts the a damage amount from the current health.
     * I use Clamp to make sure the health stays between 0 and the maximum health.
     * I display the enemy's remaining health in the Unity Console.
     * I play the damage sound effect when the enemy gets hit.
     * I update the health slider to show the new health value.
     * If the enemy has no more health left i call the die() function. 
     */
    public void hurtEnemy(float amount)
    {
        Health -= amount;
        Health = Mathf.Clamp(Health, 0, maxHealth);

        Debug.Log("Enemy hit! Remaining health: " + Health);

        PlaySFX(damageClip);

        UpdateUI();

        if(Health <= 0)
        {
            Die();
        }
    }

    /*
     In my UpdateIdle I control the what happens to the enemy when the enemy is the idle state.
    I decrease the the wait Timer over time using Time.deltaTime.
    I check if the enemy has finished waiting at its current waypoint and check 
    if there have been any waypoints assinged and if there are no waypoints assigned i stop
    this fuction.
    I increase the index to move to the next waypoints in the array
    I check if the index has gone past the last waypoint and if looping is enabled, i reset the index
    back to the first waypoint.
    If looping is disabled, i keep the enemy at the last waypoint and stop it from moving to another one.
    Once the enemy finished waiting, i change its state to patrol.
     */
    public void updateIdle()
    {
        waitTimer -= Time.deltaTime;

        if (waitTimer <= 0f)
        {
            if (waypoints == null || waypoints.Length == 0)
            {
                return;
            }

            index++;

            if (index >= waypoints.Length)
            {
                if (loop)
                {
                    index = 0;
                }
                else
                {
                    index = waypoints.Length - 1;
                    return;
                }
            }

            state = EnemyState.Patrol;
            goTo(index);
        }
    }

    

    /*In my updatePatrol i control how the enemy moves between its assinged waypoints.
     I check for waypoints if there are no waypoints i change the state to idle.
     I check if the waypoints exist before moving to one.
    I use Vector3.MoveToward tomove the enemy towards the the target waypoint
     */
    public void updatePatrol()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            state = EnemyState.Idle;
            return;
        }

        Transform targetWaypoint = waypoints[index];

        if (targetWaypoint == null)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetWaypoint.position,
            patrolSpeed * Time.deltaTime
        );

        Vector3 direction = targetWaypoint.position - transform.position;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        if (Vector3.Distance(transform.position, targetWaypoint.position) <= 0.1f)
        {
            state = EnemyState.Idle;
            waitTimer = waitAtPoint;
        }
    }


    /*
     In my updateChase i control the enemy when it is the chase state.
    I check if the player exist before trying to chase
     */
    public void updateChase()
    {
        if (!player)
        {
            return;
        }

        //if the player exists, i call the chaselayer to handle the actaul movement.
        ChasePlayer();
    }

    /*
     In my ChasePlayer in handle the enemy's movement and rotation when the enemy chases the player.
     I use Vector3.MoveTowards to move the enemy towards the player's position
    I calculate the direction between the enemy and the player so the enemy can face the player.
    I rotate the enemy to face towards the player.
     */
    private void ChasePlayer()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            chaseSpeed * Time.deltaTime
        );

       
        Vector3 direction = player.position - transform.position;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

   

    public void goTo(int i)
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            return;
        }

        if (i < 0 || i >= waypoints.Length)
        {
            return;
        }

        index = i;
    }

    /*
    This function is used to play the audio clips assigned to the enemy
    I check that both the audio and AudioSource have been assigned.

     
     */
    void PlaySFX(AudioClip clip)
    {
        if (clip && Audio)
        {
            Audio.PlayOneShot(clip);
        }
    }


    //This function destroy the gameObject this script is attached to.
    public void Die()
    {
        Destroy(gameObject);
    }


    //This displays the enemy's dections and losing ranges as wire shperes in the scene view.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseRange);
    }
}