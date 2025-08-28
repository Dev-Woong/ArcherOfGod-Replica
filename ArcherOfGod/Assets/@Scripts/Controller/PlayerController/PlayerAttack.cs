using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private PlayerMovement _playerMovement;
    private bool _canAttack;
    public bool _onGround = false; 
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerMovement = GetComponent<PlayerMovement>();
    }
    void CanShoot()
    {
        _canAttack = _playerMovement.StopPlayer;
        _animator.SetBool("isAttack", _canAttack);
    }

    // Update is called once per frame
    void Update()
    {
        if ( _onGround == true)
        {
            CanShoot();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            _onGround = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            _onGround = false;
        }
    }
}
