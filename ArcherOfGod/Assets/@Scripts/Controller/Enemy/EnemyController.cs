using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Playables;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private LayerMask _objLayerMask;
    [SerializeField] private Animator _animator;
    [SerializeField] private ObjectStatus _objectStatus;
    
    
   
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _objectStatus = GetComponent<ObjectStatus>();

    }
    private void Start()
    {
        _objLayerMask = _objectStatus.SetObjectLayerMask();
    }
    private void Update()
    {
        if (_objectStatus._curHp <= 0)
        {
            _animator.SetTrigger("Die");
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
                _objectStatus.OnDamage(arrowSet.Damage, arrowSet.SpecialAbility);
                if (arrowSet.HitEffectName != "")
                {
                    var hitEffect = ObjectPoolManager.instance.GetObject(arrowSet.HitEffectName);
                    hitEffect.transform.position = arrowSet.transform.position + arrowSet.HitEffectPos;
                    hitEffect.transform.localScale = arrowSet.transform.localScale;
                }
                switch (arrowSet.SpecialAbility)
                {
                    case SpecialAbility.FIREBURN:
                        StartCoroutine(_objectStatus.BurningAbilityDamage(arrow));
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
