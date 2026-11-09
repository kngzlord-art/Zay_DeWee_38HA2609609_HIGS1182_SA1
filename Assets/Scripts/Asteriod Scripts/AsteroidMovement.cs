using Unity.VisualScripting;
using UnityEngine;

public class AsteroidAI : MonoBehaviour
{

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;


    private Transform player;

    private void Start()
    {
        // I use FindGameObjectWithTag to find the player in the scene using its Player tag.
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        // I check if the player was found before getting its Transform component.
        if (playerObj != null)
        {
            // I store the player's Transform so that the asteroid knows where to move.
            player = playerObj.transform;
        }
    }

    private void Update()
    {
        // I check if the player exists before trying to move towards it.
        if (player == null)
            return;

        // I call the Movement function to make the asteroid follow the player.
        Movement();
    }

    private void Movement()
    {
        // I use Vector3.MoveTowards to move the asteroid towards the player's position.
        // Time.deltaTime makes the movement consistent across different frame rates.
        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime
        );
    }
}