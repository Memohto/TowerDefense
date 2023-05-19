using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _followSpeed;
    [SerializeField] private float _YOffset;

    private void FixedUpdate() {
        Vector3 newPosition = new Vector3(_target.position.x, _target.position.y + _YOffset, -10f);
        transform.position = Vector3.Slerp(transform.position, newPosition, _followSpeed * Time.deltaTime);
    }
}
