using System;
using UnityEngine;

[Serializable]
public class ScreenButton
{
    [SerializeField] Vector2 topLeft, bottomRight;

    public Vector2 TopLeft { get { return topLeft; } }

    public Vector2 BottomRight { get { return bottomRight; } }
}