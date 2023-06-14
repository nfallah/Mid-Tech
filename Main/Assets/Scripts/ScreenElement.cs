using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenElement : MonoBehaviour
{
    private enum Side { LEFT, RIGHT, DOWN, UP, BACK, FORWARD }

    [SerializeField] float physicalHeight;

    [SerializeField] int startingHeight; // Calculated from the top of the texture, also measured natively (e.g. pixels over Unity units)

    [SerializeField] Side side;

    [SerializeField] string screenName;

    [SerializeField] Texture2D texture;

    [SerializeField] Vector3 offset;

    private float currentHeightRatio, heightRatio;

    private int currentHeight, maxClampHeight, maxHeight;

    private Mesh mesh;

    private Vector2 dimensions;

    private readonly int[] triangles = new int[] { 0, 1, 3, 3, 1, 2 };

    private void Start()
    {
        /*
         * Ensures the physical height does not convert into a decimal pixel height.
         * E.g. "1.681" is fine as its pixel height is "1681", but no additional decimals would be allowed past this point.
         */
        if (physicalHeight * 1000 % 1 >= 0.0001)
        {
            throw new Exception("Physical height cannot have decimals; terminating");
        }

        // If the physical screen has a larger display than the entire texture, the script terminates.
        if (texture.height < (int)(physicalHeight * 1000))
        {
            Debug.Log(texture.height);
            Debug.Log(physicalHeight * 1000);
            throw new Exception("Texture pixel height smaller than the physical height of the screen; terminating.");
        }

        maxClampHeight = texture.height - (int)(physicalHeight * 1000);

        // Ensures the starting pixel height is not invalid, i.e. the display does not break.
        if (startingHeight > maxClampHeight)
        {
            throw new Exception("Starting pixel height is larger than the maximum pixel height; terminating.");
        }

        heightRatio = physicalHeight / (texture.height / 1000f);
        maxHeight = texture.height;
        dimensions = new Vector2(texture.width / 1000f, physicalHeight);
        CreateScreen();
    }

    private void CreateScreen()
    {
        GameObject screen = new GameObject(screenName);

        screen.transform.parent = transform;
        screen.transform.localPosition = offset;
        mesh = new Mesh();

        MeshFilter meshFilter = screen.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = screen.AddComponent<MeshRenderer>();
        Material material = new Material(Shader.Find("Standard"));

        material.SetFloat("_Glossiness", 0);
        material.SetTexture("_MainTex", texture);
        meshRenderer.material = material;
        mesh.vertices = GetVertices();
        mesh.normals = GetNormals();
        mesh.triangles = triangles;
        currentHeight = startingHeight;
        mesh.uv = GetUV();
        mesh.RecalculateBounds();
        meshFilter.mesh = mesh;
    }

    public void Scroll(int amount)
    {
        currentHeight = Mathf.Clamp(currentHeight + amount, 0, maxClampHeight);
        mesh.uv = GetUV();
    }

    private Vector2[] GetUV()
    {
        currentHeightRatio = (float)currentHeight / maxHeight;

        return new Vector2[]
        {
            new Vector2(0, 1 - currentHeightRatio - heightRatio),
            new Vector2(0, 1 - currentHeightRatio),
            new Vector2(1, 1 - currentHeightRatio),
            new Vector2(1, 1 - currentHeightRatio - heightRatio)
        };
    }

    private Vector3[] GetNormals()
    {
        Vector3 direction;

        switch (side)
        {
            case Side.LEFT:
                direction = Vector3.left;
                break;
            case Side.RIGHT:
                direction = Vector3.right;
                break;
            case Side.DOWN:
                direction = Vector3.down;
                break;
            case Side.UP:
                direction = Vector3.up;
                break;
            case Side.BACK:
                direction = Vector3.back;
                break;
            case Side.FORWARD:
                direction = Vector3.forward;
                break;
            default:
                throw new Exception("GetNormals() call failed -- invalid side specified.");
        }

        return new Vector3[] { direction, direction, direction, direction };
    }

    private Vector3[] GetVertices()
    {
        switch (side)
        {
            case Side.LEFT:
                return new Vector3[]
                {
                    new Vector3(0, 0, 0),
                    new Vector3(0, dimensions.y, 0),
                    new Vector3(0, dimensions.y, -dimensions.x),
                    new Vector3(0, 0, -dimensions.x)
                };
            case Side.RIGHT:
                return new Vector3[]
                {
                    new Vector3(0, 0, 0),
                    new Vector3(0, dimensions.y, 0),
                    new Vector3(0, dimensions.y, dimensions.x),
                    new Vector3(0, 0, dimensions.x)
                };
            case Side.DOWN:
                return new Vector3[]
                {
                    new Vector3(0, 0, 0),
                    new Vector3(0, 0, dimensions.y),
                    new Vector3(-dimensions.x, 0, dimensions.y),
                    new Vector3(-dimensions.x, 0, 0)
                };
            case Side.UP:
                return new Vector3[]
                {
                    new Vector3(0, 0, 0),
                    new Vector3(0, 0, dimensions.y),
                    new Vector3(dimensions.x, 0, dimensions.y),
                    new Vector3(dimensions.x, 0, 0)
                };
            case Side.BACK:
                return new Vector3[]
                {
                    new Vector3(0, 0, 0),
                    new Vector3(0, dimensions.y, 0),
                    new Vector3(-dimensions.x, dimensions.y, 0),
                    new Vector3(-dimensions.x, 0, 0)
                };
            case Side.FORWARD:
                return new Vector3[]
                {
                    new Vector3(0, 0, 0),
                    new Vector3(0, dimensions.y, 0),
                    new Vector3(dimensions.x, dimensions.y, 0),
                    new Vector3(dimensions.x, 0, 0)
                };
            default:
                throw new Exception("GetVertices() call failed -- invalid side specified.");
        }
    }
}