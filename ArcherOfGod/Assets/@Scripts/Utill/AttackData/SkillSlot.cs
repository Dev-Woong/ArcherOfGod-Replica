using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour
{
    
    [SerializeField] private Image _skillIcon;
    [SerializeField] private Image _onCoolTimeImage;
    [SerializeField] private TMP_Text _coolTimeText;
    public AttackData AttackData;
    [SerializeField] private float _coolTime;
    [SerializeField] private Color32 _coolColor;
    [SerializeField] private Color32 _retrunColor;
    private bool _canUseSkill = true;
    public event Action<SkillSlot> OnPressed;
    public string TriggerName => AttackData?.AnimationTriggerName;
    public bool IsCooling => !_canUseSkill;
    public float Cooldown => AttackData ? AttackData.CoolTime : 0f;
    void Start()
    {   
        if (AttackData.SkillIcon != null)
        {
            _skillIcon.sprite = AttackData.SkillIcon;
        }
        _coolColor = new Color32(61, 61, 61, 255);
        _retrunColor = new Color32(255, 255, 255, 255);
        _coolTimeText.text = "";
        _coolTime = 0;
        _canUseSkill = true;
        _onCoolTimeImage.enabled = false;
    }

    private void CoolDownSlot()
    {
        if (_canUseSkill == false)
        {
            _coolTime -= Time.deltaTime;
            if (_coolTime > 0)
            {
                _onCoolTimeImage.enabled = true;
                _coolTimeText.enabled = true;
                _coolTimeText.text = $"{(int)_coolTime}";
            }
            else
            {
                _onCoolTimeImage.enabled = false;
                _coolTimeText.enabled = false;
                _coolTimeText.text = "";
                _skillIcon.color = _retrunColor;
                _canUseSkill = true;
            }
        }
    }
    public void OnClickSkillButton()
    {
        if (_canUseSkill == false || AttackData == null)
        {
            return;
        }
        _canUseSkill = false;
        _coolTime = AttackData.CoolTime;
        _skillIcon.color = _coolColor;
        OnPressed?.Invoke(this);
    }
    private void Update()
    {
        CoolDownSlot();
    }
}
