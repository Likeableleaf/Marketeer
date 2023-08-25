using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Dictionary<Vector2, GameObject> GameGrid = new Dictionary<Vector2, GameObject>();

    public void AddObjectToGrid(Vector2 position, GameObject obj)
    {
        // Add the object to the GameGrid dictionary using its position as the key
        GameGrid[position] = obj;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
