using UnityEngine;

[System.Serializable]
public class MovementSetup
{
    [field: SerializeField, Tooltip("The speed at which the character moves.")] public float MoveSpeed { get; private set; } = 10f;
    [field: SerializeField, Tooltip("The speed at which the character rotates.")] public float RotationSpeed { get; private set; } = 5f;

    [field: SerializeField, Tooltip("What is considered ground for the character.")] public LayerMask GroundLayer { get; private set; }
    [field: SerializeField, Tooltip("The radius of the sphere used for ground checking.")] public float GroundCheckRadius { get; private set; } = 0.5f;
    [field: SerializeField, Tooltip("The distance for the sphere cast used for ground checking.")] public float GroundCheckDistance { get; private set; } = 0.5f;

    [field: SerializeField, Tooltip("The speed multiplier when the character is crouching.")] public float CrouchSpeedMultiplier { get; private set; } = 0.5f;
    [field: SerializeField, Tooltip("The speed multiplier when the character is sprinting.")] public float SprintSpeedMultiplier { get; private set; } = 1.5f;
}

[System.Serializable]
public class JumpSetup
{
    [field: SerializeField, Tooltip("The height reached when jumping.")] public float JumpHeight { get; private set; } = 3f;
    [field: SerializeField, Tooltip("Whether the character can perform jumps in the air.")] public bool CanAirJump { get; private set; } = false;
    [field: SerializeField, Tooltip("The maximum number of jumps allowed in the air.")] public int MaxAirJumps { get; private set; } = 0;
}

[System.Serializable]
public class DashSetup
{
    [field: SerializeField, Tooltip("The speed multiplier when the character is dashing.")] public float DashSpeedMultiplier { get; private set; } = 2f;
    [field: SerializeField, Tooltip("The duration of the dash in seconds.")] public float DashDuration { get; private set; } = 0.5f;
    [field: SerializeField, Tooltip("The cooldown time between dashes in seconds.")] public float DashCooldown { get; private set; } = 1f;
    [field: SerializeField, Tooltip("What the character can go through when dashing.")] public LayerMask DashIgnoresWhat { get; private set; }
}

public class CharacterMovement : MonoBehaviour
{
    [Header("Setups")]
    public MovementSetup movementSetup;
    public JumpSetup jumpSetup;
    public DashSetup dashSetup;

    [Header("References")]
    [SerializeField] private GameObject TPCam;
    private CharacterController _controller;
    private Vector3 _movement;
    private float _gravity = -9.81f;
    private float _groundedGravity = -0.1f;
    private bool _jumpPressed;
    private bool _isGrounded;
    private bool _isCrouching;
    private bool _isSprinting;
    private bool _isDashing;
    private bool _isJumping;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!TryGetComponent(out _controller))
        {
            Debug.LogError("CharacterMovement requires a CharacterController component.");
        }
        else {  _controller = GetComponent<CharacterController>(); }
        Subscribe(true);
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfGrounded();
        HandleRotation();
        HandleMovement();
        _controller.Move(_movement * Time.deltaTime);
        HandleGravity();
        HandleJump();
    }

    void HandleRotation()
    {
        Vector2 lookDir = PlayerInputHandler.Instance.GetPLayerLook();
        transform.Rotate(0f, lookDir.x * movementSetup.RotationSpeed * Time.deltaTime, 0f);
    }

    void HandleMovement()
    {
        Vector2 movementDir = PlayerInputHandler.Instance.GetPlayerMovement();
        if (_isGrounded)
        {
            Vector3 movement = (transform.rotation * new Vector3(movementDir.x, 0f, movementDir.y)).normalized;
            _movement = new Vector3(movement.x, _movement.y, movement.z) * movementSetup.MoveSpeed;
        }
    }

    void CheckIfGrounded()
    {
        _isGrounded = Physics.SphereCast(transform.position, movementSetup.GroundCheckRadius, Vector3.down, out RaycastHit hit, movementSetup.GroundCheckDistance, movementSetup.GroundLayer);
    }
    void SetCrouch(bool enable)
    {
        Debug.Log("Crouch: " + enable);
        _isCrouching = enable;
    }

    void SetSprint(bool enable)
    {
        Debug.Log("Sprint: " + enable);
        _isSprinting = enable;
    }

    void HandleJump()
    {
        if (_isGrounded && !_isJumping && _jumpPressed)
        {
            _isJumping = true;
            _movement.y = Mathf.Sqrt(jumpSetup.JumpHeight * -2f * _gravity);
            Debug.Log("Jump");
        }
        else if (_isJumping && !_jumpPressed && _isGrounded)
        {
            _isJumping = false;
        }
    }

    void Dash()
    {
        Debug.Log("Dash");
    }

    void HandleGravity()
    {
        if (!_isGrounded)
        {
            _movement.y += _gravity * Time.deltaTime;
        }
        else
        {
            _movement.y = _groundedGravity; // Small negative value to keep the character grounded
        }
    }

    private void onJump(bool isPressed)
    {
        _jumpPressed = isPressed;
    }

    private void Subscribe(bool isSubscribed)
    {
        if (isSubscribed)
        {
            PlayerInputHandler.Jump += onJump;
            PlayerInputHandler.Dash += Dash;
            PlayerInputHandler.isPressedCrouch += SetCrouch;
            PlayerInputHandler.isPressedSprint += SetSprint;
        }
        else
        {
            PlayerInputHandler.Jump -= onJump;
            PlayerInputHandler.Dash -= Dash;
            PlayerInputHandler.isPressedCrouch -= SetCrouch;
            PlayerInputHandler.isPressedSprint -= SetSprint;
        }
    }
}
