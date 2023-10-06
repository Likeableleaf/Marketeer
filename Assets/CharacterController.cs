using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : GridObject
{
    private float speed = 3f;
   


    // Start is called before the first frame update
    new void Start()
    {

    }

    // Update is called once per frame
    new void Update()
    {
        Vector3 newPosition;
        Vector3 currentPosition = transform.position;
        newPosition = currentPosition;
        if (Input.GetKey(KeyCode.W))
        {
            newPosition.z +=  speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            newPosition.x +=  speed * Time.deltaTime; 
        }
        if (Input.GetKey(KeyCode.A))
        {
            newPosition.x += -speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            newPosition.z += -speed * Time.deltaTime;
        }

        Vector3 direction = newPosition - currentPosition;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.3f);
        }
        transform.position = newPosition;


    }
}
