using System.Collections;
using UnityEngine;

#region Setup Classes
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
    [field: SerializeField, Tooltip("Whether the character can dash in the air.")] public bool CanAirDash { get; private set; } = false;
}
#endregion Setup Classes

public class CharacterMovement : MonoBehaviour
{
    #region Attributes
    [Header("Setups")]
    public MovementSetup movementSetup;
    public JumpSetup jumpSetup;
    public DashSetup dashSetup;

    [Header("References")]
    [SerializeField] private GameObject TPCam;
    [SerializeField] private float _camMaxXAngle = 85f;

    private float _camXAngle;
    private CharacterController _controller;
    private PlayerInputHandler _playerInputHandler;

    private Vector3 _movement;
    private Vector3 _lastNonZeroMovement;

    private float _gravity = -9.81f;
    private float _groundedGravity = -0.1f;
    private float _currentMoveSpeed;
    private bool _isGrounded;

    private bool _isCrouching;
    private bool _isSprinting;
    private bool _isJumping;
    private bool _isDashing;
    private Coroutine _dashCoroutine;
    private float _dashCooldownTimer;
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
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimers();
        CheckIfGrounded();
        HandleRotation();
        HandleCrouch();
        HandleSprint();
        HandleDash();
        HandleMovement();
        _controller.Move(_movement * _currentMoveSpeed * Time.deltaTime);

        HandleGravity();
        HandleJump();
    }
    #endregion MonoBehaviour Flow

    #region Movement Checks
    void CheckIfGrounded()
    {
        _isGrounded = Physics.SphereCast(transform.position, movementSetup.GroundCheckRadius, Vector3.down, out RaycastHit hit, movementSetup.GroundCheckDistance, movementSetup.GroundLayer);
    }

    bool CheckCeiling()
    {
        return Physics.SphereCast(transform.position, movementSetup.GroundCheckRadius, Vector3.up, out RaycastHit hit, movementSetup.GroundCheckDistance);
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
        Vector2 movementDir = PlayerInputHandler.Instance.GetPlayerMovement();

        if (_isGrounded)
        {
            Vector3 movement = (transform.rotation * new Vector3(movementDir.x, 0f, movementDir.y)).normalized;
            _movement = new Vector3(movement.x, _movement.y, movement.z);
            if (movementDir != Vector2.zero)
            {
                _lastNonZeroMovement = _movement;
            }
        }
        if (_isDashing)
        {
            _movement = _lastNonZeroMovement;
        }
    }

    void HandleCrouch()
    {
        if (_isSprinting || _isDashing) { return; }
        if (_isGrounded && _playerInputHandler.CrouchPressed())
        {
            Debug.Log("Crouching");
            _isCrouching = true;
            _currentMoveSpeed = movementSetup.MoveSpeed * movementSetup.CrouchSpeedMultiplier;
        }
        else if (!_playerInputHandler.CrouchPressed() && !CheckCeiling())
        {
            _isCrouching = false;
            _currentMoveSpeed = movementSetup.MoveSpeed;
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
            _isSprinting = false;
            _currentMoveSpeed = movementSetup.MoveSpeed;
        }
    }

    void HandleJump()
    {
        if(_isDashing) { return; }
        if (_isGrounded && !_isJumping && _playerInputHandler.JumpPressed() && !_isCrouching)
        {
            _isJumping = true;
            _movement.y = Mathf.Sqrt(jumpSetup.JumpHeight * -2f * _gravity);
            Debug.Log("Jump");
        }
        else if (_isJumping && !_playerInputHandler.JumpPressed() && _isGrounded)
        {
            _isJumping = false;
        }
    }

    void HandleDash()
    {
        if(_isCrouching) { return; }
        if ((_isGrounded || dashSetup.CanAirDash) && !_isDashing && !_isCrouching && _playerInputHandler.DashPressed() && _dashCooldownTimer <= 0f)
        {
            if (_dashCoroutine != null)
            {
                StopCoroutine(_dashCoroutine);
            }
            _dashCoroutine = StartCoroutine(Dash());
        }
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
    #endregion Movement Handlers

    private void UpdateTimers()
    {
        if (_dashCooldownTimer > 0f) { _dashCooldownTimer -= Time.deltaTime; }
    }

    void Jump()
    {

    }
    IEnumerator Dash()
    {
        Debug.Log("Dash");
        float startTime = Time.time;
        _isDashing = true;
        _currentMoveSpeed = movementSetup.MoveSpeed * dashSetup.DashSpeedMultiplier;

        while (Time.time < startTime + dashSetup.DashDuration)
        {
            yield return null;
        }  
        _currentMoveSpeed = movementSetup.MoveSpeed;
        _dashCooldownTimer = dashSetup.DashCooldown;
        _isDashing = false;
        yield break;
    }
}
