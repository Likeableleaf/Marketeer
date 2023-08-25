using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject tilePrefab; // The prefab for a single grid tile
    public int gridSizeX = 20;     // Number of tiles along the X-axis
    public int gridSizeZ = 20;     // Number of tiles along the Z-axis
    public float tileSize = 1.0f; // Size of each tile

    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateGrid()
    {
        for (int x = -gridSizeX/2; x < gridSizeX/2; x++)
        {
            for (int z = -gridSizeX/2; z < gridSizeZ/2; z++)
            {
                Vector3 tilePosition = new Vector3(x * tileSize + tileSize/2, 0, z * tileSize + tileSize / 2);
                Instantiate(tilePrefab, tilePosition, Quaternion.identity);
            }
        }
    }
}
