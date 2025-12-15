using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swinging101 : Shot {

    public override void shapeRoom(DungeonRoom room, mapTerrain creator, int dir) {
        doorSpace = 3;
        Debug.Log("door space: " + doorSpace);
        room.doors[(dir+3)%4] = true;

        Vector2Int mid = new Vector2Int(Map.S.worldSizeX/2, Map.S.worldSizeY/2);
        if (dir == 1) {
            //room.doors[0] = true;
        } else {
            //room.doors[2] = true;    
        }

        Vector2Int entrance = mid + (GM.S.dirs[((dir+2)%4)*2] * (Map.S.worldSizeX / 3));
        Vector2Int exit = mid + (GM.S.dirs[((dir+3)%4)*2] * (Map.S.worldSizeX / 3));
        Vector2Int elbow = new Vector2Int(entrance.x, exit.y);
        room.pathRooms(entrance, elbow);
        room.pathRooms(exit, elbow);

        Vector2Int d = GM.S.dirs[dir*2];
        d *= Map.S.worldSizeX / 3;
        Vector2Int across = mid + d;
        room.pathRooms(entrance, across);
        //

        d = GM.S.dirs[((dir+1)%4) * 2] * (Map.S.worldSizeX / 3);
        Vector2Int perp = mid + d;//, 25);
        room.pathRooms(entrance, perp);
        int size = Map.S.worldSizeX / 3;
        creator.clearCircle(across, size);
        creator.clearCircle(perp, size);

        Vector2Int entry = GM.S.dirs[dir*2] * (Map.S.worldSizeX/8);
        Debug.Log(entry);
        Vector2Int anteChamber = entrance + entry;//new Vector2Int(entrance.x + 15, entrance.y);
        creator.clearCircle(anteChamber, size + 10);
        room.dm.spawnLitterEnemy(anteChamber.x, entrance.y, room);
        room.dm.spawnLitterEnemy(anteChamber.x, entrance.y, room);
        room.dm.spawnLitterEnemy(anteChamber.x, entrance.y, room);
        room.dm.spawnDebrisCircle(anteChamber, 15, room);

        DungeonObject dun = room.dm.spawnTarget(0, -1, across, room);
        dun.partOfWave = true;
        dun = room.dm.spawnTarget(0, -1, perp, room);
        dun.partOfWave = true;
        float swingLevel = Grader.S.getSkillPercent(2, 0);
        
        dun = room.dm.makeEnemy(anteChamber.x, anteChamber.y, room);
        Enemy baddy = dun.GetComponent<Enemy>();
        baddy.setStats(swingLevel);
        room.addToWave(dun);

        room.newWave();
        dun = room.dm.makeEnemy(across.x, across.y, room);
        baddy = dun.GetComponent<Enemy>();
        baddy.setStats(swingLevel/2);
        room.addToWave(dun);
        dun = room.dm.makeEnemy(perp.x, perp.y, room);
        baddy = dun.GetComponent<Enemy>();
        baddy.setStats(swingLevel/2);
        room.addToWave(dun);
        //creator. clearCircle(mid, 50);
    }
}
