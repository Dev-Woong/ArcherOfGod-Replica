using TMPro;
using UnityEngine;

public class DamageText : PoolAble
{
    [Header("Motion")]
    [SerializeField] float moveSpeed = 1.5f;     // ���� �������� �ӵ� (����/��)
    [SerializeField] float gravity = 0f;         // ��¦ �������� �ϰ� ������ > 0
    [SerializeField] Vector2 spawnJitter = new(0.1f, 0.05f); // ���� ���� ������

    [Header("Visual")]
    [SerializeField] float lifeTime = 1.0f;      // ǥ�� �ð�(��)
    [SerializeField] float maxFontSize = 15f;    // ���� ��Ʈ ũ��
    [SerializeField] float minFontSize = 10f;    // �� ��Ʈ ũ��
    [SerializeField] float alphaFadeSpeed = 3f;  // ���� ���� �ӵ� ���

    [Header("Colors")]
    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color missColor = new(1f, 1f, 1f, 0.8f);
    [SerializeField] Color critColor = new(1f, 0.3f, 0.2f, 1f);

    // --- ���� ���� ---
    TMP_Text _tmp;
    Vector3 _velocity;       // ȭ�� ���� �̵� �ӵ�
    float _timeLeft;         // ���� �ð�
    float _startFontSize;    // ������
    Color _baseColor;        // ������

    // �ܺο��� �ʿ��ϸ� ���� (��: ���� �� ����)
    public float Damage;

    void Awake()
    {
        _tmp = GetComponent<TMP_Text>();
        _startFontSize = maxFontSize; // �⺻�� ����
        _baseColor = normalColor;
    }

    void OnEnable()
    {
        // �⺻ ���� (���� ���� Setup ȣ�� ����)
        InternalResetVisuals();
    }

    void Update()
    {
        if (_timeLeft <= 0f) return;

        float dt = Time.deltaTime;
        _timeLeft -= dt;

        // �̵� (���� �������ٰ�, ���ϸ� gravity�� ����/����)
        _velocity.y -= gravity * dt;
        transform.position += _velocity * dt;

        // ��Ʈ ũ�� Lerp
        float lifeRatio = Mathf.Clamp01(1f - (_timeLeft / lifeTime)); // 0��1
        float size = Mathf.Lerp(maxFontSize, minFontSize, lifeRatio);
        _tmp.fontSize = size;

        // ���� ����
        Color c = _tmp.color;
        c.a = Mathf.Clamp01(c.a - alphaFadeSpeed * dt);
        _tmp.color = c;

        if (_timeLeft <= 0f || c.a <= 0.01f)
        {
            // Ǯ�� ��ȯ
            ReleaseObject();
        }
    }
    public void Show(float damage, Vector3 worldPos, bool isCritical = false)
    {
        Damage = damage;

        // ��ġ + �ణ�� ���� ������
        Vector3 jitter = new(
            Random.Range(-spawnJitter.x, spawnJitter.x),
            Random.Range(0f, spawnJitter.y),
            0f
        );
        transform.position = worldPos + jitter;

        // �ؽ�Ʈ/����
        if (damage <= 0f)
        {
            _tmp.text = "Miss";
            _tmp.color = missColor;
        }
        else
        {
            _tmp.text = isCritical ? Mathf.RoundToInt(damage).ToString() + "!"
                                   : Mathf.RoundToInt(damage).ToString();
            _tmp.color = isCritical ? critColor : normalColor;
        }

        // �ӵ�/Ÿ�̸� ����
        _velocity = Vector3.up * moveSpeed;
        _timeLeft = lifeTime;
        _tmp.fontSize = maxFontSize;
    }

    // ���� �ʱ�ȭ (OnEnable �� ȣ��)
    void InternalResetVisuals()
    {
        _timeLeft = 0f;                // Update�� �ٷ� Release ���ϵ���
        _velocity = Vector3.up * moveSpeed;
        _tmp.color = normalColor;      // �⺻��
        _tmp.fontSize = maxFontSize;   // �⺻ũ��
        _tmp.text = string.Empty;
    }
}