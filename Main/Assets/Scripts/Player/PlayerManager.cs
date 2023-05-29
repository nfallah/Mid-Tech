using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager instance; // Singleton state reference

    private static bool isCreated; // Utilized to enforce a singleton state pattern

    public Camera playerCamera;

    public CharacterController characterController; // The character controller that determines player movement and related collisions

    private void Awake()
    {
        // Destroys script reference if it has already been established anywhere else
        if (isCreated)
        {
            Destroy(this);
            throw new Exception("PlayerManager instance already established; terminating.");
        }

        instance = this;
        isCreated = true;
    }

    public void Lock()
    {

    }

    public void Unlock()
    {

    }

    // Instance getter method
    public static PlayerManager Instance
    {
        get
        {
            if (instance == null)
            {
                throw new Exception("PlayerManager instance not yet established; terminating.");
            }

            return instance;
        }
    }
}