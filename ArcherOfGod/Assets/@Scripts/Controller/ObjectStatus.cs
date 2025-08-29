using System.Collections;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.UI;

public class ObjectStatus : DamageAbleBase, IDamageAble
{
    [SerializeField] private LayerMask _objLayerMask;
    [SerializeField] private UnityEngine.UI.Image _filledImage;
    [SerializeField] private TMPro.TMP_Text _hpText;
    [SerializeField] private float MaxHp;
    [SerializeField] private Transform _damageTextPos;
    [SerializeField] private Animator _animator;
    [SerializeField] private Image _frozenIcon;
    [SerializeField] private Image _burningIcon;
    [SerializeField] private Image _poisoningIcon;
    [SerializeField] private Image _electricIcon;
    public float CurHp;
    private WaitForSeconds _damageInterval = new WaitForSeconds(1.2f);
    private bool _frozen = false;
    private bool _burning = false;
    private bool _poisoning = false;
    private bool _electric = false;
    private float _electricDebuffSpeed;
    void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    public void Start()
    {
        CurHp = MaxHp;

    }
    public void StatusICon()
    {
        _frozenIcon.enabled = _frozen;
        _poisoningIcon.enabled = _poisoning;
        _burningIcon.enabled = _burning;
        _electricIcon.enabled = _electric;
    }
    public void FilledHpImage()
    {
        _filledImage.fillAmount = CurHp / MaxHp;
        if (CurHp > 0)
        {
            _hpText.text = $"{CurHp} / {MaxHp}";
        }
        else
        {
            _hpText.text = $"0 / {MaxHp}";
        }
    }
    public void Update()
    {
        FilledHpImage();
        StatusICon();
        _animator.SetBool("EnterBattle", GameManager.Instance.GameStart);
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
            if (_objLayerMask == LayerMask.GetMask("Enemy"))
            {
                GameManager.Instance.BattleFinish(true);
            }
            if (_objLayerMask == LayerMask.GetMask("Player"))
            {
                GameManager.Instance.BattleFinish(false);
            }
            _animator.SetTrigger("Die");
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
        sprite.color = new Color(0,0,1,sprite.color.a);
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
        _burning = true;
        while (curCount < count)
        {
            if (CurHp <= 0) yield break;
            yield return _damageInterval;
            OnDamage(damage, arrowSet.SpecialAbility);
            curCount++;
        }
        _burning = false;
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
            if (CurHp <= 0) yield break;
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
    public IEnumerator ElectricAbility(GameObject arrow)
    {
        int curCount = 0;
        var arrowSet = arrow.GetComponent<ArrowSet>();
        int count = arrowSet.ElectricTickCount;
        float damage = arrowSet.ElectricTickDamage;
        _electricDebuffSpeed = arrowSet.ElectricDebuffSpeed;
        _electric = true;
        while (curCount < count)
        {
            if (CurHp <= 0) yield break;
            yield return _damageInterval;
            OnDamage(damage, arrowSet.SpecialAbility);
            curCount++;
        }
        _electric = false;
    }
    public float ReturnElectricDebuffSpeed()
    {
        return _electricDebuffSpeed;
    }
    public bool ReturnElectricStatus()
    {
        return _electric;
    }
}
