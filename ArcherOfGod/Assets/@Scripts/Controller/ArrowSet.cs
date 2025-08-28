using System.Net.Mail;
using UnityEngine;

public class ArrowSet : MonoBehaviour
{
    public LayerMask TargetMask;
    public float Damage;
    public SpecialAbility SpecialAbility;
    public int TickCount;
    public float TickDamage;
    public string HitEffectName;
    public Vector3 HitEffectPos;
    
    public void SetUpTarget(AttackData attackData)
    {
        TargetMask = attackData.TargetLayer;
        Damage = attackData.Damage;
        SpecialAbility = attackData.SpecialAbility;
        TickCount = attackData.AbilityDamageTickCount;
        TickDamage = attackData.AbilityTickDamage;
        HitEffectName = attackData.HitEffectPrefabName;
        HitEffectPos = attackData.HitEffectPos;
    }
}
