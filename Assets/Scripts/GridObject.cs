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

    public Vector3 RoundVector3(Vector3 vector, int decimalPlaces)
    {
        float multiplier = Mathf.Pow(10f, decimalPlaces);
        return new Vector3(
            Mathf.Round(vector.x * multiplier) / multiplier,
            Mathf.Round(vector.y * multiplier) / multiplier,
            Mathf.Round(vector.z * multiplier) / multiplier
        );
    }
}