using UnityEngine;

public class StudentManager : MonoBehaviour
{
    [SerializeField] GameObject back, front;

    [SerializeField] float fadeEnd, fadeStart;

    private Material backMaterial, frontMaterial;

    // Start is called before the first frame update
    void Start()
    {
        backMaterial = back.GetComponent<MeshRenderer>().material;
        frontMaterial = front.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        Fade();
    }

    private void Fade()
    {
        float distance = (Global.player.transform.position - transform.position).magnitude;
        if (distance > fadeStart) return;
        float clampedDistance = Mathf.Clamp(distance - fadeEnd, 0, fadeStart);
        float fadeValue = clampedDistance / (fadeStart - fadeEnd);
        Color temp = backMaterial.color;
        temp.a = fadeValue;
        backMaterial.color = frontMaterial.color = temp;
    }
}