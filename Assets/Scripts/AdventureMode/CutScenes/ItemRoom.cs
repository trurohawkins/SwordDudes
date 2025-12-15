using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRoom : Shot {

    public override void shapeRoom(DungeonRoom room, mapTerrain creator, int dir) {
        room.doors[(dir + 2) % 4] = true;
        int doorSpace = 5;
        room.setDoorSpace(doorSpace);
        Vector2Int center = Vector2Int.zero;
        if (dir == 0) {
            center = new Vector2Int((int)(Map.S.worldSizeX * 0.5f), (int)(Map.S.worldSizeY * 0.25f));
            creator.clearRect(center, (int)(Map.S.worldSizeX * 0.1f), (int)(Map.S.worldSizeY * 0.4f));
        } else if (dir == 1) {
            center = new Vector2Int((int)(Map.S.worldSizeX * 0.75f), Map.S.worldSizeY / 2);
            creator.clearRect(center, (int)(Map.S.worldSizeX * 0.4f), (int)(Map.S.worldSizeY * 0.1f));
        } else if (dir == 2) {
            center = new Vector2Int((int)(Map.S.worldSizeX * 0.5f), (int)(Map.S.worldSizeY * 0.75f));
            creator.clearRect(center, (int)(Map.S.worldSizeX * 0.1f), (int)(Map.S.worldSizeY * 0.4f));
        } else if (dir == 3) {
            center = new Vector2Int((int)(Map.S.worldSizeX * 0.25f), Map.S.worldSizeY / 2);
            creator.clearRect(center, (int)(Map.S.worldSizeX * 0.4f), (int)(Map.S.worldSizeY * 0.1f));
        }
        creator.clearCircle(new Vector2Int((int)(Map.S.worldSizeX * 0.5f), Map.S.worldSizeY / 2), Map.S.worldSizeX / 3);
        

        room.dm.spawnHeal(new Vector2Int(Map.S.worldSizeX/2, Map.S.worldSizeY/2), room);
        room.setUp = true;
    }
}
