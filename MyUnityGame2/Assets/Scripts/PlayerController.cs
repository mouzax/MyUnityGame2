using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    float moveSpeed = 5;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal"); // A/D keys or Left/Right Arrow keys
        float verticalInput = Input.GetAxis("Vertical");   // W/S keys or Up/Down Arrow keys

        // Calculate movement direction
        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f);

        // Normalize the movement vector to prevent faster diagonal movement
        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }

        // Apply movement to the player's position
        transform.Translate(movement * moveSpeed * Time.deltaTime);
    }
}
