using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float movementSpeed;

    [SerializeField] CharacterController controller;

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal") * movementSpeed * Time.deltaTime;
        float z = Input.GetAxisRaw("Vertical") * movementSpeed * Time.deltaTime;

        controller.Move(x * transform.right + z * transform.forward);
    }
}
