using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour {
    [SerializeField] private float _seconds;
    private void Start() {
        StartCoroutine(DestroyCoroutine());
    }

    private IEnumerator DestroyCoroutine() {
        yield return new WaitForSeconds(_seconds);
        Destroy(gameObject);
    }
}
