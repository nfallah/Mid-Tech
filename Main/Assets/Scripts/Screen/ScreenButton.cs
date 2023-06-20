using System;
using UnityEngine;

[Serializable]
public class ScreenButton
{
    // Utilized for organizational purposes within the inspector
    [SerializeField] string buttonName;

    [SerializeField] Vector2 topLeft, bottomRight;

    public Vector2 TopLeft { get { return topLeft; } }

    public Vector2 BottomRight { get { return bottomRight; } }
}