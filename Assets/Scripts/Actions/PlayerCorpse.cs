using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCorpse : DungeonObject {
    FighterAnimator anim;
    PlayerAnimator pa;
    int num;
    SwordSoul soul;

     protected override void Awake() {
        base.Awake();
        anim = gameObject.GetComponentInChildren<FighterAnimator>();
        if (anim) {
            self.skin = anim.gameObject;
        }
        pa = gameObject.GetComponentInChildren<PlayerAnimator>();
    }

    public override bool spawnIn() {
        if (anim) {
            if (deathState != 2) {
                anim.setDeath(1);
            } else {
                anim.setDeath(2);
            }
            anim.setDir(dirState);
            anim.setAlpha(1);
        }

        return base.spawnIn();
    }

    public override void cleanUp() {
        base.cleanUp();
        if (anim) {
            // bug with turning off aniamtors in unity, changes the values
            anim.setDeath(0);
            anim.setAlpha(0);
        } else {
            Debug.Log("no anim " + name);
        }
    }

    int deathState = 0;
    int dirState = 0;

    public void getInfo(SwordSoul s, int pNum, int dir, bool burn) {
        anim = gameObject.GetComponentInChildren<FighterAnimator>();
        /*
        anim.setEyes (2);
	    anim.setDir (4);
        */
        num = pNum;
        if (s) {
            soul = s;
            anim.setColors(soul, GameInfo.S.colors[pNum]);
        }
        if (burn) {
            deathState = 2;
            anim.setDeath(2);
        } else {
            anim.setDeath(1);
        }
        dirState = dir;
        anim.setDir(dir);
        if (GM.S.curDM) {
            GM.S.curDM.getCurRoom().addObj(this);
        }
    }
}
