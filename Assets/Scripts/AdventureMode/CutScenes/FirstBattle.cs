using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBattle : Shot {
    List<DungeonObject> wave2;
    
    public override void shapeRoom(DungeonRoom room, mapTerrain creator, int dir) {
        base.shapeRoom(room, creator, dir);
        for (int i = 0; i < 4; i++) {
            if (i == dir || i == (dir+2) % 4) {
                room.doors[i] = true;
            } else {
                room.doors[i] = false;
            }
        }
        Vector2Int center = new Vector2Int(Map.S.worldSizeX/2, Map.S.worldSizeY/2);
        creator.fillCircle(center, 48);
        creator.clearCircle(center, 30);
        //room.dm.spawnDebris(center, 25, room).spawnIn();
        spawnEnemy(center.x - 3, center.y - 3, 5).spawnIn();
        spawnEnemy(center.x + 3, center.y - 3, 5).spawnIn();
        spawnEnemy(center.x, center.y + 3, 5).spawnIn();
        wave2 = new List<DungeonObject>();
        for (int i = 0; i < 4; i++) {
            DungeonObject dun = room.dm.makeEnemy(creator.spawns[i].x, room.creator.spawns[i].y, room);
            room.addToWave(dun);
            defense e = dun.GetComponent<defense>();
            e.setMaxHealth(10);
        }
        if (!narrator.getBroom()) {
            //creator.clearRect((int)(Map.S.worldSizeX * 0.666f), (int)(Map.S.worldSizeY * 0.2f), 20, 10);
            room.dm.spawnSword(10, new Vector2Int((int)(Map.S.worldSizeX * 0.666f) + 5, (int)(Map.S.worldSizeY * 0.2f)), room);
        }
    }

    void FixedUpdate() {
        /*
        if (wave2.Count > 0 && myRoom.getTargetCount() < 5) {
            for (int i = 0; i < wave2.Count; i++) {
                wave2[i].spawnIn();
            }
            wave2.Clear();
        }
        */
    }

    DungeonObject spawnEnemy(int cx, int cy, float health) {
        DungeonObject dun = myRoom.dm.makeEnemy(cx, cy, myRoom);
        if (dun) {
            defense e = dun.GetComponent<defense>();
            e.setMaxHealth(health);
        }
        return dun;
    }

}
