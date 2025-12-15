using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fightBroom : Shot {
	
    public override void shapeRoom(DungeonRoom room, mapTerrain creator, int dir) {
        base.shapeRoom(room, creator, dir);
        //Debug.Log(dir);
        //room.doors[dir] = false;
        for (int i = 0; i < 4; i++) {
            //if (i != (dir + 2) % 4) {
                room.doors[i] = false;
            //}
        }
        DungeonObject dun = room.dm.spawnBoss(10, 1, room);
        defense def = dun.GetComponent<defense>();
        if (def) {
            //def.setMaxHealth(50);
        }
        room.closeDoor = true;
        //speaker = 2;
    }

    public override void roomBeat(DungeonRoom room) {
        base.roomBeat(room);
        //room.beatRoom(false);
        //room.dm.progress++;
        room.dm.noRoomClear = false;

        //room.dm.newSpawnRoom(room);
    }
}
