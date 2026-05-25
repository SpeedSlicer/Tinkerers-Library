using System;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    CharacterController controller;
    [SerializeField] private Transform headRotation;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        controller = GetComponent<CharacterController>();
    }
    public void Move(Vector3 mV)
    {
        controller.Move(mV);
    }  
    
    public void AngleHead(float degrees)
    {
        headRotation.Rotate(0, Math.Clamp(degrees, -90, 90), 0);
    }
    public bool IsGrounded()
    {
        return controller.isGrounded;
    }
    public Vector3 GetVelocity()
    {
        return controller.velocity;
    }
}
