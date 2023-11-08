using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Wall : GridObject
{
    public bool Walkable = false;
    public List<Vector3> WallAtOffsets;

    protected override void AddSelfToGrid()
    {
        if (!Walkable) 
        {  
            //If 1x1 and no offset, just add normally
            if(WallAtOffsets.Count == 0)
            {
                // Add the object to the GridManager's GameGrid
                gridManager.AddObjectAt(transform.position, -10, this);
            }
            else
            {
                foreach (Vector3 offset in WallAtOffsets)
                {
                    // Get the Y rotation of the Wall.
                    float yRotation = transform.rotation.eulerAngles.y;

                    // Rotate the offset by the Y rotation of the Wall.
                    Vector3 rotatedOffset = Quaternion.Euler(0, yRotation, 0) * offset;

                    // Add the rotated offset to the Wall's position and use it in gridManager.AddObjectAt.
                    Vector3 finalPosition = transform.position + rotatedOffset;
                    gridManager.AddObjectAt(finalPosition, -10, this);
                }
            }
            GridPositon = transform.position;

            //add sound effect of a new customer coming into the store
        }
    }
}
