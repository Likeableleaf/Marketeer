using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class MovingGridObject : GridObject
{
    protected Stack<Vector3> path;
    protected float moveTime = 1.0f; //Number of turns to move a spot
    protected Vector3 nextStep;
    protected bool moving = false;
    protected float startTime;
    protected Vector3 startPosition;


    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();

        //If not on grid, move to closest grid spot
        transform.position = IsValidGridPosition(transform.position) ? transform.position : ClosestGridPos(transform.position); 

        //Set the moveTime to the time a Turn is
        moveTime = gridManager.turnInterval;
    }

    // Update is called once per frame
    protected new void Update()
    {
        base.Update();

        if (moving)
        {
            float timeSinceStarted = Time.time - startTime;
            float percentageComplete = timeSinceStarted / moveTime;

            transform.position = Vector3.Lerp(startPosition, nextStep, percentageComplete);

            if (percentageComplete >= 1.0f)
            {
                moving = false;
                transform.position = nextStep;
            }
            Vector3 direction = nextStep - startPosition;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, percentageComplete);
            }
        }
    }

    //Is called once per turn by the GridManager
    public override void Turn()
    {
        base.Turn();
    }

    //AStar! You know this one, I hope.
    public PathTile AStarPathFromTo(Vector3 start, Vector3 end)
    {
        if (!IsValidGridPosition(start) || !IsValidGridPosition(end))
        {
            Debug.Log($"Invalid starting or ending position {name} {start} {!IsValidGridPosition(start)} {end} {!IsValidGridPosition(end)}");
            return null;
        }
        if(start == end)
        {
            //Just standing still then, no path needed
            return null;
        }
        HashSet<Vector3> visitedSet = new();
        PriorityQueue<PathTile, float> frontier = new();

        PathTile startTile = new PathTile(start, 0);
        PathTile endTile = new PathTile(end, 0);

        frontier.Enqueue(startTile, startTile.GetGH());
        for (int i = 0; i < 100; i++)
        {
            if (frontier.Count == 0)
            {
                return null;
            }
            PathTile currTile = frontier.Dequeue();
            if (currTile.GetPosition().x == endTile.GetPosition().x && currTile.GetPosition().z == endTile.GetPosition().z)
            {
                return currTile;
            }
            visitedSet.Add(currTile.GetPosition());
            frontier.EnqueueRange(GenerateFrontier(currTile, endTile, visitedSet).Select(tuple => (tuple.Item1, tuple.Item2)));
        }
        return null;
    }

    private List<Tuple<PathTile, float>> GenerateFrontier(PathTile currTile, PathTile endTile, HashSet<Vector3> visitedTiles)
    {
        List<Tuple<PathTile, float>> localFrontier = new();
        Vector3 currPosition = currTile.GetPosition();

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                Vector3 TileToCheck = new Vector3(x, 0, z) + currPosition;
                if (CheckFrontierValidity(visitedTiles, TileToCheck, currTile, endTile))
                {
                    PathTile newTile = new PathTile(TileToCheck, currTile.GetStartDistance() + 1, endTile, currTile);
                    localFrontier.Add(new Tuple<PathTile, float>(newTile, newTile.GetGH()));
                }
            }
        }
        return localFrontier;
    }

    private bool CheckFrontierValidity(HashSet<Vector3> visitedTiles, Vector3 TileToCheck, PathTile currTile, PathTile endTile)
    {
        int currTileTime = currTile.GetStartDistance();
        Vector3 currTilePos = currTile.GetPosition();
        Dictionary<Vector3, Dictionary<int, GridObject>> GameGrid = gridManager.GameGrid;
        //Check if tile already Visited
        if (visitedTiles.Contains(TileToCheck))
        {
            return false;
        }
        //Checks related to if the tile will be blocked when we want it
        if (GameGrid.ContainsKey(TileToCheck))
        {
            Dictionary<int, GridObject> dictToCheck = GameGrid[TileToCheck];

            //If the tile will be full the moment we want it
            if (dictToCheck.ContainsKey(currTileTime + 1))
            {
                return false;
            }
            //If the tile is a wall or something else that doesnt move
            if (dictToCheck.ContainsKey(-10))
            {
                return false;
            }
            //If the tile is the final tile for another moving object
            if (dictToCheck.ContainsKey(-5))
            {
                //If the tile is the final tile for this
                //Only one gridobject can claim a tile as their final at a time, others must wait
                if (TileToCheck == endTile.GetPosition())
                {
                    return false;
                }
                //If this object is just passing by before the final-tile-claimer gets here
                if (!(dictToCheck.Keys.OrderByDescending(key => key).FirstOrDefault() > currTileTime))
                {
                    return false;
                }
            }
        }

        //Checks related to surrounding Tiles

        //Check if objects want to pass straight through each other
        if (GameGrid.ContainsKey(currTilePos) && GameGrid.ContainsKey(TileToCheck))
        {
            if (GameGrid[currTilePos].ContainsKey(currTileTime + 1) && GameGrid[TileToCheck].ContainsKey(currTileTime))
            {
                return false;
            }
        }
        //Checks related to moving Diagonal
        if (AreDiagonal(TileToCheck, currTilePos))
        {
            //Check if object want to pass diagonally through each other
            Vector3 adjPos1 = new(TileToCheck.x, 0, currTilePos.z);
            Vector3 adjPos2 = new(currTilePos.x, 0, TileToCheck.z);
            //Check if the two shared adjacent tiles have anything planned to be on them
            if (GameGrid.ContainsKey(adjPos1) && GameGrid.ContainsKey(adjPos2))
            {
                if (GameGrid[adjPos1].ContainsKey(currTileTime + 1) && GameGrid[adjPos2].ContainsKey(currTileTime))
                {
                    return false;
                }
                if (GameGrid[adjPos1].ContainsKey(currTileTime) && GameGrid[adjPos2].ContainsKey(currTileTime + 1))
                {
                    return false;
                }
            }
            
            //Check if trying to move diagonally through a wall/shelf
            if (GameGrid.ContainsKey(adjPos1) && GameGrid[adjPos1].ContainsKey(-10) ||
                GameGrid.ContainsKey(adjPos2) && GameGrid[adjPos2].ContainsKey(-10))
            {
                return false;
            }
        }

        //If nothing is saying this will be a problem, return true
        return true;
    }

    public Stack<Vector3> UnwrapPath(PathTile pathHolder)
    {
        Stack<Vector3> orderedPath = new();
        PathTile currPath = pathHolder;
        gridManager.AddObjectAt(currPath.GetPosition(), -5, this);
        while (true)
        {
            orderedPath.Push(currPath.GetPosition());
            gridManager.AddObjectAt(currPath.GetPosition(), currPath.GetStartDistance(), this);
            currPath = currPath.GetPrevious();
            if (currPath == null)
            {
                return orderedPath;
            }
        }
    }

    protected static List<T> ShuffleList<T>(List<T> list)
    {
        List<T> shuffledList = new List<T>(list);
        int n = shuffledList.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = shuffledList[k];
            shuffledList[k] = shuffledList[n];
            shuffledList[n] = value;
        }
        return shuffledList;
    }

    private bool AreDiagonal(Vector3 position1, Vector3 position2)
    {
        Vector3 diff = position1 - position2;
        return Mathf.Abs(diff.x) == 1 && Mathf.Abs(diff.z) == 1;
    }
}

public class PathTile
{
    private Vector3 position;
    private int startDistance;
    private float ghDistance;
    private PathTile previousTile;

    //Constructors
    public PathTile(Vector3 inPosition, int inStartDistance)
    {
        position = inPosition;
        startDistance = inStartDistance;
        ghDistance = float.MaxValue;
        previousTile = null;
    }

    public PathTile(Vector3 inPosition, int inStartDistance, PathTile inEndTile, PathTile inPreviousTile)
    {
        position = inPosition;
        startDistance = inStartDistance;
        ghDistance = GenerateGH(inEndTile);
        previousTile = inPreviousTile;
    }

    //Generates GH from distance to start and ManhattanDistance
    private float GenerateGH(PathTile inEnd)
    {
        float manhattanDistance = Math.Abs(position.x - inEnd.GetPosition().x) + Math.Abs(position.z - inEnd.GetPosition().z);
        return startDistance + manhattanDistance;
    }

    public Vector3 GetPosition() { return position; }
    public float GetGH() { return ghDistance; }
    public PathTile GetPrevious() { return previousTile; }
    public int GetStartDistance() { return startDistance; }
    public override string ToString()
    {
        string previousPosition = previousTile != null ? previousTile.GetPosition().ToString() : "null";
        return $"Position: {position}, Start Distance: {startDistance}, GH Distance: {ghDistance}, Previous Tile Position: {previousPosition}";
    }
}