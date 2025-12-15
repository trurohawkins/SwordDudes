using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
    public Sprite[] tiles;

    public Sprite getTile() {
        return tiles[Random.Range(0, tiles.Length)];
    }
}
