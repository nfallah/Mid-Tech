using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenElementManager : MonoBehaviour
{
    [SerializeField] bool active;

    [SerializeField] Camera playerCamera;

    [SerializeField] float scrollSpeed; // Acounts for the multiplication of Time.deltaTime

    [SerializeField] Sprite cursorSprite, linkSprite;

    [SerializeField] string screenElementName; // The current screen element, used with the inspector for clarity

    [SerializeField] Vector2 startingCursorPos; // Relative to the position of parent GameObject, inputted in Unity units (not pixels)

    private GameObject mouse, cursor, link;

    [SerializeField] ScreenElement currentScreenElement;

    private string currentCollisionKey;

    public Vector2 cursorPos;

    private void Start() // Assign the currentScreenElement that the mouse is hovered on top of. Could eventually program this to occur by script (Ray from camera to startingCursorPos)
    {
        // Things may break however since the Update method sets it to null very quickly (currentScreenElement)
        mouse = new GameObject("Mouse");
        mouse.transform.SetParent(transform);
        mouse.transform.localPosition = startingCursorPos;
        cursor = new GameObject("Cursor");
        cursor.transform.SetParent(mouse.transform);
        cursor.transform.localPosition = new Vector3(0.0245f, -0.0375f, -0.001f); // Tested values
        cursor.AddComponent<SpriteRenderer>().sprite = cursorSprite;
        link = new GameObject("Link");
        link.transform.SetParent(mouse.transform);
        link.transform.localPosition = new Vector3(0.008f, -0.048f, -0.001f); // Tested values
        link.AddComponent<SpriteRenderer>().sprite = linkSprite;
        cursorPos = GetCursorPos(mouse.transform.position); // Instead of using startingCursorPos mousePos is used because startingCursorPos is relatiove to the GameObject, whereas we need global coordinates.
        CollisionCheck();
        enabled = active;
    }

    private void Update()
    {
        Ray r = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(r, out RaycastHit rh) && rh.transform.gameObject.layer == 8 && rh.transform.parent.GetComponent<ScreenElement>().screenEM == this)
        {
            Cursor.visible = false;
            currentScreenElement = rh.transform.parent.GetComponent<ScreenElement>();
            screenElementName = currentScreenElement.screenName;
            mouse.transform.position = rh.point;

            // Calculates the cursor position relative to the current screen element; used primarily in the detection of UI collisions.
            cursorPos = GetCursorPos(rh.point);
            CollisionCheck();

            if (currentScreenElement.canScroll)
            {
                float y = -Input.mouseScrollDelta.y * scrollSpeed * Time.deltaTime;

                currentScreenElement.Scroll(y);
            }
        }

        else
        {
            Cursor.visible = true;
            screenElementName = null;
            currentScreenElement = null;
            cursorPos = new Vector2(-1, -1);
        }
    }

    private Vector2 GetCursorPos(Vector2 cursorPos)
    {
        return new Vector2(
            (cursorPos.x - currentScreenElement.offset.x - transform.position.x) * 1000,
            currentScreenElement.screenSize.y * 1000 - (cursorPos.y - currentScreenElement.offset.y - transform.position.y) * 1000 + currentScreenElement.down
            );
    }

    private void CollisionCheck()
    {
        for (int i = 0; i < currentScreenElement.collisions.Length; i++)
        {
            Vector4 currentCollision = currentScreenElement.collisions[i];

            if (BoundsCheck(currentCollision))
            {
                currentCollisionKey = currentScreenElement.collisionKeys[i];
                cursor.SetActive(false);
                link.SetActive(true);
                return;
            }

            currentCollisionKey = "";
            cursor.SetActive(true);
            link.SetActive(false);
        }
    }

    private bool BoundsCheck(Vector4 currentCollision)
    {
        return (cursorPos.x >= currentCollision.x && cursorPos.y >= currentCollision.y && cursorPos.x <= currentCollision.z && cursorPos.y <= currentCollision.w);
    }
}