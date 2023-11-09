using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class MovingGridObject : GridObject
{
    public float turnsPerTile = 1; //Number of tiles to walk in a single turn //Max 1
    protected Stack<Vector3> path;
    protected float moveTime; //Number of turns to move a spot
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
        moveTime = gridManager.turnInterval/(1/turnsPerTile);
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

        PathTile startTile = new PathTile(start, 0, turnsPerTile);
        PathTile endTile = new PathTile(end, 0, turnsPerTile);

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
                    PathTile newTile = new PathTile(TileToCheck, currTile.GetStartDistance() + turnsPerTile, endTile, currTile, turnsPerTile);
                    localFrontier.Add(new Tuple<PathTile, float>(newTile, newTile.GetGH()));
                }
            }
        }
        return localFrontier;
    }

    private bool CheckFrontierValidity(HashSet<Vector3> visitedTiles, Vector3 TileToCheck, PathTile currTile, PathTile endTile)
    {
        float currTileTime = currTile.GetStartDistance();
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
            if (dictToCheck.ContainsKey((int)Math.Ceiling(currTileTime + turnsPerTile)))
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
            if (GameGrid[currTilePos].ContainsKey((int)Math.Ceiling(currTileTime + turnsPerTile)) && GameGrid[TileToCheck].ContainsKey((int)currTileTime))
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
                if (GameGrid[adjPos1].ContainsKey((int)Math.Ceiling(currTileTime + turnsPerTile)) && GameGrid[adjPos2].ContainsKey((int)currTileTime))
                {
                    return false;
                }
                if (GameGrid[adjPos1].ContainsKey((int)currTileTime) && GameGrid[adjPos2].ContainsKey((int)Math.Ceiling(currTileTime + turnsPerTile)))
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
            for(int i = 0; i < turnsPerTile; i++) 
            {
                orderedPath.Push(currPath.GetPosition());
                gridManager.AddObjectAt(currPath.GetPosition(), (int)Math.Ceiling(currPath.GetStartDistance())-i, this);
                if((int)currPath.GetStartDistance() == 0) { break; }
            }
            currPath = currPath.GetPrevious();
            if (currPath == null)
            {
                return orderedPath;
            }
        }
    }

    public Vector3 UnwrapPathFirstStep(PathTile pathHolder)
    {
        PathTile currPath = pathHolder;
        PathTile prevPath = null;
        while (true)
        {
            prevPath = currPath.GetPrevious();
            
            if (prevPath.GetPrevious() == null)
            {
                gridManager.AddObjectAt(currPath.GetPosition(), (int)Math.Ceiling(currPath.GetStartDistance()), this);
                gridManager.AddObjectAt(currPath.GetPosition(), -5, this);
                return currPath.GetPosition();
            }
            currPath = prevPath;
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

    void PrintPath()
    {
        if (path == null) { return; }
        foreach (Vector3 point in path)
        {
            Debug.Log(point);
        }
    }
}

public class PathTile
{
    private Vector3 position;
    private float startDistance;
    private float ghDistance;
    private float turnsPerTile;
    private PathTile previousTile;

    //Constructors
    public PathTile(Vector3 inPosition, float inStartDistance, float inTurnsPerTile)
    {
        position = inPosition;
        startDistance = inStartDistance;
        ghDistance = float.MaxValue;
        previousTile = null;
        turnsPerTile = inTurnsPerTile;
    }

    public PathTile(Vector3 inPosition, float inStartDistance, PathTile inEndTile, PathTile inPreviousTile, float inTurnsPerTile)
    {
        position = inPosition;
        startDistance = inStartDistance;
        turnsPerTile = inTurnsPerTile;
        ghDistance = GenerateGH(inEndTile);
        previousTile = inPreviousTile;
    }

    //Generates GH from distance to start and ManhattanDistance
    private float GenerateGH(PathTile inEnd)
    {
        float manhattanDistance = Math.Abs(position.x - inEnd.GetPosition().x) + Math.Abs(position.z - inEnd.GetPosition().z);
        return startDistance + manhattanDistance * turnsPerTile;
    }

    public Vector3 GetPosition() { return position; }
    public float GetGH() { return ghDistance; }
    public PathTile GetPrevious() { return previousTile; }
    public float GetStartDistance() { return startDistance; }
    public override string ToString()
    {
        string previousPosition = previousTile != null ? previousTile.GetPosition().ToString() : "null";
        return $"Position: {position}, Start Distance: {startDistance}, GH Distance: {ghDistance}, Previous Tile Position: {previousPosition}";
    }
}