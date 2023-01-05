using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ScreenElement : MonoBehaviour
{
    public ScreenElementManager screenEM; // The brains of a set of ScreenElement instances

    public bool canScroll; // Determines whether the ScreenElementManager instance is allowed to scroll when hovered on ScreenElement instances

    public string screenName; // Name of ScreenElement instance, assigned in the inspector for clarity

    public Vector2 offset; // How far from the parent the ScreenElement instance will be

    public Vector4[] collisions; // Each vector is formatted and inserted into the editor as such: top-left into bottom-right coordinates (x, y, x+1, y+1) [+1 for last 2 because shenanigans]

    public String[] collisionKeys; // Corresponds to each collision coordinate set, used to program commands within Unity scripts as this is the key that is returned.

    [HideInInspector] public float sstsr; // Screen size to texture size ratio, can be at most 1

    /*[HideInInspector]*/ public float down; // How far down the texture the monitor is displaying, from the top, inputted in pixels

    [HideInInspector] public GameObject screenElement; // Assigned once the scene runs

    [SerializeField] bool visibleOnStart; // Should this ScreenElement instace be visible on start? SHOULD IDEALLY REPLACE THIS BY A LAYER MECHANIC (OR RATHER JUST COMPLEMENTED BY)

    public int screenMaterialTextureHeight, startingHeight; // These values are inputted in pixels (not Unity units); the origin is at the top-left of any ScreenElement.

    [SerializeField] Material screenMaterial; // Material with the corresponding texture to display

    public Vector2 screenSize; // The dimensions of the ScreenElement instance, inputted in Unity units (not pixels)

    private Mesh m;

    private MeshRenderer mr;

    private readonly int[] triangles = new int[] { 0, 1, 3, 3, 1, 2 };

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
        screenElement.transform.localPosition = offset;
        m = screenElement.AddComponent<MeshFilter>().mesh;
        mr = screenElement.AddComponent<MeshRenderer>();

        m.vertices = new Vector3[] // Vertices are relative to the position of the GameObject in the scene
        {
                    Vector3.zero, // Bottom left
                    new Vector3(0, screenSize.y, 0), // Top left
                    new Vector3(screenSize.x, screenSize.y, 0), // Top right
                    new Vector3(screenSize.x, 0, 0) // Bottom right
        };

        m.triangles = triangles;
        mr.material = screenMaterial;
        screenElement.AddComponent<MeshCollider>();
        screenElement.layer = 8; // Adds the "Screen Element" layer for raycasting
    }
}