using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Wall : GridObject
{
    public bool Walkable = false;

    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected new void Update()
    {
        base.Update();
    }

    protected override void AddSelfToGrid()
    {
        if (!Walkable) 
        {
            // Add the object to the GridManager's GameGrid
            gridManager.AddObjectAt(transform.position, -10, gameObject);
            GridPositon = transform.position;
        }
    }
}
