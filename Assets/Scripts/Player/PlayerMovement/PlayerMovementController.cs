using UnityEngine;

#region Parameters Classes
[System.Serializable]
public class MovementParameters
{
    [field: SerializeField, Tooltip("The speed at which the character moves.")] public float MoveSpeed { get; private set; } = 10f;
    [field: SerializeField, Tooltip("The speed at which the character rotates.")] public float RotationSpeed { get; private set; } = 5f;
    [field: SerializeField, Tooltip("Whether the character can move in the air.")] public bool AirControl { get; private set; } = false;

    [field: SerializeField, Tooltip("What is considered ground for the character.")] public LayerMask GroundLayer { get; private set; }
    [field: SerializeField, Tooltip("The radius of the sphere used for ground checking.")] public float GroundCheckRadius { get; private set; } = 0.5f;
    [field: SerializeField, Tooltip("The distance for the sphere cast used for ground checking.")] public float GroundCheckDistance { get; private set; } = 0.5f;

    [field: SerializeField, Tooltip("The radius of the sphere used for ceiling checking.")] public float CeilingCheckRadius { get; private set; } = 0.5f;
    [field: SerializeField, Tooltip("The distance for the sphere cast used for ceiling checking.")] public float CeilingCheckDistance { get; private set; } = 0.5f;

    [field: SerializeField, Tooltip("The speed multiplier when the character is crouching.")] public float CrouchSpeedMultiplier { get; private set; } = 0.5f;
    [field: SerializeField, Tooltip("The speed multiplier when the character is sprinting.")] public float SprintSpeedMultiplier { get; private set; } = 1.5f;
}

[System.Serializable]
public class JumpParameters
{
    [field: SerializeField, Tooltip("The force impulse applied when jumping.")] public float JumpForce { get; private set; } = 5f;
    [field: SerializeField, Tooltip("Whether the character can perform jumps in the air.")] public bool CanAirJump { get; private set; } = false;
    [field: SerializeField, Tooltip("The maximum number of jumps allowed in the air.")] public int MaxAirJumps { get; private set; } = 0;
}
#endregion ParametersClasses


public class PlayerMovementController : MonoBehaviour
{
    #region Attributes
    [Header("Parameters")]
    public MovementParameters movementParams;
    public JumpParameters jumpParams;

    [Header("References")]
    [SerializeField] private GameObject FPCam; // Anchor for the camera
    [SerializeField] private float _camMaxXAngle = 85f;
    private float _camXAngle;

    protected PlayerMovementStateMachine _movementStateMachine;
    private PlayerInputHandler _playerInputHandler;
    private Rigidbody _rb;

    public bool IsMoving { get; set; }
    public bool IsCrouching { get; set; }
    public bool IsSprinting { get; set; }
    public bool IsJumping { get; set; }

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    private bool _isGrounded;
    private bool _ceilingFree;
    #endregion Attributes

    #region Monobehaviour Flow
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerInputHandler = PlayerInputHandler.Instance;
        if (!TryGetComponent<Rigidbody>(out _rb))
        {
            Debug.LogError("Rigidbody component not found on the player.");
        }
        _rb = GetComponent<Rigidbody>();
        _movementStateMachine = CreateMovementStateMachine();
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfGrounded();
        CheckCeiling();
        MoveInput = _playerInputHandler.GetPlayerMovement();
        LookInput = _playerInputHandler.GetPLayerLook();
        _movementStateMachine.UpdateStateMachine();
    }

    void FixedUpdate()
    {
        _movementStateMachine.FixedUpdateStateMachine();
    }
    #endregion Monobehaviour Flow

    #region Movement Checks
    void CheckIfGrounded()
    {
        bool groundDetected = Physics.BoxCast(transform.position, new Vector3(0.5f, movementParams.GroundCheckRadius, 0.5f), Vector3.down, Quaternion.identity, movementParams.GroundCheckDistance, movementParams.GroundLayer);
        _isGrounded = groundDetected && _rb.linearVelocity.y <= 0f; // Ensure the character is moving downwards or stationary to be considered grounded
    }

    void CheckCeiling()
    {
        _ceilingFree = !Physics.SphereCast(transform.position, movementParams.CeilingCheckRadius, Vector3.up, out RaycastHit hit, movementParams.CeilingCheckDistance);
    }
    #endregion Movement Checks

    #region Movement Handlers
    public void Jump()
    {

    }
    #endregion Movement Handlers

    #region Methods
    PlayerMovementStateMachine CreateMovementStateMachine()
    {
        return new PlayerMovementStateMachine(this);
    }
    #endregion Methods
}
