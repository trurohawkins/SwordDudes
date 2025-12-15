using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRoom : DungeonRoom {
    public Vector2Int enemyHealth;

    public override void shapeRoom(int dir) {
        Vector2Int center = new Vector2Int(Map.S.worldSizeX / 2, Map.S.worldSizeY / 2);
        creator.clearCircle(center, Map.S.worldSizeX - doorSpace*2);
    }

    public override bool setUpRoom(int dir) {
        if (base.setUpRoom(dir)) {
            challengeType = 1;
            int difficulty = 1;//dm.getSkillPower(1);
            int numFoes = difficulty + 1;
            Vector2Int pos = randomRoom();//new Vector2Int(Random.Range(0, Map.S.worldSizeX), Random.Range(0, Map.S.worldSizeY));
            Debug.Log(pos);
            /*
            List<DungeonObject> spawned = spawnAround(guy, pos, numFoes, 5, 10);
            for (int i = 0; i < spawned.Count; i++) {
                Enemy e = spawned[i].GetComponent<Enemy>();
                e.name = "Enemy " + i;
                e.setStats(dm.getProgress(1));
            }
            */
        }
        return true;
    }
}
