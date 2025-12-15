using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GloopMon : Enemy {

    public override void getTarget() {
        base.getTarget();
        for (int i = 0; i < 3; i++) {
            pather.setRange(i, 1 + i, 3 + i);
        }
    }

    void dirForAttack(int dir) {
        self.changeDirection(dir);
        anim.setDir(dir);
    }

    public override void doAttack() {
        if (!attacking) {
            float dist = 0;
            if (target) {
                dist = Vector2Int.Distance(self.centerPoint, target.centerPoint);
                //Debug.Log(dist + " < " + attackRange);
            }
            if ((attackNoTarget || target) && (pather.curDest.x < 0 || moveAndShoot)) {// && dist <= attackRange) {
                if (attackTimer >= attackInterval) {
                    attacking = true;
                    if (anim) {
                        anim.attack(1);
                        Vector2 dir = target.centerPoint - self.centerPoint;
                        dir.Normalize();
                        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y)) {
                            if (dir.x > 0) {
                                dirForAttack(6);
                            } else {
                                dirForAttack(2);
                            }
                        } else {
                            if (dir.y > 0) {
                                dirForAttack(0);
                            } else {
                                dirForAttack(4);
                            }
                        }
                        //anim.setDir(GM.S.convertVectorToDir(dir));
                        if (waitWhileAttack) {
                            anim.setWalking(false);
                        }
                    }
                    attackTimer = 0;
                } else {
                    attackTimer++;
                }
            }
        } else {
            if (attackTimer == (int)(attackLength * attackPoint)) {
                GameObject a = Instantiate(attack, transform.parent);//.GetComponent<Form>();
                Form att = a.GetComponent<Form>();
                Vector2Int adjust = GM.S.dirs[self.direction] * 3;
                if (self.direction == 0 || self.direction == 4) {
                    att.width = 5;
                    att.length = 5;
                    adjust.x += 1;
                } else if (self.direction == 2 || self.direction == 6) {
                    att.length = 5;
                    att.width = 6;
                    adjust.y -= 1;
                }
                if (self.direction == 4) {
                    adjust.y -= 2;
                }
                if (self.direction == 6) {
                    adjust.x += 3;
                }
                att.squareBody();
                att.parent = self;
                aPos = self.centerPoint + adjust;
                Map.S.spawnForm(a, aPos.x, aPos.y);                    
                if (!att.spawned) {
                    att.die();
                } else {
                    att.etheral = false;
                    //Animator attAnim = att.GetComponentInChildren<Animator>();
//                    Debug.Log("attacking in " + self.direction);
                    /*
                    if (self.direction == 0 || self.direction == 4) {
                        attAnim.Play("GoopMonBlastF");
                    } else if (self.direction == 2 || self.direction == 6) {
                        attAnim.Play("GoopMonBlastS");
                    } 
                    */
                    Attack attack = a.GetComponent<Attack>();
                    attack.setDamages(damage, knockback, stagger);
                    attack.setTeam(team);
                    //curAttack = att.GetComponent<Attack>();
                    //curAttack.getCreator(this);
                }
            }
            if (attackTimer >= attackLength) {
                attackTimer = 0;
                attacking = false;
                if (anim) {
                    anim.attack(0);
                }
            } else {
                attackTimer++;
            }
        }
    }
}
