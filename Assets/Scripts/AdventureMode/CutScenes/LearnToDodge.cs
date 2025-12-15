using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearnToDodge : Shot {

    List<DungeonObject> enemies;

    public override void shapeRoom(DungeonRoom room, mapTerrain creator, int dir) {
        base.shapeRoom(room, creator, dir);
        creator.fillRect(new Vector2Int(Map.S.worldSizeX / 2, Map.S.worldSizeY / 3), Map.S.worldSizeX, 7);
        TargetRoom tr = room.GetComponent<TargetRoom>();
        tr.doors[0] = false;
        /*
        tr.spawnSwitch((int)(Map.S.worldSizeX * 0.35), (int)(Map.S.worldSizeY * 0.55f), -1, false);
        tr.spawnSwitch((int)(Map.S.worldSizeX * 0.65), (int)(Map.S.worldSizeY * 0.55f), -1, false);
        tr.spawnSwitch((int)(Map.S.worldSizeX * 0.5), (int)(Map.S.worldSizeY * 0.75f), -1, false);
        */
        Vector2Int targ = new Vector2Int ((int)(Map.S.worldSizeX * 0.5), (int)(Map.S.worldSizeY * 0.8));
        creator.fillCircle(targ, 35);
        creator.clearCircle(targ, 25);
        tr.spawnTarget(targ.x, targ.y, 1, -1);
        room.dm.spawnDebris(targ, 200, room);
        StoryPoint stry = room.setUpObj(storyPoint[0], new Vector2Int(45, 45), false).GetComponent<StoryPoint>();
        stry.getDialogue(narrator.dia);
        //enemies = new List<DungeonObject>();
        DungeonObject dun = room.dm.makeEnemy(creator.spawns[3].x, creator.spawns[3].y, room);
        if (dun) {
            //enemies.Add(dun);
            room.addToWave(dun);
        }
        DungeonObject dun2 = room.dm.makeEnemy(creator.spawns[1].x, creator.spawns[1].y, room);
        if (dun2) {
            room.addToWave(dun2);
        }
        if (!narrator.getBroom()) {
            room.dm.spawnSword(10, new Vector2Int((int)(Map.S.worldSizeX * 0.666f) + 5, (int)(Map.S.worldSizeY * 0.2f)), room);
        }
    }

    void FixedUpdate() {
        /*
        if (enemies.Count > 0 && myRoom.getTargetCount() < 2) {
            Debug.Log("fuck");
            for (int i = 0; i < enemies.Count; i++) {
                enemies[i].spawnIn();
            }
            enemies.Clear();
        }
        */
    }
}
