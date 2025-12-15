using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotAnim : FighterAnimator {
    string faceAct;
    SwordSoul soul;

    public override void Awake() {
        base.Awake();
        curAction = faceAct = actions[0];
    }

    public override void setState() {
        //Debug.Log("setting state");
        for (int i = 0; i < names.Length; i++) {
            if (i == 0) {
                //Debug.Log("pot" + names[i] + curAction + dir);
                anims[i].Play("pot" + names[i] + curAction + dir);
            } else {
                //Debug.Log("pot" + names[i] + faceAct + dir);
                anims[i].Play("pot" + names[i] + faceAct + dir);
            }
        }
    }

    public override void setColors(SwordSoul mySoul, int colorNum) {
        cNum = colorNum;//GameInfo.S.colors[pNum];
        sr[0].color = sr[2].color = mySoul.mainColor[cNum];
        sr[1].color = mySoul.subColor[cNum];
        soul = mySoul;
    }

    public override void lerpHeat(float fade) {
        if (soul) {
            if (sr[1].color.a != 0) {
                sr[1].color = Color.Lerp(soul.subColor[cNum], soul.heatColorA, fade);
            }
        } else {
            Debug.LogError("no soul on " + name);
        }
    }

    public override void respawn() {
        setDeath(0);
        lerpHeat(0);
    }

    public override void swingBegin() {
        faceAct = actions[3];
        setState();
    }

    public override void swingOver(bool gettingInput, bool swingDetached) {
        //if (!gettingInput) {
        if (faceAct != actions[0]) {
            faceAct = actions[0];
            setState();
        }
        //}
    }

    public override void cantSwing(bool begin) {
        if (noSwing != begin && !staggered) {
            if (begin) {
                if (dodgeState != 2) {
                    faceAct = actions[4];
                }
            } else {
                if (dodgeState == 0) {
                    faceAct = actions[0];
                } else {
                    faceAct = actions[2];
                }
            }
            setState();
            noSwing = begin;
        }
    }

    public override void hurt(bool begin) {
        if (hurting != begin) {
            if (begin) {
                if (!staggered) {
                    faceAct = actions[5];
                }
            } else {
                if (!staggered) {
                    faceAct = actions[0];
                }
            }
            setState();
            hurting = begin;
        }
    }

    public override void stagger(bool begin) {
        if (staggered != begin) {
            if (begin) {
                faceAct = actions[6];
                if (walking) {
                    setWalking(false);
                }
            } else {
                faceAct = actions[0];
            }
            staggered = begin;
            setState();
        }
    }

    public override void knock(bool begin) {
        if (knocked != begin) {
            if (begin) {
                curAction = actions[7];
            } else {
                curAction = actions[0];
            }
            knocked = begin;
            setState();
        }
    }

    public override void setDeath(int dead) {
        if (deathState != dead) {
            if (dead == 1) {
                curAction = faceAct = actions[8];
                setAlphaPart(1, 0);
            } else if (dead == 2) {
                curAction = faceAct = actions[9];
                lerpHeat(1);
            } else {
                curAction = faceAct = actions[0];
                setAlphaPart(1, 1);
            }
            deathState = dead;
            setState();
        }
    }

    public override void setWalking(bool on) {
        if (walking != on) {
            if (!on || (dodgeState == 0 || deathState == 0)) {
                if (on) {
                    curAction = actions[1];
                } else {
                    curAction = actions[0];
                }
                walking = on;
                setState();
            }
        }
    }

    public override void setWalkingSpeed(float n_speed) {
		anims[0].speed = Mathf.Clamp(n_speed, 0.2f, 1);
	}

    public override void setDodge(int state) {
        if (dodgeState != state) {
            if (soul) {
                if (state == 2) {
                    sr[0].color = dodgeColor;//soul.powerColor[cNum];
                    sr[1].color = dodgeColor;//soul.powerColor[cNum];
                } else {
                    sr[0].color = soul.mainColor[cNum];
                    sr[1].color = soul.subColor[cNum];
                }
            }
            if (state == 0) {
                walking = !walking;
                setWalking(!walking);
                faceAct = actions[0];
            } else {
                curAction = actions[2];
                faceAct = actions[2];
            }
            setState();
            dodgeState = state;
        }
    }

    public override void setAlpha(float val) {
		for (int i = 0; i < sr.Length; i++) {
            if (i == 1 && deathState == 1) {
                continue;
            }
            setAlphaPart(i, val);
		}
	}

    public void setAlphaPart(int i, float val) {
		Color c = sr[i].color;
		c.a = val;
		sr[i].color = c;
    }
}
