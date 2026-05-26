using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateTop : PlayerFunctionalState
{
    public PlayerStateTop(PlayerMachine playerMachine) : base(playerMachine) { }

    public override void Enter()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }   
    public override void Exit()
    {
    }
    public override void Update()
    {
    }
    public override void UpdatePhysics()
    {
        if(Math.Abs(machine.InputManager.GetMouseInput().magnitude) > 0)
        {
            machine.player.Rotate(new Vector3(0, machine.InputManager.GetMouseInput().x * machine.player.MouseSensitivity, 0));
            machine.player.AngleHead(machine.InputManager.GetMouseInput().y * machine.player.MouseSensitivity);
        }
    }


}
