using UnityEngine;

public class Translator : MonoBehaviour
{
    [SerializeField] protected char _axis = 'x';
    [SerializeField] protected float _translationAmplitude = 5f;
    [SerializeField] protected float _translationFrequency = 1f;
    [SerializeField] protected bool _reverseDirection = false;
    protected Vector3 _startingPosition;
    protected float _dirSign = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {
        _startingPosition = transform.position;
        _dirSign = _reverseDirection ? -1f : 1f;
    }

    // Update is called once per frame
    protected virtual void Update(){ }
}
