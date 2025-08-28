using Mono.Cecil.Cil;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.VFX;

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
        if (attackData.UseCameraShake == true)
        {
            _impulseSource.GenerateImpulse(attackData.ShakeDir);
        }
        var rangeAttackObject = ArrowObjectManager.instance.GetObject(attackData.ArrowPrefabName);
        rangeAttackObject.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        rangeAttackObject.GetComponent<ArrowSet>().SetUpTarget(attackData);
        if (rangeAttackObject.GetComponentInChildren<ParticleSystem>() != null)
        {
            var particle = rangeAttackObject.GetComponentInChildren<ParticleSystem>();
            particle.Play();
        }
        
        var trail = rangeAttackObject.GetComponentInChildren<TrailRenderer>(true);
        trail.emitting = true;
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
            rangeAttackObject.GetComponent<ArrowController>().enabled = true;
            var arrow = rangeAttackObject.GetComponent<ArrowController>();
            arrow.spriteForwardIsUp = false;
            arrow.speed = attackData.ArrowVelocity;
            Vector2 startPoint = transform.position+ attackData.ArrowSpawnPos;

            arrow.LaunchAlongHeight(startPoint,_target,attackData.SetCurvePos.x,attackData.SetCurvePos.y);
        }
        
    }
}
