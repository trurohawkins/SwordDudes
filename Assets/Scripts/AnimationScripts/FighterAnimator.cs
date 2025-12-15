using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterAnimator : MonoBehaviour {

    public string character;
    protected string curAction;

    public bool walking;
    public bool noSwing;
    public int dodgeState = 0;
    public bool hurting = false;
    public bool knocked;
    public bool staggered = false;
    public int deathState = 0;
    public int direction;
    int attackState = 0;
    protected char dir;
    protected int curDir;
    protected int cNum;
    protected Color dodgeColor;
    public GameObject[] parts;
    public string[] names;
    public string[] actions;
    protected Animator[] anims;
    protected SpriteRenderer[] sr;

    public virtual void Awake() {
        anims = new Animator[parts.Length];
        sr = new SpriteRenderer[parts.Length];
        for (int i = 0 ; i < parts.Length; i++) {
            anims[i] = parts[i].GetComponent<Animator>();
            sr[i] = parts[i].GetComponent<SpriteRenderer>();
        }
        dir = 'F';
        curDir = 4;
        curAction = actions[2];
    }

    public virtual void setState() {
       // Debug.LogError("setting state");
        for (int i = 0; i < names.Length; i++) {
            anims[i].Play(character + names[i] + curAction + dir);
        }
    }

    public virtual void setColors(SwordSoul mySoul, int pNum) { }
    
    public virtual void spawn() { }

    //0-alive 1-normaldeath 2-burndeath
    public virtual void setDeath(int dead) { }

    public virtual void respawn() { }

    public virtual void swingInput(bool begin) { }

    public virtual void swingBegin() { }

    public virtual void lerpHeat(float fade) { }
    
    public virtual void disembodiedState(bool begin, bool dead) { }
    
    public virtual void swingOver(bool gettingInput, bool swingDetached) { }

    public virtual void cantSwing(bool begin) { }

    public virtual void hurt(bool begin) {
        Debug.LogError("anim hurt " + begin);
        if (hurting != begin) {
            if (begin) {
                if (!staggered) {
                    // hurt
                    curAction = actions[1];
                }
            } else {
                if (!staggered) {
                    //idle
                    curAction = actions[2];
                }
            }
            setState();
            hurting = begin;
        }    
    }

    public virtual void stagger(bool begin) {
        if (staggered != begin) {
            if (begin) {
                //staggered
                curAction = actions[4];
                if (walking) {
                    setWalking(false);
                }
            } else {
                //idle
                curAction = actions[2];
            }
            staggered = begin;
            setState();
        }    
    }

    public virtual void knock(bool begin) {
        if (knocked != begin) {
            if (begin) {
                curAction = actions[3];
            } else {
                curAction = actions[0];
            }
            knocked = begin;
            setState();
        }    
    }

    public virtual void setDir(int d) {
        if (curDir != d) {
            if (d == 0) {
                dir = 'B';
            } else if (d == 4) {
                dir = 'F';
            } else { //if (d == 2 || d == 6) {
                dir = 'S';
                flipX(d < 4);
            }
            curDir = d;
            setState();
        }    
    }

    public virtual void setWalking(bool on) {
        if (walking != on && !hurting) {
            if (!on || (dodgeState == 0 || deathState == 0)) {
                if (on) {
                    curAction = actions[5];
                } else {
                    curAction = actions[2];
                }
                walking = on;
                setState();
            }
        }    
    }

    public virtual void setWalkingSpeed(float n_speed) { }

    public virtual void attack(int state) {
        if (attackState != state) {
            if (state == 1) {
                curAction = actions[0];
            } else {
                curAction = actions[2];
            }
            attackState = state;
            setState();
        }
    }

    /*
     * 0 - no dodge
     * 1 - starting up
     * 2 - dodging
     * 3 - disembodied
     */
    public virtual void setDodge(int state) { }

    public void setDodgeColor(Color dc) {
        dodgeColor = dc;
    }

    public virtual void setAlpha(float val) { }

    void flipX(bool flip) {
        for (int i = 0; i < sr.Length; i++) {
            sr[i].flipX = flip;
        }
    }
}
