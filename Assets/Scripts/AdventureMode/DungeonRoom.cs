using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom : MonoBehaviour {
    public Vector2Int pos;
    public bool beat;
    public bool readyToGo; // set to true after spawned in, and turned off when cleaned up
    public bool closeDoor;
    public DungeonRoom[] neighbors;
    public bool[] doors;

    public int numNeigh = 4;
    public mapTerrain creator;
    public DungeonMaster dm;
    public GameObject door;
    Door[] curDoors;
    public Vector2Int[] poles;
    public GameObject guy;

    public int targetCount;
    public List<DungeonObject> denizens;
    public int doorSpace = 5;
    public Grade grd;
    List<Node> subRooms;
    public List<List<DungeonObject>> waves;
    List<Form> bloods;
    public bool getPrize;

    public virtual void Awake() {
        neighbors = new DungeonRoom[numNeigh];
        doors = new bool[numNeigh];
        grd = gameObject.GetComponent<Grade>();
        curDoors = new Door[numNeigh];
        denizens = new List<DungeonObject>();
        roomSize = new Vector2Int(Map.S.worldSizeX / roomNum.x, Map.S.worldSizeY / roomNum.y);
        //spawnInHallList = new List<Vector3Int>();
        spawnInRoomList = new List<Vector3Int>();
        waves = new List<List<DungeonObject>>();
        bloods = new List<Form>();
        hallSizes = new Vector2Int(6, 10);
    }

    public void setDoorSpace(int ds) {
        doorSpace = ds;
        poles = new Vector2Int[numNeigh];
        poles[0] = new Vector2Int(Map.S.worldSizeX / 2, Map.S.worldSizeY - doorSpace);
        poles[1] = new Vector2Int(doorSpace, Map.S.worldSizeY / 2);
        poles[2] = new Vector2Int(Map.S.worldSizeX / 2, doorSpace);
        poles[3] = new Vector2Int(Map.S.worldSizeX - doorSpace, Map.S.worldSizeY / 2);
        if (creator) {
            int spawnSpace = 2;
            for (int i = 0; i < 4; i++) {
                Vector2Int d = GM.S.dirs[(i*2)];
                if (i == 0 || i == 3) {
                    d *= (spawnSpace+1);
                } else {
                    d *= (spawnSpace);
                }
                //Vector2Int d = GM.S.dirs[(i*2)];// * (doorSpace-2);
                creator.spawns[i] = new Vector3Int (poles[i].x - d.x, poles[i].y - d.y, 0);
            }
        }
    }
    
    public float doorPercent = 0.75f;

    protected virtual void setUpDoors(int dir) {
        if (dir == -1) {
            for (int i = 0; i < 4; i++) {
			    doors[i] = true;
            }
        } else {
            checkDoors(dir);
        }
        for (int i = 0; i < numNeigh; i++) {
            if (doors[i]) {
                setUpDoor(poles[i].x, poles[i].y, i);
            }
        }
    }

    public void checkDoors(int dir) {
        Debug.Log("checking doors");
        for (int i = 0; i < numNeigh; i++) {
            if (i == (dir + numNeigh/2) % numNeigh || (dm.nextRoom.Length == 4 && dm.nextRoom[i])) {
                doors[i] = true;
            } else {
                DungeonRoom neigh = dm.checkRoom(this, i);
                if (Random.value < doorPercent) {
                //if (dm.canRoomsConnect(this, neigh, i)) {// && Random.value < doorPercent) {
                    int newDoor = (i+2) % numNeigh;

                    if (neigh && !neigh.doors[newDoor]) {
                        Debug.Log("we should connect these guys " + pos + " and " + neigh.pos);
                        neigh.doors[newDoor] = true;
                        neigh.setUpDoor(newDoor);
                        // to turn off graphic
                        neigh.curDoors[newDoor].gameObject.SetActive(false);//cleanUp();
                        //neigh.curDoors[newDoor].setLock(false);
                    }
                    //Debug.Log("setting door " + i + " on room " + pos + " to true");
                    doors[i] = true;
                    continue;
                } else if (neigh) {
                    dm.disconnectRoom(this, neigh, i);
                }
                doors[i] = false;
            }
        }
    }


    public int challengeType;
    public int floor = 5;
    public Color wallColor;
    //expecting full world
    public void spawnRoom(int dir) {
        creator.spawnBgFloor(floor);
        creator.setWallColor(wallColor);
        creator.fullWorld(false);
        creator.fullWorld(true);
        creator.pathStart();
        if (!setUp) {
            for (int i = 0; i < GM.S.players.Count; i++) {
                if (bloods.Count > i) {
                    GM.S.players[i].GetComponent<defense>().linkBloodToRoom(bloods[i], dir == -1);
                } else {
                    Form blood = GM.S.players[i].GetComponent<defense>().linkBloodToRoom(null, dir == -1);
                    bloods.Add(blood);
                    DungeonObject obj = blood.GetComponent<DungeonObject>();
                    //obj.spawned = true;
                    addObj(obj);
                }

            }
            if (myShot && !myShot.randomRoom) {
                myShot.shapeRoom(this, creator, dir);
                
                for (int i = 0; i < numNeigh; i++) {
                    if (doors[i]) {
                        setUpDoor(poles[i].x, poles[i].y, i);
                    }
                }
                
            } else {
                //Debug.Log("setting up without shot");
                shapeRoom(dir);
                setUpDoors(dir);
                setUpRoom(dir);
                if (getTargetCount() <= 0) {
                   beatRoom(false);
                }
            } 
            //checkDoors(dir); 

            for (int i = 0; i < denizens.Count; i++) {
                if (!spawnObj(denizens[i])) {
                    i--;
                }
            }
            wallsAndDoors();
            for (int i = 0; i < numNeigh; i++) {
                if (doors[i]) {
                    Door d = curDoors[i];// setUpDoor(poles[i].x, poles[i].y, i).GetComponent<Door>();
                    if ((!closeDoor && (i == (dir + 2) % 4) && dir != -1) || beat) {
                        d.setLock(false);
			        }
                    DungeonRoom neigh = dm.checkRoom(this, i);
                    if (neigh) {
                        neighbors[i] = neigh;
                        int newDoor = (i+2) % numNeigh;
                        neigh.neighbors[newDoor] = this;
                        if (!neigh.doors[newDoor]) {
                            Debug.Log("we should connect these guys " + pos + " and " + neigh.pos);
                            neigh.doors[newDoor] = true;
                            neigh.setUpDoor(newDoor);
                            // to turn off graphic
                            neigh.curDoors[newDoor].gameObject.SetActive(false);//cleanUp();
                        }
                        //neigh.curDoors[newDoor].setLock(false);
                    }
                }

            }
            //wallsAndDoors();
            saveWorld();
            // i think, because  Iwas setting setUp to true in the SHots, but that seems unecesary since we always want a room setup at this point
            setUp = true;
        } else {
            loadWorld();
            if (!gameObject.GetComponent<BossRoom>()) {
                //Debug.Log(dir);
                if (doors[(dir+2)%4] == false) {
                    doors[(dir+2)%4] = true;
                    Debug.Log("goots make a door that I came thru");
                }
            }
            // in case the number of doors has changed since we were here last
            wallsAndDoors();
            for (int i = 0; i < denizens.Count; i++) {
                if (!spawnObj(denizens[i])) {//, poses[i].x, poses[i].y);
                    i--;
                }
            }
            for (int i = 0; i < numNeigh; i++) {
                if (!closeDoor && curDoors[i] && ((i == (dir + 2) % 4 && dir != -1) || beat)) {
                    curDoors[i].setLock(false);
                }
            }
            //wallsAndDoors();
        }
        if (dir == -1) {
            if (GameInfo.S.numPlayers == 1) {
                for (int i = 0; i < 4; i++) {
                    Vector2Int pos = new Vector2Int(Map.S.worldSizeX/2, Map.S.worldSizeY/2);
			        creator.spawns[i] = new Vector3Int(pos.x, pos.y, 0);
                }
            } else {
                for (int i = 0; i < 4; i++) {
                    Vector2Int pos = new Vector2Int(Map.S.worldSizeX/2, Map.S.worldSizeY/2) + GM.S.dirs[i*2] * 4;
			        creator.spawns[i] = new Vector3Int(pos.x, pos.y, 0);
                }
            }
        } else {
            setDoorSpace(doorSpace);
            for (int i = 0; i < 4; i++) {
                Vector2Int d = GM.S.dirs[(i*2)];
                if (i == 0 || i == 3) {
                    d *= (doorSpace+1);
                } else {
                    d *= (doorSpace);
                }
                // done in setdoorspace function now
                //creator.spawns[i] = new Vector3Int (poles[i].x - d.x, poles[i].y - d.y, 0);
                //Debug.Log("setting spawns " + creator.spawns[i]);
            }
        }
        if (boomBox.S) {
            if (enemiesInRoom()) {
                boomBox.S.beat(1);
            } else {
                boomBox.S.beat(0);
            }
        }
        readyToGo = true;
    }

    public void addObj(DungeonObject obj) {
        obj.transform.parent = transform;
        denizens.Add(obj);
        obj.receiveMaster(this);
    }

    public Shot myShot;

    public virtual void shapeRoom(int dir) {
        if (dir == -1) {
            Vector2Int center = new Vector2Int(Map.S.worldSizeX / 2, Map.S.worldSizeY / 2);
            creator.clearCircle(center, Map.S.worldSizeX - doorSpace*2);
            //dm.spawnDebris(center, 10, 10);
            //spawnAround(dm.debrisis, center, 40, 1, 6);
            //createDungeon();
        } else {
//            creator.clearCircle(new Vector2Int(Map.S.worldSizeX / 2, Map.S.worldSizeY / 2), Map.S.worldSizeX - doorSpace/2);
            createDungeon();
        }
    }

    public bool setUp = false;
    public virtual bool setUpRoom(int dir) {
        setUp = true;
        if (dir != -1) {
            return true;
        } else {
            challengeType = 69;
            return false;
        }
    }


    public virtual void beatRoom(bool paused) {
        beat = true;
        if (paused) {
            dm.completeSkill(challengeType);
            if (myShot) {
                //Debug.Log("after text for " + myShot.name);
                myShot.roomBeat(this);
            }
        }
        StartCoroutine("beatingRoom", paused);
    }

    IEnumerator beatingRoom(bool paused) {
        if (paused) {
            if (boomBox.S) {
                boomBox.S.yesPress(0);
            }
            if (Grader.S) {
                Grader.S.beatRoom();
            }
            yield return new WaitForSeconds(1f);
        }
        for (int i = 0; i < numNeigh; i++) {
            if (doors[i] && curDoors[i]) {
                curDoors[i].setLock(false);
                int n = (i + 2) % 4;
                if (neighbors[i]) {
                    if (neighbors[i].curDoors[n]) {
                        Debug.Log("to the " + i + " we are unlocking door " + n);
                        neighbors[i].curDoors[n].setLock(false);
                    }
                }
                if (paused) {
                    if (boomBox.S) {
                        boomBox.S.yesPress(0);
                    }
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }
        if (getPrize) {
            dm.spawnHeal(new Vector2Int(Map.S.worldSizeX/2, Map.S.worldSizeY/2), this).spawnIn();
        }
        beatOfRoom();
        yield return new WaitForEndOfFrame();
    }

    public virtual void beatOfRoom() { }

    public List<DungeonObject> spawnAround(GameObject type, Vector2Int pos, int num, int minDist, int maxDist) {
        List<DungeonObject> spawned = new List<DungeonObject>();
        //Vector2Int pos = new Vector2Int(Map.S.worldSizeX/2, Map.S.worldSizeY/2);
        for (int i = 0; i < num; i++) {
            int d = Random.Range(0, GM.S.dirs.Length);
            DungeonObject cur = setUpObj(type, pos, false);
            Vector2Int dir = Vector2Int.zero;
            if (cur) {
                while (maxDist >= minDist && dir == Vector2Int.zero) {
                    for (int j = 0; j < GM.S.dirs.Length; j++) {
                        dir = GM.S.dirs[(d + j) % GM.S.dirs.Length];
                        if (cur.self.checkBody(pos + (dir * maxDist), false, false) && !checkNearDoors(pos + (dir * maxDist))) {
                            break;
                        } else {
                            dir = Vector2Int.zero;
                        }
                    }
                    maxDist--;
                }
            } else {
                Debug.Log("no object spwned");
            }
            pos += dir * maxDist;
            if (dir == Vector2Int.zero) {
                Debug.LogWarning("no found spot");
            } else {
                //Debug.Log("chosen: " + pos);
            }

            cur.pos = pos;
            spawnObj(cur);
            spawned.Add(cur);
        }
        return spawned;
    }

    public bool checkNearDoors(Vector2Int pos, int dist) {
        for (int i = 0; i < poles.Length; i++) {
            //if (doors[(i + numNeigh/2) % numNeigh] && Vector2Int.Distance(pos, poles[i]) < dist) {
            if (doors[i] && Vector2Int.Distance(pos, poles[i]) < dist) {
                return true;
            }
        }
        return false;
    }

    public bool checkNearDoors(Vector2Int pos) {
        return checkNearDoors(pos, 10);
    }

    public void circleRoom() {
        //15 - 50 for dodging
        float dia = Random.Range(27,40);
        //13 for dodging, rework to percent, perhaps 0.5
        int num = (int)(dia * Random.Range(1.33f, 2f));
        //Debug.Log("circle room - dia: " + dia + ", num: " + num);
        //spawnInHallList = new List<Vector3Int>();
        spawnInRoomList = new List<Vector3Int>();
        creator.pathStart();
        Vector2Int pre = Vector2Int.zero;
        for (int i = 0; i < num; i++) {
            int angle = i * (360 / num);
            int x = (int)Mathf.Round(Map.S.worldSizeX/2 + (dia * Mathf.Sin(angle)));
            int y = (int)Mathf.Round(Map.S.worldSizeY/2 + (dia * Mathf.Cos(angle)));
            Vector2Int pos = new Vector2Int(x, y);
            int size = Random.Range(8, 18);
            makeNode(pos);
            if (pre != Vector2Int.zero) {
                connectPath(pre, pos);
            }
            pre = pos;
            creator.clearCircle(pos, size);
            //spawnInHallList.Add(new Vector3Int(pos.x, pos.y, size));
            spawnInHall(pos, size);
        }
        if (Random.value > 0.0f) {
            int max = (int)(dia * 2 - 2);
            int size = Random.Range(3, max);
            Vector2Int cen = new Vector2Int(Map.S.worldSizeX/2, Map.S.worldSizeY/2);
            creator.clearCircle(cen, size);
            if (size >= max * 0.15f) {
                spawnInRoomList.Add(new Vector3Int(cen.x, cen.y, size));
            }
        }
        wallsAndDoors();
        /*
        for (int i = 0; i < spawnInHallList.Count; i++) {
            spawnInHall(new Vector2Int(spawnInHallList[i].x, spawnInHallList[i].y), spawnInHallList[i].z);
        }
        spawnInHallList.Clear();
        */
    }
        	
	public Vector2Int roomNum;
    Vector2Int roomSize;
    public bool cardinalsOnly = false;//true;

	public void createDungeon() {
        cardinalsOnly = Random.value > 0.5f;
		Vector2Int spawnRoom = new Vector2Int(Random.Range(0, roomNum.x), Random.Range(0, roomNum.y));
		//int room = dungeonRoomSize - 10;
        Vector2Int room = new Vector2Int(roomSize.x - 10, roomSize.y - 10);
        //spawnInHallList = new List<Vector3Int>();
        spawnInRoomList = new List<Vector3Int>();
        subRooms = new List<Node>();
        creator.pathStart();
        if (Random.value > 0.5f) {
            for (int x = 0; x < roomNum.x; x++) {
                for (int y = 0; y < roomNum.y; y++) {
                    Vector2Int sp = new Vector2Int(roomSize.x * x + roomSize.x/2, roomSize.y * y + roomSize.y/2);
                    createRoom(sp, roomSize, true);
                }
            }
        } else {
            createRoom(new Vector2Int(Map.S.worldSizeX/2, Map.S.worldSizeY/2), roomSize, true);
            for (int i = poles.Length-1; i >= 0; i--) {
                Vector2Int sp = poles[i];//new Vector2Int(roomSize.x * x + roomSize.x/2, roomSize.y * y + roomSize.y/2);
                createRoom(sp, roomSize, false);
            }
        }

        for (int i = 0; i < subRooms.Count; i++) {
            Node n1 = subRooms[i];
            for (int j = 1; j < subRooms.Count; j++) {
                Node n2 = subRooms[(i+j) % subRooms.Count];
                n1.addNeighbor(n2);
            }
        }
        for (int i = 0; i < subRooms.Count; i++) {
            //subRooms[i].print();
        }
        List<Node> neigh = new List<Node>();
        List<Node> from = new List<Node>();
        List<Node> visit = new List<Node>();

        Node cur = subRooms[Random.Range(0, subRooms.Count)];
        while (visit.Count < subRooms.Count) {
            if (!visit.Contains(cur)) {
                visit.Add(cur);
                for (int i = 0 ; i < cur.neighbors.Count; i++) {
                    Node n = cur.neighbors[i];
                    if (!visit.Contains(n) && !neigh.Contains(n)) {
                        neigh.Add(n);
                        from.Add(cur);
                    }
                }
            }
            if (neigh.Count == 0) {
                break;
            }
            int c = Random.Range(0, neigh.Count);
            Node pre = cur;//from[c];
            cur = neigh[c];
            
            connectRoom(pre.pos, cur.pos);
            connectPath(pre.pos, cur.pos);
            neigh.RemoveAt(c);
            from.RemoveAt(c);
        }
        /*
		List<Vector2Int> neighbors = new List<Vector2Int>();
		List<Vector2Int> from = new List<Vector2Int>();
		List<Vector2Int> visited = new List<Vector2Int>();
		Vector2Int cur = spawnRoom;

		while (visited.Count < roomNum.x * roomNum.y) {
			if (!visited.Contains(cur)) {
				visited.Add(cur);
               // pathNode p = creator.makePathNode(new Vector2Int(roomSize.x * cur.x + roomSize.x/2, roomSize.y * cur.y + roomSize.y/2));
                //p.name = "PN " + cur.x + ", " + cur.y;
				addRoomNeighbors(cur, neighbors, from, visited);
			}
			if (neighbors.Count == 0) {
				//Debug.Log("no more neighbors");
				break;
			}
			int c = Random.Range(0, neighbors.Count);
			cur = neighbors[c];
			Vector2Int pre = from[c];
			connectRoom(pre, cur);
            connectPath(new Vector2Int(roomSize.x * pre.x + roomSize.x/2, roomSize.y * pre.y + roomSize.y/2) , new Vector2Int(roomSize.x * cur.x + roomSize.x/2, roomSize.y * cur.y + roomSize.y/2));
			neighbors.RemoveAt(c);
			from.RemoveAt(c);
		}
        */
        wallsAndDoors();
	}

    public void createRoom(Vector2Int pos, Vector2Int size, bool spawn) {
        if (Random.value > 0.5f) {
			int sizeX =  Random.Range(roomSize.x - 15, roomSize.x + 10);//78);
			int sizeY =  Random.Range(roomSize.y - 15, roomSize.y + 10);//78);
            creator.clearRect(pos, sizeX, sizeY);
        } else {
            creator.clearCircle(pos, Random.Range(roomSize.x-25, roomSize.x));//Mathf.Max(sizeX, sizeY));
        }
        Node n = new Node(pos);
        subRooms.Add(n);
        if (spawn) {
            spawnInRoomList.Add(new Vector3Int(pos.x, pos.y, roomSize.x));
        }
        makeNode(pos);
    }

    public void connectPath(Vector2Int pre, Vector2Int cur) {
        pathNode a = null;
        pathNode b = null;
        for (int i = 0; i < creator.paths.Count; i++) {
            if (creator.paths[i].getPos() == pre) {
                a = creator.paths[i];
            } else if (creator.paths[i].getPos() == cur) {
                b = creator.paths[i];
            }
            if (a != null && b != null) {
                break;
            }
        }
        connectNeighbors(a, b);
    }

    void connectNeighbors(pathNode a, pathNode b) {
        Vector2Int dir = b.getPos() - a.getPos();
        int d = -1;
        for (int i = 0; i < 8; i++) {
            Vector2Int ch = GM.S.dirs[i];
            if (sign(dir.x) == sign(ch.x) && sign(dir.y) == sign(ch.y)) {
                d = i;
                break;
            }
        }
        creator.setNeighbors(a, b, d);
    }

    int sign(float f) {
        if (f == 0) {
            return 0;
        } else {
            return (int)Mathf.Sign(f);
        }
    }

    pathNode makeNode(int x, int y) { return makeNode(new Vector2Int(x, y)); }
    pathNode makeNode(Vector2Int pos) {
        pathNode p = creator.makePathNode(pos);
        p.name = "PN " + pos;
        p.transform.parent = transform;
        return p;
    }

    public void pathRooms(Vector2Int a, Vector2Int b) {
        connectRoom(a, b);
        pathNode pa = creator.makePathNode(a);
        pathNode pb = creator.makePathNode(b);
        connectNeighbors(pa, pb);
    }

    public Vector2Int randomRoom() {
        return new Vector2Int(Random.Range(0, roomNum.x) * roomSize.x + roomSize.x/2, Random.Range(0, roomNum.y) * roomSize.y + roomSize.y/2);
    }

    public void wallsAndDoors() {
        for (int i = 0; i < Map.S.worldSizeX; i++) {
            for (int k = 0; k < Map.S.worldSizeY; k++) {
                
                if (i <= doorSpace || i > Map.S.worldSizeX - doorSpace - 2 || k <= doorSpace || k > Map.S.worldSizeY - doorSpace - 2) {
                    if (!Map.S.world[i,k].checkID(12)) { //door id == =12
                        Map.S.world[i,k].formEnter(Map.S.terrain);
                    }
                } else if (checkNearDoors(new Vector2Int(i, k))) {
                    Map.S.world[i,k].removeAllID(-1);
                }
            }
        }
    }


	void addRoomNeighbors(Vector2Int room, List<Vector2Int> neigh, List<Vector2Int> from, List<Vector2Int> visit) {
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				Vector2Int n = room + new Vector2Int(x, y);
				if (n.x > -1 && n.y > -1 && n.x < roomNum.x && n.y < roomNum.y) {
                    if (!cardinalsOnly || (x == 0 || y == 0)) {
					    if (!neigh.Contains(n) && !visit.Contains(n)) {
						    neigh.Add(n);
						    from.Add(room);
					    }
                    }
				}
			}
		}
	}
    
    public List<Vector3Int> spawnInHallList;
    public List<Vector3Int> spawnInRoomList;
    public float dodgeWallChance = 0;
    Vector2Int hallSizes;

    public virtual void connectRoom(Vector2Int a, Vector2Int b) {
        Vector2Int room = new Vector2Int(roomSize.x - 20, roomSize.y - 20);
		Vector2 dir = b - a;
        dir.Normalize();
		Vector2 cur = a;//new Vector2Int(roomSize.x * a.x + roomSize.x/2, roomSize.y * a.y + roomSize.y/2);
		Vector2 dest = b;//new Vector2Int(roomSize.x * b.x + roomSize.x/2, roomSize.y * b.y + roomSize.y/2);
		int width = Random.Range(4, 10);//(room.x + room.y)/4;// + Random.Range(0, 4);
        float pre = Mathf.Infinity;
        while (Vector2.Distance(cur, dest) < pre) {
            pre = Vector2.Distance(cur, dest);
            if (pre < 15 && Random.value < dodgeWallChance) {
                //break;
            }

            int size = Random.Range(hallSizes.x, hallSizes.y);

            Vector2Int c = new Vector2Int((int)cur.x, (int)cur.y);
            if (Random.value > 0.5) {
			    creator.clearRect(c, size, size);
            } else {
                creator.clearCircle(c, size);
            }
            spawnInHall(c, size);
			cur += dir * 2;// * (size/2);//width;
        }
	}

    public void setHallSize(int min, int max) {
        hallSizes = new Vector2Int(min ,max);
    }

    public virtual void spawnInHall(Vector2Int pos, int size) {}

    //public virtual void spawnInRoom(Vector2Int pos) { }

    public virtual void cleanUpRoom() {
        //Debug.Log(name + " cleaning up");
        if (myShot) {
            Debug.Log("cleaning up room with " + myShot.name);
            myShot.cleanUpRoom(this, creator);
        }
        for (int i = 0; i < denizens.Count; i++) {
            if (denizens[i]) {
                denizens[i].gameObject.SetActive(false);
                denizens[i].cleanUp();
            }
        }
        Map.S.removeUnImportant();
        StopCoroutine("beatingRoom");
        readyToGo = false;
        /*
        for (int i = 0; i < numNeigh; i++) {
            if (curDoors[i]) {
                curDoors[i].die();
            }
        }
        */
    }

    public void setUpDoor(int i) {
        if (doors[i]) {
            setUpDoor(poles[i].x, poles[i].y, i);
        }
    }

    DungeonObject setUpDoor(int xPos, int yPos, int dir) {
		int wid = 5;
		int len = 2;

		//creator.clearRect(new Vector2Int(xPos, yPos), wid, len);// wid + xPosMod, len + yPosMod);
		//GameObject d = Instantiate(door);
        DungeonObject d = logObj(door);//, new Vector2Int(xPos, yPos), false);
        d.pos = new Vector2Int(xPos, yPos);
        d.name = name + " DOOR: " + dir;
		Form doo = d.GetComponent<Form>();

        Transform skin = doo.skin.transform;
        if (dir == 1) {
            skin.eulerAngles = new Vector3(0, 0, 90);
            skin.position = doo.transform.position + new Vector3(-0.1f, 0, 0);
        } else if (dir == 2) {
            skin.eulerAngles = new Vector3(0, 0, 180);
            skin.position = doo.transform.position + new Vector3(0, -0.1f, 0);
        } else if (dir == 3) {
            skin.eulerAngles = new Vector3(0, 0, 270);
            skin.position = doo.transform.position + new Vector3(-0.9f, 0, 0);
        }
        if (dir % 2 == 1) {
			wid = 2;
			len = 5;
		}
        doo.width = wid;
		doo.length = len;
		doo.squareBody();
		//Map.S.spawnForm(d, xPos, yPos);

		Door dr = d.GetComponent<Door>();
		dr.dir = dir;
		curDoors[dir] = dr;
        //spawnObj(d);
        //denizens.Add(doo);
        return d;
	}

    public DungeonObject setUpObj(GameObject o, int xPos, int yPos) {
        return setUpObj(o, new Vector2Int(xPos, yPos));
    }

    public int sameDistanceThreshold = 2;

    public DungeonObject setUpObj(GameObject o, Vector2Int pos) {
        return setUpObj(o, pos, false);
    }

    public DungeonObject setUpObj(GameObject o, Vector2Int pos, bool checkSamePos) {
        if (checkSamePos) {
            for (int i = 0; i < denizens.Count; i++) {
                if (Vector2Int.Distance(denizens[i].pos,pos) <= sameDistanceThreshold) {
                    Debug.Log(name + " same position " + denizens[i].pos + "  " + pos);
                    return null;
                }
            }
        }
        //GameObject e = Instantiate(o, transform);
        DungeonObject obj = logObj(o);//e.GetComponent<DungeonObject>();
        if (!obj.self) {
            Debug.Log(obj.name + " object not made");
        }
        obj.self.squareBody();
       // Debug.Log(obj.name + " at " + pos);
        Vector2Int spawnPos = Map.S.findSpawnPos(obj.self, pos);
        //Debug.Log("spwend at + " + spawnPos);
        //if (!obj.self.checkBody(pos, false, false)) {
        if (spawnPos.x < 0 || spawnPos.y < 0) {
            Debug.LogWarning(obj.name + " object could not be spawned at " + pos);
            for (int i = 0; i < obj.self.curCollided.Count; i++) {
                Debug.LogWarning("hit " + obj.self.curCollided[i]);
            }
            denizens.Remove(obj);
            Destroy(obj.gameObject);
            return null;
        }
        
        obj.pos = spawnPos;// new Vector2Int((int)(Map.S.worldSizeX/2), (int)(Map.S.worldSizeY * 0.75f));
        if (obj.isTarget) {
            targetCount++;
        }
        return obj;
    }

    public void addTarget(DungeonObject obj) {
        if (!denizens.Contains(obj)) {
            denizens.Add(obj);
        }
        targetCount++;
    }

    LargeMass curDone;
    CreepDebris curDebris;

    public DungeonObject logObj(GameObject o) {
        GameObject e = Instantiate(o, transform);
        Form f = e.GetComponent<Form>();
        if (f && f.id == 37) {
            if (curDone) {
                Destroy(e);
                return curDone;
            } else {
                curDone = f.GetComponent<LargeMass>();
            }
        } else {
            CreepDebris cd = e.GetComponent<CreepDebris>();
            if (cd) {
                if (curDebris) {
                    Destroy(e);
                    return curDebris;
                } else {
                    curDebris = cd;
                }
            }
        }
        DungeonObject obj = e.GetComponent<DungeonObject>();
        obj.receiveMaster(this);
        denizens.Add(obj);
        return obj;
    }

    public bool spawnObj(DungeonObject dunObj) { 
        if (!dunObj.spawned) {
            dunObj.gameObject.SetActive(true);
            // use cautiously, mostly for doors at this poit
            //    note, fix doors so they dont need this
            if (dunObj.clearSpace == 0) {
                creator.clearRect(dunObj.pos, dunObj.self.width, dunObj.self.length);
            } else if (dunObj.clearSpace > 0) {
                creator.removeTerrainCircle(dunObj.pos, dunObj.clearSpace);
            }
            
            if (!dunObj.spawnIn()) {
                if (dunObj.isTarget) {
                    Debug.LogWarning(name + " remove this guy("+dunObj.name+") from play");
                    targetCount--;
                    denizens.Remove(dunObj);
                    Destroy(dunObj.gameObject);
                    return false;
                }
            } 
        }
        return true;
        /*
        Form obj = dunObj.self;
        obj.active = true;
        
        Map.S.spawnForm(obj.gameObject, dunObj.pos.x, dunObj.pos.y);
        */
    }

    public bool enemiesInRoom() {
        for (int i = 0; i < denizens.Count; i++) {
            if (denizens[i].GetComponent<Enemy>() || denizens[i].GetComponent<BossEnemy>()) {
                return true;
            }
        }
        return false;
    }

    bool checkWaveComplete() {
        if (!enemiesInRoom()) {
            for (int i = 0; i < denizens.Count; i++) {
                if (denizens[i].partOfWave) {
                    return false;
                }
            }
            return true;
        } else {
            return false;
        }
    }

    public virtual void getDigs(mapTerrain mt, DungeonMaster n_dm) {
        creator = mt;
        dm = n_dm;
        setDoorSpace(dm.baseDoorSpace);
        dm.addRoom(this);
    }

    public void addObj(DungeonObject dun, Vector2Int pos) {
        dun.pos = pos;
        denizens.Add(dun);
        dun.transform.parent = transform;
    }

    //true if wave complete or room beat
    public void targetMet(DungeonObject obj) {
        Debug.Log(obj.name + " says target challenge met " + targetCount);
        if (!denizens.Contains(obj)) {
            Debug.LogError(obj.name + " is not in the denizens list");
        } else {
            if (obj.curCorpse) {
                //Debug.Log("it has a corpse");
                DungeonObject d = obj.curCorpse.GetComponent<DungeonObject>();
                if (d) {
                    //Debug.Log("its a dungeon object");
                    d.pos = obj.pos; 
                    denizens.Add(d);
                    d.transform.parent = transform;
                }
            }
            if (obj.removeOnMet) {
                denizens.Remove(obj);
            }
            if (obj.isTarget) {
                meetTarget();
            }
        }
    }

    public void meetTarget() {
        //Debug.Log("TARGET MET " + waves.Count + " " + targetCount);
        if (waves.Count < 1) {
            if (targetCount > 1) {
                targetCount--;
            } else {
                waveComplete();
                beatRoom(true);
            }
        } else {
            targetCount--;
            if (checkWaveComplete()) { //targetCount <= waveCount()) {
                waveComplete();
                spawnWave();
                if (myShot && myShot.waveText.Length > 0) {
                    myShot.narrator.dia.readText(myShot.waveText);
                    myShot.waveText = new string[0];
                }
            }
        }
    }

    void waveComplete() {
        for (int i = 0; i < denizens.Count; i++) {
            denizens[i].waveComplete();
        }
    }
    
    void spawnWave() {
        for (int i = 0; i < waves[0].Count; i++) {
            denizens.Add(waves[0][i]);
            dm.setSpawner(waves[0][i]);
            //waves[0][i].spawnIn();
        }
        waves.RemoveAt(0);
    }

    public void addToWave(DungeonObject dun) {
        if (waves.Count == 0) {
            newWave();
        }
        denizens.Remove(dun);
        dun.addToWave();
        waves[waves.Count-1].Add(dun);
    }

    public void newWave() {
        waves.Add(new List<DungeonObject>());
    }

    int waveCount() {
        int w = 0;
        for (int i = 0; i < waves.Count; i++) {
            w += waves[i].Count;
        }
        return w;
    }
    public int debrisCount;
    int fullDebris;
    bool clean;

    public void addDebris(int num) {
        debrisCount += num;
        fullDebris = debrisCount;
        clean = false;
    }

    public void resetDebris() {
        debrisCount = 0;
        fullDebris = 0;
    }

    public void cleanDebris() {
        if (!clean) {
            if (curDebris && curDebris.isTarget) {
                debrisCount--;
                if (debrisCount < fullDebris * curDebris.cleanPercent) {
                    meetTarget();
                    clean = true;
                }
            }
        }
    }
    
    public void setCleanPercent(float newPercent) {
        if (curDebris) {
            curDebris.cleanPercent = newPercent;
        }
    }

    public DungeonRoom checkNeighbor(int dir) {
        if (dir >= 0 && dir < neighbors.Length) {
            return neighbors[dir];//check];// != null;
        } else {
            return null;
        }
    }

    public void addNeighbor(DungeonRoom other, int dir) {
        if (dir >= 0 && dir < numNeigh) {
           //Debug.Log(pos + " now has neighbor: " + other.pos + " in direction " + dir);
            if (neighbors[dir] == null) {
                neighbors[dir] = other;
                //if (!doors[dir]) {
                    doors[dir] = true;//(dir + numNeigh/2) % numNeigh] = true;
                //}
            }
        } else {
            Debug.Log("bad dir " + dir);
        }
    }

    public void removeDenizen(DungeonObject d) {
        denizens.Remove(d);
    }

    public int getTargetCount() {
        return targetCount;
    }

    public void disconnectAllRooms() {
        for (int i = 0; i < 4; i++) {
            if (neighbors[i] != null) {
                int d = (i + numNeigh/2) % numNeigh;
                neighbors[i].neighbors[d] = null;
                //neighbors[i].doors[d] = false;
            }
        }
    }

    bool[,] save;
    pathNode[] savePath;
    public void saveWorld() {
        save = new bool[Map.S.worldSizeX, Map.S.worldSizeY];
        for (int x = 0; x < Map.S.worldSizeX; x++) {
            for (int y = 0; y < Map.S.worldSizeY; y++) {
                if (Map.S.world[x,y].within.Contains(Map.S.terrain)) {
                    save[x,y] = true;
                } else {
                    save[x,y] = false;
                }
            }
        }
        savePath = new pathNode[creator.paths.Count];
        //Debug.Log("saving " + savePath.Length + " path nodes");
        for (int i = 0; i < creator.paths.Count; i++) {
            savePath[i] = creator.paths[i];
        }
    }

    public void loadWorld() {
        if (save == null) {
            Debug.Log(name + " bad save");
            return;
        }
        for (int x = 0; x < Map.S.worldSizeX; x++) {
            for (int y = 0; y < Map.S.worldSizeY; y++) {
                if (save[x,y]) {
                    Map.S.world[x,y].formEnter(Map.S.terrain);
                } else {
                    Map.S.world[x,y].empty();
                }
            }
        }
        creator.paths = new List<pathNode>();
        for (int i = 0; i < savePath.Length; i++) {
            creator.paths.Add(savePath[i]);
        }
    }

    /*
    public void addDenizen(Form d) {
        denizens.Add(d);
    }
    */
}
