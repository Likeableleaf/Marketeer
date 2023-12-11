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

    [SerializeField] public float delay = 2.0f;
    [SerializeField] private bool open = false;
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform startPos2;
    [SerializeField]  private float doorProg = 0f;
    private float door2Prog = 0f;

    [SerializeField] private AudioClip DoorOpen;
    [SerializeField] private AudioClip DoorClose;
    [SerializeField]private AudioSource DoorAudio;
    [SerializeField] private Manager manager;
    private bool doorCanOpen = true;
    private bool doorCanClose = false;
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

    private void OpenDoor() {
        // Play animation based on door version
        doorCanClose = true;
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

                if (manager.getStrikerCount() < 3 && doorCanOpen == true)
                {
                    DoorAudio.Stop();
                    DoorAudio.Play();
                    DoorAudio.PlayOneShot(DoorOpen, 0.5f);
                    doorCanOpen = false;
                }

                break;
            case DoorStyle.DoubleDoor:
                // On trigger swing door1


                // Reflect for door2


                // Play Door opening sound
                if (manager.getStrikerCount() < 3 && doorCanOpen == true)
                {
                    DoorAudio.Stop();
                    DoorAudio.Play();
                    DoorAudio.PlayOneShot(DoorOpen, 0.5f);
                    doorCanOpen = false;
                }   
                break;
            case DoorStyle.HingedDoor:
                // On trigger swing door 
                if (doorProg <= 1.0f) {
                    doorProg += Time.deltaTime;
                }                
                door.transform.position = Vector3.Lerp(startPos.transform.position, endPos.transform.position, doorProg);
                door.transform.rotation = Quaternion.Lerp(startPos.transform.rotation, endPos.transform.rotation, doorProg);

                // Play Door opening sound
                if (manager.getStrikerCount() < 3 && doorCanOpen == true)
                {
                    DoorAudio.Stop();
                    DoorAudio.Play();
                    DoorAudio.PlayOneShot(DoorOpen, 0.5f);
                    doorCanOpen = false;
                }

                break;
        }
        if (door.transform.position == endPos.transform.position) {
            open = true;            
        }
    }

    private void CloseDoor() {
        open = false;
        doorCanOpen = true;
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
                if (manager.getStrikerCount() < 3 && doorCanClose == true)
                {
                    DoorAudio.Stop();
                    DoorAudio.Play();
                    DoorAudio.PlayOneShot(DoorClose, 0.5f);
                    doorCanClose = false;
                }

                break;
            case DoorStyle.DoubleDoor:
                // On trigger swing door1


                // Reflect for door2

                // Play Door closing sound
                if (manager.getStrikerCount() < 3 && doorCanClose == true)
                {
                    DoorAudio.Stop();
                    DoorAudio.Play();
                    DoorAudio.PlayOneShot(DoorClose, 0.5f);
                    doorCanClose = false;
                }

                break;
            case DoorStyle.HingedDoor:
                // On trigger swing door 
                if (doorProg >= 0.0f) {
                    doorProg -= Time.deltaTime;
                }
                door.transform.position = Vector3.Lerp(startPos.transform.position, endPos.transform.position, doorProg);
                door.transform.rotation = Quaternion.Lerp(startPos.transform.rotation, endPos.transform.rotation, doorProg);

                // Play Door closing sound
                if (manager.getStrikerCount() < 3 && doorCanClose == true)
                {
                    DoorAudio.Stop();
                    DoorAudio.Play();
                    DoorAudio.PlayOneShot(DoorClose, 0.5f);
                    doorCanClose = false;
                }

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
