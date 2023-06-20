using System;
using UnityEngine;

public class Screen : MonoBehaviour
{
    public enum Side { LEFT, RIGHT, DOWN, UP, BACK, FORWARD }

    [SerializeField] float raycastDistance, scrollSpeed;

    [SerializeField] Side screenSide;

    [SerializeField] Sprite cursorSprite, linkSprite;

    [SerializeField] Vector2 startingMousePosition;

    [SerializeField] Vector3 cameraPosition;

    private readonly float mouseRaycastDistance = 0.0011f, mouseRaycastOffset = 0.001f;

    private GameObject cursor, link, mouse;

    private int currentScreenButtonIndex;

    private ScreenElement currentScreenElement;

    private Vector2 mousePosition;

    // Tested values
    private readonly Vector3
    cursorPos = new Vector3(0.0245f, -0.0375f, -0.001f),
    linkPos = new Vector3(0.008f, -0.048f, -0.001f);

    private void Start()
    {
        CreateCursor();
        enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(Settings.playerInteractKey))
        {
            PlayerManager.Instance.SwitchState(PlayerManager.State.SCREEN);
            return;
        }

        Ray ray = PlayerManager.Instance.playerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance) && ScreenElementCheck(hit))
        {
            Cursor.visible = false;
            mouse.transform.position = hit.point;
            currentScreenElement = hit.transform.GetComponent<ScreenElementReference>().ScreenElement;
            UpdateMousePosition();

            bool shouldDisplayLink = ScreenButtonCheck();

            cursor.SetActive(!shouldDisplayLink);
            link.SetActive(shouldDisplayLink);

            float scrollDistance = -Input.mouseScrollDelta.y * scrollSpeed * Time.deltaTime;

            currentScreenElement.Scroll(scrollDistance);

            // If the mouse hovers on a valid screen button, it will attempt to execute its corresponding event.
            if (Input.GetMouseButtonDown(0) && shouldDisplayLink)
            {
                currentScreenElement.ScreenEvent.Execute(currentScreenButtonIndex);
            }
        }

        else
        {
            Cursor.visible = true;
            currentScreenElement = null;
        }
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
        mouse.transform.localEulerAngles = MouseSideRotation;
        cursor.layer = link.layer = 9; // "Interactable (UI)" layer

        Ray ray = new Ray(mouse.transform.position - mouseRaycastOffset * Direction, Direction);

        if (Physics.Raycast(ray, out RaycastHit hit, mouseRaycastDistance) && ScreenElementCheck(hit))
        {
            currentScreenElement = hit.transform.GetComponent<ScreenElementReference>().ScreenElement;
            UpdateMousePosition();
        }

        else { throw new Exception("Mouse cursor not situated on top of a valid screen element; terminating."); }

        bool shouldDisplayLink = ScreenButtonCheck();

        cursor.SetActive(!shouldDisplayLink);
        link.SetActive(shouldDisplayLink);
    }

    private void UpdateMousePosition()
    {
        Vector2 bottomLeftDistance = GlobalToLocalPosition(mouse.transform.localPosition - currentScreenElement.ScreenObject.transform.localPosition);

        bottomLeftDistance.y = currentScreenElement.PhysicalHeight - bottomLeftDistance.y;
        bottomLeftDistance *= 1000;
        bottomLeftDistance.y += currentScreenElement.CurrentHeight;
        mousePosition = bottomLeftDistance;
    }

    private bool ScreenElementCheck(RaycastHit hitInfo)
    {
        return hitInfo.transform.gameObject.layer == 9 && hitInfo.transform.GetComponent<ScreenElementReference>().ScreenElement.Screen.Equals(this);
    }

    private bool ScreenButtonCheck()
    {
        int traversalIndex = 0;

        foreach (ScreenButton screenButton in currentScreenElement.ScreenButtons)
        {
            Vector2 topLeft = screenButton.TopLeft, bottomRight = screenButton.BottomRight;

            if (mousePosition.x >= topLeft.x && mousePosition.x <= bottomRight.x && mousePosition.y >= topLeft.y && mousePosition.y <= bottomRight.y)
            {
                currentScreenButtonIndex = traversalIndex;
                return true;
            }

            traversalIndex++;
        }

        currentScreenButtonIndex = -1;
        return false;
    }

    private Vector2 GlobalToLocalPosition(Vector3 globalPosition)
    {
        switch (screenSide)
        {
            case Side.LEFT:
                return new Vector2(-globalPosition.z, globalPosition.y);
            case Side.RIGHT:
                return new Vector2(globalPosition.z, globalPosition.y);
            case Side.DOWN:
                return new Vector2(-globalPosition.x, globalPosition.z);
            case Side.UP:
                return new Vector2(globalPosition.x, globalPosition.z);
            case Side.BACK:
                return new Vector2(-globalPosition.x, globalPosition.y);
            case Side.FORWARD:
                return new Vector2(globalPosition.x, globalPosition.y);
            default:
                throw new Exception("GlobalToLocalPosition call failed -- invalid side specified.");
        }
    }

    public Vector3 LocalToGlobalPosition(Vector2 screenPosition)
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

    public Vector3 CameraPosition
    {
        get
        {
            return transform.position + LocalToGlobalPosition(cameraPosition) - Direction * cameraPosition.z;
        }
    }

    public Vector3 CameraRotation
    {
        get
        {
            switch (screenSide)
            {
                case Side.LEFT:
                case Side.RIGHT:
                case Side.DOWN:
                case Side.UP:
                    return MouseSideRotation;
                case Side.BACK:
                case Side.FORWARD:
                    return -MouseSideRotation;
                default:
                    throw new Exception("CameraRotation call failed -- invalid side specified.");
            }
        }
    }

    private Vector3 MouseSideRotation
    {
        get
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
    }

    private Vector3 Direction
    {
        get
        {
            switch (screenSide)
            {
                case Side.LEFT:
                    return Vector3.right;
                case Side.RIGHT:
                    return Vector3.left;
                case Side.DOWN:
                    return Vector3.up;
                case Side.UP:
                    return Vector3.down;
                case Side.BACK:
                    return Vector3.back;
                case Side.FORWARD:
                    return Vector3.forward;
                default:
                    throw new Exception("Direction call failed -- invalid side specified.");
            }
        }
    }

    public Side ScreenSide { get { return screenSide; } }
}