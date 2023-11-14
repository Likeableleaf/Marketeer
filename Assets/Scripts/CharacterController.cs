using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class CharacterController : GridObject
{
    //Event for when Inventory changes
    [System.Serializable]
    public class InventoryUpdatedEvent : UnityEvent<int[]> { }
    public static InventoryUpdatedEvent OnInventoryUpdated = new InventoryUpdatedEvent();

    public float moveSpeed = 3f;
    public float turnSpeed = 5f;
    public float turnSpeed3D = 120;
    public float horizontalSpeed = 0.2f;
    public float verticalSpeed = 0.2f;
    public float radiusOfSatisfaction;
    public int maxInventorySize = 16;
    public int inventoryRefillAmount = 4;
    public float disposalFee = 0.1f;
    public int[] Inventory = new int[Enum.GetValues(typeof(GroceryType)).Length];
    public int dimension = 2;
    public bool helpingCustomer = false;
    //Access each index with (int)GroceryType.Type

    [SerializeField]
    private Rigidbody rb;
    private float y_axis = 0f; 

    // Start is called before the first frame update
    new void Start() {
        // Find the GridManager in the scene
        gridManager = FindObjectOfType<GridManager>();

        //Set the default amount of each item to the max value
        for (int i = 0; i < Inventory.Length; i++) {
            Inventory[i] = 0;
        }
    }

    // Update is called once per frame
    new void Update() {
        Movement();

        if (Input.GetKeyDown(KeyCode.Space)) {
            Interact();
        }
    }

    private void Movement() {
        Vector3 moveDirection = Vector3.zero;
        if (dimension == 3)
        {
            if (Input.GetKey(KeyCode.W))
            {
                moveDirection += transform.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                moveDirection -= transform.forward;
            }
            if (Input.GetKey(KeyCode.A)) 
            {
                moveDirection -= transform.right;
            }
            if (Input.GetKey(KeyCode.D))
            {
                moveDirection += transform.right;
            }
            float horizontal = horizontalSpeed * Input.GetAxis("Horizontal");
            y_axis += verticalSpeed * Input.GetAxis("Vertical");
            y_axis = Mathf.Clamp(y_axis, -60, 60);

            // Check if paused
            if (Time.timeScale != 0) {
                transform.Rotate(new Vector3(0, horizontal, 0));
                gridManager.PlayerCamera.transform.eulerAngles = new Vector3(y_axis, gridManager.PlayerCamera.transform.eulerAngles.y, gridManager.PlayerCamera.transform.eulerAngles.z);
            }
            
            moveDirection.Normalize();
            moveDirection *= moveSpeed;
            rb.velocity = moveDirection;
        }
        else 
        {
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
    }

    private void Interact() {
        //Check if there is a tile in front
        Vector3 TileInFront = GetTileInFront(transform.position, transform.rotation.eulerAngles.y);
        if (gridManager.GameGrid.ContainsKey(TileInFront)) {
            //Get the dictionary for the tile
            Dictionary<int, GridObject> TileDictionary = gridManager.GameGrid[TileInFront];
            if (TileDictionary.ContainsKey(-10)) {
                GridObject tileObject = TileDictionary[-10];
                if (tileObject is Shelf shelf) {
                    FillShelf(shelf);
                }
                else if(tileObject is BackShelf backShelf) {
                    RefillInventoryItem(backShelf.groceryType);
                } 
                else if(tileObject is Dumpster) {
                    EmptyInventory();
                }
                else if(tileObject is VendingMachine vendingMachine && GridManager.IsCashAvailable(vendingMachine.shiftCost) && dimension != vendingMachine.shiftDimension){
                    GridManager.RemoveCash(vendingMachine.shiftCost);
                    gridManager.ShiftDimension(vendingMachine.shiftDimension);
                    dimension = vendingMachine.shiftDimension;
                }
            }
        }
        //Otherwise do nothing
    }

    private void FillShelf(Shelf shelf)
    {
        Inventory[(int)shelf.groceryType] = shelf.RefillShelf(Inventory[(int)shelf.groceryType]);
    }

    private int SumInventorySize()
    {
        int sum = 0;
        foreach(int item in Inventory)
        {
            sum += item;
        }
        return sum;
    }

    // Clear out the Inventory List 
    public void EmptyInventory() {
        GridManager.RemoveCash(SumInventorySize() * disposalFee);
        Inventory = new int[Enum.GetValues(typeof(GroceryType)).Length];
        OnInventoryUpdated.Invoke(Inventory);
    }
    // Add to an Item of the Inventory
    public void RefillInventoryItem(GroceryType grocery)
    {
        Inventory[(int)grocery] += Mathf.Min(maxInventorySize - SumInventorySize(), inventoryRefillAmount);
        OnInventoryUpdated.Invoke(Inventory);
    }
}
