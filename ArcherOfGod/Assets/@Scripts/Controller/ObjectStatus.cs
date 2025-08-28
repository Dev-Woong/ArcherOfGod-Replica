using System.Collections;
using UnityEngine;

public class ObjectStatus : DamageAbleBase, IDamageAble
{
    [SerializeField] private LayerMask _objLayerMask;
    public float _curHp;
    [SerializeField] private Transform _damageTextPos;
    private WaitForSeconds _damageInterval = new WaitForSeconds(1.2f);
    private bool _frozen = false;
    void Start()
    {
    }
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
        hudText.GetComponent<DamageText>().Show(finalDmg, _damageTextPos.position);
        if (_curHp <= 0)
        {
            DamageAble = false;
            gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            
        }
    }
    public LayerMask SetObjectLayerMask()
    {
        return _objLayerMask;
    }
    //IEnumerator FrozenAbility(SpecialAbility specialAbility)
    //{

    //}
    public IEnumerator BurningAbilityDamage(GameObject arrow)
    {
        int curCount = 0;
        var arrowSet = arrow.GetComponent<ArrowSet>();
        int count = arrowSet.TickCount;
        float damage = arrowSet.TickDamage;
        
        while (curCount <count)
        {
            yield return _damageInterval;
            OnDamage(damage, arrowSet.SpecialAbility);
            curCount++;
        }
    }
}
