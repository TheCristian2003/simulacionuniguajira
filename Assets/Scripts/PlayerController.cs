using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 4f;
    public float runSpeed = 8f;
    public float gravity = -20f;
    public float jumpHeight = 1.5f;
public float rotationSpeed = 120f;
    [Header("Modelo visual")]
    public Transform visualModel;
    public Vector3 visualLocalPosition = Vector3.zero;

    [Header("Cámara")]
    public Transform cameraTransform;
    public float mouseSensitivity = 0.1f;

    [Header("Animación")]
    public Animator playerAnimator;
    
    private bool isOnStairs = false;

    private CharacterController controller;
    private float verticalVelocity;
    private float cameraXRotation = 15f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null)
            cameraTransform = GetComponentInChildren<Camera>()?.transform;

        if (playerAnimator == null)
            playerAnimator = GetComponentInChildren<Animator>();

        if (controller != null)
        {
            controller.stepOffset = 0.4f;
            controller.slopeLimit = 45f;
        }

        Cursor.lockState = CursorLockMode.Locked;

        if (visualModel == null && playerAnimator != null)
        {
            visualModel = playerAnimator.transform;
        }

        if (visualModel != null)
        {
            visualLocalPosition = visualModel.localPosition;
        }
    }

    void Update()
    {
        RotateCamera();
        MovePlayer();

        if (visualModel != null)
        {
            visualModel.localPosition = visualLocalPosition;
        }
    }

    void RotateCamera()
    {
        if (Mouse.current == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseDelta.x);

        if (cameraTransform != null)
        {
            cameraXRotation -= mouseDelta.y;
            cameraXRotation = Mathf.Clamp(cameraXRotation, -10f, 60f);
            cameraTransform.localRotation = Quaternion.Euler(cameraXRotation, 0f, 0f);
        }
    }

    void MovePlayer()
{
    Vector2 input = Vector2.zero;

    if (Keyboard.current != null)
    {
        if (Keyboard.current.wKey.isPressed) input.y = 1;
        if (Keyboard.current.sKey.isPressed) input.y = -1;
        if (Keyboard.current.aKey.isPressed) input.x = -1;
        if (Keyboard.current.dKey.isPressed) input.x = 1;
    }

    // A y D giran el personaje, W y S lo mueven adelante/atrás
    float turnAmount = input.x * rotationSpeed * Time.deltaTime;
    transform.Rotate(Vector3.up * turnAmount);

    Vector3 move = transform.forward * input.y;

    if (controller.isGrounded && verticalVelocity < 0)
        verticalVelocity = -2f;

    if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && controller.isGrounded)
    {
        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    verticalVelocity += gravity * Time.deltaTime;

    bool isRunning = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;
    float currentSpeed = isRunning ? runSpeed : walkSpeed;

    Vector3 finalMove = move * currentSpeed;
    finalMove.y = verticalVelocity;

    controller.Move(finalMove * Time.deltaTime);

    if (playerAnimator != null)
    {
        playerAnimator.SetFloat("Speed", move.magnitude * currentSpeed);
        playerAnimator.SetBool("isWalking", input.magnitude > 0.1f);
        playerAnimator.SetBool("isRunning", isRunning && input.magnitude > 0.1f);
    }
}

    public void SetOnStairs(bool value, bool ascending = true)
    {
        
        isOnStairs = value;

        if(playerAnimator != null)
        {
            playerAnimator.SetBool("isOnStairs", value);
        }
    }
    
}