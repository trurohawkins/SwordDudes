using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBattle : Shot {

    public override void shapeRoom(DungeonRoom room, mapTerrain creator, int dir) {
        int xPos = dir == 1 ? 30 : 60;
        for (int i = 0; i < 4; i++) {
            room.doors[i] = true;
        }
        Vector2Int center = new Vector2Int(46, 46);
        creator.clearRect(center, 45, 45);

        Enemy baddy = makeEnemy(xPos, 30, room);
        if (baddy) {
            baddy.setStats(0.3f);
            baddy.longRange(0.2f);
            baddy.moveAndShoot = true;
            baddy.setSize(3);
            baddy.setWeight(20);
            baddy.setStaggerDef(15, 25);
            baddy.setKnockback(0.3f);
            baddy.attackDieOnImpact = false;
            baddy.senseRange = 25;
        }
        Enemy boogie = makeEnemy(xPos, 50, room);
        if (boogie) {
            boogie.setStats(0.3f);
            boogie.setWeight(2);
            boogie.senseRange = 25;
            
        }

        room.wallsAndDoors();
        room.setUp = true;
    }
}
