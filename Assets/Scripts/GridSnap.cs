using UnityEngine;

public class GridSnap : MonoBehaviour
{
    // public float gridSize = 1.0f; // Adjust this to match your grid cell size
    // public float speed = 5.0f; // Adjust the movement speed

    // void Update()
    // {
    //     // Handle input movement
    //     float horizontalInput = Input.GetAxis("Horizontal");
    //     float verticalInput = Input.GetAxis("Vertical");
    //     Vector2 movement = new Vector2(horizontalInput, verticalInput);

    //     // Move the GameObject
    //     if (movement != Vector2.zero)
    //     {
    //         Vector3 newPosition = transform.position + (new Vector3(horizontalInput, verticalInput, 0) * speed * Time.deltaTime);
    //         transform.position = newPosition;
    //     }

    //     // Snap the GameObject to the grid
    //     SnapToGrid();
    // }

    // void SnapToGrid()
    // {
    //     // Get the current position
    //     Vector3 currentPosition = transform.position;

    //     // Snap the position to the grid
    //     float snappedX = Mathf.Round(currentPosition.x / gridSize) * gridSize;
    //     float snappedY = Mathf.Round(currentPosition.y / gridSize) * gridSize;

    //     // Update the GameObject's position
    //     transform.position = new Vector3(snappedX, snappedY, currentPosition.z);
    // }

    public float gridSize = 1.0f; // Change this to match your grid cell size.

    void Update()
    {
        // Round the position of the GameObject to the nearest grid position.
        float x = Mathf.Round(transform.position.x / gridSize) * gridSize;
        float y = Mathf.Round(transform.position.y / gridSize) * gridSize;
        float z = transform.position.z; // Use z-position as-is if it's a 2D grid.

        // Set the GameObject's position to the snapped coordinates.
        transform.position = new Vector3(x, y, z);
    }
}