using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveGrounded : PlayerGrounded
{
    public PlayerMoveGrounded(PlayerMachine playerMachine) : base(playerMachine)
    {
    }
    public override void Enter()
    {
        base.Enter();
        machine.InputManager.moveCanceledAction += HandleStopMoving;
    }

    private void HandleStopMoving(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            machine.Velocity = new Vector3(0, -2f, 0);
            machine.ChangeState(PlayerState.GROUNDED);
        }
    }

    public override void Exit()
    {
        base.Exit();
        machine.InputManager.moveCanceledAction -= HandleStopMoving;
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        Vector2 input = machine.InputManager.GetMovementInput();
        Vector3 playerMovement = new Vector3(input.x * machine.Speed, machine.Velocity.y, input.y * machine.Speed);
        machine.Velocity = playerMovement;
    }

    public override void SpecificSetup()
    {
        
    }
    public override void Update()
    {
        base.Update();
    }
}
