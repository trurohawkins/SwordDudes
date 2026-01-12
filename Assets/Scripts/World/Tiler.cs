using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiler : MonoBehaviour {
	public int id = -1;
	public int priority = -1;
    public GameObject[] tiles;
    public Tile[] set;
	public bool cornerPriority;
    bool[,] world;
    public Vector2 size;
	public Color color;
	public int sorting;
    Vector2Int[] d;
    Vector2Int[] d8;

    private void Awake() {
        d = new Vector2Int[4];
        d8 = new Vector2Int[8];
        d[0] = new Vector2Int(0, 1);
        d[1] = new Vector2Int(-1, 0);
        d[2] = new Vector2Int(0, -1);
        d[3] = new Vector2Int(1, 0);
        
        d8[0] = new Vector2Int(0, 1);
        d8[1] = new Vector2Int(-1, 1);
        d8[2] = new Vector2Int(-1, 0);
        d8[3] = new Vector2Int(-1, -1);
        d8[4] = new Vector2Int(0, -1);
        d8[5] = new Vector2Int(1, -1);
        d8[6] = new Vector2Int(1, 0);
        d8[7] = new Vector2Int(1, 1);

        set = new Tile[tiles.Length];
        for (int i = 0; i < tiles.Length; i++) {
            set[i] = Instantiate(tiles[i], transform).GetComponent<Tile>();
        }
		if (id < 0) {
			id = Random.Range(0, 9999999);
		}
    }

	/*
    public void setWorld(PathNode[,] w, int x, int y) {
        size = new Vector2Int(x, y);
        world = new bool[x,y];
        for (int i = 0; i < x; i++) {
            for (int j = 0; j < y; j++) {
                world[i, j] = !w[i, j].empty;
            }
        }
    }
	*/

	bool checkSpot(int x, int y) {
		if (x >= 0 && y >= 0 && x < Map.S.worldSizeX && y < Map.S.worldSizeY) {
			Tiler t = Map.S.world[x,y].getTiler();
			if (t) {
				if (t.id == id) {//Map.S.world[x,y].GetTiler() == this) {
					return true;
				}
			}
		}
		return false;
	}

    public int calcTile(int x, int y, SpriteRenderer sr) {
        if (x >= 0 && y >= 0 && x < Map.S.worldSizeX && y < Map.S.worldSizeY) {
            int mostOpen = -1;
			int start = 0;
			int startSide = start;
			for (int i = 0; i < 4; i++) {
				int openSides = 0;
				start = (start + i) % 4;
				for (int j = 0; j < 4; j++) {
					int cur = (start + j) % 4;
					//if (getFormID(x + d[cur][0], y + d[cur][1]) != t->typeID) {
					if (!checkSpot(x + d[cur][0], y + d[cur][1])) {
						openSides++;
					} else {
						break;
					}
				}
				if (openSides > mostOpen) {
					mostOpen = openSides;
					startSide = start;
				}
			}

			if (mostOpen == 1) {
				int oppoSide = (startSide + 2) % 4;
				//if (getFormID(x + d[oppoSide][0], y + d[oppoSide][1]) != t->typeID) {
				if (!checkSpot(x + d[oppoSide][0], y + d[oppoSide][1])) {
					mostOpen = 5;
				} else {
					int nextCorn = ((startSide*2) + 3) % 8;
					int preCorn = ((startSide*2) + 5) % 8;
					//float nc = getFormID(x + d8[nextCorn][0], y + d8[nextCorn][1]);
					bool nc = checkSpot(x + d8[nextCorn][0], y + d8[nextCorn][1]);
					//float pc = getFormID(x + d8[preCorn][0], y + d8[preCorn][1]);
					bool pc = checkSpot(x + d8[preCorn][0], y + d8[preCorn][1]);
					//if (nc != t->typeID && pc != t->typeID) {
					if (!nc && !pc) {
						mostOpen = 13;
					//} else if (nc != t->typeID){ 
					} else if (!nc){ 
						mostOpen = 11;
					//} else if (pc != t->typeID) {
					} else if (!pc) {
						mostOpen = 12; 
					}
				}
			} else if (mostOpen == 2) {
					int oppoCorn = ((startSide*2) + 5) % 8;
					//if (getFormID(x + d8[oppoCorn][0], y + d8[oppoCorn][1]) != t->typeID) {
					if (!checkSpot(x + d8[oppoCorn][0], y + d8[oppoCorn][1])) {
						mostOpen = 14;
					}
			} else if (mostOpen == 0) {
				int check = 0;
				int mostCorn = -1;
				start = 7;
				int s = 7;
				for (int i = 0; i < 4; i++) {
					int corners = 0;
					start = (s + (i * 2)) % 8;
					for (int j = 0; j < 4; j++) {
						int cur = (start + (j * 2)) % 8;
						//if (getFormID(x + d8[cur][0], y + d8[cur][1]) != t->typeID) {
						if (!checkSpot(x + d8[cur][0], y + d8[cur][1])) {
							corners++;
						} else {
							break;
						}
					}
					if (corners > mostCorn) {
						check = i;
						mostCorn = corners;
					}
				}
				if (mostCorn != 0) {
					mostOpen = 6 + (mostCorn -1);
					startSide = check;
				}
			}
			//Debug.Log(sr.transform.parent.name + " has " + mostOpen + " sides open and rotation of " + startSide);
			sr.sprite = set[mostOpen].getTile();
			sr.transform.eulerAngles = new Vector3 (0, 0, startSide * 90);
			return mostOpen;
		}
		return -1;
    }

	public Sprite getTile(int sidesOpen) {
		return set[sidesOpen].getTile();
	}
}
