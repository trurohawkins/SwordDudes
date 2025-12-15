using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BroomCloset : Shot {

    GameObject spawnedBroom;

    public override void shapeRoom(DungeonRoom room, mapTerrain creator, int dir) {
        Debug.Log(dir);
        room.doors[(dir + 2) % 4] = true;
        int doorSpace = 5;
        room.setDoorSpace(doorSpace);
        DungeonObject dun = room.logObj(storyPoint[0]);
        StoryPoint stry = dun.GetComponent<StoryPoint>();
        stry.getDialogue(narrator.dia);
        Vector2Int center;
        if (dir == 3) {
            center = new Vector2Int((int)(Map.S.worldSizeX * 0.25f), Map.S.worldSizeY / 2);
        } else {
            center = new Vector2Int((int)(Map.S.worldSizeX * 0.75f), Map.S.worldSizeY / 2);
        }
        creator.clearCircle(new Vector2Int((int)(Map.S.worldSizeX * 0.5f), Map.S.worldSizeY / 2), Map.S.worldSizeX / 3);
        creator.clearRect(center, (int)(Map.S.worldSizeX * 0.4f), (int)(Map.S.worldSizeY * 0.1f));

        spawnedBroom = room.dm.spawnSword(7, new Vector2Int(Map.S.worldSizeX/2, Map.S.worldSizeY/2), room);
        room.setUp = true;
    }

    public override void cleanUpRoom(DungeonRoom room, mapTerrain creator) {
        base.cleanUpRoom(room, creator);
        if (!spawnedBroom) {
            narrator.setBroom(true);
        }
    }

}
