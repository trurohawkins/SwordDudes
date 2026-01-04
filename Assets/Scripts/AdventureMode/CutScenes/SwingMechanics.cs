using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingMechanics : Shot {

    public override void shapeRoom(DungeonRoom room, mapTerrain creator, int dir) {
        room.doors[(dir+1)%4] = true;
        room.doors[(dir+3)%4] = true;
        Vector2Int center = new Vector2Int(Map.S.worldSizeX/2, Map.S.worldSizeY/2);
        creator.clearCircle(center, 40);
        List<Vector2Int> corners = new List<Vector2Int>();
        corners.Add(new Vector2Int((int)(Map.S.worldSizeX * 0.1f), (int)(Map.S.worldSizeY * 0.9f)));
        corners.Add(new Vector2Int((int)(Map.S.worldSizeX * 0.1f), (int)(Map.S.worldSizeY * 0.1f)));
        corners.Add(new Vector2Int((int)(Map.S.worldSizeX * 0.9f), (int)(Map.S.worldSizeY * 0.1f)));
        corners.Add(new Vector2Int((int)(Map.S.worldSizeX * 0.9f), (int)(Map.S.worldSizeY * 0.9f)));

        room.setHallSize(10, 15);
        room.connectRoom(corners[0], corners[2]);
        room.connectRoom(corners[3], corners[1]);

        int timer = 25;
        DungeonObject dun = room.dm.spawnTarget(0, timer, new Vector2Int(center.x + 5, center.y + 5), room);
        dun.partOfWave = true;
        dun = room.dm.spawnTarget(0, timer, new Vector2Int(center.x - 4, center.y - 4), room);
        dun.partOfWave = true;
       
        //room.newWave();
        
        for (int i = 0; i < corners.Count; i++) {
            dun = room.dm.spawnKnocker(corners[i].x, corners[i].y, room);
            room.addToWave(dun);
            Enemy mon = dun.GetComponent<Enemy>();
            mon.setStats(0.2f);
            mon.setAttackLengthRange(0.75f);
            mon.setKnockback(0.1f);
        }
        
        room.getPrize = true;
    }

    public override void roomBeat(DungeonRoom room) {
        base.roomBeat(room);
        //room.dm.progress
    }
}
