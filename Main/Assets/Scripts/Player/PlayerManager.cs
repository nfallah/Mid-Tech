using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public enum State { UNLOCKED, DIALOGUE, LOCKED, SCREEN }

    private static PlayerManager instance; // Singleton state reference
    
    private static bool isCreated; // Utilized to enforce a singleton state pattern

    [SerializeField] PlayerCrosshair playerCrosshair;

    [SerializeField] PlayerGravity playerGravity;

    [SerializeField] PlayerTransform playerTransform;

    public Camera playerCamera;

    public CharacterController characterController; // The character controller that determines player movement and related collisions

    [SerializeField] float cameraTransitionTime;

    private Screen targetScreen;

    private State currentState, prevState;

    private Vector3 prevCameraPos, prevCameraRot;

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
        SwitchState(currentState);
    }

    public void SwitchState(State newState)
    {
        prevState = currentState;
        currentState = newState;

        switch (newState)
        {
            case State.UNLOCKED:
                UnlockedState();
                break;
            case State.DIALOGUE:
                DialogueState();
                break;
            case State.LOCKED:
                LockedState();
                break;
            case State.SCREEN:
                ScreenState();
                break;
        }
    }

    public void SwitchState(State newState, bool alterState)
    {
        State prevState = this.prevState;
        State currentState = this.currentState;

        SwitchState(newState);

        if (!alterState)
        {
            this.prevState = prevState;
            this.currentState = currentState;
        }
    }

    private void UnlockedState()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerCrosshair.enabled = true;
        playerCrosshair.CrosshairVisibility = true;
        playerGravity.CanJump = true;
        playerTransform.enabled = true;
    }

    private void DialogueState()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerCrosshair.enabled = false;
        playerCrosshair.CrosshairVisibility = false;
        playerGravity.CanJump = false;
        playerTransform.enabled = false;
    }

    private void LockedState()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerCrosshair.enabled = false;
        playerCrosshair.CrosshairVisibility = false;
        playerGravity.CanJump = false;
        playerTransform.enabled = false;
    }

    /*
     * First disables relevant UI (and corresponding interactions), then the player movement script, and then moves the camera to the screen.
     * Furthermore, enables interactions with the screen.
     */
    private void ScreenState()
    {
        if (prevState != State.SCREEN)
        {
            SwitchState(State.LOCKED, false);
            prevCameraPos = playerCamera.transform.position;
            prevCameraRot = playerCamera.transform.eulerAngles;
            StartCoroutine(FadeIn());
        }
        else
        {
            targetScreen.enabled = false;
            SwitchState(State.LOCKED, false);
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeIn()
    {
        Vector3 startPos = prevCameraPos, endPos = targetScreen.CameraPosition;
        Quaternion startRot = Quaternion.Euler(prevCameraRot), endRot = Quaternion.Euler(targetScreen.CameraRotation);

        float timer = 0, timerPercent;

        while (timer != cameraTransitionTime)
        {
            timer = Mathf.Clamp(timer + Time.deltaTime, 0, cameraTransitionTime);
            timerPercent = timer / cameraTransitionTime;
            playerCamera.transform.position = Vector3.Lerp(startPos, endPos, timerPercent);
            playerCamera.transform.rotation = Quaternion.Slerp(startRot, endRot, timerPercent);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        Cursor.lockState = CursorLockMode.None;
        playerCrosshair.enabled = false;
        playerCrosshair.CrosshairVisibility = false;
        playerGravity.CanJump = false;
        playerTransform.enabled = false;
        targetScreen.enabled = true;
    }

    private IEnumerator FadeOut()
    {
        Vector3 startPos = targetScreen.CameraPosition, endPos = prevCameraPos;
        Quaternion startRot = Quaternion.Euler(targetScreen.CameraRotation), endRot = Quaternion.Euler(prevCameraRot);

        float timer = 0, timerPercent;

        while (timer != cameraTransitionTime)
        {
            timer = Mathf.Clamp(timer + Time.deltaTime, 0, cameraTransitionTime);
            timerPercent = timer / cameraTransitionTime;
            playerCamera.transform.position = Vector3.Lerp(startPos, endPos, timerPercent);
            playerCamera.transform.rotation = Quaternion.Slerp(startRot, endRot, timerPercent);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        targetScreen = null;
        SwitchState(State.UNLOCKED);
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

    public Screen TargetScreen { set { targetScreen = value; } }
}