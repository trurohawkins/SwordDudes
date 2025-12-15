using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadeBlock : Purpose {
    public Shade spirit;
    
    public override void callAction(Form poo, int state, int x, int y) {
       if (state == 1) {
            attackChunk ac = poo.GetComponent<attackChunk>();
            bool teamHit = false;
            if (spirit.will.team != -1) {
                if (ac) {
                    if (ac.getAttack()) {
                        if (ac.getAttack().team == spirit.will.team) {
                            teamHit = true;
                        }
                    }
                }
                Player p = poo.GetComponent<Player>();
                if (p) {
                    if (p.team == spirit.will.team) {
                        teamHit = true;
                    }
                }
            }
            if (!teamHit) {
                //spirit.shadeBodyHit(poo, x, y);
                Map.S.world[x,y].formLeave(self);
            }
        }
    }

}
