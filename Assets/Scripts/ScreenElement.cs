using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ScreenElement : MonoBehaviour
{
    public enum Axis { X, Y, Z }

    public enum Direction { NEGATIVE, POSITIVE }

    [SerializeField] ScreenElementManager screenEM;

    [HideInInspector] public float sstsr; // Screen size to texture size ratio, can be at most 1

    [HideInInspector] public float down; // How far down the texture the monitor is displaying, from the top, inputted in pixels

    [HideInInspector] public GameObject screenElement; // Assigned once the scene runs

    [SerializeField] Axis screenAxis;

    [SerializeField] bool visibleOnStart;

    [SerializeField] Direction screenDirection;

    [SerializeField] int screenMaterialTextureHeight, startingHeight /* From the top, inputted in pixels */;

    [SerializeField] Material screenMaterial;

    [SerializeField] string screenName;

    [SerializeField] Vector2 screenSize;

    private Mesh m;

    private MeshRenderer mr;

    private readonly int[] trianglesNeg = new int[] { 2, 1, 3, 3, 1, 0 }, trianglesPos = new int[] { 0, 1, 3, 3, 1, 2 };

    private void Start()
    {
        CreateScreen(); // Instantiates the physical GameObject
        sstsr = screenSize.y * 1000 / screenMaterialTextureHeight;
        down = startingHeight;
        Scroll(0);
        screenElement.SetActive(visibleOnStart);
    }

    public void Scroll(float amount)
    {
        down = Mathf.Clamp(down + amount, 0, (1 - sstsr) * screenMaterialTextureHeight);

        float shtsr = down / screenMaterialTextureHeight; // Current height to texture size ratio, can be at most 1 - sstsr

        m.uv = new Vector2[]
        {
            new Vector2(0, 1 - shtsr - sstsr), // Bottom left
            new Vector2(0, 1 - shtsr), // Top left
            new Vector2(1, 1 - shtsr), // Top right
            new Vector2(1, 1 - shtsr - sstsr) // Bottom right
        };
    }

    private void CreateScreen()
    {
        screenElement = new GameObject(screenName);
        screenElement.transform.SetParent(screenEM.transform);
        screenElement.transform.localPosition = Vector3.zero;
        m = screenElement.AddComponent<MeshFilter>().mesh;
        mr = screenElement.AddComponent<MeshRenderer>();

        switch (screenAxis)
        {
            case Axis.X:
                m.vertices = new Vector3[]
                {
                    Vector3.zero, // Bottom left
                    new Vector3(0, screenSize.y, 0), // Top left
                    new Vector3(screenSize.x, screenSize.y, 0), // Top right
                    new Vector3(screenSize.x, 0, 0) // Bottom right
                };

                break;

            case Axis.Y:
                m.vertices = new Vector3[]
                {
                    Vector3.zero, // Bottom left
                    new Vector3(0, 0, screenSize.y), // Top left
                    new Vector3(screenSize.x, 0, screenSize.y), // Top right
                    new Vector3(screenSize.x, 0, 0) // Bottom right
                };

                break;

            case Axis.Z:
                m.vertices = new Vector3[]
                {
                    Vector3.zero, // Bottom left
                    new Vector3(0, screenSize.y, 0), // Top left
                    new Vector3(0, screenSize.y, screenSize.x), // Top right
                    new Vector3(0, 0, screenSize.x) // Bottom right
                };

                break;
        }

        if (screenDirection == Direction.POSITIVE)
        {
            m.triangles = trianglesPos;
        }

        else
        {
            m.triangles = trianglesNeg;
        }

        mr.material = screenMaterial;
        screenElement.AddComponent<MeshCollider>();
        screenElement.layer = 8; // Adds the "Screen Element" layer for Raycast conditionals
    }
}