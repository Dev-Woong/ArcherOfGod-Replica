using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Playables;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;

public class EnemyController : DamageAbleBase, IDamageAble
{
    [SerializeField] private float _curHp;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _damageTextPos;
    [SerializeField] private GameObject _damageText;
    [SerializeField] private ObjectStatus _objectStatus;
    [SerializeField] private LayerMask _objLayerMask;
    private WaitForSeconds _damageInterval = new WaitForSeconds(1f);
    
    public override void OnDamage(float damage, SpecialAbility skillAbility)
    {
        HitLogic(skillAbility, damage);
    }
    protected virtual void HitLogic(SpecialAbility ability, float causerAtk)
    {
        float finalDmg;
        finalDmg = causerAtk;
        _curHp -= finalDmg;
        var hudText = ObjectPoolManager.instance.GetObject("DamageText");
        hudText.GetComponent<DamageText>().Show(finalDmg,_damageTextPos.position);
        
        if (_curHp <= 0)
        {
            DamageAble = false;
            gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            _animator.SetTrigger("Die");
        }
    }
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _objectStatus = GetComponent<ObjectStatus>();
    }
    void Start()
    {
        _objLayerMask = _objectStatus.SetObjectLayerMask();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator BurningAbilityDamage(SpecialAbility ability, int tickCount, float tickDamage)
    {
        int curCount = 0;
        while (curCount < tickCount)
        {
            yield return _damageInterval;
            OnDamage(tickDamage,ability);
            curCount++;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            var arrow = collision.gameObject;
            var arrowSet = arrow.GetComponent<ArrowSet>();
            if (arrowSet.TargetMask == _objLayerMask)
            {
                OnDamage(arrowSet.Damage, arrowSet.SpecialAbility);
                switch (arrowSet.SpecialAbility)
                {
                    case SpecialAbility.FIREBURN:
                        var hitEffect = ObjectPoolManager.instance.GetObject(arrowSet.HitEffectName);
                        hitEffect.transform.position = arrow.transform.position + arrowSet.HitEffectPos;
                        hitEffect.transform.localScale = arrow.transform.localScale;
                        
                        StartCoroutine(BurningAbilityDamage(arrowSet.SpecialAbility, arrowSet.TickCount, arrowSet.TickDamage));
                        break;
                }
                if (collision.GetComponentInChildren<ParticleSystem>() != null)
                {
                    var particle = collision.GetComponentInChildren<ParticleSystem>();
                    particle.Stop();
                }
                arrow.GetComponent<ArrowController>().Hittarget = true;
                arrow.transform.rotation = Quaternion.Euler(0, 0, 0);
                arrow.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
                var trail = arrow.GetComponentInChildren<TrailRenderer>();
                trail.emitting = false;
                trail.Clear();
                arrow.GetComponent<PoolAble>().ReleaseObject();
            }
        }
    }
}
