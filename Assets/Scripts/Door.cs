using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // Variable cache
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject door2;
    [SerializeField] private DoorStyle DoorVersion;
    [SerializeField] private GameObject startPos;
    [SerializeField] private GameObject startPos2;
    [SerializeField] private GameObject endPos;
    [SerializeField] private GameObject endPos2;

    private void onTriggerEnter(Collider other) {

        Debug.Log("Triggered");
        
        // Play animation based on door version
        switch(DoorVersion) {
            case DoorStyle.SlidingDoor:
                // Move door
                door.transform.position = Vector3.Lerp(door.transform.position, endPos.transform.position, Time.deltaTime);

                // Move door2
                door2.transform.position = Vector3.Lerp(door2.transform.position, endPos2.transform.position, Time.deltaTime);
                
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
    }

    private void ontTriggerExit(Collider other) {

        Debug.Log("Triggered");
        
        // Play animation based on door version
        switch(DoorVersion) {
            case DoorStyle.SlidingDoor:
                // On trigger move door to (0.0f, 0.0f, 0.0f)
                door.transform.position = Vector3.Lerp(door.transform.position, startPos.transform.position, Time.deltaTime);

                // Move door2
                door2.transform.position = Vector3.Lerp(door2.transform.position, startPos2.transform.position, Time.deltaTime);

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
    }

    private enum DoorStyle {
        SlidingDoor,
        DoubleDoor, 
        HingedDoor,
    }
}
