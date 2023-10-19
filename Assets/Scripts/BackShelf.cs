using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackShelf : Wall
{
    // Variable Cache
    public GameObject[] ShelfIcons;
    public Sprite[] icons;
    public GroceryType groceryType;
    public int refillAmount = 4;
    
    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();
        IconDisplay(groceryType);
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

    public int RefillItem(int inventoryMaxSize, int inventorySize)
    {
        return Mathf.Min(inventoryMaxSize - inventorySize, refillAmount);
    }
}
