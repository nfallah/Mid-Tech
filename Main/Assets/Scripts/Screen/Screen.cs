using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Screen : MonoBehaviour
{
    public enum Side { LEFT, RIGHT, DOWN, UP, BACK, FORWARD }

    [SerializeField] bool useCursor;

    [SerializeField] ScreenElement[] screenElements;

    [SerializeField] Side screenSide;

    [SerializeField] Sprite cursorSprite, linkSprite;

    [SerializeField] Vector2 startingMousePosition;

    private GameObject cursor, link, mouse;

    private ScreenElement currentScreenElement;

    private Vector2 mousePosition;

    private readonly Vector3
    cursorPos = new Vector3(0.0245f, -0.0375f, -0.001f),
    linkPos = new Vector3(0.008f, -0.048f, -0.001f);

    private void Start()
    {
        if (useCursor) CreateCursor();
    }

    private void CreateCursor()
    {
        mouse = new GameObject("Mouse");
        mouse.transform.parent = transform;
        mouse.transform.localPosition = LocalToGlobalPosition(startingMousePosition);
        cursor = new GameObject("Cursor", typeof(SpriteRenderer));
        cursor.GetComponent<SpriteRenderer>().sprite = cursorSprite;
        cursor.transform.parent = mouse.transform;
        cursor.transform.localPosition = cursorPos;
        link = new GameObject("Link", typeof(SpriteRenderer));
        link.GetComponent<SpriteRenderer>().sprite = linkSprite;
        link.transform.parent = mouse.transform;
        link.transform.localPosition = linkPos;
        mouse.transform.localEulerAngles = MouseSideRotation();

        //bool shouldDisplayLink = ScreenButtonCheck();

        //cursor.SetActive(!shouldDisplayLink);
        //link.SetActive(shouldDisplayLink);
    }

    private bool ScreenButtonCheck()
    {
        foreach (ScreenButton screenButton in currentScreenElement.ScreenButtons)
        {
            Vector2 topLeft = screenButton.TopLeft, bottomRight = screenButton.BottomRight;

            if (mousePosition.x >= topLeft.x && mousePosition.x <= bottomRight.x && mousePosition.y >= topLeft.y && mousePosition.y <= bottomRight.y) return true;
        }

        return false;
    }

    private Vector3 LocalToGlobalPosition(Vector2 screenPosition)
    {
        switch (screenSide)
        {
            case Side.LEFT:
                return new Vector3(0, screenPosition.y, -screenPosition.x);
            case Side.RIGHT:
                return new Vector3(0, screenPosition.y, screenPosition.x);
            case Side.DOWN:
                return new Vector3(-screenPosition.x, 0, screenPosition.y);
            case Side.UP:
                return new Vector3(screenPosition.x, 0, screenPosition.y);
            case Side.BACK:
                return new Vector3(-screenPosition.x, screenPosition.y, 0);
            case Side.FORWARD:
                return new Vector3(screenPosition.x, screenPosition.y, 0);
            default:
                throw new Exception("LocalToGlobalPosition call failed -- invalid side specified.");
        }
    }

    private Vector3 MouseSideRotation()
    {
        switch (screenSide)
        {
            case Side.LEFT:
                return Vector3.up * 90;
            case Side.RIGHT:
                return Vector3.down * 90;
            case Side.DOWN:
                return new Vector3(-90, 180, 0);
            case Side.UP:
                return Vector3.right * 90;
            case Side.BACK:
                return Vector3.up * 180;
            case Side.FORWARD:
                return Vector3.zero;
            default:
                throw new Exception("MouseSideRotation call failed -- invalid side specified.");
        }
    }

    public Side ScreenSide { get { return screenSide;  } }
}