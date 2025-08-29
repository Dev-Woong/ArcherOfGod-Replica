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
                Debug.LogWarning($"SkillSlot�� ��ư {btn.name}�� ����");
                continue;
            }

            // ��ư Ŭ�� -> ���� ó�� (��üũ/������/ī��Ʈ�ٿ�)
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(slot.OnClickSkillButton);

            // ������ ���ȴٴ� �̺�Ʈ�� ��Ʈ�ѷ��� ���� -> �ִϸ��̼� Ʈ���� ���
            slot.OnPressed += OnSkillSlotPressed;
        }
    }

    private void OnSkillSlotPressed(SkillSlot slot)
    {
        // ���Կ��� �ٷ� Ʈ���Ÿ� �ޱ�
        var skillSlotData = slot;
        if (string.IsNullOrEmpty(skillSlotData.TriggerName))
        {
            Debug.LogWarning("Skill TriggerName�� �������");
            return;
        }
        if (skillSlotData.Jump == true)
        {
            _animator.SetTrigger("Jump");
        }
        _animator.SetTrigger(skillSlotData.TriggerName);
        _animator.SetBool("isRun", false);
        _playerMovement.Moveable = false;
        // ���� ������ �ִϸ��̼� �̺�Ʈ���� ó�� (�� ��ȹ���)   
    }
}
