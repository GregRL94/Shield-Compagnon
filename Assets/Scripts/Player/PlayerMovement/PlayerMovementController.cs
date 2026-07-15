using UnityEngine;

#region Parameters Classes
[System.Serializable]
public class MovementParameters
{
    [field: SerializeField, Tooltip("The speed at which the character moves.")] public float BaseMoveSpeed { get; private set; } = 10f;
    [field: SerializeField, Tooltip("The speed at which the character rotates.")] public float BaseRotationSpeed { get; private set; } = 5f;
    [field: SerializeField, Tooltip("Whether the character can move in the air.")] public bool AirControl { get; private set; } = false;

    [field: SerializeField, Tooltip("What is considered ground for the character.")] public LayerMask GroundLayer { get; private set; }
    [field: SerializeField, Tooltip("The radius of the sphere used for ground checking.")] public Vector3 GroundCheckBox { get; private set; } = new Vector3(0.5f, 0.5f, 0.5f);
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
    public MovementParameters MovementParams;
    public JumpParameters JumpParams;

    [Header("References")]
    [SerializeField] private GameObject FPCam; // Anchor for the camera
    [SerializeField] private float _camMaxXAngle = 85f;
    private float _camXAngle;

    public PlayerInputHandler PlayerInputs { get; private set; }
    protected PlayerMovementStateMachine _movementStateMachine;
    private Rigidbody _rb;

    public bool IsMoving { get; set; }
    public bool IsCrouching { get; set; }
    public bool IsSprinting { get; set; }
    public bool IsJumping { get; set; }
    public bool IsFalling { get; set; }
    public bool IsGrounded { get; set; }
    public bool IsCeilingFree { get; set; }

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    #endregion Attributes

    #region Monobehaviour Flow
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerInputs = PlayerInputHandler.Instance;
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
        MoveInput = PlayerInputs.GetPlayerMovement();
        LookInput = PlayerInputs.GetPlayerLook();
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
        bool groundDetected = Physics.BoxCast(transform.position, MovementParams.GroundCheckBox, Vector3.down, Quaternion.identity, MovementParams.GroundCheckDistance, MovementParams.GroundLayer);
        IsGrounded = groundDetected && _rb.linearVelocity.y <= 0f; // Ensure the character is moving downwards or stationary to be considered grounded
    }

    void CheckCeiling()
    {
        IsCeilingFree = !Physics.SphereCast(transform.position, MovementParams.CeilingCheckRadius, Vector3.up, out RaycastHit hit, MovementParams.CeilingCheckDistance);
    }
    #endregion Movement Checks

    #region Movement Handlers
    public void HandleRotation(float rotationSpeed)
    {
        // transform.Rotate(0f, LookInput.x * rotationSpeed * Time.fixedDeltaTime, 0f);
        _rb.angularVelocity = new Vector3(0f, LookInput.x * rotationSpeed * Time.fixedDeltaTime, 0f);
        _camXAngle = Mathf.Clamp(_camXAngle - LookInput.y * rotationSpeed * Time.fixedDeltaTime, -_camMaxXAngle, _camMaxXAngle);
        FPCam.transform.localEulerAngles = new Vector3(_camXAngle, 0f, 0f);
    }

    public void HandleMovement(float speed)
    {
        _rb.linearVelocity = (transform.rotation * new Vector3(MoveInput.x, 0f, MoveInput.y)).normalized * speed;
    }

    public void HandleJump(Vector3 jumpForce)
    {
        _rb.AddForce(jumpForce, ForceMode.Impulse);
    }
    #endregion Movement Handlers

    #region Methods
    PlayerMovementStateMachine CreateMovementStateMachine()
    {
        return new PlayerMovementStateMachine(this);
    }
    #endregion Methods
}
