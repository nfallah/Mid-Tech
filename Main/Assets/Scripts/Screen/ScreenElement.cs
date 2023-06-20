using System;
using UnityEngine;

public class ScreenElement : MonoBehaviour
{
    [SerializeField] Screen screen;

    [SerializeField] bool locked;

    [SerializeField] Color emissionColor;

    [SerializeField] float physicalHeight;

    [SerializeField] int startingHeight; // Calculated from the top of the texture, also measured natively (e.g. pixels over Unity units)

    [SerializeField] ScreenButton[] screenButtons;

    [SerializeField] string screenName;

    [SerializeField] Texture2D texture;

    [SerializeField] ScreenEvent screenEvent; 

    [SerializeField] Vector2 offset;

    private float currentHeight, currentHeightRatio, heightRatio;

    private GameObject screenObject;

    private int maxClampHeight, maxHeight;

    private Mesh mesh;

    private Vector2 dimensions;

    private readonly int[] triangles = new int[] { 0, 1, 3, 3, 1, 2 };

    private void Awake()
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

        // Ensures both the screen element its parent screen manager are attached to the same GameObject, preventing any potential bugs.
        if (!transform.Equals(screen.transform))
        {
            throw new Exception("Screen element must be attached to the same GameObject as the screen; terminating.");
        }

        heightRatio = physicalHeight / (texture.height / 1000f);
        maxHeight = texture.height;
        dimensions = new Vector2(texture.width / 1000f, physicalHeight);
        CreateScreen();
    }

    private void CreateScreen()
    {
        screenObject = new GameObject(screenName);

        screenObject.transform.parent = transform;
        screenObject.transform.localPosition = screen.LocalToGlobalPosition(offset);
        mesh = new Mesh();

        MeshFilter meshFilter = screenObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = screenObject.AddComponent<MeshRenderer>();
        Material material = new Material(Shader.Find("Standard"));

        material.SetFloat("_Glossiness", 0);
        material.SetTexture("_MainTex", texture);
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", emissionColor);
        meshRenderer.material = material;
        mesh.vertices = Vertices;
        mesh.normals = Normals;
        mesh.triangles = triangles;
        currentHeight = startingHeight;
        mesh.uv = UV;
        mesh.RecalculateBounds();
        meshFilter.mesh = mesh;
        screenObject.AddComponent<MeshCollider>();
        screenObject.layer = 9;
        
        ScreenElementReference screenElementReference = screenObject.AddComponent<ScreenElementReference>();

        screenElementReference.ScreenElement = this;
    }

    public void Scroll(float amount)
    {
        if (locked) return;
        currentHeight = Mathf.Clamp(currentHeight + amount, 0, maxClampHeight);
        mesh.uv = UV;
    }

    private Vector2[] UV
    {
        get
        {
            currentHeightRatio = currentHeight / maxHeight;

            return new Vector2[]
            {
            new Vector2(0, 1 - currentHeightRatio - heightRatio),
            new Vector2(0, 1 - currentHeightRatio),
            new Vector2(1, 1 - currentHeightRatio),
            new Vector2(1, 1 - currentHeightRatio - heightRatio)
            };
        }
    }

    private Vector3[] Normals
    {
        get
        {
            Vector3 direction;

            switch (screen.ScreenSide)
            {
                case Screen.Side.LEFT:
                    direction = Vector3.left;
                    break;
                case Screen.Side.RIGHT:
                    direction = Vector3.right;
                    break;
                case Screen.Side.DOWN:
                    direction = Vector3.down;
                    break;
                case Screen.Side.UP:
                    direction = Vector3.up;
                    break;
                case Screen.Side.BACK:
                    direction = Vector3.back;
                    break;
                case Screen.Side.FORWARD:
                    direction = Vector3.forward;
                    break;
                default:
                    throw new Exception("GetNormals call failed -- invalid side specified.");
            }

            return new Vector3[] { direction, direction, direction, direction };
        }
    }

    private Vector3[] Vertices
    {
        get
        {
            switch (screen.ScreenSide)
            {
                case Screen.Side.LEFT:
                    return new Vector3[]
                    {
                    new Vector3(0, 0, 0),
                    new Vector3(0, dimensions.y, 0),
                    new Vector3(0, dimensions.y, -dimensions.x),
                    new Vector3(0, 0, -dimensions.x)
                    };
                case Screen.Side.RIGHT:
                    return new Vector3[]
                    {
                    new Vector3(0, 0, 0),
                    new Vector3(0, dimensions.y, 0),
                    new Vector3(0, dimensions.y, dimensions.x),
                    new Vector3(0, 0, dimensions.x)
                    };
                case Screen.Side.DOWN:
                    return new Vector3[]
                    {
                    new Vector3(0, 0, 0),
                    new Vector3(0, 0, dimensions.y),
                    new Vector3(-dimensions.x, 0, dimensions.y),
                    new Vector3(-dimensions.x, 0, 0)
                    };
                case Screen.Side.UP:
                    return new Vector3[]
                    {
                    new Vector3(0, 0, 0),
                    new Vector3(0, 0, dimensions.y),
                    new Vector3(dimensions.x, 0, dimensions.y),
                    new Vector3(dimensions.x, 0, 0)
                    };
                case Screen.Side.BACK:
                    return new Vector3[]
                    {
                    new Vector3(0, 0, 0),
                    new Vector3(0, dimensions.y, 0),
                    new Vector3(-dimensions.x, dimensions.y, 0),
                    new Vector3(-dimensions.x, 0, 0)
                    };
                case Screen.Side.FORWARD:
                    return new Vector3[]
                    {
                    new Vector3(0, 0, 0),
                    new Vector3(0, dimensions.y, 0),
                    new Vector3(dimensions.x, dimensions.y, 0),
                    new Vector3(dimensions.x, 0, 0)
                    };
                default:
                    throw new Exception("GetVertices call failed -- invalid side specified.");
            }
        }
    }

    public float CurrentHeight { get { return currentHeight; } }

    public float PhysicalHeight { get { return physicalHeight; } }

    public GameObject ScreenObject {  get { return screenObject; } }

    public Screen Screen { get { return screen; } }

    public bool Locked { get { return locked; } set { locked = value; } }

    public ScreenButton[] ScreenButtons { get { return screenButtons; } }

    public ScreenEvent ScreenEvent { get { return screenEvent; } }
}