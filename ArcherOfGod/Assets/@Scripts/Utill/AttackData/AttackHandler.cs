
using Unity.Cinemachine;
using UnityEngine;

public class AttackHandler : MonoBehaviour
{
    [SerializeField] private GameObject _target;
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    public void CreateRangeAttack(AttackData attackData) // AnimationEvent
    {
        if (attackData.ArrowPrefabName == "")
        {
            Debug.LogWarning("투사체 프리팹 이름 설정을 안했습니다!");
            return;
        }
        if (attackData.AttackSFX != null)
            SFXManager.s_Instance.PlayAttackSFX(attackData.AttackSFX);
        if (attackData.EffectPrefabName != "")
        {
            var skillEffect = ObjectPoolManager.Instance.GetObject(attackData.EffectPrefabName);
            skillEffect.transform.position = transform.position+ attackData.EffectPos;
            if (transform.localScale.x == -1)
            {
                skillEffect.transform.localScale = new Vector3(-1, 1, 1);
            }
        }
        if (attackData.UseCameraShake == true)
        {
            _impulseSource.GenerateImpulse(attackData.ShakeDir);
        }
        if (attackData.CanMove == false)
        {
            GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, GetComponent<Rigidbody2D>().linearVelocity.y);
            GetComponent<Rigidbody2D>().gravityScale = 1;
        }
        var rangeAttackObject = ArrowObjectManager.Instance.GetObject(attackData.ArrowPrefabName);
        rangeAttackObject.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        rangeAttackObject.GetComponent<ArrowSet>().SetUpTarget(attackData);
        if (rangeAttackObject.GetComponentInChildren<ParticleSystem>() != null)
        {
            var particle = rangeAttackObject.GetComponentInChildren<ParticleSystem>();
            particle.Play();
        }
        if (rangeAttackObject.GetComponentInChildren<TrailRenderer>(true) != null)
        {
            var trail = rangeAttackObject.GetComponentInChildren<TrailRenderer>();
            trail.emitting = true;
        }
        float moveVelocity = attackData.ArrowVelocity;
        if (attackData.UseBezierCurve == false)
        {
            rangeAttackObject.GetComponent<ArrowController>().enabled = false;
            if (transform.localScale.x == -1)
            {
                rangeAttackObject.transform.position = transform.position - attackData.ArrowSpawnPos;
                rangeAttackObject.transform.localScale = new Vector3(-1,1,1);
                
                moveVelocity *= -1;
            }
            else
            {
                rangeAttackObject.transform.position = transform.position + attackData.ArrowSpawnPos;
            }
            
            rangeAttackObject.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(1,0)* moveVelocity;
            Debug.Log(rangeAttackObject.GetComponent<Rigidbody2D>().linearVelocity);
        }
        else
        {
            if (transform.localScale.x == -1)
            {
                rangeAttackObject.transform.position = transform.position - attackData.ArrowSpawnPos;
                rangeAttackObject.transform.eulerAngles = new Vector3(0, 0, 150);
            }
            else 
            {
                rangeAttackObject.transform.position = transform.position + attackData.ArrowSpawnPos;
                
            }
            rangeAttackObject.GetComponent<ArrowController>().enabled = true;
            var arrow = rangeAttackObject.GetComponent<ArrowController>();
            arrow.spriteForwardIsUp = false;
            arrow.speed = attackData.ArrowVelocity;
            

            arrow.LaunchAlongHeight(rangeAttackObject.transform.position, _target,attackData.SetCurvePos.x,attackData.SetCurvePos.y);
        }
        
    }
}
