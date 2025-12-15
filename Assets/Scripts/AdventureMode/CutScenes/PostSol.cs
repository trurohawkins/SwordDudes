using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostSol : Shot {
    public override void shapeRoom(DungeonRoom room, mapTerrain creator, int dir) {
        base.shapeRoom(room, creator, dir);
        room.beatRoom(false);
        //room.dm.progress++;
        room.dm.newSpawnRoom(room);
    }
}
