using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    #region Events
    public static Action Attack;
    public static Action Interact;
    public static Action<bool> Jump;
    public static Action<bool> Dash;
    public static Action<bool> isPressedCrouch;
    public static Action<bool> isPressedSprint;
    #endregion Events

    #region Attributes
    public static PlayerInputHandler Instance { get; private set; }
    InputSystem_Actions _inputActions;
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
    #endregion Input Getters

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
            _inputActions.Player.Interact.performed += ctx => Interact?.Invoke();
        }
        else
        {
            _inputActions.Player.Attack.performed -= ctx => Attack?.Invoke();
            _inputActions.Player.Interact.performed -= ctx => Interact?.Invoke();
            _inputActions.Player.Disable();
        }        
    }
    #endregion Subriptions
}
