using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class walkFloor : LargeMass {

    public GameObject done;
    int fullBody;
    public float percentPass;
    LargeMass floor;
    int allOn = -1;

    public override void callAction(Form enemy, int state, int x, int y) {
        if (enemy.id == 1 || enemy.id == 2) {
            turnOn(x, y);
            if (allOn == -1 && body.Count > 0) {
                if (body.Count < fullBody * percentPass) {
                    allOn = body.Count;
                    
                }
            }
        }
    }

    public override IEnumerator callAction(int delay) {
        for (int i = 0; i < allOn; i++) {
            if (body.Count == 0) {
                allOn = -1;
                master.targetMet(this);
            }
            if (speedCounter > speed) {
                int cur = Random.Range(0, body.Count);
                if (cur < body.Count) {
                    turnOn(body[cur].x, body[cur].y);
                    speedCounter = 0;
                }
            } else {
                speedCounter++;
            }
		    yield return new WaitForFixedUpdate ();
        }
    }

    void turnOn(int x, int y) {
        /*
        Map.S.world[x,y].formLeave(self);
        Vector2Int pos = new Vector2Int(x, y);
        body.Remove(pos);
        */
        Vector2Int pos = new Vector2Int(x, y);
        removeBody(pos);
        floor.spawnBody(pos, false);
    }

    public override void receiveMaster(DungeonRoom n_master) {
        base.receiveMaster(n_master);
        floor = master.logObj(done).GetComponent<LargeMass>();
    }

    public override bool spawnIn() {
        //floor.spawnIn();
        bool goodSpwn = base.spawnIn();
        fullBody = body.Count;
        Map.S.spawnForm(gameObject, 0, 0);
        for(int i = 0; i < body.Count; i++) {
            Cell c = Map.S.world[body[i].x, body[i].y];
            if (!c.within.Contains(self)) {
                Debug.Log("I didnt make it into " + c.name);
            }
        }
        return goodSpwn;
    }

    public override void cleanUp() {
        base.cleanUp();
        self.removeForm();
    }
}
