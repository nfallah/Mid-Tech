using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Camera characterCamera;

    [SerializeField] CharacterController characterController;

    [SerializeField] float lookSensitivity, moveSensitivity;

    private Vector3 cameraLook, characterLook;

    private void Awake()
    {
        Global.player = this;        
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cameraLook = characterCamera.transform.localEulerAngles;
        characterLook = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        // Looking
        float xLook = -Input.GetAxis("Mouse Y") * lookSensitivity * Time.deltaTime;
        float yLook = Input.GetAxis("Mouse X") * lookSensitivity * Time.deltaTime;

        cameraLook.x = Mathf.Clamp(cameraLook.x + xLook, -89.9f, 89.9f);
        characterLook.y += yLook % 360;
        characterCamera.transform.localEulerAngles = cameraLook;
        transform.eulerAngles = characterLook;

        // Moving
        float xMove = Input.GetAxisRaw("Horizontal") * moveSensitivity * Time.deltaTime;
        float zMove = Input.GetAxisRaw("Vertical") * moveSensitivity * Time.deltaTime;
        characterController.Move(xMove * transform.right + zMove * transform.forward);
    }
}
