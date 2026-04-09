using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public static Action Interact;
    public static Action Jump;
    public static Action<bool> isPressedCrouch;
    public static Action<bool> isPressedSprint;

    public static PlayerInputHandler Instance { get; private set; }
    InputSystem_Actions _inputActions;

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

    public Vector2 GetPlayerMovement()
    {
        return _inputActions.Player.Move.ReadValue<Vector2>();
    }

    public Vector2 GetPLayerLook()
    {
        return _inputActions.Player.Look.ReadValue<Vector2>();
    }

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
            _inputActions.Player.Interact.performed += ctx => Interact?.Invoke(); // On passe le contexte de l'action en lambda
            _inputActions.Player.Jump.performed += ctx => Jump?.Invoke();
            _inputActions.Player.Crouch.started += ctx => isPressedCrouch?.Invoke(true);
            _inputActions.Player.Crouch.canceled += ctx => isPressedCrouch?.Invoke(false);
            _inputActions.Player.Sprint.started += ctx => isPressedSprint?.Invoke(true);
            _inputActions.Player.Sprint.canceled += ctx => isPressedSprint?.Invoke(false);
        }
        else
        {
            _inputActions.Player.Interact.performed -= ctx => Interact?.Invoke();
            _inputActions.Player.Jump.performed -= ctx => Jump?.Invoke();
            _inputActions.Player.Crouch.started -= ctx => isPressedCrouch?.Invoke(true);
            _inputActions.Player.Crouch.canceled -= ctx => isPressedCrouch?.Invoke(false);
            _inputActions.Player.Sprint.started -= ctx => isPressedSprint?.Invoke(true);
            _inputActions.Player.Sprint.canceled -= ctx => isPressedSprint?.Invoke(false);
            _inputActions.Player.Disable();
        }        
    }
}
