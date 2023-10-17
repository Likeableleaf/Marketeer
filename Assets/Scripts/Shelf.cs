using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Shelf : Wall
{
    public GameObject[] ShelfVariants;
    public GameObject CurrentShelfObject;
    public GameObject[] ShelfIcons;
    public Sprite[] icons;
    public bool Backshelf;
    public int capacity = 0;
    public GroceryType groceryType;
    public Vector3 InFrontOfShelfPos;

    // Start is called before the first frame update
    protected new void Start()    {
        base.Start();

        //Adds self to the groceryDictionary
        gridManager.groceryDictionary[groceryType].Add(this);

        // Displays Icon if Backshelf
        if (Backshelf) {
            IconDisplay(groceryType);
        }

        //Sets the position of the InFrontOfShelfPos
        InFrontOfShelfPos = new Vector3 (0, 0, 1f);
        // Get the Y rotation of the shelf.
        float yRotation = transform.rotation.eulerAngles.y;
        // Rotate the offset by the Y rotation of the Shelf and add the shelf position
        // Round it to be usable
        InFrontOfShelfPos = RoundVector3(transform.position + Quaternion.Euler(0, yRotation, 0) * InFrontOfShelfPos, 1);
    }

    //Returns excess
    public int RefillShelf(int amount) {
        ShelfVariants[capacity].SetActive(false);
        int excessAmount = -Math.Min(0, capacity - amount);
        capacity = Math.Clamp(capacity - amount, 0, ShelfVariants.Length - 1);
        ShelfVariants[capacity].SetActive(true);
        return excessAmount;
    }

    public void RemoveItem(int amount) {
        if (HasStock(amount)) {
            ShelfVariants[capacity].SetActive(false);
            capacity += amount;
            ShelfVariants[capacity].SetActive(true);
        }        
    }

    public bool HasStock(int amount)
    {
        return capacity + amount < ShelfVariants.Length;
    }

    // Displays different icons depending on groceryType
    public void IconDisplay(GroceryType groceryType)
    {
        foreach (GameObject icon in ShelfIcons)
        {
            SpriteRenderer spriteRenderer = icon.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = icons[(int)groceryType];
        }
    }
}
