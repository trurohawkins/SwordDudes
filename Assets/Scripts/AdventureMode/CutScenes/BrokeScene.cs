using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokeScene : Shot {

    public override void shapeRoom(DungeonRoom room, mapTerrain creator, int dir) {
        //base.shapeRoom(room, creator, dir);
        room.beatRoom(false);
        room.setUpRoom(-1);
    }
}
