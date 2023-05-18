using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnace : Generator, IInteractable
{
    [Header("Furnace Settings")]
    [SerializeField] private int _storageLimit;

    public int _storedCount;

    private void Start() {
        Generate();
    }

    private void Update() {
        if (!IsGenerating && _storedCount < _storageLimit) {
            Generate();
        }
    }

    protected override void GenerateObject() {
        _storedCount++;
    }

    public void Interact(PlayerController player) {
        if (_storedCount > 0 && player.CurrentItem == Item.None) {
            player.CurrentItem = Item.Bullet;
            _storedCount--;
        }
    }
}
