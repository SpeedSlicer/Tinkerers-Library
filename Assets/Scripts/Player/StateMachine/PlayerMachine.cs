using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum PlayerState
{
    GROUNDED,
    AIRBORNE,
    MOVING,
    MOVING_AIRBORNE
}

[RequireComponent(typeof(CharacterController))]
public class PlayerMachine : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject mainCamera; 
    public InputManager InputManager { get; private set; }
    public Player player { get; private set; }
    public GameObject MainCamera => mainCamera;

    [field: SerializeField] public float Speed { get; private set; } = 10f;
    [field: SerializeField] public float Gravity { get; private set; } = -20f;
    [field: SerializeField] public float JumpHeight { get; private set; } = 5f;
    [field: SerializeField] public float TerminalVelocity { get; private set; } = 50f;
    [field: SerializeField] public float StaticHold { get; private set; } = -2f;


    [SerializeField] private NetworkVariable<PlayerState> currentState = new NetworkVariable<PlayerState>(PlayerState.GROUNDED);
    private Dictionary<PlayerState, PlayerFunctionalState> playerLookup = new Dictionary<PlayerState, PlayerFunctionalState>();
    private PlayerFunctionalState activeState;

    public Vector3 Velocity;
    private void Awake()
    {
        player = GetComponent<Player>();

        playerLookup.Add(PlayerState.GROUNDED, new PlayerGrounded(this));
        playerLookup.Add(PlayerState.AIRBORNE, new PlayerAirborne(this));
        playerLookup.Add(PlayerState.MOVING, new PlayerMoveGrounded(this));
        playerLookup.Add(PlayerState.MOVING_AIRBORNE, new PlayerMoveAirborne(this));
    }

    public override void OnNetworkSpawn()
    {
        currentState.OnValueChanged += OnStateNetworkValueChanged;

        if (!IsOwner) return;

        InputManager = GetComponent<InputManager>();

        SetLocalState(currentState.Value);
    }

    public override void OnNetworkDespawn()
    {
        currentState.OnValueChanged -= OnStateNetworkValueChanged;
    }

    void Update()
    {
        if (!IsOwner) return;

        activeState?.HandleInput();
        activeState?.Update();
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;
        activeState?.UpdatePhysics();
        player.Move(Velocity * Time.fixedDeltaTime);
    }

    public void ChangeState(PlayerState newState)
    {
        if (!IsOwner) return;

        SetLocalState(newState);

        currentState.Value = newState;
    }

    private void SetLocalState(PlayerState targetState)
    {
        if (playerLookup.TryGetValue(targetState, out PlayerFunctionalState nextState))
        {
            if (activeState == nextState) return;

            activeState?.Exit();
            activeState = nextState;
            activeState.Enter();
        }
    }

    private void OnStateNetworkValueChanged(PlayerState previousValue, PlayerState newValue)
    {
        if (IsOwner) return;

    }

}
