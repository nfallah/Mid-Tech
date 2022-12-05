using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenElementManager : MonoBehaviour
{
    [SerializeField] bool canHover, canScroll;

    [SerializeField] float scrollSpeed; // Acounts for the multiplication of Time.deltaTime

    [SerializeField] ScreenElement currentScreenElement; // The current screen element, gives added functionality

    [SerializeField] Vector3 startingCursorPos; // Inputted in pixels

    [SerializeField] ScreenElement[] screenElements; // Set of related screen elements working under an instance of the screen element manager

    private GameObject mouse, cursor, link;

    private Vector2 cursorPos;

    private void Start()
    {
        if (canHover) // if you spawn right on top of a interactable link then make sure to change cursor as needed.
        { // also change rotations of cursor depending on the axis, just move axis and direction to this script instead of all the children screen elements.
            mouse = new GameObject("Mouse");
            mouse.transform.SetParent(transform);
            mouse.transform.localPosition = startingCursorPos / 1000;
            cursor = new GameObject("Cursor", typeof(SpriteRenderer));
            
        }
    }

    private void Update()
    {
        float y = Input.mouseScrollDelta.y;

        if (y != 0 && canScroll)
        {
            currentScreenElement.Scroll(-y * scrollSpeed * Time.deltaTime);
        }
    }
}