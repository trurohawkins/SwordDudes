using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashAttack : AreaAttack {

    public DungeonMaster master;
    public DungeonRoom room;

    protected override void endAttack() {
        Vector2Int pos = self.centerPoint;
        int wid = self.width + 3;
        int len = self.length + 3;
        
        base.endAttack();
        master.spawnDebrisRing(pos, new Vector2Int(wid, len), room);
    }
}
