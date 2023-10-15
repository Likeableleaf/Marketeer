using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // Variable cache
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject door2;
    [SerializeField] private DoorStyle DoorVersion;
    [SerializeField] private GameObject endPos;
    [SerializeField] private GameObject endPos2;

    [SerializeField] private float delay = 2.0f;
    [SerializeField] private bool open = false;
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform startPos2;
    [SerializeField]  private float doorProg = 0f;
    private float door2Prog = 0f;


    private void Update() {
        // If delay is up close door
        if (delay <= 0.0f) {
            // Close door
            CloseDoor();
        } else if (0.0f <= delay && delay <= 2.0f && open) {
            // Decrease timer
            delay -= Time.deltaTime;
        } else if (delay == 3.0f) {
            // Open Door
            OpenDoor();
        } else {
            // Do nothing
        }
    }

    private void OnTriggerEnter(Collider other) {
        delay = 3.0f;
    }

    private void OnTriggerExit(Collider other) {
        delay = 2.0f;        
    }

    private void OpenDoor() {
        // Play animation based on door version
        switch(DoorVersion) {
            case DoorStyle.SlidingDoor:
                // Move door
                if (doorProg <= 1.0f) {
                    doorProg += Time.deltaTime;
                }                
                door.transform.position = Vector3.Lerp(startPos.transform.position, endPos.transform.position, doorProg);

                // Move door2
                if (door2Prog <= 1.0f) {
                    door2Prog += Time.deltaTime;
                }                
                door2.transform.position = Vector3.Lerp(startPos2.transform.position, endPos2.transform.position, door2Prog);
                 
                // Play Door opening sound

                break;
            case DoorStyle.DoubleDoor:
                // On trigger swing door1


                // Reflect for door2


                // Play Door opening sound

                break;
            case DoorStyle.HingedDoor:
                // On trigger swing door 


                // Play Door opening sound

                break;
        }
        if (door.transform.position == endPos.transform.position) {
            open = true;            
        }
    }

    private void CloseDoor() {
        open = false;
        // Play animation based on door version
        switch(DoorVersion) {
            case DoorStyle.SlidingDoor:
                // Move door
                if (doorProg >= 0.0f) {
                    doorProg -= Time.deltaTime;
                }
                door.transform.position = Vector3.Lerp(startPos.transform.position, endPos.transform.position, doorProg);

                if (door2Prog >= 0.0f) {
                    door2Prog -= Time.deltaTime;
                }                
                door2.transform.position = Vector3.Lerp(startPos2.transform.position, endPos2.transform.position, door2Prog);

                // Play Door closing sound

                break;
            case DoorStyle.DoubleDoor:
                // On trigger swing door1


                // Reflect for door2

                // Play Door closing sound

                break;
            case DoorStyle.HingedDoor:
                // On trigger swing door 


                // Play Door closing sound

                break;
        }
        if (door.transform.position == startPos.transform.position) {
            delay = 2.0f;
        }
    }

    private enum DoorStyle {
        SlidingDoor,
        DoubleDoor, 
        HingedDoor,
    }
}
