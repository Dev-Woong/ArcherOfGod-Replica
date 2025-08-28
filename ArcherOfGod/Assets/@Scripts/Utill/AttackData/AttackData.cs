using Unity.VisualScripting;
using UnityEngine;

public enum SpecialAbility
{
    NONE = 0,
    FIREBURN,
    POISON,
    LIGHTNING,
    FREEZE,
    EXPLOSION,
}

[CreateAssetMenu(fileName = "NewAttackData", menuName = "Attacks/AttackData")]
public class AttackData : ScriptableObject
{
    [Header("스킬 이름")]
    public string AttackDataName;
    [Header("스킬 아이콘")]
    public Sprite SkillIcon;
    [Header("해당 애니메이션 이름")]
    public string AnimationTriggerName;
    [Header("타겟 레이어")]
    public LayerMask TargetLayer;
    [Header("화살 프리팹 이름")]
    public string ArrowPrefabName;
    [Header("화살 특수능력")]
    public SpecialAbility SpecialAbility;
    public int AbilityDamageTickCount;
    public float AbilityTickDamage;
    public Vector3 ArrowSpawnPos;
    [Header("베지어 커브")]
    public bool UseBezierCurve;
    public Vector2 SetCurvePos;
    [Header("화살 속도")]
    public float ArrowVelocity;
    [Header("데미지")]
    public float Damage;
    [Header("쿨타임")]
    public float CoolTime;
    [Header("도약 사격 여부")]
    public bool JumpAttack;
    public Vector2 JumpForce;
    [Header("넉백 여부")]
    public bool Knockback;
    public Vector2 KnockbackForce;
    [Header("이펙트")]
    public string EffectPrefabName;
    public Vector3 EffectPos;
    [Header("히트 이펙트")]
    public string HitEffectPrefabName;
    public Vector3 HitEffectPos;

    [Header("효과음")]
    public AudioClip AttackSFX;
    public AudioClip HitSFX;

    [Header("카메라 효과")]
    public bool UseCameraShake;
    public float ShackPower;
    public Vector2 ShakeDir;
    public float ShakeDuration;
}