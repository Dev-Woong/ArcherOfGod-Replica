using Mono.Cecil.Cil;
using UnityEngine;
using static Unity.Cinemachine.IInputAxisOwner.AxisDescriptor;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator anim;
    [SerializeField] LayerMask _wallMask;
    [SerializeField] LayerMask _arrowMask;
    [SerializeField] ObjectStatus _objectStatus;

    [Header("Move")]
    [SerializeField] float _moveSpeed = 2.5f;
    [SerializeField] float _patrolChangeInterval = 2.5f;
    [SerializeField] Vector2 _patrolChangeJitter = new(0.8f, 1.8f);

    [Header("Combat Random")]
    [SerializeField] Vector2 _attackCooldownRange = new(1.6f, 3.0f);
    [SerializeField, Range(0f, 1f)] float _attackChance = 0.5f;
    [SerializeField] Vector2 _skillCooldownRange = new(4.5f, 7.5f);
    [SerializeField, Range(0f, 1f)] float _skillChance = 0.3f;

    [Header("Animation Params")]
    [SerializeField] string _runBool = "IsRun";
    [SerializeField] string _attackTrigger = "Attack";

    [Header("Facing & Flip")]
    [SerializeField] bool _faceRightDefault = true;
    [SerializeField] bool _flipByScaleX = true;

    [Header("Difficulty")]
    [SerializeField] DifficultyProfile _defaultProfile;


    // --- ���� ���� ---
    DifficultyProfile _profile;
    float _nextPatrolSwitchAt;
    float _nextAttackReadyAt;
    float _nextSkillReadyAt;

    int _moveDir;       // -1, 0, +1
    bool _isActing;     // ����/��ų ��
    float _reactionDelay; // ���̵��� ���� ����

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!anim) anim = GetComponent<Animator>();
        if (!_objectStatus) _objectStatus = GetComponent<ObjectStatus>();
    }

    void OnEnable()
    {
        ApplyDifficulty(_defaultProfile);    // ������ ����
        _moveDir = _faceRightDefault ? 1 : -1;
        SchedulePatrolSwitch();
        ScheduleAttack();
        ScheduleSkill();
    }

    public void ApplyDifficulty(DifficultyProfile p)
    {
        if (p == null) return;
        _profile = p;

        // �̵�/����
        _moveSpeed = p.moveSpeed;
        _patrolChangeInterval = p.patrolChangeInterval;
        _patrolChangeJitter = p.patrolChangeJitter;

        // ���� ��
        _attackCooldownRange = p.attackCooldownRange;
        _attackChance = p.attackChance;
        _skillCooldownRange = p.skillCooldownRange;
        _skillChance = p.skillChance;

        _reactionDelay = p.reactionDelay;
    }

    void Update()
    {
        if (_isActing || _objectStatus.ReturnFrozenStatus()==true)
        {
            SetRun(false);
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // ��/�������� ����
        bool hitWall = Physics2D.OverlapCircle(transform.position, 2f, _wallMask);
        if (_profile.tier >= DifficultyTier.Hard)
        {
            bool DetectArrow = Physics2D.OverlapCircle(transform.position, 6, _arrowMask);
            if (hitWall ||DetectArrow)
            {
                Flip();
                SchedulePatrolSwitch(true);
            }
        }
        else
        {
            if (hitWall)
            {
                Flip();
                SchedulePatrolSwitch(true);
            }
        }

        // ��Ʈ�� ����/���� ���
        if (Time.time >= _nextPatrolSwitchAt)
        {
            // 0(����) Ȯ�� ����, ��/�� ����
            int r = Random.Range(0, 100);
            if (r < 15) _moveDir = 0;
            else _moveDir = (Random.value < 0.5f) ? -1 : 1;

            SchedulePatrolSwitch();
        }

        // �̵�
        rb.linearVelocity = new Vector2(_moveDir * _moveSpeed, rb.linearVelocity.y);
        SetRun(_moveDir != 0);

        // ����/��ų �õ�
        TryAttackOrSkill();
    }

    void TryAttackOrSkill()
    {
        if (!_objectStatus.ReturnFrozenStatus())
        {
            if (Time.time >= _nextSkillReadyAt && Random.value < _skillChance)
            {
                StartCoroutine(ActionWithDelay(() => StartSkill(), _reactionDelay));
                ScheduleSkill();
                return;
            }

            if (Time.time >= _nextAttackReadyAt && Random.value < _attackChance)
            {
                StartCoroutine(ActionWithDelay(() => StartAttack(), _reactionDelay));
                ScheduleAttack();
            }
        }
    }

    System.Collections.IEnumerator ActionWithDelay(System.Action act, float delay)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);
        act?.Invoke();
    }

    // === Actions ===
    void StartAttack()
    {
        _isActing = true;
        transform.localScale = new Vector3(-1, 1, 1);
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        if (!string.IsNullOrEmpty(_attackTrigger)) anim.SetTrigger(_attackTrigger);
        // ���� Ÿ���� �ִϸ��̼� �̺�Ʈ AE_AttackHit()���� ó��
    }

    void StartSkill()
    {
        _isActing = true;
        transform.localScale = new Vector3(-1, 1, 1);
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        int randomSkill = Random.Range(0, _defaultProfile.SkillAttackDatas.Length);
        string randomSkillAnim = _defaultProfile.SkillAttackDatas[randomSkill].AnimationTriggerName;
        if (!string.IsNullOrEmpty(randomSkillAnim)) anim.SetTrigger(randomSkillAnim);
        // ���� ĳ��Ʈ�� AE_SkillCast()���� ó��
    }

    // === Animation Events ===
    // �ִϸ��̼� Ŭ������ �̺�Ʈ�� ȣ������
    public void AE_ActionEnd() { _isActing = false; }



    // === Helpers ===
    void SetRun(bool run)
    {
        if (!string.IsNullOrEmpty(_runBool))
            anim.SetBool(_runBool, run);
    }

    void Flip()
    {
        _moveDir = -_moveDir;
        if (_flipByScaleX)
        {
            var s = transform.localScale;
            transform.localScale = new Vector3(Mathf.Abs(s.x) * (_moveDir >= 0 ? 1f : -1f), s.y, s.z);
        }
        // SpriteRenderer.flipX�� ���� ������ ���⿡ ���
    }

    void SchedulePatrolSwitch(bool afterFlip = false)
    {
        float baseT = _patrolChangeInterval;
        float jitter = Random.Range(_patrolChangeJitter.x, _patrolChangeJitter.y);
        _nextPatrolSwitchAt = Time.time + baseT + jitter * (afterFlip ? 0.5f : 1f);
    }

    void ScheduleAttack()
    {
        _nextAttackReadyAt = Time.time + Random.Range(_attackCooldownRange.x, _attackCooldownRange.y);
    }

    void ScheduleSkill()
    {
        _nextSkillReadyAt = Time.time + Random.Range(_skillCooldownRange.x, _skillCooldownRange.y);
    }

}
