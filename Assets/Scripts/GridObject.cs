using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class GridObject : MonoBehaviour
{
    private GridManager gridManager;

    private void Start()
    {
        // Find the GridManager in the scene
        gridManager = FindObjectOfType<GridManager>();

        // Call the method to add the object to the grid
        AddSelfToGrid();
    }

    private void AddSelfToGrid()
    {
        // Get the X and Z position of the object
        Vector2 position = new Vector2(transform.position.x, transform.position.z);

        // Add the object to the GridManager's GameGrid
        gridManager.AddObjectToGrid(position, gameObject);
    }

    private List<Vector2> AStarPathFromTo(Vector2 start, Vector2 end) 
    {
        HashSet<Vector2> visitedSet = new();
        PriorityQueue<PathTile, float> frontier = new();

        PathTile startTile = new PathTile(start, 0);
        PathTile endTile = new PathTile(end, 0);

        frontier.Enqueue(startTile, startTile.GetGH());
        while (true)
        {
            if(frontier.Count == 0)
            {
                return null;
            }
            PathTile currTile = frontier.Dequeue();
            if(currTile.GetPosition().x==endTile.GetPosition().x && currTile.GetPosition().y == endTile.GetPosition().y)
            {
                return UnwrapPath(currTile);
            }
            visitedSet.Add(currTile.GetPosition());
            frontier.EnqueueRange(GenerateFrontier(currTile, endTile, visitedSet).Select(tuple => (tuple.Item1, tuple.Item2)));
        }
    }

    private List<Tuple<PathTile,float>> GenerateFrontier(PathTile currTile, PathTile endTile, HashSet<Vector2> visitedTiles)
    {
        List<Tuple<PathTile,float>> localFrontier = new();
        Vector2 currPosition = currTile.GetPosition();

        for(int x = -1; x <= 1; x++) 
        { 
            for(int z = -1; z <= 1; z++)
            {
                Vector2 TileToCheck = new Vector2(x, z) + currPosition;
                if (!gridManager.GameGrid.ContainsKey(TileToCheck) && !visitedTiles.Contains(TileToCheck))
                {
                    PathTile newTile = new PathTile(TileToCheck, currTile.GetStartDistance() + 1, endTile, currTile);
                    localFrontier.Add(new Tuple<PathTile, float>(newTile, newTile.GetGH()));
                }
            }
        }
        return localFrontier;
    }

    private static List<Vector2> UnwrapPath(PathTile pathHolder) 
    {
        List<Vector2> orderedPath = new();
        PathTile currPath = pathHolder;
        while(true)
        {
            orderedPath.Insert(0, currPath.GetPosition());
            currPath = currPath.GetPrevious();
            if(currPath == null)
            {
                return orderedPath;
            }
        }
    }
}

public class PathTile
{
    private Vector2 position;
    private int startDistance;
    private float ghDistance;
    private PathTile previousTile;

    //Constructors
    public PathTile(Vector2 inPosition, int inStartDistance)
    {
        position = inPosition;
        startDistance = inStartDistance;
        ghDistance = float.MaxValue;
        previousTile = null;
    }

    public PathTile(Vector2 inPosition, int inStartDistance, PathTile inEndTile, PathTile inPreviousTile)
    {
        position = inPosition;
        startDistance = inStartDistance;
        ghDistance = GenerateGH(inEndTile);
        previousTile = inPreviousTile;
    }

    //Generates GH from distance to start and ManhattanDistance
    private float GenerateGH(PathTile inEnd)
    {
        float manhattanDistance = Math.Abs(position.x - inEnd.GetPosition().x) + Math.Abs(position.y - inEnd.GetPosition().y);
        return startDistance + manhattanDistance;
    }

    public Vector2 GetPosition(){ return position; }
    public float GetGH() { return ghDistance; }
    public PathTile GetPrevious() { return previousTile; }
    public int GetStartDistance() { return startDistance; }
}