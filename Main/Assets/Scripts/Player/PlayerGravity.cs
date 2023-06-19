using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerGravity : MonoBehaviour
{
    [SerializeField] PlayerManager manager;

    [SerializeField] float gravityConstant, jumpVelocity, terminalVelocity;

    private bool canJump, isJumping;

    private const float epsilon = 0.8f;

    private float yVelocity;

    private void Update()
    {
        Vector3 oldPos = transform.position;

        manager.characterController.Move(Vector3.down * epsilon);

        bool isFalling = oldPos != transform.position;

        manager.characterController.Move(Vector3.down * (transform.position.y - oldPos.y));

        if (isFalling)
        {
            yVelocity = Mathf.Clamp(yVelocity - gravityConstant * Time.deltaTime, -terminalVelocity, terminalVelocity);
        }
        else
        {
            isJumping = false;
            yVelocity = 0;
        }

        if (Input.GetKey(Settings.playerJumpKey) && !isJumping && canJump)
        {
            yVelocity += jumpVelocity;
            isJumping = true;
        }

        oldPos = transform.position;

        manager.characterController.Move(Vector3.up * yVelocity * Time.deltaTime);

        if (transform.position == oldPos && yVelocity > 0)
        {
            yVelocity = 0;
        }
    }

    public bool CanJump { set { canJump = value; } }
}