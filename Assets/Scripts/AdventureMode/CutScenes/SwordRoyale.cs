using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordRoyale : Shot {
        public override void shapeRoom(DungeonRoom room, mapTerrain creator, int dir) {
        base.shapeRoom(room, creator, dir);
        for (int i = 0; i < 4; i++) {
            //if (i != (dir + 2) % 4) {
                room.doors[i] = true;
            //}
        }
        room.dm.spawnSwordEnemy(60, 45, room);

        DungeonObject dun = room.dm.spawnSwordEnemy(30, 45, room);
        room.addToWave(dun);
    }
}
