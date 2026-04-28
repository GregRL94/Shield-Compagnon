using UnityEngine;

public interface IHittable
{
    /// <summary>
    /// Simple hit with damage.
    /// </summary>
    /// <param name="damage">The amount of damage to apply. Must be a non-negative value.</param>
    public void TakeHit(float damage);
}
