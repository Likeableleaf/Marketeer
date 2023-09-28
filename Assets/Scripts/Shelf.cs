using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shelf : Wall
{
    public GameObject[] ShelfVariants;
    public GameObject CurrentShelfObject;
    public int capacity = 0;

    // Start is called before the first frame update
    protected new void Start()    {
        base.Start();
    }

    // Update is called once per frame
    protected new void Update()    {
        base.Update();
    }

    public void FillShelf() {
        ShelfVariants[capacity].SetActive(false);
        capacity = 0;
        ShelfVariants[capacity].SetActive(true);
    }

    public void ConsumeItems() {
        if (capacity < 4) {
            ShelfVariants[capacity].SetActive(false);
            capacity += 1;
            ShelfVariants[capacity].SetActive(true);
        }        
    }
}
