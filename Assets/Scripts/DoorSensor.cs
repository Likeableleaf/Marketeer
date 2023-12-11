using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSensor : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private bool playerBlocked;
    private Door doorControl;

    private void Start() {
        doorControl = door.GetComponent<Door>();
    }
    private void OnTriggerEnter(Collider other) {
        doorControl.delay = 3.0f;
        if (playerBlocked && other.CompareTag("Player")) {
            PushPlayerBack(other.GetComponent<Rigidbody>());
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) {
            PushPlayerBack(other.GetComponent<Rigidbody>());
        }
    }

    private void OnTriggerExit(Collider other) {
        doorControl.delay = 2.0f;        
    }

    private void PushPlayerBack(Rigidbody playerRb) {
        // Apply force to the player
        Debug.Log("PushaBit");
        playerRb.velocity = -playerRb.velocity;
    }
}
