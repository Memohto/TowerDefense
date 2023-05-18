using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Generator : MonoBehaviour
{
    [Header("Generator Settings")]
    [SerializeField] protected GameObject _object;
    [SerializeField] protected float _timeToGenerate;

    public bool IsGenerating { get; private set; }

    protected void Generate() {
        StartCoroutine(GenerateCoroutine());
    }

    protected abstract void GenerateObject();

    private IEnumerator GenerateCoroutine() {
        IsGenerating = true;
        yield return new WaitForSeconds(_timeToGenerate);
        GenerateObject();
        IsGenerating = false;
    }
}
