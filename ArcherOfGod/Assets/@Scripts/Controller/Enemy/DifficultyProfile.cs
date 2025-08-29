using UnityEngine;

public enum DifficultyTier { Easy, Normal, Hard, Insane }

[CreateAssetMenu(fileName = "AiDifficulty", menuName = "Enemys/DifficultyAiData")]
public class DifficultyProfile : ScriptableObject
{
    public DifficultyTier tier = DifficultyTier.Normal;
    public AttackData NormalAttackData;
    public AttackData[] SkillAttackDatas;
    [Header("Move / Patrol")]
    public float moveSpeed = 2.5f;
    public float patrolChangeInterval = 2.5f;
    public Vector2 patrolChangeJitter = new(0.8f, 1.8f);

    [Header("Combat Frequency")]
    public Vector2 attackCooldownRange = new(1.6f, 3.0f);
    [Range(0f, 1f)] public float attackChance = 0.5f;

    public Vector2 skillCooldownRange = new(4.5f, 7.5f);
    [Range(0f, 1f)] public float skillChance = 0.3f;

    [Header("Projectile (����)")]
    public float projectileSpeed = 8f;

    [Header("Awareness (����)")]
    public float reactionDelay = 0.2f; // ���� �� �׼Ǳ��� ����
}
