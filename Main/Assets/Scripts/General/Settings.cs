using UnityEngine;

public class Settings
{
    // Private constructor; ensures there is no instance creation
    private Settings() {}

    public static float lookSensitivity = 300;

    public static KeyCode dialogueInteractKey = KeyCode.Space,
    playerInteractKey = KeyCode.E,
    playerJumpKey = KeyCode.Space;
}