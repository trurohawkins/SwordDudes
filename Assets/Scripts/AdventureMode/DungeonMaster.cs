using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMaster : MonoBehaviour {
    public DungeonRoom curRoom;
    public Vector2Int dunSize;
    public int roomNum = 3;
    public int progress = 0;
    int levelsBefore = 0;
    public int baseDoorSpace = 10;
    public GameObject[] roomTypes;
    public GameObject powerRoom;

    public int[] bossOrder;
    public GameObject boss;
    public GameObject[] enemies;
    public GameObject heal;
    public GameObject target;
    public GameObject targetPedastle;
    public GameObject dummy;
    public GameObject spawner;

    public Storybo storybo;
    int level = 0;
    int power = 1;
    public int curBoss;
    List<DungeonRoom> rooms;
    public int numNeigh = 4;
    Vector2Int[] dirs;
    mapTerrain creator;
    // 0 - targets 1 - enemies
    public int skillLevel;
    int[] skillLevels;
    public int skillCap = 10;
    public GameObject debris;
    public GameObject walkFloor;
    public GameObject sword;
    public bool[] nextRoom;
    public Grader judge;
    public bool respawning;

    void Awake() {
        storybo = gameObject.GetComponent<Storybo>();
        judge = gameObject.GetComponent<Grader>();
        dirs = new Vector2Int[4];
        dirs[0] = new Vector2Int(0, 1);
        dirs[1] = new Vector2Int(-1, 0);
        dirs[2] = new Vector2Int(0, -1);
        dirs[3] = new Vector2Int(1, 0);

        skillLevels = new int[2];
        //only apply when loading
        //levelsBefore = progress * Random.Range(3, 5);

        
        creator = gameObject.GetComponent<mapTerrain>();
        rooms = new List<DungeonRoom>();

        deadRooms = new List<Vector2Int>();
        //deadRooms.Add(new Vector2Int(2,2));
        bossRooms = new List<Vector2Int>();
        //Debug.Log("loading data");
        loadData();
        storybo.setBoard(progress);
    }

    public bool noRoomClear = false;

    public DungeonRoom newRoom(DungeonRoom cur, int dir) {
        DungeonRoom room = null;
        if (cur && dir != -1) {
            Vector2Int pos = getPos(cur.pos, dir);
            for (int i = 0; i < bossRooms.Count; i++) {
                if (pos == bossRooms[i]) {
                    Shot s = storybo.getBossShot(progress);
                    if (s) {
                        room = Instantiate(roomTypes[s.roomType]).GetComponent<DungeonRoom>();
                        s.myRoom = room;
                        room.myShot = s;
                        //noRoomClear = s.noRoomClear;
                    } else {
                         room = Instantiate(roomTypes[0]).GetComponent<DungeonRoom>();
                    }
                    curBoss++;
                    break;
                }
            }
        }
        if (!room) {
            Shot s = storybo.startNextShot(dir, progress);
            if (s) {
                room = Instantiate(roomTypes[s.roomType]).GetComponent<DungeonRoom>();
                s.myRoom = room;
                room.myShot = s;
               // noRoomClear = s.noRoomClear;
            } else {
                //noRoomClear = false;
                if (getsPowerUp()) {
                    room = Instantiate(roomTypes[2]).GetComponent<DungeonRoom>();
                } else {
                    room = Instantiate(roomTypes[1]).GetComponent<DungeonRoom>();
                }
            }
        }
        return room;
    }

    public void clearRooms() {
        clearRooms(roomNum);
    }

    public void clearRooms(int remaining) {
        while (rooms.Count > remaining) {
            int rem = Mathf.Min(remaining, 1);
            DungeonRoom dr = rooms[rem].GetComponent<DungeonRoom>();
            dr.disconnectAllRooms();

            rooms.RemoveAt(rem);
            TargetRoom tr = dr.GetComponent<TargetRoom>();
            if (tr) {
                if (tr.getSkills() != null) {
                    judge.releaseSkills(tr.getSkills());
                }
            }
            Destroy(dr.gameObject);
        }
    }

    public void loadRoom(int dir) {
        if (curRoom) {
			curRoom.cleanUpRoom();
		}
        if (respawning) {
            curRoom = rooms[0];
            respawning = false;
        } else {
            DungeonRoom checkedRoom = checkRoom(curRoom, dir);
		    if (dir == -1 || (checkedRoom == null)) {//!curRoom.checkNeighbor(dir))) {
			    DungeonRoom room = newRoom(curRoom, dir);//Instantiate(dungeonRoom).GetComponent<DungeonRoom>();
                // adding room to rooms
			    if (curRoom) {
				    connectRooms(curRoom, room, dir);
			    } else {
				    room.getDigs(creator, this);
			    }
                if (!noRoomClear && rooms.Count > roomNum) {
                    clearRooms();
                }
			    curRoom = room;
		    } else if (curRoom) {
                //Debug.Log("cur: " + curRoom.name + " checked: " + checkedRoom.name);
			    curRoom = checkedRoom;
                storybo.setCurShot(curRoom.myShot);
                // revsiting, makes room at end of list, so less likely to be removed
                    // dont do to main room
                if (curRoom != rooms[0]) {
                    rooms.Remove(curRoom);
                    rooms.Add(curRoom);//Insert(rooms.Count-1, curRoom);
                }
		    }
        }
        judge.newRoom(curRoom);
        curRoom.spawnRoom(dir);
        if (storybo) {
            storybo.startShot();
        }
		Debug.Log("current room: " + curRoom.name);
    }

    public void addBossRoom() {
        bool goodSpawn = false;
        Vector2Int spawn = new Vector2Int(dunSize.x/2, dunSize.y/2);
        if (rooms.Count > 0) {
            spawn = rooms[0].pos;
        }
        int noLoop = dunSize.x * dunSize.y;
        do {
            goodSpawn = true;
            int x = Random.Range(0, dunSize.x);
            int y = Random.Range(0, dunSize.y);
            for (int i = 0; i < rooms.Count; i++) {
                if (rooms[i].pos.x == x && rooms[i].pos.y == y) {
                    goodSpawn = false;
                    break;
                }
            }

            goodSpawn = getDist(spawn.x, x, 0) + getDist(spawn.y, y, 1) > 1 && goodSpawn;
            if (goodSpawn) {
                Debug.Log("room added " + x + ", " + y);
                bossRooms.Add(new Vector2Int(x, y));
            } else {
                noLoop--;
            }
        } while (!goodSpawn && noLoop > 0);
        if (noLoop < 0) {
            Debug.LogError("couldn;t add boss room");
        }
    }

    public void makeBossRoom(Vector2Int room) {
        bossRooms.Clear();
        bossRooms.Add(room);
    }

    public DungeonRoom getCurRoom() {
        return curRoom;
    }

    public void newSpawnRoom(DungeonRoom room) {
        rooms.Remove(room);
        rooms.Insert(0, room);
    }

    public void removeRoom(DungeonRoom room) {
        rooms.Remove(room);
    }

    public void loadData() {
        if (progress < 1) {
            for (int i = 0; i < GM.S.getNumPlayers(); i++) {
                GM.S.hasSwords[i] = false;
            }
            dunSize = new Vector2Int(3, 3);
            //bossRooms.Add(new Vector2Int(0, 0));
            bossRooms.Add(new Vector2Int(1, 2));
            noRoomClear = true;
            if (boomBox.S) {
                boomBox.S.themeMusic(true);
            }
        } else if (progress < 2) {
            GameInfo.S.songs[0] = 7;
            noRoomClear = false;
            dunSize = new Vector2Int(3, 3);
            bossRooms.Clear();
            bossRooms.Add(new Vector2Int(0, 0));
            //addBossRoom();
            if (boomBox.S) {
                boomBox.S.startSwordSongs(false);
            }
        } else {
            noRoomClear = false;
            dunSize = new Vector2Int(3, 3);
            bossRooms.Clear();
            addBossRoom();
            if (boomBox.S) {
                boomBox.S.startSwordSongs(false);
            }
        }
    }

    public Vector2Int getDir(int d) {
        if (d < 0) {
            return dirs[0];
        }
        return dirs[d % 4];
    }

    public DungeonRoom checkRoom(DungeonRoom roo, int dir) {
        if (!roo || dir == -1) {
            return null;
        }
       
        // check if we already have a neighbor
        DungeonRoom next = roo.checkNeighbor(dir);
        Vector2Int pos = getPos(roo.pos, dir);//.pos + dirs[dir];
        //Debug.Log("checking neighbor for room at pos " + roo.pos + " dir: " + dir + " pos: " + pos);
        if (!next) {
            // see if one room should be the neighbor
            for (int i = 0; i < rooms.Count; i++) {
                if (rooms[i].pos == pos) {
                    next = rooms[i];
                    //Debug.Log("got one at " + pos);
                    break;
                }
            }
        }
        return next;
    }

    Vector2Int getPos(Vector2Int pos, int dir) {
        pos += dirs[dir];
        //Debug.Log("before: " + pos);
        for (int i = 0; i < 2; i++) {
            if (pos[i] < 0) {
                pos[i] = dunSize[i] + pos[i];
            } else {
                pos[i] = pos[i] % dunSize[i];
            }
        }
        //Debug.Log("after: " + pos);
        return pos;
    }

    int getDist(int a, int b, int axis) {
        int d1 = 0;
        int d2 = 0;
        int ta = a;
        int tb = b;
        while (ta != tb) {
            ta = (ta + 1) % dunSize[axis];
            d1++;
        }
        while (a != b) {
            if (a - 1 >= 0) {
                a -= 1;
            } else {
                a = dunSize[axis] - 1;
            }
            d2++;
        }

        return Mathf.Min(d1, d2);
    }

    public void addRoom(DungeonRoom roo) {
        if (!rooms.Contains(roo)) {
            if (rooms.Count == 0) {
                roo.pos = new Vector2Int(dunSize.x / 2, dunSize.y / 2);
                roo.name = "DUN: " + roo.pos.x + ", " + roo.pos.y;
            }
            rooms.Add(roo);
        } else { 
            Debug.LogWarning("double add of room: " + roo.name);
        }
    }

    public void connectRooms(DungeonRoom a, DungeonRoom b, int dir) {
        if (!b) {
            Debug.LogError("cant connect room a: " + a.name + " to rooom b, there is no b");
            return;
        }
        a.addNeighbor(b, dir);
        int d = (dir + numNeigh/2) % numNeigh;
        b.addNeighbor(a,  d);//(dir + numNeigh/2) % numNeigh);
        b.pos = getPos(a.pos, dir);// a.pos + dirs[dir];
        b.name = "DUN: " + b.pos.x + ", " + b.pos.y;
        b.getDigs(creator, this);
        addRoom(b);
    }

    public void disconnectRoom(DungeonRoom a, DungeonRoom b, int dir) {
        a.neighbors[dir] = null;
        a.doors[dir] = false;
        int d = (dir + numNeigh/2) % numNeigh;
        b.neighbors[d] = null;
        b.doors[d] = false;
    }

    List<Vector2Int> deadRooms;
    List<Vector2Int> bossRooms;

    public bool canRoomsConnect(DungeonRoom a, DungeonRoom b, int dir) {
        if (a) {
            //Debug.Log("checking " + a.name);
            if (!b) {
                //Debug.Log("in dir: " + dir + " there is no neighbor");
                Vector2Int check = getPos(a.pos, dir);
                for (int i = 0; i < deadRooms.Count; i++) {
                    if (check == deadRooms[i]) {
                        return false;
                    }
                }
                return false;
            }
            int d = (dir + numNeigh/2) % numNeigh;
            if (b.doors[d] == true) {//a.doors[d] == true && b.doors[dir] == true) {
                //Debug.Log(a.name + " can connect with " + b.name);
                return true;
            }
            Debug.Log(a.name + " cannot connect with " + b.name);
            return true;
        }
        return false;
    }

    public bool getPowerUp = false;
    int powerUp = 0;
    bool getsPowerUp() {
        return getPowerUp;
        if (powerUp < 2 && Random.value < 0.5f) {
            powerUp++;
            return true;
        }
        return false;
    }

    public Vector2Int getSkill() {
        return judge.getSkill();
        /*
        int lowest = skillCap;// (int)(Mathf.Infinity);
        int skill = 0;
        bool allBeat = true;
        for (int i = 0; i < skillLevels.Length; i++) {
            if (lowest > skillLevels[i]) {
                lowest = skillLevels[i];
                skill = i;
                allBeat = false;
            }
        }
        if (allBeat) {
            Debug.Log("beat skills");
            return -1;//bossOrder[level % bossOrder.Length];
        } else {
            return 0;//skill;
        }
        */
    }

    public int getSkillLevel(int skill, int type) {
        //Debug.Log("skill: " + skill + ", type: " + type + " = " + judge.getSkillLevel(skill, type));
        return judge.getSkillLevel(skill, type);
    }

    public float getProgress(int skill) {
        return (float)skillLevels[skill] / skillCap;
    }

    public void completeSkill(int skill) {
        //Debug.Log("completing room with skill " + skill);
        if (skill >= 0 && skill < skillLevels.Length) {
            if (skillLevels[skill] < skillCap) {
                skillLevels[skill]++;
            }
        } else {
            levelUp();
        }
    }

    public int getLevels() {
        return levelsBefore + judge.getRoomsBeaten();
    }

    public int getPower() {
        return level + power;
    }

    public int getAvgLevel() {
        return judge.getAvgLevel();
    }

    public int getBossSword() {
        return bossOrder[progress % bossOrder.Length];
        /*
        if (!storybo) {
            //return bossOrder[level % bossOrder.Length];
            return bossOrder[curBoss % bossOrder.Length];
        } else {
            return bossOrder[storybo.getBoss()];
        }
        */
    }

    public void beatBoss() {
        progress++;
    }

    public void levelUp() {
        for (int i = 0; i < skillLevels.Length; i++) {
            skillLevels[i] = 0;
        }
        level++;
    }

    public DungeonObject makeEnemy(int xp, int yp, DungeonRoom room) {
        return spawnEnemy(xp, yp, room, "Enemy", 0);
    }

    public DungeonObject spawnLitterEnemy(int xp, int yp, DungeonRoom room) {
        return spawnEnemy(xp, yp, room, "LitterBug", 1);
    }

    public DungeonObject spawnKnocker(int xp, int yp, DungeonRoom room) {
        return spawnEnemy(xp, yp, room, "Knocker", 2);
    }

    public DungeonObject spawnGloopMon(int xp, int yp, DungeonRoom room) {
        return spawnEnemy(xp, yp, room, "GloopMon", 3);
    }

    public DungeonObject spawnEnemy(int xp, int yp, DungeonRoom room, string name, int type) {
        DungeonObject dun = room.setUpObj(enemies[type], new Vector2Int(xp, yp));
        if (dun) {
            Enemy e = dun.GetComponent<Enemy>();
            e.name = name + room.denizens.Count;
            e.setStats(getProgress(1));
            return dun;
        }
        return null;
    }

    public DungeonObject spawnBoss(int song, int level, DungeonRoom room) {
        int np = GM.S.getNumPlayers() + 1;
        Vector3Int spawnPoint = new Vector3Int(Map.S.worldSizeX/2, Map.S.worldSizeY/2, -90);
        DungeonObject dun = room.setUpObj(boss, spawnPoint.x, spawnPoint.y);
        if (dun) {
            InputComputer ic = dun.GetComponent<InputComputer>();
            ic.difficultyLevel = level;
            Debug.Log("boss difficulty: " + level);
            Player pp = dun.GetComponent<Player>();
            BossEnemy b = dun.GetComponent<BossEnemy>();
            b.setSong(song);
            pp.soulType = GameInfo.S.souls[song];
            //need change pNum?
		    pp.setInfo(np - 1, GameInfo.S.colors[np-1], GameInfo.S.playerColors [np-1], -1);
		    pp.setSpawnInfo (spawnPoint);
		    GM.S.spawnPlayer (pp.gameObject, spawnPoint.x, spawnPoint.y);
            pp.dead = false;
            room.spawnObj(dun);
            dun.receiveMaster(room);

            return dun;
        } else {
            return null;
        }
    }

    public DungeonObject spawnSwordEnemy(int xp, int yp, DungeonRoom room) {
        DungeonObject dun = room.setUpObj(enemies[4], xp, yp);
        if (dun) {
            Vector3Int spawnPoint = new Vector3Int(xp, yp, 90);
            int np = 2;
            InputComputer ic = dun.GetComponent<InputComputer>();
            ic.difficultyLevel = 0;
            Player pp = dun.GetComponent<Player>();
            BossEnemy b = dun.GetComponent<BossEnemy>();
            b.setSong(10);
            pp.soulType = GameInfo.S.souls[10];
            //need change pNum?
		    pp.setInfo(np - 1, GameInfo.S.colors[np-1], GameInfo.S.playerColors [np-1], -1);
		    pp.setSpawnInfo (spawnPoint);
            dun.receiveMaster(room);
        }
        return dun;
    }

    public CreepDebris spawnDebris(Vector2Int center, int sizeX, int sizeY, DungeonRoom room) {
        //int dimension = 10;
        DungeonObject dirt = room.logObj(debris);
        Form d = dirt.GetComponent<Form>();
        CreepDebris cd = dirt.GetComponent<CreepDebris>();
        cd.setPhysical(center, sizeX, sizeY, true);
        return cd;
    }

    public CreepDebris spawnDebris(Vector2Int center, int size, DungeonRoom room) {
        //int dimension = 10;
        DungeonObject dirt = room.logObj(debris);
        return spawnMass(dirt, center, size).GetComponent<CreepDebris>();
    }

    public CreepDebris spawnDebrisRing(Vector2Int center, Vector2Int size, DungeonRoom room) {
        DungeonObject dirt = room.logObj(debris);
        Form d = dirt.GetComponent<Form>();
        CreepDebris cd = dirt.GetComponent<CreepDebris>();
        cd.setPhysicalRing(center, size, 1);
        return cd;
    }

    public CreepDebris spawnDebrisCircle(Vector2Int center, int size, DungeonRoom room) {
        DungeonObject dirt = room.logObj(debris);
        Form d = dirt.GetComponent<Form>();
        CreepDebris cd = dirt.GetComponent<CreepDebris>();
        cd.setPhysical(center, size, size, true);
        return cd;
    }

    public LargeMass spawnMass(DungeonObject dun, Vector2Int center, int size) {
        LargeMass lm = dun.GetComponent<LargeMass>();
        lm.setPhysical(center, size);
        return lm;
    }

    public LargeMass spawnMass(DungeonObject dun, Vector2Int center, int x, int y, bool circle) {
        LargeMass lm = dun.GetComponent<LargeMass>();
        lm.setPhysical(center, x, y, circle);
        return lm;
    }

    public LargeMass spawnWalk(Vector2Int center, int size, DungeonRoom room) {
        DungeonObject walk = room.logObj(walkFloor);
        return spawnMass(walk, center, size);
    }

    public LargeMass spawnWalk(Vector2Int center, int x, int y, DungeonRoom room) {
        DungeonObject walk = room.logObj(walkFloor);
        return spawnMass(walk, center, x, y, true);
    }

    public void spawnPowerUp(DungeonRoom room, Vector2Int pos) {
        spawnSword(bossOrder[Random.Range(0,5)], pos, room);
    }

    public DungeonObject setUpSword(int type, Vector2Int pos, DungeonRoom room) {
        DungeonObject dun = room.setUpObj(sword, pos);
        if (dun) {
            SwordPickup sp = dun.GetComponent<SwordPickup>();
            sp.setType(type);//swordType = type;
            return dun;
        }
        return null;
    }

    public GameObject spawnSword(int type, Vector2Int pos, DungeonRoom room) {
        //Form s = Instantiate(sword).GetComponent<Form>();
        //s.squareBody();
        DungeonObject dun = room.setUpObj(sword, pos);
        if (dun) {
            SwordPickup sp = dun.GetComponent<SwordPickup>();
            sp.setType(type);//swordType = type;
            room.spawnObj(dun);
            return dun.gameObject;
        }
        return null;
        //sp.receiveMaster(this);
        //Map.S.spawnForm(s.gameObject, pos.x, pos.y);
    }

    public DungeonObject spawnTarget(int con, int time, Vector2Int pos, DungeonRoom room) {
        DungeonObject dun = room.setUpObj(target, pos);
        if (dun) {
            Obstacle o = dun.GetComponent<Obstacle>();
            o.setCondition(con);
            o.timeReset = time;
            pos.y -= 3;
            /*
            DungeonObject obj = room.setUpObj(targetPedastle, pos, false);
            o.getIndicator(obj.transform.GetChild(0).GetComponent<SpriteRenderer>());
            */
            return dun;
        }
        return null;
    }

    public DungeonObject spawnDummy(int x, int y, DungeonRoom room) { return spawnDummy(new Vector2Int(x, y), room); }
    public DungeonObject spawnDummy(Vector2Int pos, DungeonRoom room) {
        DungeonObject dun = room.setUpObj(dummy, pos);
        if (dun) {
            SwordSoul ss = dun.GetComponent<SwordSoul>();
            ss.setColors(0);
            circleAttack ca = dun.GetComponent<circleAttack>();
            ca.name = "dummySword";
            if (room.spawnObj(dun)) {
                ca.meetBody(dun.self);
                ca.spawnSword();
            }
            return dun;
        }
        return null;
    }

    public DungeonObject spawnHeal(Vector2Int pos, DungeonRoom room) {
        DungeonObject dun = room.setUpObj(heal, pos);
        if (dun) {
            room.spawnObj(dun);
            return dun;
        }
        return null;
    }

    public spawningEnemy setSpawner(DungeonObject spawn) {
        spawningEnemy se = Instantiate(spawner, new Vector3(spawn.pos.x, spawn.pos.y, 0), transform.rotation, transform).GetComponent<spawningEnemy>();
        se.whatToSpawn = spawn;
        return se;
    }

    public void getCreator(mapTerrain mt) {
        creator = mt;
    }
}
