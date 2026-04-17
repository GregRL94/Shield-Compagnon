using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    [Header("Movement")]
    public Vector3 velocity = new Vector3(3f, 3f, 0f); // Initial direction + speed
    public float speed = 5f;

    [Header("Bounce")]
    public LayerMask bounceableLayers; // Layers the object can bounce off
    public float raycastDistance = 0.1f; // How far ahead to detect a surface

    void Update()
    {
        Move();
    }

    void Move()
    {
        Vector3 moveDirection = velocity.normalized;
        float moveDistance = speed * Time.deltaTime;

        // Cast a ray in the direction of movement to detect surfaces ahead
        if (Physics.Raycast(transform.position, moveDirection, out RaycastHit hit, moveDistance + raycastDistance, bounceableLayers))
        {
            // --- Law of Reflection ---
            // Reflect the velocity across the surface normal
            velocity = Vector3.Reflect(velocity, hit.normal);

            // Move to the hit point, then continue remaining movement in new direction
            float remainingDistance = moveDistance - hit.distance;
            transform.position = hit.point + velocity.normalized * remainingDistance;
        }
        else
        {
            // No surface ahead — move normally
            transform.position += moveDirection * moveDistance;
        }
    }
}
