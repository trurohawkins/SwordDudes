using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : DungeonObject {
    public int dir;
    bool locked = true;
    public Sprite open;
    SpriteRenderer sr;
    Sprite closed;

    protected override void Awake() {
        base.Awake();
        sr = gameObject.GetComponentInChildren<SpriteRenderer>();
        closed = sr.sprite;
    }

    public override void callAction(Form poo, int state, int x, int y) {
        if (!locked && active) {
            if (poo.id == 1) { // && state == 0) {
                GM.S.destroyWorld(dir);
                //active = false;
            }
        }
    }

    public bool checkLocked() {
        return locked;
    }

    public override bool spawnIn() {
        if (!spawned) {
            if (dir == 0) {
                 master.creator.clearRect(pos, 5, 2);
            } else if (dir == 2) {
                master.creator.clearRect(pos, 5, 3);
            } else if (dir == 1) {
                master.creator.clearRect(pos, 3, 5);
            } else if (dir == 3) {
                master.creator.clearRect(pos, 2, 5);
            }
        }
        return base.spawnIn();
    }

    public void setLock(bool val) {
        if (locked != val) {
            locked = val;
            if (locked) {
               // Color c = self.color;
               // c.a = 1;
                //self.changeColorN(c);
                sr.sprite = closed;
                //self.height = 20;
            } else {
               // Color c = self.color;
               // c.a = 0;
                //self.changeColorN(Color.black);
                sr.sprite = open;
                //self.height = 1;
                self.leaveSpace();
                if (dir % 2 == 0) {
                    //self.length = 1;
                    if (dir == 2) {
                        //self.centerPoint.y -= 1;
                        //pos = self.centerPoint;
                    }
                } else if (dir % 2 == 1) {
                    //self.width = 1;
                    if (dir == 1) {
                        //self.centerPoint.x -= 1;
                        //pos = self.centerPoint;
                    }
                }
                self.squareBody();
                self.drawBody();
                //Debug.Log(self.color + " " + self.name);
            }
        }
    }
}