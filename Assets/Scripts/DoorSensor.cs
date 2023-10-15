using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSensor : MonoBehaviour
{
    [SerializeField] private GameObject door;
    private Door doorControl;

    private void Start() {
        doorControl = door.GetComponent<Door>();
    }
    private void OnTriggerEnter(Collider other) {
        doorControl.delay = 3.0f;
    }

    private void OnTriggerExit(Collider other) {
        doorControl.delay = 2.0f;        
    }
}
