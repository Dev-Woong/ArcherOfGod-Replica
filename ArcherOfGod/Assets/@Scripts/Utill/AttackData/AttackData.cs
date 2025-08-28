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
    [Header("��ų �̸�")]
    public string AttackDataName;
    [Header("��ų ������")]
    public Sprite SkillIcon;
    [Header("�ش� �ִϸ��̼� �̸�")]
    public string AnimationTriggerName;
    [Header("Ÿ�� ���̾�")]
    public LayerMask TargetLayer;
    [Header("ȭ�� ������ �̸�")]
    public string ArrowPrefabName;
    [Header("ȭ�� Ư���ɷ�")]
    public SpecialAbility SpecialAbility;
    public int AbilityDamageTickCount;
    public float AbilityTickDamage;
    public Vector3 ArrowSpawnPos;
    [Header("������ Ŀ��")]
    public bool UseBezierCurve;
    public Vector2 SetCurvePos;
    [Header("ȭ�� �ӵ�")]
    public float ArrowVelocity;
    [Header("������")]
    public float Damage;
    [Header("��Ÿ��")]
    public float CoolTime;
    [Header("���� ��� ����")]
    public bool JumpAttack;
    public Vector2 JumpForce;
    [Header("�˹� ����")]
    public bool Knockback;
    public Vector2 KnockbackForce;
    [Header("����Ʈ")]
    public string EffectPrefabName;
    public Vector3 EffectPos;
    [Header("��Ʈ ����Ʈ")]
    public string HitEffectPrefabName;
    public Vector3 HitEffectPos;

    [Header("ȿ����")]
    public AudioClip AttackSFX;
    public AudioClip HitSFX;

    [Header("ī�޶� ȿ��")]
    public bool UseCameraShake;
    public float ShackPower;
    public Vector2 ShakeDir;
    public float ShakeDuration;
}