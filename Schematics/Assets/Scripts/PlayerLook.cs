using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    // Can optimize this script later.

    [SerializeField] Camera lookCamera;

    [SerializeField] float lookSpeed;

    private Vector3 cameraLook, playerLook;

    private void Start()
    {
        cameraLook = lookCamera.transform.localEulerAngles;
        playerLook = transform.eulerAngles;
    }

    void Update()
    {
        float x = -Input.GetAxis("Mouse Y") * lookSpeed * Time.deltaTime;
        float y = Input.GetAxis("Mouse X") * lookSpeed * Time.deltaTime;

        cameraLook.x += x;
        playerLook.y += y;

        cameraLook.x = Mathf.Clamp(cameraLook.x, -90, 90);
        playerLook.y %= 360;

        lookCamera.transform.localEulerAngles = cameraLook;
        transform.eulerAngles = playerLook;
    }
}