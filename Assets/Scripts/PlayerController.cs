using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private Collider2D _collider;
    private float _gravityScale;

    private void Start() {
        _gravityScale = _rigidBody.gravityScale;
    }
    private void Update() {
        GetInput();
        Detection();
        Move();
        Jump();
        Climb();
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
    [SerializeField] LayerMask _groundLayer;
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
    [SerializeField] private float _moveSpeed = 4;
    [SerializeField] private float _acceleration = 2;
    [SerializeField] private float _lerpSpeed = 200;
    private void Move() {
        float acceleration = _isGrounded ? _acceleration : _acceleration * 0.1f;
        float moveSpeed = _isClimbing && !_isGrounded ? _moveSpeed * 0.1f : _moveSpeed;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            if (_rigidBody.velocity.x > 0) _horizontalInput = 0;
            _horizontalInput = Mathf.MoveTowards(_horizontalInput, -1, acceleration * Time.deltaTime);
        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            if (_rigidBody.velocity.x < 0) _horizontalInput = 0;
            _horizontalInput = Mathf.MoveTowards(_horizontalInput, 1, acceleration * Time.deltaTime);
        } else {
            _horizontalInput = Mathf.MoveTowards(_horizontalInput, 0, acceleration * 2 * Time.deltaTime);
        }

        Vector3 targetVelocity = new Vector3(_horizontalInput * moveSpeed, _rigidBody.velocity.y);
        _rigidBody.velocity = Vector3.MoveTowards(_rigidBody.velocity, targetVelocity, _lerpSpeed * Time.deltaTime);
    }
    #endregion

    [Header("Jump")]
    [SerializeField] private float _jumpForce = 15;
    [SerializeField] private float _coyoteTime = 0.2f;
    [SerializeField] private float _fallMultiplier = 7;
    private bool _hasJumped;
    private void Jump() {
        if (_isClimbing) return;

        if (Input.GetKeyDown(KeyCode.Space) && (_isGrounded || Time.time < _timeLeftGrounded + _coyoteTime) && !_hasJumped) {
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, _jumpForce);
            _hasJumped = true;
        }
        if (_rigidBody.velocity.y < 0 || _rigidBody.velocity.y > 0 && !Input.GetKey(KeyCode.C)) {
            _rigidBody.velocity += _fallMultiplier * Physics.gravity.y * Vector2.up * Time.deltaTime;
        }
    }

    [Header("Climb")]
    [SerializeField] private float _climbSpeed = 5;
    private void Climb() {
        if (_isClimbing)
        {
            _rigidBody.gravityScale = 0;
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, _verticalInput * _climbSpeed * Time.deltaTime);
        }
        else {
            _rigidBody.gravityScale = _gravityScale;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Generator generator = collision.gameObject.GetComponent<Generator>();
        if (generator != null && !generator.IsGenerating) {
            generator.Generate();
        } else if (collision.CompareTag("Ladder")) {
            _isOnLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Ladder")) {
            _isOnLadder = false;
            _isClimbing = false;
        }
    }
}
