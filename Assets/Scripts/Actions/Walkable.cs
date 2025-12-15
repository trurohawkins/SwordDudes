using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walkable : DungeonObject {

    public int resetTime = -1;
    bool stoodOn = false;

    public override void callAction(Form poo, int state, int x, int y) {
        //base.callAction(poo, state, x, y);
        if (poo.id == 1) {
            if (state == 1 && !stoodOn) {
                stoodOn = true;

                dying(false);
                if (curCorpse) {
                    beatTarget bt = curCorpse.GetComponent<beatTarget>();
                    bt.setConditions(0, resetTime);
                    bt.receiveMaster(master);
                    bt.type = 1;
                }
                if (master) { // when we spawn under a player this is fucked up
                    master.targetMet(this);
                }
            }
        }
    }
}
