using System.Collections;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.UI;

public class ObjectStatus : DamageAbleBase, IDamageAble
{
    [SerializeField] private LayerMask _objLayerMask;
    [SerializeField] private UnityEngine.UI.Image _filledImage;
    [SerializeField]private float MaxHp;
    [SerializeField] private Transform _damageTextPos;
    [SerializeField] private Animator _animator;
    public float CurHp;
    private WaitForSeconds _damageInterval = new WaitForSeconds(1.2f);
    private bool _frozen = false;
    private bool _poisoning = false;
    public void Start()
    {
        _animator = GetComponent<Animator>();
        CurHp = MaxHp;
    }
    public void FilledHpImage()
    {
        _filledImage.fillAmount = CurHp / MaxHp;
    }
    public void Update()
    {
        FilledHpImage();
    }
    public override void OnDamage(float damage, SpecialAbility skillAbility)
    {
        HitLogic(skillAbility, damage);
    }
    protected virtual void HitLogic(SpecialAbility ability, float causerAtk)
    {
        float finalDmg;
        finalDmg = causerAtk;
        CurHp -= finalDmg;
        var hudText = ObjectPoolManager.Instance.GetObject("DamageText");
        hudText.GetComponent<DamageText>().Show(finalDmg, _damageTextPos.position);
        if (CurHp <= 0)
        {
            DamageAble = false;
            _animator.SetTrigger("Die");
            StopAllCoroutines();
            gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
    public LayerMask SetObjectLayerMask()
    {
        return _objLayerMask;
    }
    public IEnumerator FrozenAbility(GameObject arrow, Animator animator, SpriteRenderer sprite)
    {
        _frozen = true;
        animator.speed = 0;
        sprite.color = Color.blue;
        var arrowSet = arrow.GetComponent<ArrowSet>();
        yield return new WaitForSeconds(arrowSet.FrozenTime);
        animator.speed = 1;
        sprite.color = Color.white;
        _frozen = false;
    }
    public bool ReturnFrozenStatus()
    {
        return _frozen;
    }
    public IEnumerator BurningAbilityDamage(GameObject arrow)
    {
        int curCount = 0;
        var arrowSet = arrow.GetComponent<ArrowSet>();
        int count = arrowSet.BurningTickCount;
        float damage = arrowSet.BurningTickDamage;

        while (curCount < count)
        {
            yield return _damageInterval;
            OnDamage(damage, arrowSet.SpecialAbility);
            curCount++;
        }
    }
    public IEnumerator PoisonAbility(GameObject arrow)
    {
        int curCount = 0;
        var arrowSet = arrow.GetComponent<ArrowSet>();
        int count = arrowSet.PoisonTickCount;
        float damage = arrowSet.PoisonTickDamage;
        _poisoning = true;
        while (curCount < count)
        {
            yield return _damageInterval;
            OnDamage(damage, arrowSet.SpecialAbility);
            curCount++;
        }
        _poisoning = false;
    }
    public bool ReturnPoisonStatus()
    {
        return _poisoning;
    }
}
