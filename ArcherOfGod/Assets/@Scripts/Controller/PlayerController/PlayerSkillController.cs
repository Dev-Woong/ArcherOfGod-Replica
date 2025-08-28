using Mono.Cecil.Cil;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
public class PlayerSkillController : MonoBehaviour
{
    [SerializeField] Rigidbody2D _rigidbody2D;
    [SerializeField] Animator _animator;
    [SerializeField] Button[] _skillButtons;
    public bool Moveable = true;
    private void Awake()
    {
        if (!_animator) _animator = GetComponent<Animator>();
        if (!_rigidbody2D) _rigidbody2D = GetComponent<Rigidbody2D>();
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
        var trigger = slot.TriggerName;
        if (string.IsNullOrEmpty(trigger))
        {
            Debug.LogWarning("Skill TriggerName�� �������");
            return;
        }
        _animator.SetTrigger(trigger);
        _animator.SetBool("isRun", false);
        Moveable = false;
        // ���� ������ �ִϸ��̼� �̺�Ʈ���� ó�� (�� ��ȹ���)
    }
    public void CanMove() // AnimationEvent
    {
        Moveable = true;
    }
}
