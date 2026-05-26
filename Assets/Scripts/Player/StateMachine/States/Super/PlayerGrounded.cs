using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGrounded : PlayerStateTop
{
    bool isJumping;
    public PlayerGrounded(PlayerMachine playerMachine) : base(playerMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        isJumping = false;
        SpecificSetup();
        machine.InputManager.jumpAction += HandleJump;
        machine.InputManager.movePreformedAction += HandleMove;
    }

    public virtual void SpecificSetup()
    {
        if (Math.Abs(machine.InputManager.GetMovementInput().magnitude) > 0)
        {
            machine.ChangeState(PlayerState.MOVING);
            return;
        }
        machine.Velocity.x = 0;
        machine.Velocity.z = 0;
    }
    protected void HandleJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Jump Triggered Successfully");
            isJumping = true;
        }
    }
    protected void HandleMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            machine.ChangeState(PlayerState.MOVING);
        }
    }

    public override void Exit()
    {
        base.Exit();
        machine.InputManager.jumpAction -= HandleJump;
        machine.InputManager.movePreformedAction -= HandleMove;
        isJumping = false;
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        if (machine.player.GetVelocity().y < 0f && machine.player.IsGrounded() && !isJumping)
        {
            SnapToGround();
        }
        else if (isJumping)
        {
            machine.Velocity.y = Mathf.Sqrt(-2f * machine.Gravity * machine.JumpHeight);
        }
    }

    public override void Update()
    {
        base.Update();
        if (!machine.player.IsGrounded())
        {
            machine.ChangeState(PlayerState.AIRBORNE);
        }
    }
}
