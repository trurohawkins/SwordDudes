using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class beatTarget : DungeonObject {

    int timer = 220;
    int ogTime;
    int condition = 0;
    public int type = 0;

    public override IEnumerator callAction(int delay) {
        if (!master.beat) {
            if (timer > 0) {
                if (timer - 1 > 0) {
                    timer--;
                } else {
                    if (!master.beat) {
                        self.height = 0;
                        self.die();
                        master.removeDenizen(this);
                        TargetRoom tr = master.GetComponent<TargetRoom>();
                        if (type == 0) {
                            tr.spawnTarget(self.centerPoint.x, self.centerPoint.y, condition, ogTime);
                        } else {
                            tr.spawnSwitch(self.centerPoint.x, self.centerPoint.y, ogTime, false);
                        }
                    }
                }
            }
        }
        yield return new WaitForEndOfFrame();
       // return base.callAction(delay);
    }

    public void setConditions(int c, int t) {
        condition = c;
        ogTime = timer = t;
    }
}
