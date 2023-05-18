using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorSwitch : MonoBehaviour, IInteractable
{
    [SerializeField] private Elevator _elevator;

    public void Interact(PlayerController player) {
        _elevator.MovePlatform();
    }
}
