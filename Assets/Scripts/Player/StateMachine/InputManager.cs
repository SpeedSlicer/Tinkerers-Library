using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : NetworkBehaviour
{
    [SerializeField] private InputActionReference jumpActionBinding;
    [SerializeField] private InputActionReference moveActionBinding;
    [SerializeField] private InputActionReference mouseActionBinding;

    public Action<InputAction.CallbackContext> jumpAction, movePreformedAction, moveCanceledAction, mousePreformedAction, mouseCanceledAction;
    public Action<Vector2> moveActive, moveStopped, mouseActive, mouseStopped;

    private Vector2 moveInput;
    private Vector2 mouseInput;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        jumpActionBinding.action.Enable();
        moveActionBinding.action.Enable();

        jumpActionBinding.action.performed += OnJumpPerformed;
        moveActionBinding.action.performed += OnMovePreformed;
        moveActionBinding.action.canceled += OnMoveCanceled;
        mouseActionBinding.action.performed += OnMousePreformed;
        mouseActionBinding.action.canceled += OnMouseCanceled;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        jumpAction?.Invoke(context);
    }
    private void OnMovePreformed(InputAction.CallbackContext context)
    {
        movePreformedAction?.Invoke(context);
    }
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveCanceledAction?.Invoke(context);
    }
    private void OnMousePreformed(InputAction.CallbackContext context)
    {
        mousePreformedAction?.Invoke(context);
    }
    private void OnMouseCanceled(InputAction.CallbackContext context)
    {
        mouseCanceledAction?.Invoke(context);
    }
    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;


        jumpActionBinding.action.performed -= OnJumpPerformed;
        moveActionBinding.action.performed -= OnMovePreformed;
        moveActionBinding.action.canceled -= OnMoveCanceled;
        mouseActionBinding.action.performed -= OnMousePreformed;
        mouseActionBinding.action.canceled -= OnMouseCanceled;        
        jumpActionBinding.action.Disable();
        moveActionBinding.action.Disable();
    }

    private void Update()
    {
        if (!IsOwner) return;

        moveInput = moveActionBinding.action.ReadValue<Vector2>();
        mouseInput = mouseActionBinding.action.ReadValue<Vector2>();

        if (Math.Abs(moveInput.magnitude) > 0)
        {
            moveActive?.Invoke(moveInput);
        }
        else
        {
            moveStopped?.Invoke(moveInput);
        }
        if (Math.Abs(mouseInput.magnitude) > 0)
        {
            mouseActive?.Invoke(mouseInput);
        }
        else
        {
            mouseStopped?.Invoke(mouseInput);
        }
    }

    public Vector2 GetMovementInput()
    {
        return moveInput;
    }
}