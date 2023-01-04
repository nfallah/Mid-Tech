using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenElementManager : MonoBehaviour
{
    [SerializeField] Camera playerCamera;

    [SerializeField] float scrollSpeed; // Acounts for the multiplication of Time.deltaTime

    [SerializeField] Sprite cursorSprite, linkSprite;

    [SerializeField] string currentScreenElement; // The current screen element, used with the inspector for clarity

    [SerializeField] Vector2 startingCursorPos; // Relative to the position of parent GameObject, inputted in Unity units (not pixels)

    private GameObject mouse, cursor, link;

    private ScreenElement currentSE;

    private Vector2 cursorPos;

    private void Start()
    {
        mouse = new GameObject("Mouse");
        mouse.transform.SetParent(transform);
        mouse.transform.localPosition = startingCursorPos;
        cursor = new GameObject("Cursor");
        cursor.transform.SetParent(mouse.transform);
        cursor.transform.localPosition = new Vector3(0.0245f, -0.0375f, 0); // Tested values
        cursor.AddComponent<SpriteRenderer>().sprite = cursorSprite;
        link = new GameObject("Link");
        link.transform.SetParent(mouse.transform);
        link.transform.localPosition = new Vector3(0.008f, -0.048f, 0); // Tested values
        link.AddComponent<SpriteRenderer>().sprite = linkSprite;

        link.SetActive(false); // change this once collision class is already out and then script knows what to enable/disable based on collision input.
    }

    private void Update()
    {
        Ray r = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(r, out RaycastHit rh) && rh.transform.gameObject.layer == 8 && rh.transform.parent.GetComponent<ScreenElement>().screenEM == this)
        {
            currentSE = rh.transform.parent.GetComponent<ScreenElement>();
            currentScreenElement = currentSE.screenName;
            mouse.transform.position = rh.point;

            if (currentSE.canScroll)
            {
                float y = -Input.mouseScrollDelta.y * scrollSpeed * Time.deltaTime;

                currentSE.Scroll(y);
            }
        }

        else
        {
            currentScreenElement = null;
            currentSE = null;
        }
    }
}