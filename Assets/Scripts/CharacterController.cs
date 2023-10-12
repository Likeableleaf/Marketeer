using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : GridObject
{
    private float speed = 3f;
    private float oldDistance = 9999;
    private GameObject closetsObject;

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

        //TODO: add an inventory for each item type for the player

        //TODO: Add a way to detect next to a shelf
        if (Input.GetKey(KeyCode.Space))
        {GameObject ObjectClosest = GetNearestObject(5);
            if(ObjectClosest.layer == LayerMask.NameToLayer("Shelf"))
            {
                Shelf currentShelf = (Shelf)ObjectClosest.GetComponent(typeof(Shelf));
                if (currentShelf.name == "BeanShelf"){
                    //TODO: Change names of all shelves to be JUST their names of their respective items and add here more shelf names to check...
                    currentShelf.RefillShelf(0);
                }
            }
        }
    }

    //grab the closes object
    private GameObject GetNearestObject(float distance)
    {
        List<GameObject> NearGameobjects = new List<GameObject>();
        if (distance != 0)
        {
            oldDistance = distance;
        }
        foreach (GameObject g in NearGameobjects)
        {
            float dist = Vector3.Distance(this.gameObject.transform.position, g.transform.position);
            if (dist < oldDistance)
            {
                closetsObject = g;
                oldDistance = dist;
            }
        }
        
        return closetsObject;
    }
}
