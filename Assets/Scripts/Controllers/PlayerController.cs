using UnityEngine;
using Fusion;
using TMPro;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour
{
    [Header("Movement Settings")] [SerializeField]
    private TextMeshProUGUI PlayerNameText;

    private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float movementSmoothTime = 0.1f;

    private Camera cameraController;

    [SerializeField] private Transform playerVisual;
    public Transform CameraPivot;
    public Transform CameraHandle;

    [Networked] private Vector3 NetworkedMovementDirection { get; set; }
    [Networked] private bool NetworkedIsSprinting { get; set; }
    [Networked] private bool NetworkedIsJumping { get; set; }
    [Networked] private float NetworkedRotationX { get; set; }
    [Networked] private float NetworkedRotationY { get; set; }

    [Networked] public NetworkString<_16> NetworkedPlayerName { get; set; }


    // Local variables
    private CharacterController _characterController;
    private Vector3 _playerVelocity;
    private bool _groundedPlayer;
    private Vector3 _currentMovementDirection;
    private Vector3 _movementDirectionVelocity;
    private PlayerInput _playerInput;
    private InputAction _lookAction;
    private Camera _mainCamera;

    // Local camera rotation 
    private float localPitch = 0f;
    private float localYaw = 0f;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
        _lookAction = _playerInput.actions["Look"];
        _mainCamera = Camera.main;
        if (_mainCamera != null)
            cameraController = _mainCamera;
    }
    

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _playerInput.enabled = true;
            NetworkedPlayerName = UIManager.Instance.PlayerName;
            _characterController.enabled = true;
        }
        else
        {
            Destroy(_playerInput);
        }

        PlayerNameText.text = NetworkedPlayerName.ToString();
    }


    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData input))
        {
            NetworkedMovementDirection = input.MovementDirection;
            NetworkedIsSprinting = input.IsSprinting;
            NetworkedIsJumping = input.IsJumping;
            NetworkedRotationY = input.RotationY;
            NetworkedRotationX = input.RotationX;
        }

        ApplyMovement();
        ApplyRotation();
    }

    private void ApplyMovement()
    {
        _groundedPlayer = _characterController.isGrounded;
        if (_groundedPlayer && _playerVelocity.y < 0)
        {
            _playerVelocity.y = -2f;
        }

        _currentMovementDirection = Vector3.SmoothDamp(
            _currentMovementDirection,
            NetworkedMovementDirection,
            ref _movementDirectionVelocity,
            movementSmoothTime);

        Vector3 move = _mainCamera != null
            ? GetCameraOrientedMovement(_currentMovementDirection)
            : _currentMovementDirection;

        float currentSpeed = NetworkedIsSprinting ? sprintSpeed : walkSpeed;
        Vector3 movement = move * currentSpeed * Runner.DeltaTime;
        _characterController.Move(movement);

        if (NetworkedIsJumping && _groundedPlayer)
        {
            _playerVelocity.y = jumpForce;
        }

        _playerVelocity.y += gravityValue * Runner.DeltaTime;
        _characterController.Move(_playerVelocity * Runner.DeltaTime);
    }

    private Vector3 GetCameraOrientedMovement(Vector3 direction)
    {
        if (_mainCamera == null) return direction;
        Vector3 cameraForward = _mainCamera.transform.forward;
        Vector3 cameraRight = _mainCamera.transform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();
        return cameraRight * direction.x + cameraForward * direction.z;
    }

    private void ApplyRotation()
    {
        if (HasStateAuthority && Time.timeScale > 0)
        {
            Quaternion targetPlayerRotation = Quaternion.Euler(0, NetworkedRotationY, 0);
            transform.rotation =
                Quaternion.Lerp(transform.rotation, targetPlayerRotation, rotationSpeed * Runner.DeltaTime);
        }
    }

    private void Update()
    {
        if (HasInputAuthority)
        {
            Vector2 lookInput = _lookAction.ReadValue<Vector2>();
            localYaw += lookInput.x * rotationSpeed * Time.deltaTime;
            localPitch -= lookInput.y * rotationSpeed * Time.deltaTime;
            localPitch = Mathf.Clamp(localPitch, -90f, 90f);
        }
    }

    private void LateUpdate()
    {
        // Chỉ cập nhật camera nếu đây là local player
        if (HasInputAuthority && _mainCamera != null)
        {
            CameraPivot.rotation = Quaternion.Euler(localPitch, localYaw, 0f);
            _mainCamera.transform.SetPositionAndRotation(CameraHandle.position, CameraHandle.rotation);
        }
    }
}