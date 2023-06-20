using UnityEngine;

public abstract class ScreenEvent : MonoBehaviour
{
    /*
     * The primary method that is called whenever a screen button is activated
     * "screenButtonIndex" corresponds to its element order assigned within the inspector
     * As such, this method should switch between these indeces via external scripts inheriting this class.
     */
    public abstract void Execute(int screenButtonIndex);
}