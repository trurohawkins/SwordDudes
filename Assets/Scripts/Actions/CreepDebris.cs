using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepDebris : LargeMass {

    public float damage;
    public float cleanPercent = 0.25f;

    public override bool spawnIn() {
        bool goodSpawn = base.spawnIn();
        master.addDebris(body.Count);
        return goodSpawn;
    }

    public override void callAction(Form enemy, int state, int x, int y) {
        if (state == 1) {
            if (enemy.id == 1) {
                Player p = enemy.GetComponent<Player>();
                if (p) {
                    if (p.isCreeping()) {
                        //Debug.Log("we got a creeper");
                    } else {
                        defense d = p.GetComponent<defense>();
                        if (d) {
                            d.takeDamage(damage, false);
                        }
                    }
                }
            } else if (enemy.id == 2) {
                attackChunk ac = enemy.GetComponent<attackChunk>();
				if (ac) {
					circleAttack ca = ac.getAttack();
                    //if (ca.swinging) {
                        if (Grader.S) {
                            //Value v = ac.GetComponent<Value>();
                            Grader.S.makeHit(ca.getBody());//, v.getValue("damage"), v.getValue("knockBack"));
                        }
                        removeBody(x, y);
                        master.cleanDebris();
                    //}
                }
            }
        }
        //base.callAction(enemy, state, x, y);
    }

    public override void cleanUp() {
        base.cleanUp();
        master.resetDebris();
    }
}
