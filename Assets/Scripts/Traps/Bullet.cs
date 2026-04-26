using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _damage = 5f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == gameObject.layer) { return; }
        if (other.TryGetComponent<IHittable>(out IHittable hittable))
        {
            hittable.TakeHit(_damage);
        }
        Destroy(gameObject);
    }
}
