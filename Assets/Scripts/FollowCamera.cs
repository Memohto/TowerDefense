using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _followSpeed;
    [SerializeField] private float _YOffset;

    private Camera _cam;
    private bool _followCursor;

    private void Start() {
        _cam = Camera.main;
    }
    private void Update() {
        _followCursor = Input.GetKey(KeyCode.LeftShift);

    }
    private void FixedUpdate() {
        Vector3 newPosition;
        if (_followCursor) {
            Vector3 cursorPos = _cam.ScreenToWorldPoint(Input.mousePosition);
            newPosition = cursorPos;
        } else {
            newPosition = new Vector3(_target.position.x, _target.position.y + _YOffset, -10f);
        }
        float followSpeed = !_followCursor ? _followSpeed : _followSpeed * 0.1f;
        transform.position = Vector3.Slerp(transform.position, newPosition, followSpeed * Time.deltaTime);
    }
}
