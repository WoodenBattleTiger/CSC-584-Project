using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding {
public class Coin : MonoBehaviour
{
    // public GameObject objectToControl; // The GameObject to appear and disappear
    // public Vector2 spawnBounds; // The bounds within which the GameObject can appear
    // public float appearanceInterval = 10.0f; // The time interval for appearance

    // private Vector3 spawnPosition; // The position where the GameObject will spawn
    // private float timer = 0.0f; // Timer to track the elapsed time


    // private void Update()
    // {
    //     // Increment the timer with the time since the last frame
    //     timer += Time.deltaTime;

    //     // Check if it's time to make the GameObject appear at a new location
    //     if (timer >= appearanceInterval)
    //     {
    //         // Reset the timer
    //         timer = 0.0f;

    //         // Generate a random position within the specified bounds
    //         float randomX = Random.Range(-spawnBounds.x, spawnBounds.x);
    //         float randomY = Random.Range(-spawnBounds.y, spawnBounds.y);

    //         spawnPosition = new Vector3(randomX, randomY, 0f);

    //         // Set the GameObject's position to the spawn position
    //         objectToControl.transform.position = spawnPosition;

    //         // Make the GameObject appear
    //         objectToControl.SetActive(true);
    //     }


    // }

        // public float speed = 5.0f; 
        // private bool isMoving = false;

        // void Update()
        // {
           
        //     float horizontalInput = Input.GetAxis("Horizontal");
        //     float verticalInput = Input.GetAxis("Vertical");

        //     Vector2 movement = new Vector2(horizontalInput, verticalInput);

        //     isMoving = (horizontalInput != 0 || verticalInput != 0);

        //     if (isMoving)
        //     {
        //         transform.Translate(movement * speed * Time.deltaTime);
        //     }


        
        // }

}
}
