using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class CharacterController : GridObject
{
    public float moveSpeed = 3f;
    public float turnSpeed = 5f;
    public float radiusOfSatisfaction;
    public int maxInventorySize = 6;
    public int[] Inventory = new int[Enum.GetValues(typeof(GroceryType)).Length];
    //Access each index with (int)GroceryType.Type

    [SerializeField]
    private Rigidbody rb;

    // Start is called before the first frame update
    new void Start()
    {
        // Find the GridManager in the scene
        gridManager = FindObjectOfType<GridManager>();

        //Set the default amount of each item to the max value
        for (int i = 0; i < Inventory.Length; i++)
        {
            Inventory[i] = maxInventorySize;
        }
    }

    // Update is called once per frame
    new void Update()
    {
        Movement();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Interact();
        }
    }

    private void Movement()
    {
        Vector3 moveDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += Vector3.right;
        }
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        // Reset velocity to zero if no input is detected or close enough to 
        if (moveDirection == Vector3.zero)
        {
            rb.velocity = Vector3.zero;
        }
        else
        {
            // Normalize moveDirection
            moveDirection.Normalize();
            moveDirection *= moveSpeed;
            rb.velocity = moveDirection;
        }
    }

    private void Interact()
    {
        //Check if there is a tile in front
        Vector3 TileInFront = GetTileInFront(transform.position, transform.rotation.eulerAngles.y);
        if (gridManager.GameGrid.ContainsKey(TileInFront))
        {
            //Get the dictionary for the tile
            Dictionary<int, GridObject> TileDictionary = gridManager.GameGrid[TileInFront];
            if (TileDictionary.ContainsKey(-10) && TileDictionary[-10] is Shelf)
            {
                FillShelf((Shelf)TileDictionary[-10]);
            }
        }
        //Otherwise do nothing
    }

    private void FillShelf(Shelf shelf)
    {
        Inventory[(int)shelf.groceryType] = shelf.RefillShelf(Inventory[(int)shelf.groceryType]);
    }
}
