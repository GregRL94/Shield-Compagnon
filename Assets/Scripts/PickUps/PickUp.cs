using UnityEngine;

public class PickUp : MonoBehaviour
{
    // Hoovering and rotation parameters
    [SerializeField] private float _rotationSpeed = 50f;
    [SerializeField] private float _hooverAmplitude = 0.5f;
    [SerializeField] private float _hooverFrequency = 1f;

    private Vector3 _startPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _startPosition = transform.position;
        float randomRotation = Random.Range(0f, 360f);
        transform.Rotate(Vector3.up, randomRotation);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
        float hooverOffset = Mathf.Sin(Time.time * _hooverFrequency) * _hooverAmplitude;
        transform.position = _startPosition + new Vector3(0f, hooverOffset, 0f);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player picked up: " + gameObject.name);
        }
    }
}
