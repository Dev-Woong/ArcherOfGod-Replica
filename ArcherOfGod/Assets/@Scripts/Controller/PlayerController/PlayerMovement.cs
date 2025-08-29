using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    [SerializeField] private ObjectStatus _objectStatus;
    [SerializeField] private PlayerSkillController _skillController;
    [SerializeField] private Button _leftMoveBtn;
    [SerializeField] private Button _rightMoveBtn;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _dashVelocity;
    private bool _isMoveLeft;
    private bool _isMoveRight;
    public bool IsDash = false;
    public bool IsJump = false;
    public bool Moveable = true;
    public bool StopPlayer;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _skillController = GetComponent<PlayerSkillController>();
        _objectStatus = GetComponent<ObjectStatus>();
    }
    public void CanMove() // AnimationEvent
    {
        Moveable = true;
    }
    public void IsDashFalse() // AnimationEvent
    {
        IsDash = false;
        _rigidbody2D.gravityScale = 1;
    }
    public void IsJumpFalse()
    {
        IsJump = false;
        _rigidbody2D.linearVelocity = Vector2.zero;
    }
    public void AfterJumpSet() // AnimationEvent
    {
        _rigidbody2D.gravityScale = 2;
    }
    void MoveSet()
    {
        if (_leftMoveBtn.GetComponent<ButtonHolding>()._isBtnHolding == true)
        {
            if (_isMoveRight == true)
            {
                _isMoveRight = false;
            }
            _isMoveLeft = true;
            StopPlayer = false;
        }
        else
        {
            _isMoveLeft = false;
        }
        if (_rightMoveBtn.GetComponent<ButtonHolding>()._isBtnHolding == true)
        {
            if (_isMoveLeft == true)
            {
                _isMoveLeft = false;
            }
            _isMoveRight = true;
            StopPlayer = false;
        }
        else
        {
            _isMoveRight = false;   
        }
    }
    private void MoveLeft()
    {
        if (_leftMoveBtn.GetComponent<ButtonHolding>()._isBtnHolding == true)
        {
            _rigidbody2D.linearVelocity = _moveSpeed * Vector2.left;
            transform.localScale = new Vector2(-1, 1);
            _animator.SetBool("isRun", true);
        }
    }
    private void MoveRight()
    {
        if (_rightMoveBtn.GetComponent<ButtonHolding>()._isBtnHolding == true)
        {
            _rigidbody2D.linearVelocity = _moveSpeed  * Vector2.right;
            transform.localScale = Vector2.one;
            _animator.SetBool("isRun", true);
        }
    }
    private void NoneInput()
    {
        if (_isMoveLeft == false && _isMoveRight == false && IsDash == false &&IsJump==false)
        {
            transform.localScale = Vector2.one;
            _rigidbody2D.linearVelocity = new Vector2(0, _rigidbody2D.linearVelocityY);
            _animator.SetBool("isRun", false);
            StopPlayer = true;
        }
    }
    public void Dash(AttackData attackData) // Animation Event
    {
        IsDash = true;
        var dash = ObjectPoolManager.Instance.GetObject(attackData.MoveEffectName);
        dash.transform.position = transform.position + attackData.MoveEffectPos;
        if (transform.localScale.x == -1)
        {
            dash.transform.localScale = new Vector3(-1, 1, 1);
            _rigidbody2D.linearVelocity = Vector2.right * -attackData.MovePosition;
        }
        else
        {
            _rigidbody2D.linearVelocity = Vector2.right * attackData.MovePosition;
        }
        
    }
    public void Jump(AttackData attackData) // AnimationEvent
    {
        IsJump = true;
        //var jumpEffect =

        _rigidbody2D.linearVelocity = Vector2.up * attackData.JumpForce.y;

        _rigidbody2D.gravityScale = 0;
    }
    
    private void FixedUpdate()
    {
        if (Moveable == true && _objectStatus.ReturnFrozenStatus()==false)
        {
            MoveLeft();
            MoveRight();
        }
    }
    void Update()
    {
        MoveSet();
        NoneInput();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            _rigidbody2D.gravityScale = 1;
        }
    }
}
