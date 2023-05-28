using UnityEngine;

public class Persistence : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(this);
    }
}