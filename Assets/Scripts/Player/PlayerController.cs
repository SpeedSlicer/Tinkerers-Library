using System;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Camera playerCam;
    [Header("Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float lookSensitivity = 0.1f;

    private Vector3 velocity;
    [SerializeField] private bool isGrounded;
    private float verticalRotation = 0f;

    [Header("Input Actions")]
    [SerializeField] InputActionReference moveAction;
    [SerializeField] InputActionReference jumpAction;
    [SerializeField] InputActionReference lookAction;
    private bool wasGrounded;
    Animator anim;
    void OnEnable()
    {
        if (!IsOwner) return;
        moveAction.action.Enable();
        jumpAction.action.Enable();
        lookAction.action.Enable();
    }

    void OnDisable()
    {
        if (!IsOwner) return;
        moveAction.action.Disable();
        jumpAction.action.Disable();
        lookAction.action.Disable();
    }
    void Start()
    {
        anim = GetComponent<Animator>();
        if (IsOwner)
        {
            playerCam.GetComponent<AudioListener>().enabled = true;
            playerCam.GetComponent<Camera>().enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            playerCam.GetComponent<AudioListener>().enabled = false;
            playerCam.GetComponent<Camera>().enabled = false;
        }
    }
    void Update()
    {
        if (!IsOwner) return;
        HandleRotation();
        HandleMovement();
    }

    void HandleRotation()
    {
        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

        transform.Rotate(Vector3.up * lookInput.x * lookSensitivity);

        verticalRotation += lookInput.y * lookSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    void HandleMovement()
    {
        wasGrounded = isGrounded;
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        controller.Move(move * speed * Time.deltaTime);

        if (jumpAction.action.WasPressedThisFrame() && isGrounded)
        {
            anim.SetBool("isJumping", true);
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        anim.SetBool("isRunning", math.abs(moveInput.magnitude) > 0);
        if (!wasGrounded && isGrounded)
        {
            anim.SetBool("isJumping", false);
        }
    }
    public Camera GetMainCamera()
    {
        return playerCam;
    }
}
