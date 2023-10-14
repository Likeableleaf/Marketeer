using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : GridObject
{
    private float speed = 3f;
    private float oldDistance = 9999;
    private GameObject closetsObject = null;
    
    int[] Inventory = new int[6];

    // Start is called before the first frame update
    new void Start()
    {
        //TODO: add an inventory for each item type for the player
        Inventory[0] = 8; // [0]number of Milk
        Inventory[1] = 8; // [1]number of Eggs
        Inventory[2] = 8; // [2]number of Bread
        Inventory[3] = 8; // [3]number of Beans
        Inventory[4] = 8; // [4]number of Meat
        Inventory[5] = 8; // [5]number of Vegatables
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

        

        //TODO: add a way to restock inventory for the player
        //TODO: track a way to add all the inventory slots so it does not go over 48

        //TODO: Add a way to detect next to a shelf and allow the 0 for each refillshelf(); method to be the inventory number of that item the player has
        if (Input.GetKey(KeyCode.Space))
        {GameObject ObjectClosest = GetNearestObject(5);
            //check if object detected is a shelf
            if( ( ObjectClosest != null ) && ( ObjectClosest.layer == LayerMask.NameToLayer( "Shelf" ) ) )
            {   //instantiate object to be shelf object to use it's methods
                Shelf currentShelf = ( Shelf )ObjectClosest.GetComponent( typeof( Shelf ) );
                //check if its a bean shelf
                if (currentShelf.name == "BeanShelf"){
                    //TODO: Change names of all shelves to be JUST their names of their respective items and add here more shelf names to check...
                    currentShelf.RefillShelf(0);
                }
                if (currentShelf.name == "MeatShelf")
                {
                    //TODO: Change names of all shelves to be JUST their names of their respective items and add here more shelf names to check...
                    currentShelf.RefillShelf(0);
                }
                if (currentShelf.name == "EggsShelf")
                {
                    //TODO: Change names of all shelves to be JUST their names of their respective items and add here more shelf names to check...
                    currentShelf.RefillShelf(0);
                }
                if (currentShelf.name == "VeggiesShelf")
                {
                    //TODO: Change names of all shelves to be JUST their names of their respective items and add here more shelf names to check...
                    currentShelf.RefillShelf(0);
                }
                if (currentShelf.name == "BreadShelf")
                {
                    //TODO: Change names of all shelves to be JUST their names of their respective items and add here more shelf names to check...
                    currentShelf.RefillShelf(0);
                }
                if (currentShelf.name == "MilkShelf")
                {
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
        //Check if there was a objected that was a certain distance
        if(closetsObject != null)
        {
            return closetsObject;
        }
        else
        {
            return null;
        }
        
    }
}
