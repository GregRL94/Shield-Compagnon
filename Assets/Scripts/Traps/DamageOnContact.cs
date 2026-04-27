using UnityEngine;

public class DamageOnContact : MonoBehaviour
{
    [SerializeField] private int _damageAmount = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IHittable>(out var hittable))
        {
            hittable.TakeHit(_damageAmount);
        }
    }
}
