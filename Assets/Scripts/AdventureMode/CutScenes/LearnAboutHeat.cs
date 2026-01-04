using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearnAboutHeat : Shot {

    Vector2Int center;
    int curDir;

    public override void shapeRoom(DungeonRoom room, mapTerrain creator, int dir) {
        stories = new List<StoryPoint>();
        for (int i = 0; i < 4; i++) {
            room.doors[i] = true;
        }
        center = new Vector2Int(Map.S.worldSizeX/2, Map.S.worldSizeY/2);
        IgniteDialogue igDia = spawnStoryPoint(storyPoint[0], center.x, center.y).GetComponent<IgniteDialogue>();
        creator.clearCircle(center, 45);
        room.setHallSize(15, 20);
        /*
        Vector2Int a;
        Vector2Int b;
        if (dir % 2 == 1) {
            a = new Vector2Int(center.x, 65);
            b = new Vector2Int(center.x, 25);
        } else {
            a = new Vector2Int(25, center.y);
            b = new Vector2Int(65, center.y);
        }
        */
        Vector2Int a= new Vector2Int(center.x, 66);
        Vector2Int b = new Vector2Int(center.x, 24);
        Vector2Int c = new Vector2Int(24, center.y);
        Vector2Int d = new Vector2Int(66, center.y);
        Debug.Log(a + " " + b);
        room.connectRoom(a, b);
        room.connectRoom(c, d);
        int heat = 10;
        Obstacle ob = room.dm.spawnTarget(2, -1, center + new Vector2Int(0, -6), room).GetComponent<Obstacle>();
        ob.setHeat(heat);
        ob.partOfWave = true;
        ob = room.dm.spawnTarget(2, -1, center + new Vector2Int(0, 6), room).GetComponent<Obstacle>();
        ob.setHeat(heat);
        igDia.check = ob;
        ob.partOfWave = true;
        DungeonObject dunA = room.dm.makeEnemy(a.x, a.y, room);
        DungeonObject dunB = room.dm.makeEnemy(b.x, b.y, room);

        Enemy monA = dunA.GetComponent<Enemy>();
        monA.setStats(0.3f);
        monA.moveAndShoot = true;
        Enemy monB = dunB.GetComponent<Enemy>();
        monB.setStats(0.4f);

        room.addToWave(dunA);
        room.addToWave(dunB);
        curDir = dir;
        //room.addToWave(room.dm.spawnHeal(center + new Vector2Int(0, 12), room));
    }

    public override void roomBeat(DungeonRoom room) {
        base.roomBeat(room);
        Vector2Int offset = new Vector2Int(0, 10);// curDir % 2 == 1? new Vector2Int(0, 10) : new Vector2Int(10, 0);
        room.dm.spawnHeal(center + offset, room).spawnIn();
    }
}
