using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public static FollowCamera Instance;
    public bool ShowMap { get; set; }

    [Header("Map Settings")]
    [SerializeField] private Transform _mapCenter;
    [SerializeField] private float _maxZoom;
    [Header("Follow Settings")]
    [SerializeField] private Transform _target;
    [SerializeField] private float _minZoom;
    [SerializeField] private float _followSpeed;
    [SerializeField] private float _YOffset;

    private Camera _cam;

    private void Start() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _cam = Camera.main;
        _cam.orthographicSize = _minZoom;
    }

    private void FixedUpdate() {
        Vector3 newPosition;
        if (ShowMap) {
            _cam.orthographicSize = _maxZoom;
            transform.position = _mapCenter.position;
        } else {
            _cam.orthographicSize = _minZoom;
            newPosition = new Vector3(_target.position.x, _target.position.y + _YOffset, -10f);
            transform.position = Vector3.Slerp(transform.position, newPosition, _followSpeed * Time.deltaTime);
        }
        
    }
}
