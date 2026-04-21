using System;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class CharacterActions : MonoBehaviour
{
    public static Action<Vector3> ThrowShield;
    public static Action<Vector3> CallShield;

    [SerializeField] private GameObject _playerShield;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TestAttack()
    {
        Vector3 pos = transform.position;
        Vector3 shieldPos = _playerShield.transform.position;
        
        if (!Physics.Raycast(pos, shieldPos - pos, Vector3.Distance(pos, shieldPos), ~(1 << _playerShield.layer)))
        {
            Debug.Log("Calling Shield");
            Vector3 moveDirection = transform.rotation * Vector3.forward;
            CallShield?.Invoke(transform.position);
        }
    }

    void TestThrowShield()
    {
        Debug.Log("TestThrowShield called");
        Vector3 moveDirection = transform.rotation * Vector3.forward;
        ThrowShield?.Invoke(moveDirection);
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
    }

    void Subscribe(bool isSubscribed)
    {
        if (isSubscribed)
        {
            PlayerInputHandler.Attack += TestAttack;
            PlayerInputHandler.Interact += TestThrowShield;
        }
        else
        {
            PlayerInputHandler.Attack -= TestAttack;
            PlayerInputHandler.Interact -= TestThrowShield;
        }
    }
}
