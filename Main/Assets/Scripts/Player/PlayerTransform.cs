using System;
using UnityEngine;

public class PlayerTransform : MonoBehaviour
{
    [SerializeField] PlayerManager manager;

    [SerializeField]
    float movementSpeed;

    private Vector3 cameraLook, characterLook;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //HUEHUE: move later to PlayerManager class, as well as common values like playerlook, etc
        cameraLook = manager.playerCamera.transform.localEulerAngles;
        characterLook = transform.eulerAngles;
    }

    void Update()
    {
        // Looking
        float xLook = -Input.GetAxis("Mouse Y") * Settings.lookSensitivity * Time.deltaTime;
        float yLook = Input.GetAxis("Mouse X") * Settings.lookSensitivity * Time.deltaTime;

        cameraLook.x = Mathf.Clamp(cameraLook.x + xLook, -89.99f, 89.99f);
        characterLook.y = (characterLook.y + yLook) % 360;
        manager.playerCamera.transform.localEulerAngles = cameraLook;
        transform.eulerAngles = characterLook;

        // Moving
        float xMove = Input.GetAxisRaw("Horizontal") * movementSpeed * Time.deltaTime;
        float zMove = Input.GetAxisRaw("Vertical") * movementSpeed * Time.deltaTime;
        manager.characterController.Move(xMove * transform.right + zMove * transform.forward);
    }
}