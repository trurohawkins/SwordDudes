using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WakeUp : Shot {

    public override void shapeRoom(DungeonRoom room, mapTerrain creator, int dir) {
        int doorSpace = 5;
        room.setDoorSpace(doorSpace);
        Vector2Int center = new Vector2Int(Map.S.worldSizeX / 2, Map.S.worldSizeY / 2);
        creator.clearCircle(center, Map.S.worldSizeX - doorSpace*8);
        creator.clearRect(center, Map.S.worldSizeX - (doorSpace*2), 20);

        DungeonObject dun = room.logObj(storyPoint[0]);

        //dun.self.squareBody();
        //dun.pos = new Vector2Int(sp.x, sp.y);// new Vector2Int(50, 45);
        StoryPoint stry = dun.GetComponent<StoryPoint>();
        stry.getDialogue(narrator.dia);
        TargetRoom tr = room.GetComponent<TargetRoom>();
        spawnStoryPoint(storyPoint[1], 15, center.y);

        tr.dm.spawnWalk(new Vector2Int(45, 45), 40, 40, tr);
        //room.newWave();
        dun = room.dm.spawnLitterEnemy((int)(Map.S.worldSizeX * 0.75f), center.y,  room);
        room.addToWave(dun);
        dun = room.dm.spawnLitterEnemy(center.x, (int)(Map.S.worldSizeY * 0.25f),  room);
        room.addToWave(dun);
        dun = room.dm.spawnLitterEnemy((int)(Map.S.worldSizeX * 0.25f), center.y,  room);
        room.addToWave(dun);
        DungeonObject sword = room.dm.setUpSword(10, new Vector2Int(center.x, center.y + 25), room);
        room.addToWave(sword);//.GetComponent<DungeonObject>());

        //dun.spawnIn();

        room.doors[0] = false;
        room.doors[1] = true;
        room.doors[2] = false;
        room.doors[3] = true;

        room.setUpRoom(-1);
    }
}
