using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenElement : MonoBehaviour
{
    private enum Side { LEFT, RIGHT, DOWN, UP, BACK, FORWARD }

    [SerializeField] float height;

    [SerializeField] Side side;

    [SerializeField] string screenName;

    [SerializeField] Texture2D texture;

    [SerializeField] Vector2 offset;

    private Vector2 dimensions;

    private readonly int[] triangles = new int[] { 0, 1, 3, 3, 1, 2 };

    private void Start()
    {
        dimensions = new Vector2(texture.width / 1000f, height);
        CreateScreen();
    }

    private void CreateScreen()
    {
        GameObject screen = new GameObject(screenName);

        screen.transform.parent = transform;
        screen.transform.localPosition = Vector3.zero;

        MeshFilter meshFilter = screen.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        MeshRenderer meshRenderer = screen.AddComponent<MeshRenderer>();
        Material material = new Material(Shader.Find("Standard"));

        material.SetFloat("_Glossiness", 0);
        material.SetTexture("_MainTex", texture);
        meshRenderer.material = material;

        mesh.vertices = GetVertices();
        mesh.normals = GetNormals();
        mesh.triangles = triangles;
        mesh.uv = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0)
        };

        mesh.RecalculateBounds();
        meshFilter.mesh = mesh;
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

                };
            case Side.RIGHT:
                return new Vector3[]
                {

                };
            case Side.DOWN:
                return new Vector3[]
                {

                };
            case Side.UP:
                return new Vector3[]
                {

                };
            case Side.BACK:
                return new Vector3[]
                {
                    new Vector3(dimensions.x, 0, 0),
                    new Vector3(dimensions.x, dimensions.y, 0),
                    new Vector3(0, dimensions.y , 0),
                    new Vector3(0, 0, 0)
                };
            case Side.FORWARD:
                return new Vector3[]
                {
                    new Vector3(0, 0, 0),
                    new Vector3(0, dimensions.y , 0),
                    new Vector3(dimensions.x, dimensions.y, 0),
                    new Vector3(dimensions.x, 0, 0)
                };
            default:
                throw new Exception("GetVertices() call failed -- invalid side specified.");
        }
    }
}