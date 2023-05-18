using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour, IInteractable
{
    [Header("Elevator Settings")]
    [SerializeField] private Transform _downCheckpoint;
    [SerializeField] private Transform _upCheckpoint;
    [SerializeField] private int _storageLimit;
    [SerializeField] private float _moveSpeed;

    public int _storedCount;
    private PlatformState _platformState = PlatformState.Down;
    private bool _isMoving;
    private IEnumerator _routine;

    public void MovePlatform() {
        if (!_isMoving) {
            _routine = MovePlatformCoroutine();
            StartCoroutine(_routine);
        } else {
            StopCoroutine(_routine);
            ToggleState();
            _routine = MovePlatformCoroutine();
            StartCoroutine(_routine);
        }
        
    }

    private void ToggleState() {
        _platformState = _platformState == PlatformState.Down ? PlatformState.Up : PlatformState.Down;
    }

    private IEnumerator MovePlatformCoroutine() {
        _isMoving = true;
        bool done = false;
        while (!done) {
            if (_platformState == PlatformState.Down) {
                transform.position = Vector3.MoveTowards(transform.position, _upCheckpoint.position, _moveSpeed * Time.deltaTime);
                done = transform.position.y == _upCheckpoint.position.y;
            } else if (_platformState == PlatformState.Up) {
                transform.position = Vector3.MoveTowards(transform.position, _downCheckpoint.position, _moveSpeed * Time.deltaTime);
                done = transform.position.y == _downCheckpoint.position.y;
            }
            yield return new WaitForEndOfFrame();
        }
        ToggleState();
        _isMoving = false;
    }

    public void Interact(PlayerController player) {
        if (_storedCount < _storageLimit) {
            switch (player.CurrentItem) { 
                case Item.Bullet:
                    _storedCount++;
                    player.CurrentItem = Item.None;
                    break;
                case Item.None:
                    _storedCount--;
                    player.CurrentItem = Item.Bullet;
                    break;
                default: break;
            }
        }
    }

    private enum PlatformState { 
        Up, Down
    }
}
