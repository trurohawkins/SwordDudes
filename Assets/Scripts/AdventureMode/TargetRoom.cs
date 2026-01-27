using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetRoom : DungeonRoom {

    public GameObject floorSwitch;
    bool deadEnd;
    // 0 open space 1, dungeon, 2 circle
    int roomType = 0;
    public override void shapeRoom(int dir) {
        int count = 0;
        for (int i = 0; i < doors.Length; i++) {
            if (doors[i]) {
                count++;
            }
        }
        int difficulty = dm.getLevels();
        if (dir != -1) {
            curSkills = new List<Vector2Int>();
            for (int i = 0; i <= Mathf.Max(1, difficulty/5); i++) {
                skill = dm.getSkill();
                Debug.Log("skill: " + skill);
                grd.curSkills[skill.x][skill.y] = true;
                curSkills.Add(skill);
            }
        }
       //Power();//getSkillPower(challengeType);
        Debug.Log("difficulty: " + difficulty);
        maxMonster = 20 + difficulty;
        monsterWave = 5 + difficulty;
        /*
        creator.clearCircle(new Vector2Int(Map.S.worldSizeX / 2, Map.S.worldSizeY / 2), 25);
        basicRoom(dir);//dungeonRoom();
        return;
        */
        //Debug.Log("Difficulty: " + difficulty + " spawnChance: " + spawnChance());
        deadEnd = count <= 1;
        //creator.clearCircle(new Vector2Int(Map.S.worldSizeX / 2, Map.S.worldSizeY / 2), 50);
        //if (difficulty <= 1) {
        if (dir == -1) {
            basicRoom(dir);
        } else {
            if (Random.value < 0.25f) {
                dungeonRoom();
            } else {
                circleRoom(deadEnd || dir == -1);
            }
        }
    }

    void basicRoom(int dir) {
        roomType = 0;
        Vector2Int center = new Vector2Int(Map.S.worldSizeX / 2, Map.S.worldSizeY / 2);
        creator.clearCircle(center, Map.S.worldSizeX - doorSpace*2);
        spawnInRoomList.Add(new Vector3Int(center.x, center.y, Map.S.worldSizeX - doorSpace*2));
    }

    void dungeonRoom() {
        roomType = 1;
        hallFrequency = 5;
        createDungeon();
    }

    void circleRoom(bool center) {
        setDoorSpace(2);
        roomType = 2;
        hallFrequency = 7;
        circleRoom();
        if (center) {
            creator.clearCircle(new Vector2Int(Map.S.worldSizeX / 2, Map.S.worldSizeY / 2), 50);
        }
    }

    public List<Vector2Int> getSkills() {
        return curSkills;
    }

    List<Vector2Int> curSkills;
    Vector2Int skill;
    public override bool setUpRoom(int dir) {
        //dm.spawnDummy(new Vector2Int(45, 60), this);
        //dm.spawnDebris(new Vector2Int(42, 12), 500, this);
        monsterPoints = (int)((dm.getLevels()+1) * monsterLevel);
        if (base.setUpRoom(dir)) {
            //dm.spawnDummy(new Vector2Int(20, 20), this);
            //dm.spawnWalk(new Vector2Int(5, 5), 10, 10, this);
            //dm.spawnKnocker(30, 10, this);
            for (int i = 0; i < curSkills.Count; i++) {
                int lvl = dm.getSkillLevel(curSkills[i].x, curSkills[i].y);
                //Debug.Log(lvl + " " + spawnInRoomList.Count);
                if (spawnInRoomList != null && spawnInRoomList.Count > 0) {
                    // type of skill
                    //if (curSkills[i].x == 0) {
                        // sub category of type
                    int numTar = lvl + 2;
                    for (int j = 0; j < numTar; j++) {
                        if (spawnInRoomList.Count > 0) {
                            int p = Random.Range(0, spawnInRoomList.Count-1);
                            int size = spawnInRoomList[p].z;
                            Vector2Int pos = new Vector2Int(spawnInRoomList[p].x + Random.Range(-size/2, size/2), spawnInRoomList[p].y +  Random.Range(-size/2, size/2));
                            if (spawnInRoom(pos, spawnInRoomList[p].z, curSkills[i]) == null) {
                                Debug.Log("cpuldnt spawn here r " + pos);
                                //j--;
                            }
                        }
                    }
                    //}
                } else {
                    /*
                    Debug.Log("no rooms to spawn in");
                    int numTar = lvl + 2;
                    int minDist = 5;
                    int distance = (lvl + 1) + minDist;
                    Vector2Int pos = new Vector2Int(Map.S.worldSizeX/2, Map.S.worldSizeY/2);//randomRoom();// new Vector2Int(Random.Range(0, Map.S.worldSizeX), Random.Range(0, Map.S.worldSizeY));
                    List<DungeonObject> spawned = spawnAround(floorSwitch, pos, numTar, minDist, distance);
                    for (int j = 0; j < spawned.Count; j++) {
                        Obstacle o = spawned[j].GetComponent<Obstacle>();
                        if (o) {
                            Debug.Log(pos);
                            o.name = "OBJ " + pos;
                            o.condition = (int)Mathf.Lerp(0, 2, dm.getProgress(challengeType));
                            o.timeReset = -1;
                        }
                    }
                    */
                }
            }
            int noLoop = 0;
            //Debug.Log("spawn in room count " + spawnInRoomList.Count);
            while (monsterPoints > 0 && spawnInRoomList.Count > 0 && noLoop < 60) {
                int p = Random.Range(0, spawnInRoomList.Count-1);
                int size = spawnInRoomList[p].z;
                Vector2Int sPos = new Vector2Int(spawnInRoomList[p].x + Random.Range(-size/2, size/2), spawnInRoomList[p].y +  Random.Range(-size/2, size/2));
                if (Random.value < 0.4f) {
                    spawnMonster(sPos, 0);
                } else {
                    spawnMonster(sPos, 1);
                }
                //Debug.Log("monsterPoints: " + monsterPoints);
                noLoop++;
            }
            /*
            for (int i = 0; i < (int)((dm.getLevels()+1) * monsterLevel); i++) {
                if (spawnInRoomList.Count > 0) {
                    int p = Random.Range(0, spawnInRoomList.Count-1);
                    int size = spawnInRoomList[p].z;
                    Vector2Int sPos = new Vector2Int(spawnInRoomList[p].x + Random.Range(-size/2, size/2), spawnInRoomList[p].y +  Random.Range(-size/2, size/2));
                    DungeonObject dObj;
                    if (Random.value < 0.6f) {
                        dObj = dm.spawnLitterEnemy(sPos.x, sPos.y, this);
                    } else {
                        dObj = dm.makeEnemy(sPos.x, sPos.y, this);
                        if (dObj) {
                            Enemy mon = dObj.GetComponent<Enemy>();
                            mon.setStats(0.1f);
                        }
                    }
                    if (dObj) {
                        if (i > 5) {
                            addToWave(dObj);
                        }
                    }
                }
            }
            */
            if ((int)((dm.getLevels()+1) * monsterLevel) > 0) {
                getPrize = true;
            }
        }
        return true;
    }

    int switchCount = 0;
    void spawnSwitch(Vector2Int pos, int resetTime) {
        DungeonObject dun = setUpObj(floorSwitch, pos);
        if (dun) {
            spawnObj(dun);
            Walkable w = dun.GetComponent<Walkable>();
            w.name = "floroSwitch " + switchCount;
            switchCount++;
            w.resetTime = resetTime;
        }
    }

    public override void connectRoom(Vector2Int a, Vector2Int b) {
        if (Random.value > 0.5 && hallCount < 1) {
            if (curSkills != null) {//.Count > 0) {
                hallSpawn = true;
                skill = curSkills[Random.Range(0, curSkills.Count-1)];
                hallCount++;
            } else {
                //Debug.Log("no current skills");
            }
        } else {
            hallSpawn = false;
        }
        base.connectRoom(a, b);
    }

    int hallCount = 0;
    int hallDenizenCount = 0;
    int hallFrequency = 6;
    bool hallSpawn = true;

    int dirtHallCount = 0;
    public int maxDirt = 5;
    public float dirtHallChance = 0.5f;

    int monsterHallCount = 5;
    int maxMonsterHall;
    int maxMonster = 20;
    public float monsterLevel;
    int monsterCount;
    int monsterWave = 5;
    int monsterPoints;

    public override void spawnInHall(Vector2Int pos, int size) {
        if (myShot && !myShot.randomRoom) {
            return;
        }
        if (checkNearDoors(pos)) {// || pos.x == Map.S.worldSizeX/2) {
            //Debug.Log("near door or center " + pos);
        }
        if (hallSpawn && !checkNearDoors(pos)) {
            //Debug.Log("count: " + hallDenizenCount);
            if (hallDenizenCount % hallFrequency == 0) {
                if(spawnCheck()) {
                    int difficulty = dm.getSkillLevel(skill.x, skill.y);
                    switch(skill.x) {
                        case 0: {
                            switch(skill.y) {
                                default:
                                    // move accuracy
                                    //spawnSwitch(pos, -1);
                                    //Debug.Log("spawn walk in hall");
                                    dm.spawnWalk(pos, 10, 10, this);//.spawnIn();
                                    //dm.spawnDebris(pos, 52, this);
                                    break;
                                case 1:
                                    // creeping
                                    //dm.spawnDebris(pos, 52, this);
                                    /*if (difficulty < 2) {
                                        spawnDebris(pos, difficulty);
                                    } else {
                                    */
                                    spawnMonsterHall(pos, difficulty);
                                    //}
                                    break;
                                case 2:
                                    // speed
                                    //dm.spawnDebris(pos, 52, this);
                                    //spawnSwitch(pos, -1);
                                    dm.spawnWalk(pos, 5, 5, this);
                                    break;
                            }
                            break;
                        }
                        case 1: {
                            switch(skill.y) {
                                default:
                                    creator.fillCircle(pos, 20);
                                    break;
                            }
                            break;
                        }
                        case 2: {
                            switch(skill.y) {
                                default:
                                    spawnMonsterHall(pos, difficulty);
                                    break;
                            }
                            break;
                        }
                        default: {
                            break;
                        }
                    }
                }
            }
            hallDenizenCount++;
        }
        /*
        if (dirtHallCount < maxDirt) {
            if (Random.value < dirtHallChance) {
                dm.spawnDebris(pos, 25, this);
                dirtHallCount++;
            }
        }
        */
    }

    public DungeonObject spawnInRoom(Vector2Int pos, int size, Vector2Int skill) {
        int difficulty = dm.getSkillLevel(skill.x, skill.y);
        //Debug.Log("spawn in room difficulty: " + difficulty + " at " + pos + " in room size: " + size);
        
        if (checkNearDoors(pos)) {
            Debug.LogWarning(skill + " near door");
        }
        DungeonObject dun = null;
        switch(skill.x) {
            case 0:
                switch(skill.y) {
                    default:
                        // move accuracy
                        //spawnSwitch(pos, -1);
                        dun = dm.spawnWalk(pos, 20, 20, this);
                        break;
                    case 1:
                        // creeping
                        /*
                        if (difficulty < 1) {
                            dun = spawnDebris(pos, difficulty);
                        } else {
                        */  
                        dun = spawnMonster(pos, 4);//dm.makeEnemy(pos.x, pos.y, this);
                        if (dun) {
                            Enemy e = dun.GetComponent<Enemy>();
                            e.motionSensitive = true;
                        }
                        //}
                        break;
                    case 2:
                        // speed
                        dun = dm.spawnWalk(pos, 20, 20, this);
                        break;
                }
                Debug.Log("making " +  (int)(difficulty * monsterLevel) + " enemeis");
                /*
                for (int i = 0; i < (int)((difficulty+1) * monsterLevel); i++) {
                    dm.spawnLitterEnemy(pos.x, pos.y, this);
                }
                */
                break;
            case 1:
                switch(skill.y) {
                    default:
                        // thru obstacle
                        dun = dm.spawnWalk(pos, 20, 20, this);
                        break;
                }
                break;
            case 2:
                switch(skill.y) {
                    default:
                        // swing accuracy
                        if (difficulty < 1) {
                            int t = -1;
                            if (difficulty != 0) {
                                t = (int)Mathf.Lerp(400, 700, (3 - difficulty) / 3);
                            }
                            dun = dm.spawnTarget((int)Mathf.Lerp(0, 2, difficulty / 3), t, pos, this);
                        }/* else if (difficulty <= 4) {
                            dun = dm.spawnDummy(pos, this);
                        }*/ else {
                            //dun = dm.makeEnemy(pos.x, pos.y, this);
                            dun = spawnMonster(pos, 1);
                        }
                        break;
                    case 1:
                        // knockback
                        dun = spawnMonster(pos, 2);//dm.spawnKnocker(pos.x, pos.y, this);//dm.makeEnemy(pos.x, pos.y, this);
                        Enemy knk = dun.GetComponent<Enemy>();
                        //monsterPoints -= spawn
                        //knk.knockBackEnemy(dm.getProgress(1));
                        break;
                    case 2:
                        // stagger
                        dun = spawnMonster(pos, 3);// dm.makeEnemy(pos.x, pos.y, this);
                        /*
                        if (dun) {
                            Enemy stg = dun.GetComponent<Enemy>();
                            stg.staggerEnemy(dm.getProgress(1));
                        }
                        */
                        break;
                }
                        /*
                for (int i = 0; i < (int)(difficulty * monsterLevel) + 1; i++) {
                    dm.makeEnemy(pos.x, pos.y, this);
                }
                */
                break;
            default:
                break;
        }
        //Debug.Log(dun);
        return dun;
        //if(spawnCheck()) {
            //spawnObstacle(pos);
        //}
    }

    public bool spawnCheck() {
        return Random.value < 1;//spawnChance();
    }

    public float spawnChance() {
        return ((float)dm.skillLevel + 1) / 30;
    }

    DungeonObject spawnDebris(Vector2Int pos, int difficulty) {
        if (dirtHallCount < maxDirt * Mathf.Max(1, (difficulty * 075f))) {
            dirtHallCount++;
            return dm.spawnDebris(pos, 26 * (1+difficulty), this);
        }
        return null;
    }

    void spawnMonsterHall(Vector2Int pos, int difficulty) {
        if (monsterHallCount < maxMonsterHall * Mathf.Max(1, (difficulty * 075f))) {
            dm.makeEnemy(pos.x, pos.y, this);
            //dm.spawnDebris(pos, 26 * (1+difficulty), this);
            monsterHallCount++;
        }
    }

    public DungeonObject spawnMonster(Vector2Int pos, int type) {
        DungeonObject dun = null;
        int points = 0;
        if (type == 0) {
            dun = dm.spawnLitterEnemy(pos.x, pos.y, this);
            points = 1;
        } else if (type == 1) {
            dun = dm.makeEnemy(pos.x, pos.y, this);
            if (dun) {
                Enemy mon = dun.GetComponent<Enemy>();
                float power = Mathf.Min(1, Random.Range(0, ((dm.getLevels()+1) * monsterLevel)/maxMonster));
                //Debug.Log("power: " + power);
                mon.setStats(power);
                points = Mathf.Max(1, (int)(power * 4));
            }
        } else if (type == 2) {
            dun = dm.spawnKnocker(pos.x, pos.y, this);
            if (dun) {
                Enemy knk = dun.GetComponent<Enemy>();
                float power = Mathf.Min(1, Random.Range(0, dm.getProgress(1)));
                //Debug.Log("power: " + power);
                knk.knockBackEnemy(power);
                points = Mathf.Max(1, (int)(power * 6));
            }
        } else if (type == 3) {
            dun = dm.makeEnemy(pos.x, pos.y, this);
            if (dun) {
                Enemy stg = dun.GetComponent<Enemy>();
                float power = Mathf.Min(1, Random.Range(0.1f, dm.getProgress(1)));
                stg.staggerEnemy(power);
                points = Mathf.Max(1, (int)(power * 8));
            }
        } else if (type == 4) {
            dm.makeEnemy(pos.x, pos.y, this);
            if (dun) {
                Enemy mon = dun.GetComponent<Enemy>();
                mon.motionSensitive = true;
                float power = Mathf.Min(1, Random.Range(0.1f, ((dm.getLevels()+1) * monsterLevel)/maxMonster));
                mon.setStats(power);
                points = Mathf.Max(1, (int)(power * 2));
            }
        }
        if (points != 0 && dun) {
            //Debug.Log("monster made " + monsterCount);
            monsterCount++;
            if (monsterCount % monsterWave == 0) {
                //Debug.Log("new wave");
                newWave();
            }
            if (monsterCount >= monsterWave) {
                addToWave(dun);
            }
            monsterPoints -= points;
        }
        return dun;
    }

    void spawnObstacle(Vector2Int pos) {
        spawnSwitch(pos, -1);
    }

    // used to spawn a target in room while it is running
    public bool spawnTarget(int xPos, int yPos, int condition, int timer) {
        DungeonObject dun = dm.spawnTarget(condition, timer, new Vector2Int(xPos, yPos), this);//setUpObj(guy, xPos, yPos);
        if (dun) {
            Obstacle tar = dun.GetComponent<Obstacle>();
            //tar.setCondition(condition);
            //tar.timeReset = timer;
            spawnObj(dun);
            return true;
        } else {
            Debug.Log("could not set up floor switch");
            return false;
        }
    }

    public bool spawnSwitch(int xPos, int yPos, int timer, bool check) {
        DungeonObject dun = setUpObj(floorSwitch, new Vector2Int(xPos, yPos), check);
        if (dun) {
            Walkable walk = dun.GetComponent<Walkable>();
            walk.resetTime = timer;
            spawnObj(dun);
            return true;
        } else {
            return false;
        }
    }

    public override void beatOfRoom() {
        Debug.Log(name + " was beat");
        if (curSkills != null) {
            dm.judge.releaseSkills(curSkills);
        }
        int count = 0;
        for (int i = 0; i < doors.Length; i++) {
            if (doors[i]) {
                count++;
            }
        }
        if (count <= 1) {
            /*
            Vector2Int cen = new Vector2Int(Map.S.worldSizeX/2, Map.S.worldSizeY/2);
            for (int i = 0; i < denizens.Count; i++) {
                if (denizens[i].pos == cen) {
                    Form f = denizens[i].self;
                    Debug.Log(f.name + " got in the way");
                    f.die();
                    denizens.RemoveAt(i);
                    Destroy(f.gameObject);
                }
            }
            dm.spawnPowerUp(this, cen);
            */
        }
    }
}
