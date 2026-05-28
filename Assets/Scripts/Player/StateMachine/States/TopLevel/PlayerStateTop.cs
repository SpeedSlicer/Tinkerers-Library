using UnityEngine;

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
        base.Update();
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
    }
}
