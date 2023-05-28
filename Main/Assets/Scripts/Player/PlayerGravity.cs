using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGravity : MonoBehaviour
{
    [SerializeField] PlayerManager manager;

    [SerializeField] float gravity, // Constant for gravity
    jumpVelocity,  // How much the player's velocity should increase when jumping
    terminalVelocity; // The fastest speed the player can travel (both positive and negative)

    private const float epsilon = 0.081f; // The minimum distance from the ground that counts as being grounded

    private float yVelocity; // Current player velocity in playtime

    // FixedUpdate is utilized for physics-based processes, meaning that Time.deltaTime does not need to be utilized
    private void FixedUpdate()
    {
        manager.characterController.Move(Vector3.up * yVelocity);

        Ray r = new Ray(transform.position, Vector3.down);

        // Determines whether the position under the player is solid by a magnitude of epsilon
        if (Physics.Raycast(r, epsilon)) // Is touching the ground
        {
            manager.isJumping = false; // Allows the player to jump again
            manager.onGround = true;
            yVelocity = 0;
        }

        else // Is not touching the ground
        {
            manager.onGround = false;
            yVelocity = Mathf.Clamp(yVelocity - gravity, -terminalVelocity, terminalVelocity);
        }
    }

    private void Update()
    {
        // Allows the player to jump
        if (Input.GetKeyDown(KeyCode.Space) && manager.onGround && !manager.isJumping)
        {
            manager.isJumping = true;
            yVelocity += jumpVelocity;
        }
    }
}