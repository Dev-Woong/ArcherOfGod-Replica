using System.Net.Mail;
using UnityEngine;

public class ArrowSet : MonoBehaviour
{
    public LayerMask TargetMask;
    public float Damage;
    public SpecialAbility SpecialAbility;
    public int BurningTickCount;
    public float BurningTickDamage;
    public float FrozenTime;
    public float PoisonTickDamage;
    public int PoisonTickCount;

    public int ElectricTickCount; 
    public float ElectricTickDamage;
    public float ElectricDebuffSpeed;

    public string HitEffectName;
    public Vector3 HitEffectPos;

    public void SetUpTarget(AttackData attackData)
    {
        TargetMask = attackData.TargetLayer;
        Damage = attackData.Damage;
        SpecialAbility = attackData.SpecialAbility;
        BurningTickCount = attackData.BurningDamageTickCount;
        BurningTickDamage = attackData.BurningTickDamage;
        HitEffectName = attackData.HitEffectPrefabName;
        HitEffectPos = attackData.HitEffectPos;
        FrozenTime = attackData.ForzenTime;
        PoisonTickDamage = attackData.PoisonDebuffDamage;
        PoisonTickCount = attackData.PoisonDebuffTickCount;
        ElectricTickCount = attackData.ElectricTickCount;
        ElectricTickDamage = attackData.ElectricTickDamage;
        ElectricDebuffSpeed = attackData.ElectricSpeedDebuff;
    }

}
