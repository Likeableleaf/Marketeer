using System;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    protected GridManager gridManager;
    protected Vector3 GridPositon;

    protected const int tileWidth = 1;
    protected void Start()
    {
        // Find the GridManager in the scene
        gridManager = FindObjectOfType<GridManager>();

        // Call the method to add the object to the grid
        AddSelfToGrid();
    }

    protected void Update()
    {

    }

    public virtual void Turn()
    {

    }

    protected virtual void AddSelfToGrid()
    {
        // Add the object to the GridManager's GameGrid
        gridManager.AddGridObjectToGrid(transform.position, 0, this);
        GridPositon = transform.position;
    }
    protected bool IsValidGridPosition(Vector3 position)
    {
        return (Math.Abs(position.x) % 1 - 0.5 == 0) &&
               (Math.Abs(position.z) % 1 - 0.5 == 0);
    }

    protected static Vector3 ClosestGridPos(Vector3 origPos)
    {
        // Round the x and z components to the nearest whole number
        float newX = Mathf.Round(origPos.x - 0.5f);
        float newZ = Mathf.Round(origPos.z - 0.5f);

        return new Vector3(newX + 0.5f, 0f, newZ + 0.5f);
    }

    protected static Vector3 GetTileInFront(Vector3 origPos, float YRotation)
    {
        Vector3 currTile = ClosestGridPos(origPos);

        // Find the nearest multiple of 90 degrees for the Y rotation
        float nearestYRotation = Mathf.Round(YRotation / 90.0f) * (Mathf.PI / 2);
        return currTile + RoundVector3(new Vector3((float)Math.Sin(nearestYRotation), 0, (float)Math.Cos(nearestYRotation)),1);
    }


    public static Vector3 RoundVector3(Vector3 vector, int decimalPlaces)
    {
        float multiplier = Mathf.Pow(10f, decimalPlaces);
        return new Vector3(
            Mathf.Round(vector.x * multiplier) / multiplier,
            Mathf.Round(vector.y * multiplier) / multiplier,
            Mathf.Round(vector.z * multiplier) / multiplier
        );
    }
}