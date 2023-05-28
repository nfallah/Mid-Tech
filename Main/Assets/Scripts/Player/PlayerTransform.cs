using System;
using UnityEngine;

public class PlayerTransform : MonoBehaviour
{
    [SerializeField] PlayerManager manager;

    private static PlayerTransform instance; // Singleton state reference

    private static bool isCreated; // Utilized to establish a singleton state pattern
     
    [SerializeField] Camera characterCamera;

    [SerializeField]
    float movementSpeed,
    movementSpeedAir;

    private Vector3 cameraLook, characterLook;

    private void Awake()
    {
        // Destroys script reference if it has already been established anywhere else
        if (isCreated)
        {
            Destroy(this);
            throw new Exception("Player instance already established; terminating.");
        }

        instance = this;
        isCreated = true;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //HUEHUE: move later to PlayerManager class, as well as common values like playerlook, etc
        cameraLook = characterCamera.transform.localEulerAngles;
        characterLook = transform.eulerAngles;
    }

    void Update()
    {
        // Looking
        float xLook = -Input.GetAxis("Mouse Y") * Settings.lookSensitivity * Time.deltaTime;
        float yLook = Input.GetAxis("Mouse X") * Settings.lookSensitivity * Time.deltaTime;

        cameraLook.x = Mathf.Clamp(cameraLook.x + xLook, -89.99f, 89.99f);
        characterLook.y += yLook % 360;
        characterCamera.transform.localEulerAngles = cameraLook;
        transform.eulerAngles = characterLook;

        // Moving
        float moveSpeed = manager.onGround || manager.isJumping ? movementSpeed : movementSpeedAir;
        float xMove = Input.GetAxisRaw("Horizontal") * moveSpeed * Time.deltaTime;
        float zMove = Input.GetAxisRaw("Vertical") * moveSpeed * Time.deltaTime;
        manager.characterController.Move(xMove * transform.right + zMove * transform.forward);
    }

    // Instance getter method
    public static PlayerTransform getPlayerTransform() { return instance; }
}