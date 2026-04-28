using System;
using System.Collections;
using UnityEngine;

#region Setup Classes
[System.Serializable]
public class MovementSetup
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
public class JumpSetup
{
    [field: SerializeField, Tooltip("The height reached when jumping.")] public float JumpHeight { get; private set; } = 2f;
    [field: SerializeField, Tooltip("How long should the jump last")] public float JumpTime { get; private set; } = 0.5f;
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
    [field: SerializeField, Tooltip("Whether the character can dash in the air.")] public bool CanAirDash { get; private set; } = false;
}
#endregion Setup Classes

public class CharacterMovement : MonoBehaviour
{
    #region Events
    public static Action<float> UpdateJauge;
    #endregion Events

    #region Attributes
    [Header("Setups")]
    public MovementSetup movementSetup;
    public JumpSetup jumpSetup;
    public DashSetup dashSetup;

    [Header("References")]
    [SerializeField] private GameObject TPCam; // Anchor for the camera
    [SerializeField] private float _camMaxXAngle = 85f;
    private float _camXAngle;

    private CharacterController _controller;
    private PlayerInputHandler _playerInputHandler;

    private Vector2 _lastNonZeroMoveDir;
    private Vector2 _lastMoveDir;
    private Vector3 _movement;
    private float _gravity = -9.81f;
    private float _groundedGravity = -0.1f;
    private float _gravityMultiplier = 2f; // Multiplier for gravity when falling to create a heavier feel
    private float _maxFallingSpeed = -20f;
    private float _currentMoveSpeed;

    private bool _isGrounded;
    private bool _ceilingFree;
    private bool _isCrouching;
    private bool _isSprinting;
    private bool _isJumping;
    private bool _isDashing;
    private Coroutine _dashCoroutine;
    private float _dashCooldownTimer;
    private float _dashJaugeValue;
    private float _initialJumpSpeed;
    #endregion Attributes

    #region MonoBehaviour Flow
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerInputHandler = PlayerInputHandler.Instance;
        if (!TryGetComponent(out _controller))
        {
            Debug.LogError("CharacterMovement requires a CharacterController component.");
        }
        else {  _controller = GetComponent<CharacterController>(); }
        _currentMoveSpeed = movementSetup.MoveSpeed;
        SetupJump();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimers();
        CheckIfGrounded();
        CheckCeiling();
        HandleCrouch();
        HandleSprint();
        HandleDash();
        HandleRotation();
        HandleMovement();
        HandleGravity();
        HandleJump();
        _controller.Move(_movement * Time.deltaTime);
    }
    #endregion MonoBehaviour Flow

    #region Movement Checks
    void CheckIfGrounded()
    {
        // bool groundDetected = Physics.SphereCast(transform.position, movementSetup.GroundCheckRadius, Vector3.down, out RaycastHit hit, movementSetup.GroundCheckDistance, movementSetup.GroundLayer);
        bool groundDetected = Physics.BoxCast(transform.position, new Vector3(0.5f, movementSetup.GroundCheckRadius, 0.5f), Vector3.down, Quaternion.identity, movementSetup.GroundCheckDistance, movementSetup.GroundLayer);
        _isGrounded = groundDetected && _movement.y <= 0f; // Ensure the character is moving downwards or stationary to be considered grounded
    }

    void CheckCeiling()
    {
        _ceilingFree = !Physics.SphereCast(transform.position, movementSetup.CeilingCheckRadius, Vector3.up, out RaycastHit hit, movementSetup.CeilingCheckDistance);
    }
    #endregion Movement Checks

    #region Movement Handlers
    void HandleRotation()
    {
        Vector2 lookDir = PlayerInputHandler.Instance.GetPLayerLook();
        transform.Rotate(0f, lookDir.x * movementSetup.RotationSpeed * Time.deltaTime, 0f);
        _camXAngle = Mathf.Clamp(_camXAngle - lookDir.y * movementSetup.RotationSpeed * Time.deltaTime, -_camMaxXAngle, _camMaxXAngle);
        TPCam.transform.localEulerAngles = new Vector3(_camXAngle, 0f, 0f);
    }

    void HandleMovement()
    {
        Vector2 movementDir = Vector2.zero;
        Vector3 horizontalMovement;

        if ((_isGrounded || movementSetup.AirControl) && !_isDashing)
        {
            movementDir = PlayerInputHandler.Instance.GetPlayerMovement();
            if (movementDir != Vector2.zero) { _lastNonZeroMoveDir = movementDir; }
            _lastMoveDir = movementDir;
        }
        else if (_isDashing)
        {
            movementDir = _lastNonZeroMoveDir; // Player cannot change direction while dashing, and dash is performed in the last non-zero movement direction
        }
        else if (_isJumping)
        {
            movementDir = _lastMoveDir; // Player cannot change direction while jumping, and jump is performed in the last movement direction before the jump
        }

        horizontalMovement = (transform.rotation * new Vector3(movementDir.x, 0f, movementDir.y)).normalized * _currentMoveSpeed;
        _movement = new Vector3(horizontalMovement.x, _movement.y, horizontalMovement.z);
    }

    void HandleCrouch()
    {
        if (_isSprinting || _isDashing) { return; }
        if (_isGrounded && _playerInputHandler.CrouchPressed())
        {
            _isCrouching = true;
            _currentMoveSpeed = movementSetup.MoveSpeed * movementSetup.CrouchSpeedMultiplier;
        }
        else if (!_playerInputHandler.CrouchPressed() && _ceilingFree)
        {
            _currentMoveSpeed = movementSetup.MoveSpeed;
            _isCrouching = false;
        }
    }

    void HandleSprint()
    {
        if (_isCrouching || _isDashing) { return; }
        if (_isGrounded && _playerInputHandler.SprintPressed())
        {
            _isSprinting = true;
            _currentMoveSpeed = movementSetup.MoveSpeed * movementSetup.SprintSpeedMultiplier;
        }
        else
        {
            _currentMoveSpeed = movementSetup.MoveSpeed;
            _isSprinting = false;
        }
    }

    void HandleJump()
    {
        if(_isDashing) { return; }
        if (_isGrounded && !_isJumping && _playerInputHandler.JumpPressedThisFrame() && !_isCrouching)
        {
            _isJumping = true;
            _movement.y = _initialJumpSpeed;
        }
        else if (_isJumping && !_playerInputHandler.JumpPressedThisFrame() && _isGrounded)
        {
            _isJumping = false;
        }
    }

    void HandleDash()
    {
        if(_isCrouching) { return; }
        if ((_isGrounded || dashSetup.CanAirDash) && !_isDashing && !_isCrouching && _playerInputHandler.DashPressedThisFrame() && _dashCooldownTimer <= 0f)
        {
            // If the player tries to dash without ever moving, we set the dash direction to forward by default.
            // Otherwise, we use the last non-zero movement direction as the dash direction.
            _lastNonZeroMoveDir = _lastNonZeroMoveDir == Vector2.zero ? new Vector2(0f, 1f) : _lastNonZeroMoveDir;

            if (_dashCoroutine != null)
            {
                StopCoroutine(_dashCoroutine);
            }
            _dashCoroutine = StartCoroutine(Dash());
        }
    }

    void HandleGravity()
    {
        if (!_isGrounded && !_isDashing)
        {
            bool isFalling = _movement.y <= 0f || !_playerInputHandler.JumpPressed();
            float gravityMultiplier = isFalling ? _gravityMultiplier : 1f; // Stronger gravity when falling for a heavier feel

            // Verlet Integration
            float previousY = _movement.y;
            float newY = _movement.y + _gravity * gravityMultiplier * Time.deltaTime;
            float nextY = (previousY + newY) * 0.5f;
            _movement.y = Mathf.Max(nextY, _maxFallingSpeed); // Clamp to prevent ridiculous falling speeds
        }
        else if (!_isJumping)
        {
            _movement.y = _groundedGravity; // Keep the character grounded
        }
    }
    #endregion Movement Handlers

    #region Methods
    private void UpdateTimers()
    {
        if (_dashCooldownTimer > 0f)
        {
            _dashCooldownTimer -= Time.deltaTime;
            _dashJaugeValue = 1f - (_dashCooldownTimer / dashSetup.DashCooldown);
            UpdateJauge?.Invoke(_dashJaugeValue); // Update dash jauge value based on cooldown timer
        }
    }

    void SetupJump()
    {
        float timeToApex = jumpSetup.JumpTime / 2f;
        _initialJumpSpeed = (2f * jumpSetup.JumpHeight) / timeToApex; // v = d / t
        _gravity = (-2f * jumpSetup.JumpHeight) / (timeToApex * timeToApex); // a = 2d / t^2
    }

    public void ResetDashCooldown()
    {
        _dashCooldownTimer = 0f;
        _dashJaugeValue = 1f;
        UpdateJauge?.Invoke(_dashJaugeValue); // Update dash jauge value to full
    }

    IEnumerator Dash()
    {
        float startTime = Time.time;
        _isDashing = true;
        _currentMoveSpeed = movementSetup.MoveSpeed * dashSetup.DashSpeedMultiplier;

        while (Time.time < startTime + dashSetup.DashDuration)
        {
            yield return null;
        }  
        _currentMoveSpeed = movementSetup.MoveSpeed;
        _dashCooldownTimer = dashSetup.DashCooldown;
        _dashJaugeValue = 1f - (_dashCooldownTimer / dashSetup.DashCooldown);
        UpdateJauge?.Invoke(_dashJaugeValue); // Update dash jauge value based on cooldown timer
        _isDashing = false;
        yield break;
    }
    #endregion Methods
}
