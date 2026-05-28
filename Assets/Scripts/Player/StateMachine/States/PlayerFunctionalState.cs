using UnityEngine;

public abstract class PlayerFunctionalState
{
    protected PlayerMachine machine;
    public PlayerFunctionalState(PlayerMachine playerMachine)
    {
        this.machine = playerMachine;
    }

    public virtual void Enter() { }
    public virtual void HandleInput() { }
    public virtual void Update() { }
    public virtual void UpdatePhysics() { }
    public virtual void Exit() { }

    public Vector3 GetVelocity() => machine.Velocity;

    protected void ApplyAirborneGravity()
    {
        machine.Velocity.y += machine.Gravity * Time.deltaTime;

        if (machine.Velocity.y < -machine.TerminalVelocity)
        {
            machine.Velocity.y = -machine.TerminalVelocity;
      }
    }

    protected void SnapToGround()
    {
        machine.Velocity.y = machine.StaticHold;
    }
}
