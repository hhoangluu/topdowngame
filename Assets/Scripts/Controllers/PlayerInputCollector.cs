using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.InputSystem;

public struct NetworkInputData : INetworkInput
{
    public Vector3 MovementDirection;
    public bool IsSprinting;
    public bool IsJumping;
    public float RotationY;
    public float RotationX; 
}

// Player input collector - this handles getting inputs from the local player
// and properly sending them through the Fusion network
public class PlayerInputCollector : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner _runner;
    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _sprintAction;
    private InputAction _lookAction;
    private Camera _mainCamera;
    
    // Used to collect inputs each frame
    private NetworkInputData _inputData;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _moveAction = _playerInput.actions["Move"];
        _jumpAction = _playerInput.actions["Jump"];
        _sprintAction = _playerInput.actions["Sprint"];
        _lookAction = _playerInput.actions["Look"];
        _mainCamera = Camera.main;
    }

    public void SetRunner(NetworkRunner runner)
    {
        _runner = runner;
    }

    private void Update()
    {
        if (_runner == null || !_playerInput.enabled) return;

        // Get movement input
        Vector2 moveInput = _moveAction.ReadValue<Vector2>();
        Vector3 movementDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        // Get jumping input (true on the frame the button was pressed)
        bool jumpPressed = _jumpAction.IsPressed();

        // Get sprint input (true while button is held)
        bool sprintHeld = _sprintAction.IsPressed();

        // Get look input for rotation
        Vector2 lookInput = _lookAction.ReadValue<Vector2>();
        float rotationY = 0;
        float rotationX = 0; 
        if (_mainCamera != null)
        {
            // Calculate rotation based on mouse position or right stick
            rotationY = _mainCamera.transform.eulerAngles.y + lookInput.x;
            rotationX = _mainCamera.transform.eulerAngles.x - lookInput.y;
        }

        // Populate the input data
        _inputData = new NetworkInputData
        {
            MovementDirection = movementDirection,
            IsJumping = jumpPressed,
            IsSprinting = sprintHeld,
            RotationY = rotationY,
            RotationX = rotationX
        };
    }
    
    // Called by Fusion to collect input
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        input.Set(_inputData);
    }
    
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }
}