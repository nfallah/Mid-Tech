using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public CharacterController characterController; // The character controller that determines player movement and related collisions

    public bool onGround, // Whether the player is touching the ground or not
    isJumping; // Failsafe -- ensures the player can only jump once on the off chance its GameObject is still registered as touching the ground

    public void Lock()
    {

    }

    public void Unlock()
    {

    }
}