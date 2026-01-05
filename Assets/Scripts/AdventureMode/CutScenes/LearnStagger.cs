using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearnStagger : Shot {

    public override void shapeRoom(DungeonRoom room, mapTerrain creator, int dir) {
        for (int i = 0; i < 4; i++) {
            room.doors[i] = true;
        }
        Vector2Int center = new Vector2Int(Map.S.worldSizeX/2, Map.S.worldSizeY/2);
        creator.clearCircle(center, 45);
    }
}
