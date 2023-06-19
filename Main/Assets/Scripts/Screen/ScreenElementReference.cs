using UnityEngine;

public class ScreenElementReference : MonoBehaviour, Interactable
{
    private ScreenElement screenElement;

    // Switches to the dialogue state
    public void Execute()
    {
        PlayerManager.Instance.TargetScreen = screenElement.Screen;
        PlayerManager.Instance.SwitchState(PlayerManager.State.SCREEN);
    }

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