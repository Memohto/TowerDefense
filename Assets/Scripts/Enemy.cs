using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 2f;
    private void Update() {
        transform.Translate(Vector2.right * _moveSpeed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Door")) {
            Destroy(gameObject);
            GameManager.Instance.Lives--;
        } else if (collision.CompareTag("Explosion")) {
            Destroy(gameObject);
            GameManager.Instance.Score += 10;
        }
    }
}
