using Mono.Cecil.Cil;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
public class PlayerSkillController : MonoBehaviour
{
    [SerializeField] Rigidbody2D _rigidbody2D;
    [SerializeField] Animator _animator;
    [SerializeField] Button[] _skillButtons;
    [SerializeField] PlayerMovement _playerMovement;
    [SerializeField] private ObjectStatus _objectStatus;
    private void Awake()
    {
        if (!_animator) _animator = GetComponent<Animator>();
        if (!_rigidbody2D) _rigidbody2D = GetComponent<Rigidbody2D>();
        if (!_playerMovement) _playerMovement = GetComponent<PlayerMovement>();
        _objectStatus = GetComponent<ObjectStatus>();
    }
    void Start()
    {
        for (int i = 0; i < _skillButtons.Length; i++)
        {
            var btn = _skillButtons[i];
            if (!btn) continue;

            var slot = btn.GetComponent<SkillSlot>();
            if (!slot)
            {
                Debug.LogWarning($"SkillSlot이 버튼 {btn.name}에 없음");
                continue;
            }

            // 버튼 클릭 -> 슬롯 처리 (쿨체크/아이콘/카운트다운)
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(slot.OnClickSkillButton);

            // 슬롯이 눌렸다는 이벤트를 컨트롤러가 구독 -> 애니메이션 트리거 재생
            slot.OnPressed += OnSkillSlotPressed;
        }
    }

    private void OnSkillSlotPressed(SkillSlot slot)
    {
        // 슬롯에서 바로 트리거명 받기
        var skillSlotData = slot;
        if (string.IsNullOrEmpty(skillSlotData.TriggerName))
        {
            Debug.LogWarning("Skill TriggerName이 비어있음");
            return;
        }
        if (skillSlotData.Jump == true)
        {
            _animator.SetTrigger("Jump");
        }
        _animator.SetTrigger(skillSlotData.TriggerName);
        _animator.SetBool("isRun", false);
        _playerMovement.Moveable = false;
        // 공격 로직은 애니메이션 이벤트에서 처리 (네 계획대로)   
    }
}
