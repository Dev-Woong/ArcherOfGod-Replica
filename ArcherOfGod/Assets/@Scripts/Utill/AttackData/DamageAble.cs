using UnityEngine;
public interface IDamageAble
{
    public void TakeDamage(float Damage, SpecialAbility skillAbility);
}
public abstract class DamageAbleBase : MonoBehaviour, IDamageAble
{
    public bool DamageAble = true;
    public void TakeDamage(float Damage, SpecialAbility skillAbility)
    {
        if (DamageAble == true)
        {
            OnDamage(Damage, skillAbility);
        }
    }
    public abstract void OnDamage(float damage, SpecialAbility skillAbility);
}

