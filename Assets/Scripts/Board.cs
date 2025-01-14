using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
    public NumberLinkGame game;
    public Vector2Int size;
    public GameObject cellPrefab;
    public GameObject outlinePrefab;

    public void Initialize(Vector2Int dimensions) {
        size = dimensions;
    }

    void Start() {
        game = GameObject.Find("Numberlink Game Manager").GetComponent<NumberLinkGame>();
        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                GameObject currentCell = Instantiate(outlinePrefab, game.tileToCoord(new Vector2(x, y), size.x, size.y), Quaternion.identity, transform);
                GameObject currentOutline = Instantiate(cellPrefab, game.tileToCoord(new Vector2(x, y), size.x, size.y), Quaternion.identity, transform);
                currentCell.transform.localScale = currentCell.transform.localScale * (5f/size.x);
                currentOutline.transform.localScale = currentOutline.transform.localScale * (5f/size.x);
                Vector3 pos = currentOutline.transform.position;
                currentOutline.transform.position = new Vector3(pos.x, pos.y, pos.z - 0.01f);
            }
        }
    }
}