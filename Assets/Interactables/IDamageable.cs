using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int amount);
    void Heal(int amount);
    void OnDeath();
}
