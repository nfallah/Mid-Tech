using UnityEngine;

public class ScreenElementReference : MonoBehaviour
{
    private ScreenElement screenElement;

    public ScreenElement ScreenElement {
        get
        {
            return screenElement;
        }

        set
        {
            screenElement = value;
        }
    }
}