using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAirborne : PlayerFunctionalState
{
    public PlayerAirborne(PlayerMachine playerMachine) : base(playerMachine) { }

    public override void Enter()
    {
        if (Math.Abs(machine.InputManager.GetMovementInput().magnitude) > 0)
        {
            machine.ChangeState(PlayerState.MOVING_AIRBORNE);
            return;
        }
        machine.Velocity.x = 0;
        machine.Velocity.z = 0;
        machine.InputManager.movePreformedAction += HandleMove;
    }
    public override void Exit()
    {
        machine.InputManager.movePreformedAction -= HandleMove;
    }
    public override void Update()
    {
        if (machine.player.IsGrounded() && machine.player.GetVelocity().y <= 0f)
        {
            machine.ChangeState(PlayerState.GROUNDED);
        }
    }
    public override void UpdatePhysics()
    {
        ApplyAirborneGravity();
    }

    protected void HandleMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            machine.ChangeState(PlayerState.MOVING_AIRBORNE);
        }
    }

}
