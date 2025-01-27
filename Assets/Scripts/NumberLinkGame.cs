using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberLinkGame : MonoBehaviour {
    public Camera theCamera;
    private CameraController cameraController;
    public Number[,] allTiles;
    public List<Number> allNumbers;
    public GameObject numberPrefab;
    public GameObject boardPrefab;
    private GameObject board;
    public Number numberTouching;
    public int currentPuzzle;

    void Start() {
        cameraController = theCamera.GetComponent<CameraController>();
    }

    public void Initialize() {
        int currentPuzzle = Random.Range(0, 5);
        if (currentPuzzle == 0) { InitializeGame(new Vector2Int(6, 6), new List<(int, int, int, int, float, float, float)> {(3, 3, 3, 0, 1f, 0f, 0f), (4, 4, 4, 1, 0f, 0.5f, 0f), (3, 1, 2, 0, 0f, 0f, 1f), (1, 4, 2, 1, 1f, 1f, 0f)}); }
        else if (currentPuzzle == 1) { InitializeGame(new Vector2Int(6, 6), new List<(int, int, int, int, float, float, float)> {(2, 3, 4, 1, 1f, 0f, 0f), (3, 4, 3, 1, 0f, 0.5f, 0f), (3, 5, 5, 4, 0f, 0f, 1f), (3, 3, 5, 5, 1f, 1f, 0f)}); }
        else if (currentPuzzle == 2) { InitializeGame(new Vector2Int(6, 6), new List<(int, int, int, int, float, float, float)> {(2, 3, 4, 1, 1f, 0f, 0f), (3, 2, 0, 1, 0f, 0.5f, 0f), (3, 1, 0, 0, 0f, 0f, 1f), (1, 3, 3, 0, 1f, 1f, 0f), (0, 2, 5, 5, 1f, 0.5f, 0f)}); }
        else if (currentPuzzle == 3) { InitializeGame(new Vector2Int(6, 6), new List<(int, int, int, int, float, float, float)> {(1, 1, 3, 2, 1f, 0f, 0f), (1, 0, 3, 1, 0f, 0.5f, 0f), (5, 0, 3, 0, 0f, 0f, 1f), (3, 3, 2, 0, 1f, 1f, 0f)}); }
        else if (currentPuzzle == 4) { InitializeGame(new Vector2Int(6, 6), new List<(int, int, int, int, float, float, float)> {(0, 4, 1, 3, 1f, 0f, 0f), (0, 5, 2, 3, 0f, 0.5f, 0f), (1, 0, 3, 4, 0f, 0f, 1f), (2, 5, 2, 0, 1f, 1f, 0f), (0, 0, 3, 3, 1f, 0.5f, 0f)}); }
    }

    void Update() {
        if (Input.GetMouseButtonDown(0) && allTiles != null) {
            Vector2Int tileOn = coordToTile(Input.mousePosition, allTiles.GetLength(1), allTiles.GetLength(0), 0);
            if (!(allTiles[tileOn.x, tileOn.y] == null)) {
                numberTouching = allTiles[tileOn.x, tileOn.y];
                if (!(numberTouching == null) && (tileOn == new Vector2Int(numberTouching.startPos.x, numberTouching.startPos.y) | tileOn == new Vector2Int(numberTouching.endPos.x, numberTouching.endPos.y)))
                    numberTouching.Empty();
            }
        }
        else if (Input.GetMouseButtonUp(0) && allTiles != null) { numberTouching = null; }

        bool allSolved = true;
        foreach (Number num in allNumbers)
            if (!num.solved) allSolved = false;
        if (allSolved) {
            if (allNumbers.Count > 0) cameraController.inPuzzle = false;
            Destroy(board);
            foreach (Number num in allNumbers)
                Destroy(num.gameObject);
            allNumbers = new List<Number>();
            allTiles = null;
        }
    }

    void InitializeGame(Vector2Int boardSize, List<(int, int, int, int, float, float, float)> numberData) {
        allTiles = new Number[boardSize.x, boardSize.y];
        board = Instantiate(boardPrefab, Vector3.zero, Quaternion.identity, transform);
        board.GetComponent<Board>().Initialize(new Vector2Int(boardSize.x, boardSize.y));
        foreach ((int, int, int, int, float, float, float) number in numberData) {
            GameObject currentNumber = Instantiate(numberPrefab, Vector3.zero, Quaternion.identity, transform);
            currentNumber.GetComponent<Number>().Initialize(new Vector2Int(number.Item1, number.Item2), new Vector2Int(number.Item3, number.Item4), new Color(number.Item5, number.Item6, number.Item7));
            allNumbers.Add(currentNumber.GetComponent<Number>());
        }
    }

    public Vector2Int coordToTile(Vector3 pos, int sizeX, int sizeY, int distFromTop) {
        int tileSize = (Screen.height - 2 * distFromTop) / sizeX;
        int x = Mathf.Clamp(Mathf.FloorToInt((pos.x - 0.5f * (Screen.width - Screen.height + 2f * distFromTop)) / tileSize), 0, sizeX - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt((pos.y - distFromTop) / tileSize), 0, sizeY - 1);
        return new Vector2Int(x, y);
    }

    public Vector3 tileToCoord(Vector2 pos, int sizeX, int sizeY) {
        return new Vector3(1.5f + (5f/sizeX) * (pos.x - 1f), 1.5f + (5f/sizeY) * (pos.y - 1), -1f);
    }
}