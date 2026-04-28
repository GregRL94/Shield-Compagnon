using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _firingPoint;
    [SerializeField] private float _fireRate = 0.5f;
    private float _nextFireTime;

    // Update is called once per frame
    void Update()
    {
        if (_nextFireTime <= 0f)
        { 
            Fire();
            _nextFireTime = 1f / _fireRate;
        }
        else
        {
            _nextFireTime -= Time.deltaTime;
        }
    }

    void Fire()
    {
        Instantiate(_projectilePrefab, _firingPoint.position, _firingPoint.rotation);
    }
}
