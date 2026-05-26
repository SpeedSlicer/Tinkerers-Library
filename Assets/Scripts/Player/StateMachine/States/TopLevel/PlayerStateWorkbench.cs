using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateWorkbech : PlayerFunctionalState
{
    public PlayerStateWorkbech(PlayerMachine playerMachine) : base(playerMachine) { }

    public override void Enter()
    {
        if (Cursor.lockState != CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
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
    }


}
