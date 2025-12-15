using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LitterBug : Enemy {

    public override void doAttack() {
        if (!attacking) {
            if ((attackNoTarget || target) && (pather.curDest.x < 0 || moveAndShoot)) {// && pather.dist2Player <= attackRange) {
                if (attackTimer >= attackInterval) {
                    anim.attack(1);
                    attacking = true;
                    aPos = self.centerPoint;
                    if (waitWhileAttack) {
                        anim.setWalking(false);
                    }
                    attackTimer = 0;
                } else {
                    attackTimer++;
                }
            }
        } else {
            if (attackTimer == (int)(attackLength * attackPoint)) {
                anim.attack(1);
                GameObject a = Instantiate(attack, transform.parent);//.GetComponent<Form>();
                Form att = a.GetComponent<Form>();
                att.squareBody();
                att.parent = self;
                Map.S.spawnForm(a, aPos.x, aPos.y);                    
                if (!att.spawned) {
                    Debug.Log("spawn didnt work");
                    att.die();
                } else {
                    att.etheral = false;
                    TrashAttack ta = att.GetComponent<TrashAttack>();
                    ta.room = gameObject.GetComponent<DungeonObject>().master;
                    ta.master = ta.room.dm;
                    Value v = att.GetComponent<Value>();
                    v.setTeam(6);
                    curAttack = att.GetComponent<Attack>();
                    a.name = "LitterBlast " + Time.time;
                }
            }
            if (attackTimer >= attackLength) {
                attackTimer = 0;
                attacking = false;
                anim.attack(0);
            } else {
                attackTimer++;
            }
        }
    }
}
