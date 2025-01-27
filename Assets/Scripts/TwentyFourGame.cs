using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class TwentyFourGame : MonoBehaviour {
    
    public Camera theCamera;
    private CameraController cameraController;
    public GameObject bgPrefab;
    public GameObject tilePrefab;
    public GameObject plusPrefab;
    public GameObject minusPrefab;
    public GameObject timesPrefab;
    public GameObject dividePrefab;
    private GameObject currentBG;
    private GameObject co;
    private GameObject[] allObjects = new GameObject[8];
    private int[] allNums = new int[4];
    private Color[] allColors = {Color.red, Color.blue, Color.magenta, Color.yellow, new Color(1f, 0.75f, 0.75f), new Color(0.75f, 0.75f, 1f), new Color(0.875f, 0.75f, 0.875f), new Color(1f, 1f, 0.75f)};
    private List<int[]> allMoves = new List<int[]>();
    private int[,] allPuzzles = {{2,6,6,3}, {3,4,2,6}, {3,7,2,5}, {5,3,8,2}, {3,7,1,2}, {1,4,3,4}, {6,5,2,4}, {5,4,8,4}, {3,8,4,5}, {3,2,9,1}, {7,6,1,2}, {1,5,1,4}};
    private int selectedTile = -1;
    private int selectedOperation = -1;

    void Start() {
        cameraController = theCamera.GetComponent<CameraController>();
    }

    public void Initialize() {
        currentBG = Instantiate(bgPrefab);
        allObjects[0] = Instantiate(tilePrefab, new Vector3(1.5f, 2.75f, -1f), Quaternion.identity, transform);
        allObjects[1] = Instantiate(tilePrefab, new Vector3(4f, 2.75f, -1f), Quaternion.identity, transform);
        allObjects[2] = Instantiate(tilePrefab, new Vector3(1.5f, 4.25f, -1f), Quaternion.identity, transform);
        allObjects[3] = Instantiate(tilePrefab, new Vector3(4f, 4.25f, -1f), Quaternion.identity, transform);
        allObjects[4] = Instantiate(plusPrefab, new Vector3(0.5f, 1.25f, -1f), Quaternion.Euler(90f, 0f, 0f), transform);
        allObjects[5] = Instantiate(minusPrefab, new Vector3(2f, 1.25f, -1f), Quaternion.Euler(90f, 0f, 0f), transform);
        allObjects[6] = Instantiate(timesPrefab, new Vector3(3.5f, 1.25f, -1f), Quaternion.Euler(90f, 0f, 0f), transform);
        allObjects[7] = Instantiate(dividePrefab, new Vector3(5f, 1.25f, -1f), Quaternion.Euler(90f, 0f, 0f), transform);
        int r = UnityEngine.Random.Range(0, allPuzzles.GetLength(0));
        for (int i = 0; i < 4; i++)
            allNums[i] = allPuzzles[r, i];
        allMoves.Add((int[])allNums.Clone());
    }

    void Update() {
        if (currentBG != null) {
            if (Input.GetKeyDown(KeyCode.Z) && allMoves.Count > 1) {
                allMoves.RemoveAt(allMoves.Count - 1);
                allNums = (int[])allMoves[allMoves.Count - 1].Clone();
                selectedTile = -1;
                selectedOperation = -1;
            }
            else if (Input.GetMouseButtonDown(0)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                    co = hit.collider.gameObject;
            }

            bool a = co == allObjects[0] | co == allObjects[1] | co == allObjects[2] | co == allObjects[3];
            bool b = (co == allObjects[0] && selectedTile == 0) | (co == allObjects[1] && selectedTile == 1) | (co == allObjects[2] && selectedTile == 2) | (co == allObjects[3] && selectedTile == 3);
            bool c = (selectedOperation == 1 && Array.IndexOf(allObjects, co) < 4 && Array.IndexOf(allObjects, co) >= 0) ? Valid(allNums[selectedTile], allNums[Array.IndexOf(allObjects, co)], selectedOperation) : true;
            bool d = (selectedOperation == 3 && Array.IndexOf(allObjects, co) < 4 && Array.IndexOf(allObjects, co) >= 0) ? Valid(allNums[selectedTile], allNums[Array.IndexOf(allObjects, co)], selectedOperation) : true;
            bool e = (co == allObjects[0] && allNums[0] == -1) | (co == allObjects[1] && allNums[1] == -1) | (co == allObjects[2] && allNums[2] == -1) | (co == allObjects[3] && allNums[3] == -1);

            if (b)
                selectedOperation = -1;
            if (a && !b && selectedTile > -1 && selectedOperation > -1 && c && d && !e) {
                if (co == allObjects[0]) {
                    allNums[0] = Operate(allNums[selectedTile], allNums[0], selectedOperation);
                    allNums[selectedTile] = -1;
                    selectedTile = 0; }
                else if (co == allObjects[1]) {
                    allNums[1] = Operate(allNums[selectedTile], allNums[1], selectedOperation);
                    allNums[selectedTile] = -1;
                    selectedTile = 1; }
                else if (co == allObjects[2]) {
                    allNums[2] = Operate(allNums[selectedTile], allNums[2], selectedOperation);
                    allNums[selectedTile] = -1;
                    selectedTile = 2; }
                else if (co == allObjects[3]) {
                    allNums[3] = Operate(allNums[selectedTile], allNums[3], selectedOperation);
                    allNums[selectedTile] = -1;
                    selectedTile = 3; }
                selectedOperation = -1;
                allMoves.Add((int[])allNums.Clone());
            }
            else if (a && !b && selectedTile > -1 && selectedOperation > -1 && !(c && d) && !e) {
                selectedTile = -1;
                selectedOperation = -1;
            }
            else if (!e) {
                if (co == allObjects[0]) selectedTile = (selectedTile == 0) ? -1 : 0;
                else if (co == allObjects[1]) selectedTile = (selectedTile == 1) ? -1 : 1;
                else if (co == allObjects[2]) selectedTile = (selectedTile == 2) ? -1 : 2;
                else if (co == allObjects[3]) selectedTile = (selectedTile == 3) ? -1 : 3;
            }

            if (co == allObjects[4]) selectedOperation = (selectedOperation == 0 | selectedTile == -1) ? -1 : 0;
            else if (co == allObjects[5]) selectedOperation = (selectedOperation == 1 | selectedTile == -1) ? -1 : 1;
            else if (co == allObjects[6]) selectedOperation = (selectedOperation == 2 | selectedTile == -1) ? -1 : 2;
            else if (co == allObjects[7]) selectedOperation = (selectedOperation == 3 | selectedTile == -1) ? -1 : 3;

            for (int i = 0; i < 4; i++) {
                allObjects[i].GetComponentInChildren<TextMeshPro>().text = allNums[i].ToString();
                if (i == selectedTile) allObjects[i].GetComponent<Renderer>().material.color = new Color(1f, 0.25f, 0.25f);
                else allObjects[i].GetComponent<Renderer>().material.color = new Color(1f, 0.7f, 0.6f);
                if (allNums[i] == -1) {
                    allObjects[i].GetComponent<MeshRenderer>().enabled = false;
                    allObjects[i].GetComponentInChildren<TextMeshPro>().enabled = false; }
                else {
                    allObjects[i].GetComponent<MeshRenderer>().enabled = true;
                    allObjects[i].GetComponentInChildren<TextMeshPro>().enabled = true; }
                if (i == selectedOperation) allObjects[i+4].GetComponent<Renderer>().material.color = allColors[i];
                else allObjects[i+4].GetComponent<Renderer>().material.color = allColors[i+4];

                if (allNums.SequenceEqual(new int[4] {24, -1, -1, -1}) || allNums.SequenceEqual(new int[4] {-1, 24, -1, -1}) || allNums.SequenceEqual(new int[4] {-1, -1, 24, -1}) || allNums.SequenceEqual(new int[4] {-1, -1, -1, 24})) {
                    cameraController.inPuzzle = false;
                    Destroy(allObjects[i]);
                    Destroy(allObjects[i+4]);
                    Destroy(currentBG);
                    allMoves = new List<int[]>();
                    selectedTile = -1;
                    selectedOperation = -1; }
            }   
        }

        co = null;
    }

    int Operate(int a, int b, int operation) {
        if (operation == 0) return a + b;
        else if (operation == 1) return a - b;
        else if (operation == 2) return a * b;
        else return a / b;
    }

    bool Valid(int a, int b, int operation) {
        // Debug.Log((a, b, operation));
        if (operation == 0) return true;
        else if (operation == 1) return (a - b) >= 0;
        else if (operation == 2) return true;
        else return (float)(a / b) == (float)a / (float)b;
    }
}