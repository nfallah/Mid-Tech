using UnityEngine;

public class SilhouetteManager : MonoBehaviour
{
    [SerializeField] GameObject back, // Back GameObject of the silhouette
    front; // Front GameObject of the silhouette

    [SerializeField] float fadeEnd,// How close the player should be in order to obtain full transparency
    fadeStart; // How close the player should be in order to begin transparency

    private Material backMaterial, // Material of the silhouette back
    frontMaterial; // Material of the silhouette front

    void Start()
    {
        // Creates a reference to the front and back materials for future alpha alterations
        backMaterial = back.GetComponent<MeshRenderer>().material;
        frontMaterial = front.GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        Fade();
    }

    // Controls the silhouette fading based on fadeStart and fadeEnd based on distance from the player
    private void Fade()
    {
        float distance = (PlayerManager.Instance.transform.position - transform.position).magnitude; // Distance between silhouette and player

        if (distance > fadeStart) return; // No need for calculations if player is far away

        // Calculates the alpha value between 0 (invisible) and 1 (opaque)
        float clampedDistance = Mathf.Clamp(distance - fadeEnd, 0, fadeStart);
        float fadeValue = clampedDistance / (fadeStart - fadeEnd);

        // Assigns the new alpha value
        Color temp = backMaterial.color;
        temp.a = fadeValue;
        backMaterial.color = frontMaterial.color = temp;
    }
}