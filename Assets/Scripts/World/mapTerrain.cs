using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapTerrain : MonoBehaviour {

	public GameObject DebugCell;
	public Vector3Int[] spawns;
	bool[] spawnsClosed;
	public GameObject lava;
	public GameObject water;
	public GameObject woodFloor;
	public GameObject rock;
	public GameObject ruin;
	public GameObject RuinBridge;
	public GameObject debugFloor;

	public Color[] floors;
	public GameObject[] tFloors;
	public GameObject[] walls;
	public Vector2Int[] worldSizes;
	public Vector2Int dungeonSize;
	public bool forceSpawn = false;
	public Vector3Int[] forceSpawns;
	int force = 0;
	int xEdge;
	int yEdge;
	public GameObject floor;
	public GameObject pathParent;
	SpriteRenderer floorRend;
	int map = 0;

	public void spawnWorld(){
		if (GameInfo.S) {
			map = GameInfo.S.level;
		}
		if (map != -1) {
			Map.S.worldSizeX = xEdge = worldSizes[map].x;
			Map.S.worldSizeY = yEdge = worldSizes[map].y;
		} else {
			Map.S.worldSizeX = xEdge = dungeonSize.x;
			Map.S.worldSizeY = yEdge = dungeonSize.y;
		}
		Map.S.world = new Cell[xEdge, yEdge];
		for (int x = 0; x < xEdge; x++) {
			for (int y = 0; y < yEdge; y++) {
				GameObject tmp = Instantiate (DebugCell, new Vector3 (x, y, 0), transform.rotation, transform) as GameObject;
				Map.S.world [x, y] = tmp.GetComponent<Cell> ();
				Map.S.world [x, y].heightMod += yEdge - y*10;
				Map.S.world[x,y].setPos(x, y);
			}
		}
	}

	public void deleteWorld() {
		for (int i = 0; i < Map.S.worldSizeX; i++) {
			for (int j = 0; j < Map.S.worldSizeY; j++) {
				Destroy(Map.S.world[i,j].gameObject);
			}
		}
	}

	public GameObject dungeonRoom;
	public GameObject dungeonMaster;
	DungeonMaster dm;
	Form[] doors;
	DungeonRoom curRoom;
	int dunCount = 0;
	//direction we are coming from
	public void adventureRoom(int dir) {
		if (!dm) {
			GameObject tmp = Instantiate (walls[1]);
			Map.S.terrain = tmp.GetComponent<Form> ();
			if (GM.S.drawSprites) {
				spawnBgFloor(1);
			} else {
				bgFloor = Instantiate (debugFloor, new Vector3 (xEdge / 2f, yEdge / 2f, 0.2f), transform.rotation);
				bgFloor.transform.localScale = new Vector3 (xEdge, yEdge, 1);
				floorRend = bgFloor.GetComponent<SpriteRenderer> ();
				bgFloor.GetComponent<SpriteRenderer>().color = floors [1];
			}
			dm = Instantiate(dungeonMaster).GetComponent<DungeonMaster>();//gameObject.GetComponent<DungeonMaster>();
			GM.S.curDM = dm;
			dm.getCreator(this);
		}
		//dm.loadRoom(dir);
		//fullWorld(true);
		spawns = new Vector3Int[4];
		spawnsClosed = new bool[4];
		dm.loadRoom(dir);
		//curRoom.spawnRoom(dir);
	}

	public void fullWorld(bool full) {
		for (int x = 0; x < xEdge; x++) {
			for (int y = 0; y < yEdge; y++) {
				if (!full) {
					Map.S.world[x, y].empty();
				} else {
					Map.S.world [x, y].fill ();
				}
			}
		}
	}

	GameObject bgFloor;

	public void spawnBgFloor(int map) {
		if (bgFloor) {
			Destroy(bgFloor);
		}
		bgFloor = Instantiate (tFloors [map], new Vector3 (xEdge / 2f, yEdge / 2f, 0.2f), transform.rotation);
		bgFloor.transform.localScale = new Vector3 ((xEdge - 1) / 10f, 1, (yEdge - 1) / 10f);
		bgFloor.transform.eulerAngles = new Vector3 (-90, 0, 0);
	}

	public void setWallColor(Color c) {
		if (Map.S.terrain) {
			Tiler t = Map.S.terrain.GetComponent<Tiler>();
			t.color = c;
		}
	}

	public void createLevel() {
		if (map == -1) {
			adventureRoom(-1);
			return;
		}
		GameObject tmp = Instantiate (walls[map]);
		Map.S.terrain = tmp.GetComponent<Form> ();
		float xe = xEdge;
		float ye = yEdge;
		if (GM.S.drawSprites) {
			spawnBgFloor(map);
		} else {
			bgFloor = Instantiate (debugFloor, new Vector3 (xe / 2f, ye / 2f, 0.2f), transform.rotation);
			bgFloor.transform.localScale = new Vector3 (xe, ye, 1);
			floorRend = bgFloor.GetComponent<SpriteRenderer> ();
			bgFloor.GetComponent<SpriteRenderer>().color = floors [map];
		}
		if (map == 1) {
			Tiler t = tmp.GetComponent<Tiler>();
			t.color = new Color(0.15f, 0.1f, 0.2f);
		}
		//Debug.Log ("level creating");

		for (int x = 0; x < xEdge; x++) {
			for (int y = 0; y < yEdge; y++) {
				if (map == 0 || map == 4 || map == 2) {
					if (x == 0 || x == xEdge - 1 || y == 0 || y == yEdge - 1) {
						Map.S.world [x, y].fill ();
					}
				} else {
					Map.S.world [x, y].fill ();
				}
			}
		}

		if (map == 1) {
			float smallEdge = 0.18f;
			float bigEdge = 0.82f;
			int size = (int)(xEdge * 0.27f);

			clearCircle (new Vector2Int ((int)(xEdge * smallEdge), (int)(yEdge * smallEdge)), size);
			clearCircle (new Vector2Int ((int)(xEdge * bigEdge), (int)(yEdge * smallEdge)), size);
			clearCircle (new Vector2Int ((int)(xEdge * smallEdge), (int)(yEdge * bigEdge)), size);
			clearCircle (new Vector2Int ((int)(xEdge * bigEdge), (int)(yEdge * bigEdge)), size);

			clearCircle (new Vector2Int (xEdge / 2, yEdge / 2), (int)(xEdge * 0.48f));

			size = (int)(xEdge * 0.1f);
			int sizeY = (int)(yEdge * 0.45f);
			
			clearRect (new Vector2Int ((int)(xEdge * smallEdge), yEdge / 2), size, sizeY);
			clearRect (new Vector2Int ((int)(xEdge * bigEdge), yEdge / 2), size, sizeY);

			clearRect (new Vector2Int (xEdge / 2, yEdge / 2), (int)(xEdge * 0.65f), (int)(yEdge * 0.2f));
			
		} else if (map == 2) {
			
			Form waterForm = Instantiate (water).GetComponent<Form> ();
			Form wfForm = Instantiate (woodFloor).GetComponent<Form> ();
			Form rockForm = Instantiate (rock).GetComponent<Form> ();
			Form ruinForm = Instantiate (ruin).GetComponent<Form> ();
			int waterEdge = (int)(xEdge * 0.0583f);
			int ex = xEdge - waterEdge;
			int ey = yEdge - waterEdge;

			fillAngle (waterForm, 47, 0, (int)(-yEdge * 0.3f), (int)Mathf.Sqrt (Mathf.Pow ((ex), 2) + Mathf.Pow ((ey), 2)) + (int)(xEdge * 0.187f), (int)(yEdge * 0.125f));
			//clearAngle (324, (int)(ex * 0.26), (int)(ey * 0.24), (int)(ex * 0.19f), (int)(ey * 0.12f));


			Vector2Int lakeCenter = new Vector2Int ((int)(ex * 0.65f), (int)(ey * 0.65f));
			fillCircle (waterForm, lakeCenter, (int)(ex * 0.4f));
			lakeCenter += new Vector2Int (0, (int)(ey * 0.03f));
			clearCircle (lakeCenter, (int)(ex * 0.16f));//, (int)(ex * 0.16f));
			clearCircle (lakeCenter + new Vector2Int ((int)(ex * 0.05f), -(int)(ey * 0.075f)), (int)(ex * 0.1f));
			clearCircle (lakeCenter + new Vector2Int (-(int)(ex * 0.04f), -(int)(ey * 0.075f)), (int)(ex * 0.11f));
			for (int i = 0; i < xEdge; i++) {
				for (int j = 0; j < waterEdge; j++) {
					Map.S.world [i, yEdge - 1 - j].formEnter (waterForm);
					Map.S.world [i, 0 + j].formEnter (waterForm);
				}
			}
			for (int i = 0; i < yEdge; i++) {
				for (int j = 0; j < waterEdge; j++) {
					Map.S.world [xEdge - 1 - j, i].formEnter (waterForm);
					Map.S.world [0 + j, i].formEnter (waterForm);
				}
			}

			//fillRect (ruinForm, new Vector2Int ((int)(ex * 0.2), (int)(ey * 0.75)), (int)(ex * 0.2), (int)(ey * 0.2));
			//clearRect (new Vector2Int ((int)(ex * 0.2), (int)(ey * 0.75)), (int)(ex * 0.16), (int)(ey * 0.16));
			//clearRect (new Vector2Int ((int)(ex * 0.23), (int)(ey * 0.65)), (int)(ex * 0.17), (int)(ey * 0.09));
			//house
			fillAngle (ruinForm, 305, (int)(ex * 0.12), (int)(ey * 0.75), (int)(ex * 0.2), (int)(ey * 0.2));
			clearAngle (305, (int)(ex * 0.15), (int)(ey * 0.74), (int)(ex * 0.15), (int)(ey * 0.14));
			clearAngle (305, (int)(ex * 0.25), (int)(ey * 0.65), (int)(ex * 0.09), (int)(ey * 0.2));

			fillAngle (rockForm, 222, (int)(ex * 0.4), (int)(ey * 0.69), (int)(ex * 0.08), (int)(ey * 0.02));

			fillAngle (rockForm, 200, (int)(ex * 0.41), (int)(ey * 0.9), (int)(ex * 0.05), (int)(ey * 0.04));

			fillCircle (rockForm, new Vector2Int ((int)(ex * 0.15), (int)(ey * 0.2)), (int)(ex * 0.04));
			fillCircle (rockForm, new Vector2Int ((int)(ex * 0.19), (int)(ey * 0.28)), (int)(ex * 0.05));

			fillCircle (ruinForm, new Vector2Int ((int)(ex * 0.68), (int)(ey * 0.16)), (int)(ex * 0.05));
			fillCircle (ruinForm, new Vector2Int ((int)(ex * 0.73), (int)(ey * 0.24)), (int)(ex * 0.05));
			fillCircle (ruinForm, new Vector2Int ((int)(ex * 0.87), (int)(ey * 0.3)), (int)(ex * 0.05));

			//bridge
			GameObject bridge = Instantiate(RuinBridge);
			bridge.transform.position = new Vector3(49, 26.497f, 0);
			clearAngle(315, (int)(ex * 0.31), (int)(ey * 0.3), (int)(ex * 0.28f), (int)(ey * 0.12f));
			fillAngle (wfForm, 315, (int)(ex * 0.31), (int)(ey * 0.3), (int)(ex * 0.28f), (int)(ey * 0.12f));
		} else if (map == 3) {
			Form lavaForm = Instantiate (lava).GetComponent<Form> ();

			lavaForm.centerPoint = new Vector2Int (xEdge / 2, yEdge / 2);
			clearCircle (new Vector2Int (xEdge / 2, yEdge / 2), (int)(xEdge * 0.98f));

			//clearCircle (new Vector2Int (xEdge / 2, yEdge / 2), (int)(xEdge * 0.65f));
			fillCircle (lavaForm, new Vector2Int (Map.S.worldSizeX / 2, yEdge / 2), (int)(xEdge * 0.68f));
			clearCircle (new Vector2Int (xEdge / 2, yEdge / 2), (int)(xEdge * 0.55f));
			clearRect (new Vector2Int (xEdge / 2, yEdge / 2), (int)(xEdge * 0.8f), (int)(yEdge * 0.24f));

			fillCircle (lavaForm, new Vector2Int (Map.S.worldSizeX / 2, yEdge / 2), (int)(xEdge * 0.2f));

		} else if (map == 4) {
			Form rockForm = Instantiate (rock).GetComponent<Form> ();
			int xSpacing = xEdge/9;
			int ySpacing = yEdge/9;
			for (int x = xSpacing; x < xEdge - xSpacing; x += xSpacing) {
				for (int y = ySpacing; y < yEdge - ySpacing; y += ySpacing) {
					fillCircle (rockForm, new Vector2Int (x, y), 8);
				}
			}
			float bigEdge = 0.9f;
			float smallEdge = 0.1f;
			clearCircle(new Vector2Int ((int)(smallEdge * Map.S.worldSizeX), (int)(yEdge * bigEdge)), 20);
			clearCircle(new Vector2Int ((int)(bigEdge * Map.S.worldSizeX), (int)(yEdge * bigEdge)), 20);
			clearCircle( new Vector2Int ((int)(smallEdge * Map.S.worldSizeX), (int)(yEdge * smallEdge)), 20);
			clearCircle( new Vector2Int ((int)(bigEdge * Map.S.worldSizeX), (int)(yEdge * smallEdge)), 20);

			//clearCircle (new Vector2Int ((int)(xEdge / 2), (int)(yEdge / 2)), (int)(xEdge * 0.35));
			clearRect(new Vector2Int ((int)(xEdge / 2)-1, (int)(yEdge / 2)), (int)(xEdge * 0.4), (int)(yEdge * 0.2));
			clearRect(new Vector2Int ((int)(xEdge / 2)-1, (int)(yEdge / 2)), (int)(xEdge * 0.2), (int)(yEdge * 0.4));
		}
		setUpSpawns (map);
		setUpPath ();
	}

	public void tileWorld() {
		for (int x = 0; x < xEdge; x++) {
			for (int y = 0; y < yEdge; y++) {
				Cell c = Map.S.world[x, y];
				c.processTile (x, y);
				c.processTiler();
			}
		}
	}

	public void bigTileWorld() {
		for (int x = 0; x < xEdge; x++) {
			for (int y = 0; y < yEdge; y++) {
				if (x < xEdge) {
					bool isSquare = false;
					TileSet curTile = Map.S.world [x, y].getTile ();
					if (curTile != null) {
						List<Cell> cList = new List<Cell> ();
						for (int i = 0; i < 2; i++) {
							for (int j = 0; j < 2; j++) {
								if (!(j == 0 && i == 0)) {//so we dont grab current tile
									int xp = x + i;
									int yp = y + j;
									if (xp >= 0 && xp < xEdge && yp >= 0 && yp < yEdge) {
										Cell cur = Map.S.world [xp, yp];
										if (cur.getTile () == curTile) {
											cList.Add (cur);
										} else {
											cList.Clear ();
											i = 3;
											break;
										}
									}
								} else {
									Debug.Log ("poo");
								}
							}
						}
						if (cList.Count > 0) {
							for (int i = 0; i < cList.Count; i++) {
								cList [i].addTile (null);
							}
							Map.S.world [x, y].setTileSize (2);
							Map.S.world [x, y].processTile (x, y);
							//x++;
						} 
						Map.S.world [x, y].processTile (x, y);
					}
				}
			}
		}
	}




	public void clearCircle(int cx, int cy, int s) {
		clearCircle(new Vector2Int(cx, cy), s);
	}

	public void clearCircle(Vector2Int center, int size){
		for (int x = center.x - size / 2; x < center.x + size / 2; x++) {
			for (int y = center.y - size / 2; y < center.y + size / 2; y++){
				if (x >= 0 && x < Map.S.worldSizeX && y >= 0 && y < Map.S.worldSizeY) {
					if (Vector2Int.Distance (center, new Vector2Int (x, y)) < size / 2) {
						Map.S.world [x, y].empty ();
					}
				}
			}
		}
	}

	public void removeTerrainCircle(int cx, int cy, int s) {
		removeTerrainCircle(new Vector2Int(cx, cy), s);
	}

	public void removeTerrainCircle(Vector2Int center, int size) {
		for (int x = center.x - size / 2; x < center.x + size / 2; x++) {
			for (int y = center.y - size / 2; y < center.y + size / 2; y++){
				if (x >= 0 && x < Map.S.worldSizeX && y >= 0 && y < Map.S.worldSizeY) {
					if (Vector2Int.Distance (center, new Vector2Int (x, y)) < size / 2) {
						Map.S.world [x, y].removeAllID (-1);
					}
				}
			}
		}
	}
		
	public void fillRect(Vector2Int center, int sizeX, int sizeY) {
		fillRect(Map.S.terrain, center, sizeX, sizeY);
	}

	public void fillRect(Form f, Vector2Int center, int sizeX, int sizeY){
		int xMod = sizeX % 2;
		int yMod = sizeY % 2;
		for (int x = center.x - sizeX / 2; x < center.x + sizeX / 2 + xMod; x++) {
			for (int y = center.y - sizeY / 2; y < center.y + sizeY / 2 + yMod; y++){
				if (x >= 0 && x < Map.S.worldSizeX && y >= 0 && y < Map.S.worldSizeY) {
					Map.S.world [x, y].formEnter (f);
				}
			}
		}
	}
	
	public void clearRect(int cx, int cy, int sx, int sy) {
		clearRect(new Vector2Int(cx, cy), sx, sy);
	}

	public void clearRect(Vector2Int center, int sizeX, int sizeY){
		int xMod = sizeX % 2;
		int yMod = sizeY % 2;
		for (int x = center.x - sizeX / 2; x < center.x + sizeX / 2 + xMod; x++) {
			for (int y = center.y - sizeY / 2; y < center.y + sizeY / 2 + yMod; y++){
				if (x >= 0 && x < Map.S.worldSizeX && y >= 0 && y < Map.S.worldSizeY) {
					Map.S.world [x, y].empty ();
				}
			}
		}
	}


	void fillAngle(Form f, int angle, int sX, int sY, int len, int wid) {
		for (int k = 0; k < wid; k++) {
			float x = sX + k * Mathf.Cos (((angle+90)%360) * Mathf.PI / 180);
			float y = sY + k * Mathf.Sin (((angle + 90)%360) * Mathf.PI / 180);
			for (int i = 0; i < len; i++) {
				if (x >= 0 && x < xEdge && y >= 0 && y < yEdge) {
					fillRect (f, new Vector2Int ((int)x, (int)y), 2, 2);
					//Map.S.world [x, y].formEnter (f);
				}
				x += Mathf.Cos (angle * Mathf.PI / 180);
				y += Mathf.Sin (angle * Mathf.PI / 180);
			}
		}
	}

	void clearAngle(int angle, int sX, int sY, int len, int wid) {
		for (int k = 0; k < wid; k++) {
			float x = sX + k * Mathf.Cos (((angle + 90) % 360) * Mathf.PI / 180);
			float y = sY + k * Mathf.Sin (((angle + 90) % 360) * Mathf.PI / 180);
			for (int i = 0; i < len; i++) {
				if (x >= 0 && x < xEdge && y >= 0 && y < yEdge) {
					clearRect (new Vector2Int ((int)x, (int)y), 2, 2);
					//Map.S.world [x, y].formEnter (f);
				}
				x += Mathf.Cos (angle * Mathf.PI / 180);
				y += Mathf.Sin (angle * Mathf.PI / 180);
			}
		}
	}

	public void fillCircle(Vector2Int center, int size){
		fillCircle(Map.S.terrain, center, size);
	}

	void fillCircle(Form f, Vector2Int center, int size){
		for (int x = center.x - size / 2; x < center.x + size / 2; x++) {
			for (int y = center.y - size / 2; y < center.y + size / 2; y++){
				if (x >= 0 && x < Map.S.worldSizeX && y >= 0 && y < Map.S.worldSizeY) {
					if (Vector2Int.Distance (center, new Vector2Int (x, y)) < size / 2) {
						Map.S.world [x, y].formEnter(f);
					}
				}
			}
		}
	}


	public void setUpSpawns(int map) {
		//Debug.Log ("setting up spawns " + map);
		if (map == 0 || map == 1) {
			float smallEdge = 0.18f;
			float bigEdge = 0.82f;
			//int worldBuffer = 30;
			spawns = new Vector3Int[4];
			spawns [0] = new Vector3Int ((int)(smallEdge * Map.S.worldSizeX), (int)(bigEdge * Map.S.worldSizeY), -45);
			spawns [1] = new Vector3Int ((int)(smallEdge * Map.S.worldSizeX), (int)(smallEdge * Map.S.worldSizeY), 45);
			spawns [2] = new Vector3Int ((int)(bigEdge * Map.S.worldSizeX), (int)(bigEdge * Map.S.worldSizeY), -135);
			spawns [3] = new Vector3Int ((int)(bigEdge * Map.S.worldSizeX), (int)(smallEdge * Map.S.worldSizeY), 135);
			spawnsClosed = new bool[4];
		} else if (map == 2) {
			spawns = new Vector3Int[5];
			spawns [0] = new Vector3Int ((int)(xEdge * 0.227f), (int)(yEdge * 0.657f), 0);
			spawns [1] = new Vector3Int ((int)(xEdge * 0.1363f), (int)(yEdge * 0.1578f), 0);
			spawns [2] = new Vector3Int ((int)(xEdge * 0.6136f), (int)(yEdge * 0.921f), 0);
			spawns [3] = new Vector3Int ((int)(xEdge * 0.7818f), (int)(yEdge * 0.131f), 0);
			spawns [4] = new Vector3Int ((int)(xEdge * 0.454f), (int)(yEdge * 0.13f), 0);
			spawnsClosed = new bool[5];
		} else if (map == 3) {
			float smallEdge = 0.1f;
			float bigEdge = 0.9f;
			//int worldBuffer = 30;
			spawns = new Vector3Int[4];
			spawns [0] = new Vector3Int ((int)(smallEdge * Map.S.worldSizeX), Map.S.worldSizeY / 2, 0);
			spawns [1] = new Vector3Int ((int)(bigEdge * Map.S.worldSizeX), Map.S.worldSizeY / 2, 0);
			spawns [2] = new Vector3Int (xEdge / 2, (int)(0.95 * yEdge), 0);
			spawns [3] = new Vector3Int (xEdge / 2, (int)(0.05 * yEdge), 0);
			spawnsClosed = new bool[4];
		} else if (map == 4) {
			float smallEdge = 0.1f;
			float bigEdge = 0.9f;
			//int worldBuffer = 30;
			spawns = new Vector3Int[4];
			spawns [0] = new Vector3Int ((int)(smallEdge * Map.S.worldSizeX), (int)(yEdge * bigEdge), 0);
			spawns [1] = new Vector3Int ((int)(bigEdge * Map.S.worldSizeX), (int)(yEdge * bigEdge), 0);
			spawns [2] = new Vector3Int ((int)(smallEdge * Map.S.worldSizeX), (int)(yEdge * smallEdge), 0);
			spawns [3] = new Vector3Int ((int)(bigEdge * Map.S.worldSizeX), (int)(yEdge * smallEdge), 0);
			spawnsClosed = new bool[4];

		}
	}
		
	public Vector3Int getSpawn() {
		if (!forceSpawn) {
			int s = 0;
			int count = 0;
			do {
				s = Random.Range (0, spawns.Length);
				if (spawnsClosed != null) {
					//Debug.Log(spawnsClosed.Length + " " + s);
				}
				count++;
			} while (spawnsClosed [s] && count < spawns.Length * 2);
			if (spawnsClosed[s]) {
				Debug.LogError("spawn closed " + s + " I tried " + count + " times");
				count = 0;
				while (spawnsClosed[s] && count < spawns.Length) {
					s = (s + 1) % spawns.Length;
					count++;
				}
				if (spawnsClosed[s]) {
					Debug.LogError("there are no open spawns");
				}
			}
			spawnsClosed [s] = true;
			//Debug.Log (s);
			return spawns [s];
		} else {
			Vector3Int sp = forceSpawns [force];
			force = (force + 1) % forceSpawns.Length;
			return sp;
		}
	}

	public Vector3Int getSpawn(int num) {
		return spawns[num];
	}

	int checkRange = 10;

	public void checkSpawns(){
		for (int i = 0; i < spawns.Length; i++) {
			spawnsClosed [i] = false;
			bool spawnFree = true;
			Vector2Int cur = new Vector2Int(spawns [i].x, spawns[i].y);
			//Debug.Log ("spawn: " + i);

			for (int j = 0; j < GM.S.players.Count; j++) {
				if (GM.S.players [j]) {
					if (!GM.S.players [j].dead) {
						float dst = Vector2Int.Distance (cur, GM.S.players [j].self.centerPoint);
						//Debug.LogError (dst + " " + i);
						if (dst < 45) {
							spawnFree = false;
						}
					}
		
					/*
			if (spawnFree) {
				for (int x = cur.x - checkRange / 2; x < cur.x + checkRange / 2; x++) {
					for (int y = cur.y - checkRange / 2; y < cur.y + checkRange / 2; y++) {
						if (!Map.S.checkCell (new Vector2Int (x, y))) {
							spawnFree = false;
							//Debug.Log (i);
							break;
						} else {
							//Debug.Log (x + " , " + y);
						}
					}
					if (!spawnFree) {
						break;
					}
				}
				*/
				}
			}

			spawnsClosed [i] = !spawnFree;
			//Debug.Log (spawnsClosed [i] + " " + i);
		}
	}

	public void pathStart() {
		lowest = new float[closestNum];
		paths = new List<pathNode> ();
	}

	public GameObject node;
	public List<pathNode> paths;

	void setUpPath() {
		pathStart();
		int wx = Map.S.worldSizeX;
		int wy = Map.S.worldSizeY;
		int map = 0;
		if (GameInfo.S) {
			map = GameInfo.S.level;
		}
		int xEdge = Map.S.worldSizeX;
		int yEdge = Map.S.worldSizeY;
		//Debug.Log (xEdge + ", " + yEdge);

		if (map == 1) {
			float smallEdge = 0.18f;
			float bigEdge = 0.82f;

			pathNode a = Instantiate (node, new Vector3 ((xEdge * smallEdge), (yEdge * smallEdge), 0), transform.rotation).GetComponent<pathNode> ();
			paths.Add (a);
			a.name = "a";
			pathNode b = Instantiate (node, new Vector3 ((xEdge * smallEdge), yEdge * ((bigEdge + smallEdge) / 2), 0), transform.rotation).GetComponent<pathNode> ();
			b.name = "b";
			paths.Add (b);
			pathNode c = Instantiate (node, new Vector3 (xEdge * smallEdge, yEdge * bigEdge, 0), transform.rotation).GetComponent<pathNode> ();
			c.name = "c";
			paths.Add (c);
			pathNode d = Instantiate (node, new Vector3 (xEdge * bigEdge, yEdge * smallEdge, 0), transform.rotation).GetComponent<pathNode> ();
			d.name = "d";
			paths.Add (d);
			pathNode e = Instantiate (node, new Vector3 ((xEdge * bigEdge), yEdge * ((bigEdge + smallEdge) / 2), 0), transform.rotation).GetComponent<pathNode> ();
			e.name = "e";
			paths.Add (e);
			pathNode f = Instantiate (node, new Vector3 (xEdge * bigEdge, yEdge * bigEdge, 0), transform.rotation).GetComponent<pathNode> ();
			f.name = "f";
			paths.Add (f);
			pathNode g = Instantiate (node, new Vector3 (xEdge / 2, yEdge / 2, 0), transform.rotation).GetComponent<pathNode> ();
			g.name = "g";
			paths.Add (g);
			pathNode h1 = Instantiate (node, new Vector3 (xEdge  * 0.4f, yEdge * 0.33f), transform.rotation).GetComponent<pathNode> ();
			h1.name = "h1";
			paths.Add (h1);
			pathNode h2 = Instantiate (node, new Vector3 (xEdge * 0.6f, yEdge * 0.33f), transform.rotation).GetComponent<pathNode> ();
			h2.name = "h2";
			paths.Add (h2);
			pathNode i1 = Instantiate (node, new Vector3 (xEdge * 0.4f, yEdge * 0.66f), transform.rotation).GetComponent<pathNode> ();
			i1.name = "i1";
			paths.Add (i1);
			pathNode i2 = Instantiate (node, new Vector3 (xEdge * 0.6f, yEdge * 0.66f), transform.rotation).GetComponent<pathNode> ();
			i2.name = "i2";
			paths.Add (i2);

			setNeighbors (a, b, 0);
			setNeighbors (a, h1, 7, true);
			setNeighbors (b, c, 0);
			setNeighbors (b, g, 6);
			setNeighbors (c, i1, 5, true);
			setNeighbors (g, e, 6);
			setNeighbors (d, h2, 1, true);
			setNeighbors (e, f, 0);
			setNeighbors (e, d, 4);
			setNeighbors (f, i2, 3, true);
			setNeighbors (g, h1, 3);
			setNeighbors (g, h2, 5);
			setNeighbors (g, i1, 1);
			setNeighbors (g, i2, 7);
			setNeighbors (h1, h2, 6);
			setNeighbors (i1, i2, 6);

		} else if (map == 2) {
			pathNode p1 = Instantiate (node, new Vector3 (xEdge * 0.1791f, yEdge * 0.155f, 0), transform.rotation).GetComponent<pathNode> ();
			paths.Add (p1);
			p1.name = "p1";
			pathNode p2 = Instantiate (node, new Vector3 (xEdge * 0.0875f, yEdge * 0.28f, 0), transform.rotation).GetComponent<pathNode> ();
			paths.Add (p2);
			p2.name = "p2";
			pathNode p3 = Instantiate (node, new Vector3 (xEdge * 0.258f, yEdge * 0.26f, 0), transform.rotation).GetComponent<pathNode> ();
			paths.Add (p3);
			p3.name = "p3";
			pathNode p4 = Instantiate (node, new Vector3 (xEdge * 0.0791f, yEdge * 0.47f, 0), transform.rotation).GetComponent<pathNode> ();
			paths.Add (p4);
			p4.name = "p4";
			pathNode p5 = Instantiate (node, new Vector3 (xEdge * 0.2166f, yEdge * 0.645f, 0), transform.rotation).GetComponent<pathNode> ();
			paths.Add (p5);
			p5.name = "p5";
			pathNode p6 = Instantiate (node, new Vector3 (xEdge * 0.0875f, yEdge * 0.645f, 0), transform.rotation).GetComponent<pathNode> ();
			paths.Add (p6);
			p6.name = "p6";
			pathNode p7 = Instantiate (node, new Vector3 (xEdge * 0.0875f, yEdge * 0.875f, 0), transform.rotation).GetComponent<pathNode> ();
			paths.Add (p7);
			p7.name = "p7";
			pathNode p8 = Instantiate (node, new Vector3 (xEdge * 0.2875f, yEdge * 0.8755f, 0), transform.rotation).GetComponent<pathNode> ();
			paths.Add (p8);
			p8.name = "p8";
			pathNode p9 = Instantiate (node, new Vector3 (xEdge * 0.2875f, yEdge * 0.755f, 0), transform.rotation).GetComponent<pathNode> ();
			paths.Add (p9);
			p9.name = "p9";
			pathNode p10 = Instantiate (node, new Vector3 (xEdge * 0.3916f, yEdge * 0.72f, 0), transform.rotation).GetComponent<pathNode> ();
			paths.Add (p10);
			p10.name = "p10";
			pathNode p11 = Instantiate (node, new Vector3 (xEdge * 0.325f, yEdge * 0.47f, 0), transform.rotation).GetComponent<pathNode> ();
			paths.Add (p11);
			p11.name = "p11";
			pathNode p12 = Instantiate (node, new Vector3 (xEdge * 0.39958f, yEdge * 0.1395f, 0), transform.rotation).GetComponent<pathNode> ();
			paths.Add (p12);
			p12.name = "p12";
			pathNode p13 = Instantiate (node, new Vector3 (xEdge * 0.5625f, yEdge * 0.28f, 0), transform.rotation).GetComponent<pathNode> ();
			paths.Add (p13);
			p13.name = "p13";
			pathNode p14 = Instantiate (node, new Vector3 (xEdge * 0.6125f, yEdge * 0.095f, 0), transform.rotation).GetComponent<pathNode> ();
			paths.Add (p14);
			p14.name = "p14";
			pathNode p15 = Instantiate (node, new Vector3 (xEdge * 0.6583f, yEdge * 0.175f, 0), transform.rotation).GetComponent<pathNode> ();
			paths.Add (p15);
			p15.name = "p15";
			pathNode p16 = Instantiate (node, new Vector3 (xEdge * 0.725f, yEdge * 0.29f, 0), transform.rotation).GetComponent<pathNode> ();
			paths.Add (p16);
			p16.name = "p16";
			pathNode p17 = Instantiate (node, new Vector3 (xEdge * 0.866f, yEdge * 0.29f, 0), transform.rotation).GetComponent<pathNode> ();
			paths.Add (p17);
			p17.name = "p17";
			pathNode p18 = Instantiate (node, new Vector3 (xEdge * 0.791f, yEdge * 0.15f, 0), transform.rotation).GetComponent<pathNode> ();
			paths.Add (p18);
			p18.name = "p18";
			pathNode p19 = Instantiate (node, new Vector3 (xEdge * 0.866f, yEdge * 0.74f, 0), transform.rotation).GetComponent<pathNode> ();
			paths.Add (p19);
			p19.name = "p19";
			pathNode p20 = Instantiate (node, new Vector3 (xEdge * 0.6833f, yEdge * 0.875f, 0), transform.rotation).GetComponent<pathNode> ();
			paths.Add (p20);
			p20.name = "p20";
			pathNode p21 = Instantiate (node, new Vector3 (xEdge * 0.2125f, yEdge * 0.335f, 0), transform.rotation).GetComponent<pathNode> ();
			paths.Add (p21);
			p21.name = "p21";
			pathNode p22 = Instantiate (node, new Vector3 (xEdge * 0.4125f, yEdge * 0.88f, 0), transform.rotation).GetComponent<pathNode> ();
			paths.Add (p22);
			p22.name = "p22";
			pathNode p23 = Instantiate (node, new Vector3 (xEdge * 0.33f, yEdge * 0.645f, 0), transform.rotation).GetComponent<pathNode> ();
			paths.Add (p23);
			p23.name = "p23";
			pathNode p24 = Instantiate (node, new Vector3 (xEdge * 0.2625f, yEdge * 0.57f, 0), transform.rotation).GetComponent<pathNode> ();
			paths.Add (p24);
			p24.name = "p24";
			pathNode p25 = Instantiate (node, new Vector3 (xEdge * 0.5875f, yEdge * 0.615f, 0), transform.rotation).GetComponent<pathNode> ();
			paths.Add (p25);
			p25.name = "p25";
			pathNode p26 = Instantiate (node, new Vector3 (xEdge * 0.3875f, yEdge * 0.545f, 0), transform.rotation).GetComponent<pathNode> ();
			paths.Add (p26);
			p26.name = "p26";
			setNeighbors (p1, p2, 0);
			setNeighbors (p1, p3, 7);
			//setNeighbors (p1, p25, 0);
			setNeighbors (p2, p4, 0);
			//setNeighbors (p2, p3, 5);
			setNeighbors (p2, p21, 6);
			//setNeighbors (p2, p25, 5);
			setNeighbors (p3, p21, 1);
			setNeighbors (p3, p11, 7);
			setNeighbors (p3, p12, 5);
			setNeighbors (p3, p24, 0);
			setNeighbors (p4, p6, 0);
			//setNeighbors (p5, p21, 5);
			setNeighbors (p5, p4, 3, true);
			setNeighbors (p5, p6, 2, true);
			setNeighbors (p5, p7, 1, true);
			setNeighbors (p5, p9, 7, true);
			setNeighbors (p5, p24, 5);
			setNeighbors (p5, p23, 6);
			setNeighbors (p6, p7, 0);
			setNeighbors (p7, p8, 6);
			setNeighbors (p8, p9, 4);
			setNeighbors (p8, p22, 6);
			//setNeighbors (p10, p25, 6, true);
			//setNeighbors (p11, p25, 7, true);
			setNeighbors (p9, p10, 6);
			setNeighbors (p9, p23, 4);
			//setNeighbors (p10, p11, 4);
			setNeighbors (p10, p26, 4);
			setNeighbors (p11, p13, 5, true); 
			setNeighbors (p11, p21, 2);
			setNeighbors (p11, p24, 1);
			setNeighbors (p11, p26, 0);
			setNeighbors (p12, p13, 7);
			setNeighbors (p12, p14, 5);
			setNeighbors (p12, p15, 6);
			setNeighbors (p13, p16, 6);
			setNeighbors (p13, p25, 0, true);
			setNeighbors (p14, p15, 7, true); 
			setNeighbors (p14, p18, 6);
			setNeighbors (p15, p18, 5);
			setNeighbors (p15, p16, 7, true); 
			setNeighbors (p16, p18, 4);
			setNeighbors (p16, p19, 7);
			setNeighbors (p16, p25, 1, true);
			setNeighbors (p16, p17, 6, true); 
			setNeighbors (p17, p18, 3);
			setNeighbors (p17, p19, 0);
			setNeighbors (p19, p25, 2, true);
			setNeighbors (p19, p20, 1, true);
			setNeighbors (p20, p25, 3, true);
			setNeighbors (p21, p24, 7);
			setNeighbors (p22, p20, 6);
			setNeighbors (p22, p10, 4);
			setNeighbors (p23, p24, 3);
			setNeighbors (p26, p25, 6, true);

			//setNeighbors (p22, p24, 4);
			//setNeighbors (p9, p24, 5);
		} else if (map == 3) {
			float smallEdge = 0.1f;
			float bigEdge = 0.9f;
			float mid = 0.15f;
			float startAngle = 90;
			float angSpacing = 360 / 12;
			float radius = Mathf.Abs ((smallEdge * wx) - wx / 2);
			pathNode[] ring = new pathNode[12];
			for (int i = 0; i < 12; i++) {
				float x = wx / 2 + radius * Mathf.Sin ((startAngle + angSpacing * i) * Mathf.PI / 180);
				float y = wy / 2 + radius * Mathf.Cos ((startAngle + angSpacing * i) * Mathf.PI / 180);
				ring [i] = Instantiate (node, new Vector3 (x, y, 0), transform.rotation).GetComponent<pathNode> ();
				ring [i].name = "outer" + i;
				paths.Add (ring [i]);
			}
			int d = 5;
			for (int i = 0; i < 12; i++) {
				//Debug.Log (d);
				setNeighbors (ring [i], ring [(i + 1) % 12], d);
				if (i != 2 && i != 5 && i != 8) {
					d--;
					if (d < 0) {
						d = 7;
					}
				}
			}
			startAngle = 90f;
			radius /= 2;
			angSpacing = 360 / 6;
			pathNode[] ring2 = new pathNode[6];
			for (int i = 0; i < 6; i++) {
				float x = wx / 2 + radius * Mathf.Sin ((startAngle + angSpacing * i) * Mathf.PI / 180);
				float y = wy / 2 + radius * Mathf.Cos ((startAngle + angSpacing * i) * Mathf.PI / 180);
				ring2 [i] = Instantiate (node, new Vector3 (x, y, 0), transform.rotation).GetComponent<pathNode> ();
				ring2 [i].name = "inner" + i;
				paths.Add (ring2 [i]);
			}
			setNeighbors (ring2 [4], ring [7], 2, true);
			setNeighbors (ring2 [4], ring [8], 1, true);
			setNeighbors (ring2 [4], ring [9], 0, true);

			setNeighbors (ring2 [5], ring [11], 4, true);
			setNeighbors (ring2 [5], ring [10], 0, true);
			setNeighbors (ring2 [5], ring [9], 1, true);

			setNeighbors (ring2 [2], ring [3], 5, true);
			setNeighbors (ring2 [2], ring [4], 4, true);
			setNeighbors (ring2 [2], ring [5], 3, true);

			setNeighbors (ring2 [1], ring [3], 0, true);
			setNeighbors (ring2 [1], ring [2], 1, true);
			setNeighbors (ring2 [1], ring [1], 6, true);
			d = 3;
			for (int i = 0; i < 6; i++) {
				setNeighbors (ring2 [i], ring2 [(i + 1) % 6], d);
				d--;
				if (d < 0) {
					d = 7;
				}
			}
			setNeighbors (ring [0], ring2 [0], 2);
			setNeighbors (ring [6], ring2 [3], 6);
		} else if (map == 4) {
			int xSpacing = xEdge/9;
			int ySpacing = yEdge/9;
			List<pathNode> preCol = new List<pathNode>();
			List<pathNode> curCol = new List<pathNode>();
			for (int x = xSpacing/2; x < xEdge; x += xSpacing) {
				for (int y = ySpacing/2; y < yEdge; y += ySpacing) {
					curCol.Add(Instantiate (node, new Vector3 (x, y, 0), transform.rotation).GetComponent<pathNode> ());
					int cudungeonRooms = curCol.Count - 1;
					curCol[cudungeonRooms].name = (x/xSpacing) + " " + (y/ySpacing);
					paths.Add (curCol [cudungeonRooms]);
					if (curCol.Count > 1) {
						setNeighbors (curCol [cudungeonRooms - 1], curCol [cudungeonRooms], 0);
					}
					if (preCol.Count > 0) {
						setNeighbors (preCol [cudungeonRooms], curCol [cudungeonRooms], 6);
					}
				}
				preCol.Clear ();
				for (int i = 0; i < curCol.Count; i++) {
					preCol.Add (curCol [i]);
				}
				curCol.Clear ();
			}
		}
		Transform pp = Instantiate (pathParent, transform.position, transform.rotation).transform;
		for (int i = 0; i < paths.Count; i++) {
			paths [i].transform.parent = pp;
		}
	}

	float startWeight = 0.8f;
	int closestNum = 5;
	float[] lowest;

	public void getClosest(Vector2Int start, Vector2Int end, pathNode[] chosen) {
		int close = Mathf.Min(closestNum, paths.Count);
		for (int i = 0; i < close; i++) {
			lowest [i] = Mathf.Infinity;
			chosen [i] = null;
		}
		//pathNode[] chosen = new pathNode[close];//null;
		//Debug.Log ("getting noode for " + pos);
		float endWeight = 1 - startWeight;
		for (int i = 0; i < paths.Count; i++) {
			float d = Vector2Int.Distance (paths [i].getPos(), start) * startWeight;
			//Debug.Log (paths [i].name + " : " + paths [i].getPos () + ", " + start + " = " + d);
			float dd = Vector2Int.Distance (paths [i].getPos (), end) * endWeight;
			d += dd;	
			//Debug.Log (" dd: " + dd + " = " + d);
			//Debug.Log (paths [i].name + " : " + paths [i].getPos () + ", " + start + " = " + d);
			for (int j = 0; j < close; j++) {
				if (d < lowest [j]) {
					for (int k = close - 1; k > j; k--) {
						//Debug.Log ("K : " + k + " : " + chosen [k] + " = " + chosen [k - 1]);
						lowest [k] = lowest [k - 1];
						chosen [k] = chosen [k - 1];
					}
					lowest [j] = d;
					chosen [j] = paths [i];
					break;
				}
			}
		}
		//return chosen;
	}
	/*
	public pathNode getStart(int l, Vector2Int ai, Vector2Int dest) {
		float lowest = Mathf.Infinity;
		pathNode chosen = null;
		for (int i = 0; i < paths.Count; i++) {
			paths [i].calcScore (0, ai, dest, null, false);
			float s = paths [i].getScore (0); 
			if (s < lowest) {
				lowest = s;
				chosen = paths [i];
			}
		}
		return chosen;
	}
*/
	public void clearPathScores(int p) {
		for (int i = 0; i < paths.Count; i++) {
			paths [i].clearScore (p);
		}
	}

	
	public pathNode makePathNode(Vector2Int pos) {
		return makePathNode(pos.x, pos.y);
	}

	public pathNode makePathNode(int xp, int yp) {
		if (paths == null) {
			pathStart();
		} else {
			for (int i = 0; i < paths.Count; i++) {
				if (paths[i].getPos() == new Vector2Int(xp, yp)) {
					Debug.Log("adding redundant pos, returning original");
					return paths[i];
				}
			}
		}
		pathNode p = Instantiate(node, new Vector3(xp, yp, 0), transform.rotation).GetComponent<pathNode>();
		paths.Add(p);
		return p;
	}


	public void setNeighbors(pathNode a, pathNode b, int a2b) {
		setNeighbors (a, b, a2b, false);
	}

	void setNeighbors(pathNode a, pathNode b, int a2b, bool dodge) {
		//Debug.Log("settting neighbors " + a.name + " " + b.name + " in dir: " + a2b);
		/*
		Vector2Int ap = a.getPos ();
		Vector2Int bp = b.getPos ();
		pathNode mid = Instantiate(node, new Vector3 ((ap.x + bp.x) / 2, (ap.y + bp.y) / 2, 0), transform.rotation).GetComponent<pathNode>();
		paths.Add (mid);
		mid.name = a.name + b.name;
		a.setNeighbor (mid, a2b);
		mid.setNeighbor (a, (a2b + 4) % 8);
		b.setNeighbor (mid, (a2b + 4) % 8);
		mid.setNeighbor (b, a2b);
		*/
		//Debug.Log (a.name + " -> " + b.name + " " + a2b);
		a.setNeighbor (b, a2b, dodge);
		b.setNeighbor (a, (a2b + 4) % 8, dodge);
	}

	public bool hasPath() {
		return paths != null && paths.Count > 0;
	}
}
