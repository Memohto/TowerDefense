using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private Animator _animator;
    [SerializeField] LayerMask _groundLayer;
    [Header("Item Prefabs")]
    [SerializeField] private GameObject _bulletPrefab;
    private float _gravityScale;

    private void Start() {
        _gravityScale = _rigidBody.gravityScale;
    }
    private void Update() {
        GetInput();
        Detection();
        Move();
        Interact();
        Jump();
        Climb();
        Shooting();
    }

    #region Inputs
    private float _horizontalInput;
    private float _verticalInput;
    private void GetInput() {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
    }
    #endregion

    #region Detection
    private bool _isGrounded;
    private float _timeLeftGrounded;
    private bool _isOnLadder;
    private bool _isClimbing;
    private void Detection() {
        Vector2 checkPoint = new Vector2(transform.position.x, _collider.bounds.center.y - _collider.bounds.extents.y);
        bool grounded = Physics2D.OverlapCircle(checkPoint, 0.2f, _groundLayer);

        if (!_isGrounded && grounded) {
            _hasJumped = false;
            //_horizontalInput = 0;
        } else if (_isGrounded && !grounded) {
            _timeLeftGrounded = Time.time;
        }

        _isGrounded = grounded;

        if (_isOnLadder && _verticalInput != 0) {
            _isClimbing = true;
        }
    }
    #endregion

    #region Movement
    [Header("Movement")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _lerpSpeed;
    public bool _isFacingRight = true;
    private void Move() {
        if (IsShooting) return;
        float moveSpeed = _isClimbing && !_isGrounded || _hasSomething ? _moveSpeed * 0.1f : _moveSpeed;
        
        if (Input.GetKey(KeyCode.A)) {
            if (_rigidBody.velocity.x > 0) _horizontalInput = 0;
            _horizontalInput = Mathf.MoveTowards(_horizontalInput, -1, _acceleration * Time.deltaTime);
            if (_horizontalInput < 0 && _isFacingRight) Flip();
        } else if (Input.GetKey(KeyCode.D)) {
            if (_rigidBody.velocity.x < 0) _horizontalInput = 0;
            _horizontalInput = Mathf.MoveTowards(_horizontalInput, 1, _acceleration * Time.deltaTime);
            if (_horizontalInput > 0 && !_isFacingRight) Flip();
        }
        else {
            _horizontalInput = Mathf.MoveTowards(_horizontalInput, 0, _acceleration * 10 * Time.deltaTime);
        }

        bool isRunning = Mathf.Abs(_horizontalInput) > 0.4;
        _animator.SetBool("isRunning", isRunning);

        Vector3 targetVelocity = new Vector3(_horizontalInput * moveSpeed, _rigidBody.velocity.y);
        _rigidBody.velocity = Vector3.MoveTowards(_rigidBody.velocity, targetVelocity, _lerpSpeed * Time.deltaTime);
    }

    private void Flip() {
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
        _isFacingRight = !_isFacingRight;
    }
    #endregion

    #region Interact
    public Item CurrentItem { get; set; }
    private IInteractable _currentInteractable;
    private bool _hasSomething;
    private void Interact() {
        if (Input.GetKeyDown(KeyCode.E)) {
            if (_currentInteractable != null) {
                _currentInteractable.Interact(this);
            }
        }

        switch (CurrentItem) {
            case Item.Bullet:
                _canClimb = false;
                _hasSomething = true;
                break;
            case Item.None:
                _canClimb = true;
                _hasSomething = false;
                break;
            default: break;
        }
    }
    #endregion

    #region Jump
    [Header("Jump")]
    [SerializeField] private float _jumpForce = 15;
    [SerializeField] private float _coyoteTime = 0.2f;
    [SerializeField] private float _fallMultiplier = 7;
    private bool _hasJumped;
    private void Jump() {
        if (_isClimbing || IsShooting) return;

        float jumpForce = _hasSomething ? _jumpForce * 0.75f : _jumpForce;
        if (Input.GetKeyDown(KeyCode.Space) && (_isGrounded || Time.time < _timeLeftGrounded + _coyoteTime) && !_hasJumped) {
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, jumpForce);
            _hasJumped = true;
        }
        if (_rigidBody.velocity.y < 0 || _rigidBody.velocity.y > 0 && !Input.GetKey(KeyCode.C)) {
            _rigidBody.velocity += _fallMultiplier * Physics.gravity.y * Vector2.up * Time.deltaTime;
        }
    }
    #endregion

    #region Climb
    [Header("Climb")]
    [SerializeField] private float _climbSpeed = 5;
    private bool _canClimb = true;
    private void Climb() {
        if (_isClimbing && _canClimb) {
            _rigidBody.gravityScale = 0;
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, _verticalInput * _climbSpeed * Time.deltaTime);
        } else {
            _rigidBody.gravityScale = _gravityScale;
        }

    }
    #endregion

    #region Shooting
    public bool IsShooting { get; set; }
    private Cannon _currentCannon;
    private float _startTime;
    private void Shooting() {
        FollowCamera.Instance.ShowMap = IsShooting || Input.GetKey(KeyCode.LeftShift);
        if (IsShooting) {
            if (_currentCannon.HasBullet) {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    _startTime = Time.time;
                } else if (Input.GetKeyUp(KeyCode.Space)) {
                    float finalTime = Time.time - _startTime;
                    _currentCannon.Fire(Mathf.Clamp(finalTime, 1, 4) * 200);
                }
            }
            _currentCannon.Rotate((int)_verticalInput);
        }
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision) {
        _currentInteractable = collision.gameObject.GetComponent<IInteractable>();
        _currentCannon = collision.gameObject.GetComponent<Cannon>();
         if(collision.CompareTag("Ladder")) {
            _isOnLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        _currentInteractable = null;
        _currentCannon = null;
        if (collision.CompareTag("Ladder")) {
            _isOnLadder = false;
            _isClimbing = false;
        }
    }
}

public enum Item {
    None,
    Bullet
}
