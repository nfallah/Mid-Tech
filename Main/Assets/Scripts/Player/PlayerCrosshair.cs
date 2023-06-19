using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCrosshair : MonoBehaviour
{
    [SerializeField] PlayerManager manager;

    [SerializeField] float interactDistance;

    [SerializeField] Image crosshair;

    [SerializeField] Sprite selected, unselected;

    private void Update()
    {
        Ray r = new Ray(manager.playerCamera.transform.position, manager.playerCamera.transform.forward);

        if (Physics.Raycast(r, out RaycastHit obj, interactDistance) && (obj.transform.gameObject.layer == 8 || obj.transform.gameObject.layer == 9))
        {
            crosshair.sprite = selected;

            if (Input.GetKeyDown(Settings.playerInteractKey))
            {
                try
                {
                    Interactable interactable = obj.transform.GetComponent<Interactable>();

                    interactable.Execute();
                }

                catch
                {
                    throw new Exception("GameObject with Interactable layer has no reference to interface Interactable; terminating.");
                }
            }
        }

        else
        {
            crosshair.sprite = unselected;
        }
    }

    public bool CrosshairVisibility { set { crosshair.transform.gameObject.SetActive(value); } }
}