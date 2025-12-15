using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpRoom : DungeonRoom {

    /*
    public override void setUpDoors(int dir) {
        for (int i = 0; i < numNeigh; i++) {
            if (i == dir) {//(dir + 2) % 4) {
                doors[i] = true;
            } else {
                doors[i] = false;
                DungeonRoom neigh = dm.checkRoom(this, i);
                if (neigh) {
                    dm.disconnectRoom(this, neigh, i);
                }
            }
        }
    }
    */
    public override void shapeRoom(int dir) {
        if (dir != -1) {
            int wx = Map.S.worldSizeX;
            int wy = Map.S.worldSizeY;
            Vector2Int center = new Vector2Int(Map.S.worldSizeX/2, Map.S.worldSizeY/2);
            creator.clearCircle(center, 40);
            Vector2Int d = dm.getDir((dir+numNeigh/2)%numNeigh);
            int hi = 4;
            int lo = 2;
            if (dir % 2 == 0) {
                creator.clearRect(center + (d * (wx/4)), wx/hi, wy/lo);
            } else {
                creator.clearRect(center + (d * (wx/4)), wx/lo, wy/hi);
            }
            wallsAndDoors();
        } else {
            Vector2Int center = new Vector2Int(Map.S.worldSizeX / 2, Map.S.worldSizeY / 2);
            creator.clearCircle(center, Map.S.worldSizeX - doorSpace*2);
        }
    }

    public override bool setUpRoom(int dir) {
        if (base.setUpRoom(dir)) {
            dm.spawnSword(Random.Range(0,10), new Vector2Int(Map.S.worldSizeX/2, Map.S.worldSizeY/2), this);
        }
        return true;
    }
}
