using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearnStagger : Shot {

    public override void shapeRoom(DungeonRoom room, mapTerrain creator, int dir) {
        room.doors[1] = true;
        room.doors[0] = true;
        Vector2Int center = new Vector2Int(Map.S.worldSizeX/2, Map.S.worldSizeY/2);
        creator.clearCircle(center, 65);
        StaggerStoryPoint stgStory = spawnStoryPoint(storyPoint[0], center.x, center.y).GetComponent<StaggerStoryPoint>();

        TargetRoom tRoom = room.GetComponent<TargetRoom>();

        DungeonObject dun = tRoom.spawnMonster(new Vector2Int(center.x, center.y), 3);
        stgStory.mon = dun;
        Enemy mon = dun.GetComponent<Enemy>();
        
        mon.speed = mon.targSpeed = mon.chillSpeed = -1;
        for (int i = 1; i < 4; i++) {
            Vector2Int spawn = center + GM.S.dirs[i*2] * 25;
            dun = tRoom.spawnMonster(spawn, 3);
            mon = dun.GetComponent<Enemy>();
            mon.staggerEnemy(0.5f);
            tRoom.addToWave(dun);
        }
        
        
    }
}
