using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class OptimizedPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float upperLookLimit = 80f;
    [SerializeField] private float lowerLookLimit = 80f;

    [Header("Zoom Settings")]
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float zoomFOV = 40f;
    [SerializeField] private float zoomSpeed = 10f;

    [Header("FPS Display")]
    [SerializeField] private float fpsUpdateInterval = 0.5f;

    private Camera playerCamera;
    private CharacterController controller;
    private Vector3 moveDirection;
    private Vector3 velocity;
    private float rotationX = 0f;
    private bool isGrounded;
    private bool isSprinting = false;
    private bool isZooming = false;
    private float currentFOV;

    // FPS calculation
    private int frameCount;
    private float timeElapsed;
    private float currentFps;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        if (playerCamera == null)
        {
            Debug.LogError("Player camera not found!");
            enabled = false;
            return;
        }

        currentFOV = normalFOV;
        playerCamera.fieldOfView = normalFOV;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMovementInput();
        HandleMouseLook();
        HandleZoom();
        ApplyMovement();
        UpdateFPSDisplay();
    }

    private void HandleMovementInput()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        isSprinting = Input.GetKey(KeyCode.LeftShift);

        Vector3 move = transform.right * moveHorizontal + transform.forward * moveVertical;

        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        moveDirection = move * currentSpeed;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);

        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleZoom()
    {
        if (Input.GetMouseButtonDown(1)) // Changed to right mouse button (1)
        {
            isZooming = true;
        }
        else if (Input.GetMouseButtonUp(1)) // Changed to right mouse button (1)
        {
            isZooming = false;
        }

        float targetFOV = isZooming ? zoomFOV : normalFOV;
        currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * zoomSpeed);
        playerCamera.fieldOfView = currentFOV;
    }

    private void ApplyMovement()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        controller.Move(moveDirection * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void UpdateFPSDisplay()
    {
        frameCount++;
        timeElapsed += Time.unscaledDeltaTime;

        if (timeElapsed > fpsUpdateInterval)
        {
            currentFps = frameCount / timeElapsed;
            frameCount = 0;
            timeElapsed = 0;
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), $"FPS: {currentFps:F2}");
    }
}