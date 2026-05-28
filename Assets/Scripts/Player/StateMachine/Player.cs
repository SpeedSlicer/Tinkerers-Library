using System;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    CharacterController controller;
    [SerializeField] private Transform headRotation;
    [field: SerializeField] public float MouseSensitivity { get; private set; } = 0.5f;
    [field: SerializeField] public Vector3 Forward { get; private set; } = Vector3.forward;
    [field: SerializeField] public Vector3 Right { get; private set; } = Vector3.right;
    void Update()
    {
        Forward = this.transform.forward;
        Right = this.transform.right;
    }
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        controller = GetComponent<CharacterController>();
    }
    public void Move(Vector3 mV)
    {
        controller.Move(mV);
    }  
    public void Rotate(Vector3 rotate)
    {
        transform.Rotate(rotate);
    }
    public void AngleHead(float degrees)
    {
        float currentX = headRotation.localEulerAngles.x;
        if (currentX > 180f) currentX -= 360f;
        float newX = Mathf.Clamp(currentX + degrees, -90f, 90f);
        headRotation.localEulerAngles = new Vector3(newX, headRotation.localEulerAngles.y, headRotation.localEulerAngles.z);
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
