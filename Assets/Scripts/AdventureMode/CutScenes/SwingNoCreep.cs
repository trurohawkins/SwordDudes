using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingNoCreep : Shot {

    public override void shapeRoom(DungeonRoom room, mapTerrain creator, int dir) {
        Debug.Log("swing no creep " + dir);
        stories = new List<StoryPoint>();
        Vector2Int mid = new Vector2Int(Map.S.worldSizeX/2, Map.S.worldSizeY/2);

        creator. clearCircle(mid, 50);
        creator.clearRect(mid, 73, 19);
        CreepDebris mess = null;
        if (dir == 1) {
            room.doors[0] = true;
            creator.clearRect(new Vector2Int(mid.x, (int)(Map.S.worldSizeY * 0.75f)), 19, 27);
            creator.fillRect(new Vector2Int(mid.x, 20), 60, 32);

            DungeonObject d = room.dm.spawnSword(10, new Vector2Int(15, mid.y), room).GetComponent<DungeonObject>();
            spawnStoryPoint(storyPoint[0], 15, mid.y);
            //room.dm.spawnDebris(new Vector2Int(mid.x, (int)(Map.S.worldSizeY * 0.7f)), 30, 30, room);
            mess = room.dm.spawnDebrisCircle(new Vector2Int((int)(Map.S.worldSizeX * 0.4), (int)(Map.S.worldSizeY * 0.55f)), 30, room);
            room.dm.spawnDebrisCircle(new Vector2Int(mid.x, (int)(Map.S.worldSizeY * 0.7f)), 40, room);
            room.dm.spawnDebrisCircle(new Vector2Int((int)(Map.S.worldSizeX * 0.3), (int)(Map.S.worldSizeY * 0.6f)), 20, room);
            room.dm.spawnLitterEnemy(mid.x, (int)(Map.S.worldSizeY * 0.75f), room);
        } else {
            room.doors[2] = true;
            creator.clearRect(new Vector2Int(mid.x, (int)(Map.S.worldSizeY * 0.25f)), 19, 27);
            creator.fillRect(new Vector2Int(mid.x, Map.S.worldSizeY - 20), 60, 32);
            
            DungeonObject d = room.dm.spawnSword(10, new Vector2Int(75, mid.y), room).GetComponent<DungeonObject>();
            spawnStoryPoint(storyPoint[0], 75, mid.y);
            mess = room.dm.spawnDebrisCircle(new Vector2Int((int)(Map.S.worldSizeX * 0.4), (int)(Map.S.worldSizeY * 0.45f)), 30, room);
            room.dm.spawnDebrisCircle(new Vector2Int(mid.x, (int)(Map.S.worldSizeY * 0.3f)), 40, room);
            room.dm.spawnDebrisCircle(new Vector2Int((int)(Map.S.worldSizeX * 0.3), (int)(Map.S.worldSizeY * 0.4f)), 20, room);
        }
        mess.isTarget = true;
        room.setCleanPercent(0.65f);
        room.setUpRoom(-1);
    }
}
