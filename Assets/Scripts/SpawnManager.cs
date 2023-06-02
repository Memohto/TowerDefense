using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _spawners;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private float _minSpawnRate;
    [SerializeField] private float _maxSpawnRate;

    private void Start() {
        StartCoroutine(SpawnCoroutine());
    }

    private IEnumerator SpawnCoroutine() {
        while (true) {
            float seconds = Random.Range(_minSpawnRate, _maxSpawnRate);
            yield return new WaitForSeconds(seconds);
            int index = Random.Range(0, _spawners.Length);
            Instantiate(_enemyPrefab, _spawners[index].transform.position, _spawners[index].transform.rotation);
        }
    }
}
