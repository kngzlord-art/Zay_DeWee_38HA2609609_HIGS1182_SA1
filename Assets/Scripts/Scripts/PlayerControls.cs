using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static PowerUp;

// This script controls the player's movement, rotation, shooting, power-ups and collisions.
public class PlayerControls : MonoBehaviour
{
    [Header("Movement Settings")]
    // These variables control how fast the player moves in different situations.
    public float moveSpeed = 5f;
    public float moveSpeed1 = 10f;
    public float noFuelSpeed = 2f;
    public float mouseSen1 = 2f;

    [Header("Turn&Tilt Settings")]
    // I use these variables to control how fast the ship turns and how much it tilts.
    public float turnSpeed = 200f;
    public float tiltAmount = 20f;
    public float tiltSpeed = 5f;

    [Header("Rotation Setting")]
    // These variables are used for the ship's rotation and the camera's vertical rotation.
    public float rot;
    private float rotx = 0f;

    [Header("Shooting")]
    // These variables are used to control the laser shooting system.
    public Camera playerCamera;
    public float shootingDistance;
    public Transform laserStartpoint;
    public float fireAmount = 0.5f;
    public float laserDuration = 0.1f;
    public Color LaserColor = Color.red;
    public LayerMask shootLayers;

    // I use these variables to control the laser and the time between shots.
    private LineRenderer laser;
    private float nextFireTime;

    [Header("Fuel Settings")]
    // This controls how much fuel is used when the player moves forward.
    public float fuelUse;

    [Header("Other Settings")]
    // These variables are used for finding targets and activating the player's shield.
    private GameObject target;
    public GameObject shield;
    public string targetTag = "Asteroid1";


    // I use this variable to keep track of the player's current health.
    private float Health;

    // This checks whether the player has already died.
    private bool isDead = false;

    // I use these variables to access the player's stats and physics components.
    public PlayerStats PS;
    private Rigidbody rb;

    [Header("Audio Settings")]
    // These are the audio sources and clips used for different player actions.
    public AudioSource Audio;
    public AudioClip moveClip, shootClip, collectClip, powerUpClip, noFuelClip, damageClip, deathClip;

    // I use the Animator to control the player's movement animations.
    private Animator animator;

    // This property stores the enemy drone information.
    public string EnemyDrone { get; private set; }

    // This function runs when the player is first created in the scene.
    private void Start()
    {
        // I find the target object using the tag that I assigned.
        target = GameObject.FindGameObjectWithTag(targetTag);


        // I get the PlayerStats, Rigidbody and Animator components from the player.
        PS = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // I turn off gravity because the ship is meant to move freely in space.
        rb.useGravity = false;


        // If no camera has been assigned, I use the main camera instead.
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }


        // I create a LineRenderer component to display the laser when shooting.
        laser = gameObject.AddComponent<LineRenderer>();
        laser.positionCount = 2;
        laser.startWidth = 0.05f;
        laser.endWidth = 0.05f;
        laser.material = new Material(Shader.Find("Sprites/Default"));
        laser.startColor = LaserColor;
        laser.endColor = LaserColor;
        laser.useWorldSpace = true;
        laser.enabled = false;

        // I make sure the shield is turned off when the game starts.
        shield.SetActive(false);



    }

    // This function runs once every frame and handles the player's regular updates.
    private void Update()
    {
        // I get the player's current health from the PlayerStats script.
        if (PS != null)
        {
            Health = PS.Health;
        }


        // I check if the left mouse button has been pressed to make the player shoot.
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
            PlaySFX(shootClip);
        }

        // I call this function to handle the mouse movement and camera rotation.
        MouseMovement();

        // I check if the player's health has reached zero and if the death sequence has not already started.
        if (Health <= 0 && !isDead)
        {
            isDead = true;

            // I find the GameManager so that I can call the GameOver function.
            GameManager gameManager = FindFirstObjectByType<GameManager>();

            // If the GameManager is found, I start the game over sequence.
            if (gameManager != null)
            {
                Debug.Log("Game Manager Found");
                gameManager.GameOver();
            }
            else
            {
                // I display an error if the GameManager cannot be found.
                Debug.LogError("Game Manager Not Found");
            }
        }



    }


    // FixedUpdate is used for physics-related movement so that it runs at a fixed time interval.
    private void FixedUpdate()
    {
        // I start by using the normal movement speed.
        float currentSpeed = moveSpeed;

        // If the player has no fuel left, I reduce the movement speed.
        if (PS != null && PS.Fuel <= 0)
        {
            currentSpeed = noFuelSpeed;
        }

        // I get the horizontal and vertical input from the player.
        float turnInput = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical") * Time.deltaTime * currentSpeed;

        // I calculate how much the ship should rotate based on the horizontal input.
        float rotation = -turnInput * turnSpeed * Time.fixedDeltaTime;

        // I apply the calculated rotation to the player's Rigidbody.
        rb.MoveRotation(
            rb.rotation * Quaternion.Euler(0f, rotation, 0f)
        );


        // I check if the Space key is being held down to move the ship forward.
        if (Input.GetKey(KeyCode.Space))
        {
            // I move the ship forward using its forward direction and current speed.
            rb.linearVelocity = transform.forward * currentSpeed;

            // If the player still has fuel, I reduce the fuel while moving.
            if (PS != null && PS.Fuel > 0)
            {
                PS.losefuel(fuelUse);
            }


        }
        else
        {
            // If Space is not pressed, I stop the ship's forward movement.
            rb.linearVelocity = Vector3.zero;
        }


        // If the player has no fuel, I play the no-fuel sound.
        if (PS != null && PS.Fuel <= 0)
        {
            PlaySFX(noFuelClip);
        }



        // I call the function that controls the ship's tilt.
        TiltShip(turnInput);

        // I move the ship vertically using the calculated input.
        transform.Translate(0, y, 0);

        // I check whether the player is moving and update the movement animation.
        bool moving = (y != 0f);
        bool movement = Input.GetKey(KeyCode.Space);
        animator.SetBool("isMoving", movement);

        // I check which direction the player is turning and update the matching animations.
        bool turningLeft = (turnInput < 0f);
        bool turningRight = (turnInput > 0f);
        animator.SetBool("isTurningLeft", turningLeft);
        animator.SetBool("isTurningRight", turningRight);

        // I play the movement sound when the player is moving and the audio source is not already playing.
        if (moving && !Audio.isPlaying)
        {
            PlaySFX(moveClip);
        }

        // If the player is not moving forward, I set the idle animation to true.
        if (!movement)
        {
            animator.SetBool("isIdle", true);
        }




    }

    // This function controls how much the ship tilts when the player turns.
    private void TiltShip(float turnInput)
    {
        // I get the horizontal input and use it to calculate the ship's sideways movement.
        float x = Input.GetAxis("Horizontal") * Time.deltaTime * rot;

        // I calculate the target tilt based on the direction the player is turning.
        float targetTilt = -turnInput * tiltAmount;

        // I create the rotation that the ship should move towards.
        Quaternion targetRotation = Quaternion.Euler(
            0f,
            transform.eulerAngles.y,
            targetTilt
        );

        // I use Quaternion.Lerp to smoothly rotate the ship towards the target rotation.
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            tiltSpeed * Time.fixedDeltaTime
        );

        // I move the ship sideways using the calculated horizontal input.
        transform.Translate(x, 0, 0);

    }


    // This function handles the player's mouse movement and camera rotation.
    private void MouseMovement()
    {
        // I get the mouse movement on the X and Y axes and apply the mouse sensitivity.
        float mouseX = Input.GetAxis("Mouse X") * mouseSen1;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSen1;


        // I rotate the player horizontally based on the mouse's X movement.
        transform.Rotate(Vector3.up * mouseX);

        // I update the camera's vertical rotation using the mouse's Y movement.
        rotx -= mouseY;

        // I limit the camera rotation so that it cannot rotate too far up or down.
        rotx = Mathf.Clamp(rotx, -80f, 80f);

        // I check if the main camera exists before changing its rotation.
        if (Camera.main != null)
        {
            // I apply the vertical rotation to the player's camera.
            playerCamera.transform.localRotation =
    Quaternion.Euler(rotx, 0f, 0f);
        }
    }


    // This function handles the player's laser shooting.
    private void Shoot()
    {
        // I check if the laser starting point has been assigned in the Inspector.
        if (laserStartpoint == null)
        {
            Debug.LogWarning("Laser Startpoint has not been assigned!");
            return;
        }

        // I get the laser's starting position, direction and maximum end position.
        Vector3 start = laserStartpoint.position;
        Vector3 direction = laserStartpoint.forward;
        Vector3 end = start + direction * shootingDistance;

        // I use RaycastAll to detect all objects that are hit by the laser.
        RaycastHit[] hits = Physics.RaycastAll(
            start,
            direction,
            shootingDistance,
            shootLayers
        );

        // I sort the objects that were hit so that the closest object is checked first.
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        // I go through each object that the laser has detected.
        foreach (RaycastHit hit in hits)
        {
            // I skip the player itself and any child objects belonging to the player.
            if (hit.collider.transform == transform ||
                hit.collider.transform.IsChildOf(transform))
            {
                continue;
            }

            // I update the laser's end position to where the object was hit.
            end = hit.point;

            Debug.Log("Laser hit: " + hit.collider.name);

            // Check if the object hit is an enemy
            // I look for the EnemyAi component on the object or its parent.
            EnemyAi enemy = hit.collider.GetComponentInParent<EnemyAi>();

            // If an enemy is found, I apply damage to it.
            if (enemy != null)
            {
                // Damage the enemy instead of destroying it
                enemy.hurtEnemy(10f);

                Debug.Log("Enemy has taken 25 damage.");
            }
            else
            {
                // Check if the object is a starting asteroid
                // I find the AsteroidsSpawner so that I can notify it when an asteroid is destroyed.
                AsteroidsSpawner spawner = FindFirstObjectByType<AsteroidsSpawner>();

                if (spawner != null)
                {
                    spawner.StartingAsteroidDestroyed(hit.collider.gameObject);
                }

                // Destroy normal objects hit by the laser
                Destroy(hit.collider.gameObject);
            }

            // I stop checking because the closest object has already been hit.
            break;
        }

        // I set the start and end positions of the laser line.
        laser.SetPosition(0, start);
        laser.SetPosition(1, end);

        // I turn on the laser so that it becomes visible.
        laser.enabled = true;

        // I stop any previous laser hiding routine and start a new one.
        StopCoroutine(nameof(HideLaser));
        StartCoroutine(HideLaser());
    }

    // This function plays a sound effect using the AudioSource.
    void PlaySFX(AudioClip clip)
    {
        // I check that both the audio clip and AudioSource have been assigned before playing the sound.
        if (clip && Audio)
        {
            Audio.PlayOneShot(clip);
        }
    }

    // This coroutine waits for the laser duration before hiding the laser.
    private IEnumerator HideLaser()
    {
        // I wait for the amount of time set in the Inspector.
        yield return new WaitForSeconds(laserDuration);

        // I turn off the laser after the wait is finished.
        laser.enabled = false;
    }

    // This function is called when the player collects a power-up.
    public void ApplyPowerUp(PowerUpType type, float value, float duration)
    {
        // If the power-up is a speed boost, I start the speed boost coroutine.
        if (type == PowerUpType.speed)
        {
            StartCoroutine(SpeedBoost(value, duration));
        }

        // If the power-up is a shield, I start the shield coroutine.
        if (type == PowerUpType.Shield)
        {
            StartCoroutine(EquipSheild(duration));
        }
    }

    // This coroutine increases the player's movement speed for a set amount of time.
    IEnumerator SpeedBoost(float multiplier, float duration)
    {
        // I multiply the normal movement speed by the power-up value.
        moveSpeed *= multiplier;

        // I wait until the power-up duration has finished.
        yield return new WaitForSeconds(duration);

        // I divide the speed by the multiplier to return it to its previous value.
        moveSpeed /= multiplier;
    }

    // This coroutine activates the player's shield for a limited time.
    IEnumerator EquipSheild(float duration)
    {
        // I activate the shield object.
        shield.SetActive(true);

        // I wait until the shield duration has finished.
        yield return new WaitForSeconds(duration);

        // I deactivate the shield after the duration ends.
        shield.SetActive(false);

    }


    // This function runs when the player collides with another object.
    private void OnCollisionEnter(Collision collision)
    {
        // I check if the player has collided with an asteroid.
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            Debug.Log("Player hit by asteroid.");

            // I remove 10 health from the player and play the damage sound.
            PS.hurt(10);
            PlaySFX(damageClip);
        }

        // I check if the player has collected a money object.
        if (collision.gameObject.CompareTag("Money"))
        {
            Debug.Log("Player Obtained Money.");

            // I add 20 money to the player's total and play the collection sound.
            PS.addMoney(20);
            PlaySFX(collectClip);

            // I destroy the collected money object.
            Destroy(collision.gameObject);

        }

        // I check if the player has collected scrap.
        if (collision.gameObject.CompareTag("Scrap"))
        {
            Debug.Log("Player Obtained Scrap.");

            // I add 10 points to the player's score and play the collection sound.
            PS.addScore(10);
            PlaySFX(collectClip);

            // I destroy the collected scrap object.
            Destroy(collision.gameObject);

        }

        // I check if the player has collected a health object.
        if (collision.gameObject.CompareTag("Health"))
        {
            Debug.Log("Player Obtained Health.");

            // I restore 10 health points and play the collection sound.
            PS.heal(10);
            PlaySFX(collectClip);

            // I destroy the collected health object.
            Destroy(collision.gameObject);

        }

        // I check if the player has collided with an enemy drone.
        if (collision.gameObject.CompareTag("EnemyDrone"))
        {
            Debug.Log("Player has collided with enemy Drone and has been destroyed");

            // I remove 100 health from the player, play the death sound and destroy the drone.
            PS.hurt(100);
            PlaySFX(deathClip);

            Destroy(collision.gameObject);


        }

        // I check if the player has collided with a moving asteroid.
        if (collision.gameObject.CompareTag("MovingAsteroid"))
        {
            Debug.Log("Player has collided with a Moving asteroid. PLayer Got hurt");

            // I remove 50 health from the player and play the damage sound.
            PS.hurt(50);
            PlaySFX(damageClip);

            // I destroy the moving asteroid after the collision.
            Destroy(collision.gameObject);

        }

        // I check if the player has collided with a power-up object.
        if (collision.gameObject.CompareTag("PowerUp"))
        {
            Debug.Log("Player has collided with a PowerUp.");

            // I play the collection sound and destroy the power-up object.
            PlaySFX(collectClip);

            Destroy(collision.gameObject);

        }
    }

}