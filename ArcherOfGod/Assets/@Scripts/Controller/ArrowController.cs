using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public enum Order { Quadratic2 = 2, Cubic3 = 3 }
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float cacheBeforeTargetDistance = 0.25f; // 타겟(p2/p3)까지 남은 거리 기준
    [SerializeField] private float cacheAtLeastTRatio = 0.85f;        // 너무 짧은 사거리 대비 t 하한
    public bool Hittarget = true;
    [SerializeField] private float continueSpeed = 12f;
    private bool dirCached = false;
    private Vector2 cachedDir;   // '도달 직전'에 저장한 진행 방향
    [Header("Common")]
    public bool useWorldUp = true;           // 사이드뷰: 월드 위로 볼록 권장
    public bool spriteForwardIsUp = false;   // 스프라이트 앞이 ↑ 라면 체크 (→면 해제)
    public float turnSpeedDegPerSec = 720f;  // 회전 부드럽게

    [Header("Auto Height (Quadratic)")]
    public float heightFactor = 0.18f;       // 거리 계수
    public float speedFactor = 0.02f;       // 속도 계수
    public float minHeight = 0.4f;
    public float maxHeight = 5.0f;

    [Header("Motion")]
    public float speed = 12f;        // 유닛/초
    public bool durationAuto = true; // true: 거리/속도 → 자동 시간
    public float duration = 0.6f;    // false일 때 고정 시간
    [Header("Debug")]
    public bool drawGizmos = true;
    public Color gizmoColor = new(1f, 0.7f, 0.2f, 1f);

    // 내부 상태
    Order order = Order.Quadratic2;
    Vector2 p0, p1, p2, p3; // p3는 Cubic에서만 사용
    float totalLen, traveled, runDuration;
    bool running;

    const int LUT_N = 50;
    readonly List<float> cumLen = new(LUT_N + 1);

    // -------------------- Public API --------------------

    /// (기본) 자동 높이: 거리/속도 기반 (Quadratic)
    public void LaunchAuto(Vector2 start, Vector2 target)
    {
        Hittarget = false;
        order = Order.Quadratic2;
        p0 = start; p2 = target;

        Vector2 d = p2 - p0;
        float dist = d.magnitude;
        float h = Mathf.Clamp(heightFactor * dist + speedFactor * Mathf.Max(0.001f, speed),
                              minHeight, maxHeight);
        Vector2 up = useWorldUp ? Vector2.up : Perp(d.normalized);
        Vector2 mid = (p0 + p2) * 0.5f;
        p1 = mid + up * h;

        SetupRun(dist);
    }

    /// 꺾임 위치를 직접: 직선 비율(along 0~1) + 높이(height) (Quadratic)
    public void LaunchAlongHeight(Vector2 start, GameObject target, float along, float height)
    {
        Hittarget = false;
        dirCached = false;
        cachedDir = Vector2.zero;
        order = Order.Quadratic2;
        p0 = start;
        p2 = target.GetComponent<BoxCollider2D>().bounds.center;

        
        along = Mathf.Clamp01(along);    // 0=시작 근처, 0.5=중간, 1=끝 근처
        Vector2 d = p2 - p0;
        Vector2 basePoint = p0 + d * along;
        Vector2 up = useWorldUp ? Vector2.up : Perp(d.normalized);
        p1 = basePoint + up * height;

        SetupRun(d.magnitude);
    }

    /// 컨트롤 포인트를 월드 좌표로 직접 지정 (Quadratic)
    public void LaunchWithControlPoint(Vector2 start, Vector2 control, Vector2 target)
    {
        order = Order.Quadratic2;
        p0 = start; p1 = control; p2 = target;
        SetupRun((p2 - p0).magnitude);
    }

    /// Cubic(3차) : 컨트롤 포인트 2개 직접 지정 (더 자유로운 곡선)
    public void LaunchCubic(Vector2 start, Vector2 control1, Vector2 control2, Vector2 target)
    {
        order = Order.Cubic3;
        p0 = start; p1 = control1; p2 = control2; p3 = target;
        SetupRun((p3 - p0).magnitude);
    }

    // -------------------- Core --------------------

    void SetupRun(float dist)
    {
        BuildLengthLUT();

        // 2) durationAuto면 '곡선 총 길이 / 속도'로 시간 계산 (진짜 등속)
        runDuration = durationAuto
            ? Mathf.Max(0.001f, totalLen) / Mathf.Max(0.001f, speed)
            : Mathf.Max(0.001f, duration);

        traveled = 0f;
        transform.position = p0;
        running = totalLen > 0f;
        enabled = true;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.gameObject.layer == 8)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            _rigidbody.linearVelocity = Vector3.zero;
            if (GetComponentInChildren<ParticleSystem>() != null)
            {
                var particle = GetComponentInChildren<ParticleSystem>();
                particle.Stop();
            }
            Debug.Log("벽과 충돌");
            GetComponent<PoolAble>().ReleaseObject();
        }
    }
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (!running) return;

        // 호 길이 기준 등속
        traveled += (totalLen / runDuration) * Time.deltaTime;

        // ===== 진행 중: 도달 '직전' 방향을 캐싱 =====
        {
            float s = Mathf.Clamp01(traveled / totalLen);
            float t = FindTByArcRatio(s);

            // 현재 위치/접선
            Vector2 pos = BezierPoint(t);
            Vector2 tan = BezierTangent(t).normalized;

            // '타겟 포인트' (Quadratic은 p2, Cubic은 p3)
            Vector2 targetPoint = (order == Order.Cubic3) ? p3 : p2;

            // 남은 거리
            float remain = Vector2.Distance(pos, targetPoint);

            // 1) 남은 거리가 cacheBeforeTargetDistance 이하일 때
            // 2) 또는 짧은 경로 보호용으로 t가 일정 이상(cacheAtLeastTRatio)일 때
            if (!dirCached && (remain <= cacheBeforeTargetDistance || t >= cacheAtLeastTRatio))
            {
                if (tan.sqrMagnitude > 1e-6f)
                {
                    dirCached = true;
                    cachedDir = tan; // ★ '도달 직전'의 진행 방향을 저장
                }
            }

            // 베지어 이동/회전 적용
            transform.position = pos;
            if (tan.sqrMagnitude > 1e-6f)
            {
                float deg = Mathf.Atan2(tan.y, tan.x) * Mathf.Rad2Deg;
                if (spriteForwardIsUp) deg -= 90f;
                Quaternion targetRot = Quaternion.Euler(0, 0, deg);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot,
                    turnSpeedDegPerSec * Time.deltaTime);
            }
        }

        // ===== 도착 처리 =====
        if (traveled >= totalLen)
        {
            // 베지어 단계 종료
            running = false;

            // 못 맞췄다면: '도달 직전'에 저장한 방향으로 계속 전진
            if (Hittarget == false)
            {
                var rb = GetComponent<Rigidbody2D>();
                Vector2 forward = dirCached ? cachedDir
                                            : ((order == Order.Cubic3 ? (p3 - p0) : (p2 - p0)).normalized);

                if (rb != null)
                {
                    rb.linearVelocity = forward * (continueSpeed > 0f ? continueSpeed : speed);

                    // 진행 방향으로 즉시 정렬(선택)
                    float deg = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
                    if (spriteForwardIsUp) deg -= 90f;
                    transform.rotation = Quaternion.Euler(0, 0, deg);
                }
                else
                {
                    // Rigidbody2D가 없다면 Transform 기반 이동을 위한 별도 플래그 분기 추가 가능
                    // e.g., afterMiss = true; 를 두고 Update에서 Translate(forward * continueSpeed * dt)
                }
            }
            else
            {
                // 맞췄거나 더 진행 안 하기로 했다면 최종점에서 정지
                transform.position = (order == Order.Cubic3) ? (Vector3)p3 : (Vector3)p2;
            }
            return;
        }
    }

    // -------------------- Bezier Utils --------------------

    Vector2 BezierPoint(float t)
    {
        if (order == Order.Quadratic2)
        {
            float u = 1f - t;
            return u * u * p0 + 2f * u * t * p1 + t * t * p2;
        }
        else // Cubic
        {
            float u = 1f - t;
            return u * u * u * p0 + 3f * u * u * t * p1 + 3f * u * t * t * p2 + t * t * t * p3;
        }
    }

    Vector2 BezierTangent(float t)
    {
        if (order == Order.Quadratic2)
        {
            // B'(t) = 2 * ((1-t)(p1-p0) + t(p2-p1))
            return 2f * ((1f - t) * (p1 - p0) + t * (p2 - p1));
        }
        else // Cubic
        {
            // B'(t) = 3[(1-t)^2(p1-p0) + 2(1-t)t(p2-p1) + t^2(p3-p2)]
            float u = 1f - t;
            return 3f * (u * u * (p1 - p0) + 2f * u * t * (p2 - p1) + t * t * (p3 - p2));
        }
    }

    static Vector2 Perp(Vector2 v) => new(-v.y, v.x);

    // -------------------- Arc-Length LUT --------------------

    void BuildLengthLUT()
    {
        cumLen.Clear();
        Vector2 prev = BezierPoint(0f);
        float len = 0f;
        cumLen.Add(0f);

        for (int i = 1; i <= LUT_N; i++)
        {
            float t = i / (float)LUT_N;
            Vector2 cur = BezierPoint(t);
            len += (cur - prev).magnitude;
            cumLen.Add(len);
            prev = cur;
        }
        totalLen = len;
    }

    float FindTByArcRatio(float s)
    {
        if (s <= 0f) return 0f;
        if (s >= 1f) return 1f;
        float targetLen = s * totalLen;

        int lo = 0, hi = LUT_N;
        while (lo < hi)
        {
            int mid = (lo + hi) >> 1;
            if (cumLen[mid] < targetLen) lo = mid + 1;
            else hi = mid;
        }
        int i = Mathf.Clamp(lo, 1, LUT_N);
        float l0 = cumLen[i - 1];
        float l1 = cumLen[i];
        float seg = Mathf.Max(1e-5f, l1 - l0);
        float alpha = (targetLen - l0) / seg;
        float t0 = (i - 1) / (float)LUT_N;
        float t1 = i / (float)LUT_N;
        return Mathf.Lerp(t0, t1, alpha);
    }

    // -------------------- Debug --------------------

    void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;
        Gizmos.color = gizmoColor;

        // 런타임 설정이 있을 때만 그려줌
        if (totalLen <= 0f) return;

        Vector3 prev = BezierPoint(0f);
        for (int i = 1; i <= 40; i++)
        {
            float t = i / 40f;
            Vector3 cur = BezierPoint(t);
            Gizmos.DrawLine(prev, cur);
            prev = cur;
        }

        // 포인트 표시
        Gizmos.DrawSphere(p0, 0.04f);
        Gizmos.DrawSphere(p1, 0.04f);
        if (order == Order.Cubic3)
        {
            Gizmos.DrawSphere(p2, 0.04f);
            Gizmos.DrawSphere(p3, 0.04f);
        }
        else
        {
            Gizmos.DrawSphere(p2, 0.04f);
        }
    }
}
