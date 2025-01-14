using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Number : MonoBehaviour {
    private NumberLinkGame game;
    public GameObject dotPrefab;
    public GameObject pathPrefab;
    public GameObject vertexPrefab;
    GameObject currentPath;
    GameObject currentVertex;
    public Vector2Int startPos;
    public Vector2Int endPos;
    public Color color;
    public List<Vector2Int> path;
    public bool solved = false;

    void Start() {
        game = GameObject.Find("Numberlink Game Manager").GetComponent<NumberLinkGame>();
        game.allTiles[startPos.x, startPos.y] = this;
        game.allTiles[endPos.x, endPos.y] = this;
    }

    public void Initialize(Vector2Int start, Vector2Int end, Color col) {
        startPos = start;
        endPos = end;
        color = col;
    }

    void Update() {
        Vector2Int currentTile = game.coordToTile(Input.mousePosition, game.allTiles.GetLength(1), game.allTiles.GetLength(0), 0);
        if (game.numberTouching == this) {
            if (path.Contains(currentTile) && currentTile != path[path.Count - 1]) {
                solved = false;
                for (int i = 0; i < path.Count; i++)
                    game.allTiles[path[i].x, path[i].y] = null;
                path = path.GetRange(0, path.IndexOf(currentTile) + 1);
                for (int i = 0; i < path.Count; i++)
                    game.allTiles[path[i].x, path[i].y] = this;
            }
            else if ((!path.Contains(currentTile)) && (game.allTiles[currentTile.x, currentTile.y] is null | game.allTiles[currentTile.x, currentTile.y] == this) && ((path.Count > 0) ? (Vector2.Distance((Vector2)path[path.Count - 1], (Vector2)currentTile) == 1.0) : true) && ((path.Count > 1) ? (path[path.Count - 1] != new Vector2Int(startPos.x, startPos.y) && path[path.Count - 1] != new Vector2Int(endPos.x, endPos.y)) : true)) {
                solved = false;
                path.Add(currentTile);
                game.allTiles[currentTile.x, currentTile.y] = this;
            }
            else if (path.Count > 1 && ((path[path.Count - 1] == new Vector2Int(startPos.x, startPos.y)) || (path[path.Count - 1] == new Vector2Int(endPos.x, endPos.y)))) {
                solved = true;
            }
        }

        GameObject[] allGameObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allGameObjects) {
            if (obj.CompareTag("Dot") | obj.CompareTag("Path") && (obj.GetComponent<Renderer>().material.color == color)) {
                Destroy(obj);
            }
        }
        Debug.Log(game.coordToTile(Input.mousePosition, game.allTiles.GetLength(1), game.allTiles.GetLength(0), 0));
        Vector2Int size = new Vector2Int(game.allTiles.GetLength(1), game.allTiles.GetLength(0));
        Vector3 startRectCenter = new Vector3(game.tileToCoord(startPos, size.x, size.y).x, game.tileToCoord(startPos, size.x, size.y).y, 0f);
        Vector3 endRectCenter = new Vector3(game.tileToCoord(endPos, size.x, size.y).x, game.tileToCoord(endPos, size.x, size.y).y, 0f);
        GameObject currentStart = Instantiate(dotPrefab, startRectCenter, Quaternion.Euler(90f, 0f, 0f), transform);
        GameObject currentEnd = Instantiate(dotPrefab, endRectCenter, Quaternion.Euler(90f, 0f, 0f), transform);
        currentStart.transform.localScale = currentStart.transform.localScale * (5f/size.x);
        currentEnd.transform.localScale = currentEnd.transform.localScale * (5f/size.x);
        currentStart.GetComponent<Renderer>().material.color = color;
        currentEnd.GetComponent<Renderer>().material.color = color;
        currentStart.tag = "Dot";
        currentEnd.tag = "Dot";
        for (int tile = 1; tile < path.Count; tile++) {
            Vector2Int relativeMove = path[tile] - path[tile - 1];
            Quaternion pathRotate = (relativeMove.y == 0) ? Quaternion.identity : Quaternion.Euler(0f, 0f, 90f);
            Vector3 pathCoords = game.tileToCoord(path[tile], size.x, size.y);
            pathCoords.x -= (relativeMove == new Vector2Int(1, 0)) ? (2.5f/size.x) : (relativeMove == new Vector2Int(-1, 0) ? (-2.5f/size.x) : 0);
            pathCoords.y -= (relativeMove == new Vector2Int(0, 1)) ? (2.5f/size.x): (relativeMove == new Vector2Int(0, -1) ? (-2.5f/size.x) : 0);
            currentPath = Instantiate(pathPrefab, pathCoords, pathRotate, transform);
            currentPath.GetComponent<Renderer>().material.color = color;
            currentPath.tag = "Path";
            currentPath.transform.localScale = currentPath.transform.localScale * (5f/size.x);
            currentVertex = Instantiate(vertexPrefab, game.tileToCoord(path[tile], size.x, size.y), Quaternion.Euler(90f, 0f, 0f), transform);
            currentVertex.GetComponent<Renderer>().material.color = color;
            currentVertex.tag = "Path";
            currentVertex.transform.localScale = currentVertex.transform.localScale * (5f/size.x);
        }
        game.allTiles[startPos.x, startPos.y] = this;
        game.allTiles[endPos.x, endPos.y] = this;
    }

    public void Empty() {
        for (int i = 0; i < path.Count; i++)
            game.allTiles[path[i].x, path[i].y] = null;
        game.allTiles[startPos.x, startPos.y] = this;
        game.allTiles[endPos.x, endPos.y] = this;
        path = new List<Vector2Int>();
    }
}   