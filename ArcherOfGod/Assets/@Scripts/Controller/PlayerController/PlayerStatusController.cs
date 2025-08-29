using UnityEngine;

public class PlayerStatusController : MonoBehaviour
{
    [SerializeField] private LayerMask _objLayerMask;
    [SerializeField] private BoxCollider2D _boxCollider;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;
    [SerializeField] private ObjectStatus _objectStatus;
    [SerializeField] private GameObject _LimitsSightObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
        _objectStatus = GetComponent<ObjectStatus>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _LimitsSightObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        PoisoningStatus();
    }
    void PoisoningStatus()
    {
        _LimitsSightObject.SetActive(_objectStatus.ReturnPoisonStatus());
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
                    if (arrowSet.SpecialAbility == SpecialAbility.NONE || arrowSet.SpecialAbility == SpecialAbility.FROZEN)
                    {
                        var hitEffect = ObjectPoolManager.Instance.GetObject(arrowSet.HitEffectName);
                        hitEffect.transform.position = arrowSet.transform.position + arrowSet.HitEffectPos;
                        hitEffect.transform.localScale = arrowSet.transform.localScale;
                    }
                    else
                    {

                        var hitEffect = ObjectPoolManager.Instance.GetObject(arrowSet.HitEffectName);
                        hitEffect.transform.position = new Vector2(transform.position.x + arrowSet.HitEffectPos.x, _boxCollider.bounds.min.y + arrowSet.HitEffectPos.y);
                        hitEffect.transform.localScale = arrowSet.transform.localScale;
                    }
                }
                
                switch (arrowSet.SpecialAbility)
                {
                    case SpecialAbility.NONE:
                        break;

                    case SpecialAbility.FIREBURN:
                        StartCoroutine(_objectStatus.BurningAbilityDamage(arrow));
                        break;

                    case SpecialAbility.FROZEN:
                        StartCoroutine(_objectStatus.FrozenAbility(arrow, _animator, _spriteRenderer));
                        break;

                    case SpecialAbility.POISON:
                        StartCoroutine(_objectStatus.PoisonAbility(arrow));
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
