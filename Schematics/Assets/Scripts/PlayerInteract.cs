using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float interactLimit; // How far the raycast can reach

    [SerializeField] Camera monitorCamera, playerCamera;

    [SerializeField] KeyCode interactKey;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;    
    }

    private void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactLimit))
        {
            int layer = hit.transform.gameObject.layer;
            print(layer);

            switch (layer)
            {
                case 8: // ScreenElement
                    print("touching monitr");
                    if (Input.GetKeyDown(interactKey))
                    {
                        print("letroll");
                        // Camera blending (implement at some point?)
                        // also worry about audio listening.
                        monitorCamera.enabled = true;
                        playerCamera.enabled = false;
                        player.SetActive(false);
                        hit.transform.parent.GetComponent<ScreenElementManager>().enabled = true;
                        Cursor.lockState = CursorLockMode.None;
                    }

                    break;
            }
        }
    }
}
