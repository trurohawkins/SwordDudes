using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstSwinging : Shot {

    GameObject spawnedSword;
    public GameObject learnSwing;
    public string[] alternate;

    public override void beforeShot() {
        if (narrator.getBroom()) {
            //text = alternate;
        }
    }

    public override void shapeRoom(DungeonRoom room, mapTerrain creator, int dir) {
        
        room.setHallSize(9, 14);
        room.setDoorSpace(5);
        int sx = Map.S.worldSizeX;
        int sy = Map.S.worldSizeY;
        DungeonObject dun = room.logObj(storyPoint[0]);
        StoryPoint stry = dun.GetComponent<StoryPoint>();
        stry.getDialogue(narrator.dia);
        //room.doors[1] = room.doors[3] = true;
        List<Vector2Int> enemySpawns = new List<Vector2Int>();
        if (dir == 0) {
            room.doors[3] = true;
            // first hall
            //room.connectRoom(new Vector2Int(45, 8), new Vector2Int(45, 27));
            // long horizontal hall
            room.connectRoom(new Vector2Int(10, 20), new Vector2Int(80, 20));
            // 2 downward halls connecting to target rooms
            //room.connectRoom(new Vector2Int(10, 27), new Vector2Int(10, 5));
            //room.connectRoom(new Vector2Int (80, 27), new Vector2Int(80, 5));
            creator.clearCircle(new Vector2Int(15, 10), 20);
            creator.clearCircle(new Vector2Int(75, 10), 20);
            room.dm.spawnHeal(new Vector2Int(15, 15), room);
            room.dm.spawnHeal(new Vector2Int(75, 15), room);
            Vector2Int circle = new Vector2Int((int)(Map.S.worldSizeX * 0.5f), (int)(Map.S.worldSizeY * 0.6f));
            int size = 60;
            creator.clearCircle(circle, size);
            creator.fillCircle(circle, size - 12);
            creator.clearCircle(circle, size - 22);

            room.dm.spawnDebrisRing(circle, new Vector2Int(10, 10), room);
            room.dm.spawnLitterEnemy(circle.x, circle.y, room);

            enemySpawns.Add(new Vector2Int(circle.x, sy/2));
            enemySpawns.Add(new Vector2Int(circle.x, (sy/4)*3));
            enemySpawns.Add(new Vector2Int(sx/3, (sy/3)*2));
            enemySpawns.Add(new Vector2Int((sx/3)*2, (sy/3)*2));
            
        } else if (dir == 2) {
            room.doors[1] = true;
            // first hall
            //room.connectRoom(new Vector2Int(45, 82), new Vector2Int(45, 61));
            // long horizontal hall
            room.connectRoom(new Vector2Int(10, 71), new Vector2Int(80, 71));
            // 2 downward halls connecting to target rooms
            //room.connectRoom(new Vector2Int(10, 63), new Vector2Int(10, 85));
            //room.connectRoom(new Vector2Int (80, 63), new Vector2Int(80, 85));
            creator.clearCircle(new Vector2Int(15, 80), 18);
            creator.clearCircle(new Vector2Int(75, 80), 18);
            room.dm.spawnHeal(new Vector2Int(15, 75), room);
            room.dm.spawnHeal(new Vector2Int(75, 75), room);

            Vector2Int circle = new Vector2Int((int)(Map.S.worldSizeX * 0.5f), (int)(Map.S.worldSizeY * 0.4f));
            int size = 60;
            creator.clearCircle(circle, size);
            creator.fillCircle(circle, size - 12);
            creator.clearCircle(circle, size - 22);



            room.dm.spawnDebrisRing(circle, new Vector2Int(10, 10), room);
            room.dm.spawnLitterEnemy(circle.x, circle.y, room);
            /*
            enemySpawns.Add(new Vector2Int(70, 15));
            enemySpawns.Add(new Vector2Int(70, 10));
            enemySpawns.Add(new Vector2Int(20, 15));
            enemySpawns.Add(new Vector2Int(20, 10));
            */
        }
        room.wallsAndDoors();
        /*
        for(int i = 0; i < enemySpawns.Count; i++) {
            makeEnemyWave(enemySpawns[i].x, enemySpawns[i].y, 5, room);
        }
        */
        
        room.newWave();
        for(int i = 0; i < enemySpawns.Count; i++) {
            makeEnemyWave(enemySpawns[i].x, enemySpawns[i].y, 7, room);
        }
       
        /*
        room.doors[0] = true;
        room.doors[2] = true;
        creator.clearRect(Map.S.worldSizeX/2, Map.S.worldSizeY / 2, 19, (int)(Map.S.worldSizeY * 0.85));
        creator.clearCircle(Map.S.worldSizeX/2, Map.S.worldSizeY - doorSpace, (int)(Map.S.worldSizeX * 0.85f));
        room.wallsAndDoors();
        StoryPoint sp = room.setUpObj(storyPoint, new Vector2Int(45, 50), false).GetComponent<StoryPoint>();
        sp.getDialogue(narrator.dia);
        TargetRoom tr = room.GetComponent<TargetRoom>();
        if (tr) {
            tr.dm.spawnTarget(0, -1, new Vector2Int((int)(Map.S.worldSizeX * 0.27f), (int)(Map.S.worldSizeY * 0.85f)), tr);
            tr.dm.spawnTarget(0, -1, new Vector2Int((int)(Map.S.worldSizeX * 0.8f), (int)(Map.S.worldSizeY * 0.85f)), tr);
        }
        if (!narrator.getBroom()) {
            creator.clearRect((int)(Map.S.worldSizeX * 0.666f), (int)(Map.S.worldSizeY * 0.2f), 27, 10);
            spawnedSword = room.dm.spawnSword(10, new Vector2Int((int)(Map.S.worldSizeX * 0.666f) + 5, (int)(Map.S.worldSizeY * 0.2f)), room);
        } else {
            Debug.Log("BY GEORGE he has a broom");
        }
        */
        room.setUpRoom(-1);
    }

    DungeonObject makeEnemyWave(int xp, int yp, int hp, DungeonRoom room) {
        DungeonObject dun = room.dm.spawnLitterEnemy(xp, yp, room);
        if (dun) {
            room.addToWave(dun);
            Enemy e = dun.GetComponent<Enemy>();
            if (e) {
                //e.setDamage(5);
                //e.moveAndShoot = true;
                //e.setHealth(hp);
            } else {
                Debug.Log("not an enemy");
            }
        }
        return dun;
    }

    public override void cleanUpRoom(DungeonRoom room, mapTerrain creator) {
        base.cleanUpRoom(room, creator);
        if (!spawnedSword) {
            narrator.setBroom(true);
        }
    }
}
