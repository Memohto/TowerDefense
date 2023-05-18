using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _fireMultiplier;

    public bool HasBullet { get; private set; }

    private void Load() {
        HasBullet = true;
    }

    public void Fire(float force) {
        GameObject bulletInstance = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = bulletInstance.GetComponent<Rigidbody2D>();
        if (rb != null) rb.AddForce(transform.right * force * _fireMultiplier);
        HasBullet = false;
    }

    public void Rotate(int direction) {
        transform.Rotate(new Vector3(0, 0, _rotationSpeed) * direction * Time.deltaTime);
    }

    public void Interact(PlayerController player) {
        if (player.CurrentItem == Item.Bullet && !HasBullet) {
            Load();
            player.CurrentItem = Item.None;
        } else if (player.CurrentItem == Item.None) {
            player.IsShooting = !player.IsShooting;
        } 
    }

}
