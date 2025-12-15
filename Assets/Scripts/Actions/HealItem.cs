using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealItem : Purpose {

    public float healAmount;
    DungeonObject dun;

     protected override void Awake() {
        base.Awake();
        dun = gameObject.GetComponent<DungeonObject>();
    }

    public override void callAction(Form poo, int state, int x, int y) {
        defense def = null;
        if (poo.id == 2) {
            Debug.Log("hit by sword");
            attackChunk ac = poo.GetComponent<attackChunk>();
			if (ac) {
				circleAttack ca = ac.getAttack();
				if (ca) {
                    def = ca.getPlayer().GetComponent<defense>();
                }
            }
        } else {
            def = poo.GetComponent<defense>();
        }
        if (def && !def.checkMaxHealth()) {
            def.heal(healAmount, true);
            self.die();
            DungeonRoom room = dun.master;
            room.removeDenizen(dun);
        }
    }
}
