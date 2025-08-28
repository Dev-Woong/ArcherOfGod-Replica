using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    [SerializeField] private PlayerSkillController _skillController;
    [SerializeField] private Button _leftMoveBtn;
    [SerializeField] private Button _rightMoveBtn;
    [SerializeField] private float _moveSpeed;
    private bool _isMoveLeft;
    private bool _isMoveRight;
    public bool StopPlayer;
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _skillController = GetComponent<PlayerSkillController>();
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
        if (_isMoveLeft == false && _isMoveRight == false)
        {
            transform.localScale = Vector2.one;
            _rigidbody2D.linearVelocity = new Vector2(0, _rigidbody2D.linearVelocityY);
            _animator.SetBool("isRun", false);
            StopPlayer = true;
        }
    }
    private void FixedUpdate()
    {
        if (_skillController.Moveable == true)
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
}
