using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public GameObject cellPrefab;
    public GameObject wallPrefab1;
    public GameObject wallPrefab2;
    public GameObject wallPrefab3;
    public GameObject speedPowerup;
    public GameObject jumpPowerup;
    public GameObject healthPowerup;
    public GameObject princetonCollectible;
    public GameObject harvardCollectible;
    public GameObject yaleCollectible;
    public GameObject upennCollectible;
    public GameObject columbiaCollectible;
    public GameObject brownCollectible;
    public GameObject cornellCollectible;
    public GameObject dartmouthCollectible;
    // public MazeStateManager stateManager;

    public int width = 21;  // Must be odd numbers to have walls surrounding paths
    public int height = 21;

    public Cell[,] grid;
    private List<Vector2Int> wallList;
    public Vector2Int[] waypoints = new Vector2Int[9];

    void Start() {
        InitializeGrid();
        GenerateMaze();
        DrawMaze();
    }

    private void InitializeGrid() {
        grid = new Cell[width, height];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                grid[x, y] = new Cell();
            }
        }
    }

    void GenerateMaze() {
        wallList = new List<Vector2Int>();

        // Choose a random starting cell
        int startX = Random.Range(1, width - 1);
        int startY = Random.Range(1, height - 1);

        // Ensure starting on an odd coordinate
        startX = startX % 2 == 0 ? startX - 1 : startX;
        startY = startY % 2 == 0 ? startY - 1 : startY;

        // Mark the starting cell as a passage
        grid[startX, startY].IsVisited = true;
        grid[startX, startY].IsWall = false;

        // Add the neighboring walls to the wall list
        AddWallsToList(startX, startY);
        
        // Main loop of the algorithm
        while (wallList.Count > 0) {
            // Select a random wall from the list
            int randomIndex = Random.Range(0, wallList.Count);
            Vector2Int wall = wallList[randomIndex];
            wallList.RemoveAt(randomIndex);

            // Check if the wall divides a visited and unvisited cell
            ProcessWall(wall);
        }

        bool[,] mazeData = new bool[width, height];
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                mazeData[i, j] = grid[i, j].IsWall;
            }
        }
        System.Random rand = new System.Random(); // For consistent randomization

        // for (int i = 0; i < width; i++) {
        //     for (int j = 0; j < height; j++) {
        //         GameObject cellObj = Instantiate(cellPrefab, new Vector3(i, 0, j), Quaternion.identity);
        //         CellState cellState = cellObj.GetComponent<CellState>();
        //         cellState.x = i;
        //         cellState.y = j;
        //         cellState.isWalkable = !mazeData[i,j];
        //         stateManager.RegisterCell(cellState);
        //         // Assign random reward/penalty
        //         double roll = rand.NextDouble();
        //         if (roll < 0.2)       
        //             cellState.hiddenReward = 10f;   // good cell
        //         else if (roll < 0.4)  
        //             cellState.hiddenReward = -5f;   // bad cell
        //         else  
        //             cellState.hiddenReward = 0f;    // neutral cell

        //         stateManager.RegisterCell(cellState);

        //     }
        // }
        // // Set dimensions on stateManager for out-of-bounds checks
        // stateManager.width = width;
        // stateManager.height = height;
    }

    void AddWallsToList(int x, int y) {
        if (x - 2 > 0) 
            wallList.Add(new Vector2Int(x - 1, y));

        if (x + 2 < width - 1) 
            wallList.Add(new Vector2Int(x + 1, y));

        if (y - 2 > 0)
            wallList.Add(new Vector2Int(x, y - 1));

        if (y + 2 < height - 1)
            wallList.Add(new Vector2Int(x, y + 1));
    }

    void ProcessWall(Vector2Int wall) {
        int x = wall.x;
        int y = wall.y;

        // Determine the cells on either side of the wall
        List<Vector2Int> neighbors = GetNeighbors(x, y);

        if (neighbors.Count == 2)
        {
            Cell cell1 = grid[neighbors[0].x, neighbors[0].y];
            Cell cell2 = grid[neighbors[1].x, neighbors[1].y];

            // If one of the cells is unvisited
            if (cell1.IsVisited != cell2.IsVisited)
            {
                // Make the wall a passage
                grid[x, y].IsWall = false;

                // Mark the unvisited cell as visited
                if (!cell1.IsVisited)
                {
                    cell1.IsVisited = true;
                    cell1.IsWall = false;
                    AddWallsToList(neighbors[0].x, neighbors[0].y);
                }
                else
                {
                    cell2.IsVisited = true;
                    cell2.IsWall = false;
                    AddWallsToList(neighbors[1].x, neighbors[1].y);
                }
            }
        }
    }

    List<Vector2Int> GetNeighbors(int x, int y) {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        if (x % 2 == 1)
        {
            // Vertical wall
            if (y - 1 >= 0) neighbors.Add(new Vector2Int(x, y - 1));
            if (y + 1 < height) neighbors.Add(new Vector2Int(x, y + 1));
        }
        else if (y % 2 == 1)
        {
            // Horizontal wall
            if (x - 1 >= 0) neighbors.Add(new Vector2Int(x - 1, y));
            if (x + 1 < width) neighbors.Add(new Vector2Int(x + 1, y));
        }

        return neighbors;
    }

    void DrawMaze() {
        // Initialize waypoints
        for (int i = 0; i <= 8; i++) {
            int xSect = i / 3;
            int ySect = i % 3;
            int wayIndex = Random.Range(0, 9);
            while (waypoints[wayIndex] != new Vector2Int(0, 0)) wayIndex = Random.Range(0, 9);
            waypoints[wayIndex] = new Vector2Int(Random.Range(xSect * 11 + 1, (xSect + 1) * 11), Random.Range(ySect * 11 + 1, (ySect + 1) * 11));
            waypoints[wayIndex].x = (waypoints[wayIndex].x % 2 == 0) ? waypoints[wayIndex].x - 1 : waypoints[wayIndex].x;
            waypoints[wayIndex].y = (waypoints[wayIndex].y % 2 == 0) ? waypoints[wayIndex].y - 1 : waypoints[wayIndex].y;
        }

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Vector3 position = new Vector3(x, 0, y);
                if (grid[x, y].IsWall && !(x == width - 1 && y == height - 2)) {
                    int currentTree = Random.Range(0, 3);
                    if (currentTree == 0) Instantiate(wallPrefab1, position, Quaternion.identity, transform);
                    else if (currentTree == 1) Instantiate(wallPrefab2, position, Quaternion.identity, transform);
                    else if (currentTree == 2) Instantiate(wallPrefab3, position, Quaternion.identity, transform);
                }
                else if (waypoints.Contains(new Vector2Int(x, y))) {
                    position = new Vector3(position.x, 0.55f, position.z);
                    int index = waypoints.ToList().FindIndex(element => element == new Vector2Int(x, y));
                    if (index == 0) Instantiate(princetonCollectible, position, Quaternion.identity, transform);
                    else if (index == 1) Instantiate(harvardCollectible, position, Quaternion.identity, transform);
                    else if (index == 2) Instantiate(yaleCollectible, position, Quaternion.identity, transform);
                    else if (index == 3) Instantiate(columbiaCollectible, position, Quaternion.identity, transform);
                    else if (index == 4) Instantiate(cornellCollectible, position, Quaternion.identity, transform);
                    else if (index == 5) Instantiate(brownCollectible, position, Quaternion.identity, transform);
                    else if (index == 6) Instantiate(upennCollectible, position, Quaternion.identity, transform);
                    else if (index == 7) Instantiate(dartmouthCollectible, position, Quaternion.identity, transform);
                }
                else {
                    int currentPowerup = Random.Range(0, 80);
                    position = new Vector3(position.x, 0.55f, position.z);
                    if (!(x == 1 && y == 1)) {
                        if (currentPowerup == 0) Instantiate(speedPowerup, position, Quaternion.identity, transform);
                        else if (currentPowerup == 1) Instantiate(jumpPowerup, position, Quaternion.identity, transform);
                        else if (currentPowerup == 2) Instantiate(healthPowerup, position, Quaternion.identity, transform);
                    }
                }
            }
        }
    }
}
