using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveAirborne : PlayerAirborne
{
    public PlayerMoveAirborne(PlayerMachine playerMachine) : base(playerMachine)
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
            machine.Velocity = new Vector3(0, machine.Velocity.y, 0);
            machine.ChangeState(PlayerState.AIRBORNE);
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
        Vector3 forwardMovement = machine.player.Forward * (input.y * machine.Speed);
        Vector3 rightMovement = machine.player.Right * (input.x * machine.Speed);
        Vector3 playerMovement = forwardMovement + rightMovement;
        playerMovement.y = machine.Velocity.y; 
        machine.Velocity = playerMovement;
    }

    public override void Update()
    {
        base.Update();
    }
}
