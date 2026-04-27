using System;
using System.Collections;
using UnityEngine;

#region Setup Classes
[System.Serializable]
public class ShieldPointsOnCharacter
{
    [field: SerializeField, Tooltip("Transform of the shield on the character when at rest")] public Transform RestPoint { get; private set; }
    [field: SerializeField, Tooltip("Transform of the shield on the character when blocking")] public Transform BlockPoint { get; private set; }
    [field: SerializeField, Tooltip("Transform of the shield on the character when throwing")] public Transform ThrowPoint { get; private set; }
}

[System.Serializable]
public class ShieldThrowParameters
{
    [field: SerializeField, Tooltip("Layer mask for obstacles that can block the shield throw or call")] public LayerMask ObstaclesLayer {  get; private set; }
    [field: SerializeField, Tooltip("The minimal free of obstacles distance at which the player can throw the shield")] public float NoObstructionDistance { get; private set; } = 1f;
    [field: SerializeField, Tooltip("The distance within which the player can throwback the shield")] public float ThrowbackDistance { get; private set; } = 2f;
    [field: SerializeField, Tooltip("The angle within which the player can throwback the shield")] public float ThrowbackAngle { get; private set; } = 90f;
}
#endregion Setup Classes

public class CharacterActions : MonoBehaviour
{
    #region Events
    public static Action ShieldBlock;
    public static Action<Vector3> ShieldThrow;
    public static Action<Vector3> ShieldCall;
    public static Action<bool> ShieldAttach;
    #endregion Events

    #region Attributes
    [Header("Shield Settings")]
    [field: SerializeField, Tooltip("The player's shield prefab")] public GameObject PlayerShield { get; private set; }
    public ShieldPointsOnCharacter shieldPointsOnChar;
    public ShieldThrowParameters shieldThrowParameters;

    [Header("Player Attacks")]
    [SerializeField] private LayerMask _attacksAffectWhat;
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private float _attackRadius;

    private bool _shieldOnCharacter;
    private bool _shieldJustThrown;
    private bool _shieldBlock;
    private bool _shieldCalled;
    #endregion Attributes

    #region Monobehaviour Flow
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetShieldOnCharacter(true);
        SetShieldPositionOnChar(ShieldPosOnCharacter.Rest);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_shieldOnCharacter && !_shieldJustThrown)
        {
            if (Vector3.Distance(transform.position, PlayerShield.transform.position) <= 1f)
            {
                SetShieldOnCharacter(true);
                SetShieldPositionOnChar(ShieldPosOnCharacter.Rest);
            }
        }
        if (!_shieldOnCharacter && _shieldCalled)
        {
            TryCallShield();
        }
    }
    #endregion Monobehaviour Flow

    #region Events Management
    void OnBlock()
    {
        // Unnecessary because the throw shield action was moved on the attack input, as there was no time to implement a normal attack.
    }

    void OnBlockHold()
    {
        if (!_shieldOnCharacter)
        {
            _shieldCalled = true;
            return;
        }
        SetShieldPositionOnChar(ShieldPosOnCharacter.Block);
    }

    void OnBlockHoldEnd()
    {
        _shieldCalled = false;
        SetShieldPositionOnChar(ShieldPosOnCharacter.Rest);
    }
    #endregion Events Management

    #region Shield Actions
    void TryCallShield()
    {
        Vector3 pos = transform.position;
        Vector3 shieldPos = PlayerShield.transform.position;
        
        if (!Physics.Raycast(pos, shieldPos - pos, Vector3.Distance(pos, shieldPos), shieldThrowParameters.ObstaclesLayer))
        {
            PlayerShield.GetComponent<PlayerShield>().OnShieldCalled(transform.position);
        }
    }

    void TryThrowShield()
    {
        float distanceToShield = Vector3.Distance(transform.position, PlayerShield.transform.position);
        float angleToShield = Vector3.Angle(transform.forward, PlayerShield.transform.position - transform.position);

        if (_shieldOnCharacter)
        {
            if (!Physics.SphereCast(PlayerShield.transform.position, 0.5f, transform.forward, out RaycastHit hit, shieldThrowParameters.NoObstructionDistance, ~(1 << PlayerShield.layer)))
            {
                Debug.Log("Throwing Shield !");
                ThrowShield();
            }
        }
        else if ( distanceToShield <= shieldThrowParameters.ThrowbackDistance && angleToShield <= shieldThrowParameters.ThrowbackAngle)
        {
            Debug.Log("Throwing back shield !");
            ThrowShield();
        }
    }

    void ThrowShield()
    {
        SetShieldPositionOnChar(ShieldPosOnCharacter.Throw);
        SetShieldOnCharacter(false);
        StartCoroutine(ShieldJustThrown());
        // Vector3 moveDirection = transform.rotation * Vector3.forward;
        Vector3 moveDirection = shieldPointsOnChar.ThrowPoint.rotation * Vector3.forward;
        ShieldThrow?.Invoke(moveDirection);
        Debug.Log("Shield thrown !");
    }

    public void ResetShield()
    {
        SetShieldOnCharacter(true);
        SetShieldPositionOnChar(ShieldPosOnCharacter.Rest);
    }

    void SetShieldOnCharacter(bool shieldOnCharacter)
    {
        ShieldAttach?.Invoke(shieldOnCharacter);
        _shieldOnCharacter = shieldOnCharacter;
        string logMessage = shieldOnCharacter ? "Shield attached to character" : "Shield detached from character";
        Debug.Log(logMessage);
    }

    void SetShieldPositionOnChar(ShieldPosOnCharacter posOnChar)
    {
        Transform shieldTransform = PlayerShield.transform;
        shieldTransform.SetParent(null);

        switch (posOnChar)
        {
            case ShieldPosOnCharacter.Rest:
                shieldTransform.position = shieldPointsOnChar.RestPoint.position;
                shieldTransform.rotation = shieldPointsOnChar.RestPoint.rotation;
                shieldTransform.SetParent(transform);
                _shieldBlock = false;
                Debug.Log("Set shield to rest position");
                break;
            case ShieldPosOnCharacter.Block:
                shieldTransform.position = shieldPointsOnChar.BlockPoint.position;
                shieldTransform.rotation = shieldPointsOnChar.BlockPoint.rotation;
                shieldTransform.SetParent(transform);
                _shieldBlock = true;
                ShieldBlock?.Invoke();
                Debug.Log("Set shield to block position");
                break;
            case ShieldPosOnCharacter.Throw:
                shieldTransform.position = shieldPointsOnChar.ThrowPoint.position;
                shieldTransform.rotation = shieldPointsOnChar.ThrowPoint.rotation;
                _shieldBlock = false;
                Debug.Log("Set shield to Throw position");
                break;
        }
    }

    IEnumerator ShieldJustThrown()
    {
        _shieldJustThrown = true;
        yield return new WaitForSeconds(0.5f);
        _shieldJustThrown = false;
    }
    #endregion Shield Actions

    #region Subscriptions
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
    }

    void Subscribe(bool isSubscribed)
    {
        if (isSubscribed)
        {
            PlayerInputHandler.Attack += TryThrowShield;
            PlayerInputHandler.Block += OnBlock;
            PlayerInputHandler.BlockHold += OnBlockHold;
            PlayerInputHandler.BlockHoldEnd += OnBlockHoldEnd;
        }
        else
        {
            PlayerInputHandler.Attack -= TryThrowShield;
            PlayerInputHandler.Block -= OnBlock;
            PlayerInputHandler.BlockHold -= OnBlockHold;
            PlayerInputHandler.BlockHoldEnd -= OnBlockHoldEnd;
        }
    }
    #endregion Subscriptions
}
