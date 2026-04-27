using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    #region Events
    public static Action Attack;
    public static Action Block;
    public static Action BlockHold;
    public static Action BlockHoldEnd;
    public static Action<bool> Jump;
    public static Action<bool> Dash;
    public static Action<bool> isPressedCrouch;
    public static Action<bool> isPressedSprint;
    #endregion Events

    #region Attributes
    [SerializeField] private float _blockHoldThreshold = 0.25f; // min time to consider a right-click hold
    public static PlayerInputHandler Instance { get; private set; }
    InputSystem_Actions _inputActions;

    private bool _blockPressed;
    private bool _blockHoldSent;
    private float _currentBlockHoldTime;
    #endregion Attributes

    #region Singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
        _inputActions = new InputSystem_Actions();
        DontDestroyOnLoad(gameObject);
    }
    #endregion Singleton

    #region MonoBehaviour Flow
    private void Update()
    {
        if (!_blockPressed) { return; }
        if (_blockHoldSent) { return; }

        _currentBlockHoldTime += Time.deltaTime;
        if (_currentBlockHoldTime >= _blockHoldThreshold)
        {
            BlockHold?.Invoke();
            _blockHoldSent = true;
            Debug.Log("Block Hold signal sent");
        }
    }
    #endregion MonoBehaviour Flow

    #region Input Getters
    public Vector2 GetPlayerMovement()
    {
        return _inputActions.Player.Move.ReadValue<Vector2>();
    }

    public Vector2 GetPLayerLook()
    {
        return _inputActions.Player.Look.ReadValue<Vector2>();
    }

    public bool CrouchPressed()
    {
        return _inputActions.Player.Crouch.IsPressed();
    }

    public bool SprintPressed()
    {
        return _inputActions.Player.Sprint.IsPressed();
    }

    public bool JumpPressed()
    {
        return _inputActions.Player.Jump.IsPressed();
    }

    public bool JumpPressedThisFrame()
    {
        return _inputActions.Player.Jump.WasPressedThisFrame();
    }

    public bool DashPressedThisFrame()
    {
        return _inputActions.Player.Dash.WasPressedThisFrame();
    }
    public bool BlockPressed()
    {
        return _inputActions.Player.Block.IsPressed();
    }
    #endregion Input Getters

    #region Block Action Management
    private void BlockStart()
    {
        _currentBlockHoldTime = 0f;
        _blockPressed = true;
        _blockHoldSent = false;
        Debug.Log("Block pressed");
    }

    private void BlockEnd()
    {
        if (_currentBlockHoldTime < _blockHoldThreshold)
        {
            Block?.Invoke();
            _blockPressed = false;
            Debug.Log("Short Block signal pressed");
        }
        else
        {
            BlockHoldEnd?.Invoke();
            _blockPressed = false;
            Debug.Log("Block Hold End signal sent");
        }
    }
    #endregion Block Action Management

    #region Subriptions
    private void OnEnable()
    {
        Subscribe(true);
    }

    private void OnDisable()
    {
        Subscribe(false);
    }

    private void OnDestroy()
    {
        Subscribe(false);
        _inputActions.Dispose();
    }

    private void Subscribe(bool isSubscribed)
    {
        if (isSubscribed)
        {
            _inputActions.Player.Enable();
            _inputActions.Player.Attack.performed += ctx => Attack?.Invoke();
            _inputActions.Player.Block.started += ctx => BlockStart();
            _inputActions.Player.Block.canceled += ctx => BlockEnd();
        }
        else
        {
            _inputActions.Player.Attack.performed -= ctx => Attack?.Invoke();
            _inputActions.Player.Block.started -= ctx => BlockStart();
            _inputActions.Player.Block.canceled -= ctx => BlockEnd();
            _inputActions.Player.Disable();
        }        
    }
    #endregion Subriptions
}
