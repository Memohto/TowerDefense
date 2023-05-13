using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnace : Generator
{
    [SerializeField] private float _ejectionForce;

    protected override void GenerateObject()
    {
        GameObject instance = Instantiate(_object, transform.position, Quaternion.identity);
        Vector2 dir = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0.5f, 1f));
        instance.GetComponent<Rigidbody2D>().AddForce(dir * _ejectionForce * Time.deltaTime, ForceMode2D.Impulse);
    }
}
