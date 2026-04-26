using UnityEngine;

public class DamageOnContact : MonoBehaviour
{
    [SerializeField] private int damageAmount = 10;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit Laser: " + gameObject.name);
        }
    }
}
