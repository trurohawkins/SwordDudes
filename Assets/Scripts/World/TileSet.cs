using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSet : MonoBehaviour {
	Sprite[][] tiles;
	public Sprite[] set0;
	public Sprite[] set1;
	public Sprite[] set2;
	public Sprite[] set3;
	public int size;
	public bool edgePriority;
	public Color tileColor;
	public bool dynamic = false;

	void Awake() {
		tiles = new Sprite[4][];
		tiles [0] = set0;
		tiles [1] = set1;
		tiles [2] = set2;
		tiles [3] = set3;
	}

	public Sprite getTile(int s) {
		//Debug.Log ("tile: " + s);
		return tiles[s][Random.Range(0, tiles[s].Length)];
	}
}
