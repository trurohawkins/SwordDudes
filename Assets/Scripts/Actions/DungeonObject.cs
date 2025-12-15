using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonObject : defense {
    public bool isTarget;
    public bool removeOnMet = true;
    public DungeonRoom master;
    public Vector2Int pos;
    public GameObject[] turnOff;
    public int clearSpace = 0;// false;
    public bool killTarget;
    public bool spawned;
    public bool searchForSpot;
    public bool partOfWave = false;

    public virtual bool spawnIn() {
        if (!spawned) {
            self.active = true;
            if (postPoned) {
                readyFromWave();
            }
            if (searchForSpot) {
                if (!self.checkBody(pos, false, false)) {
                    //Debug.Log("sword cant spawn at " + pos);
                    for (int i = 2; i < 15; i+=3) {
                        float d = 0f;
                        float size = i * (Mathf.PI * 2);
                        for (float j = 0; j <= size + 1; j += 1 ) {
			                d += 360 / size;
			                int x =  (int)myDumbRound(i * Mathf.Cos (d));
			                int y = (int)myDumbRound(i * Mathf.Sin (d));
                            //Debug.Log(x + ", " + y);
                            if (self.checkBody(new Vector2Int(pos.x + x, pos.y + y), false, false)) {
                                pos = new Vector2Int(pos.x + x, pos.y + y);
                                //Debug.Log("new pos found: " + pos);
                                i = 12;
                                break;
                            }
                        }
                    }
                }
            }
            Map.S.spawnForm(gameObject, pos.x, pos.y);
            //Debug.Log(name + " " + self.spawned + " " + pos);
            if (!self.spawned) {
                return false;
            }

            //creator.clearRect(pos, obj.width, obj.length);

            for (int i = 0; i < turnOff.Length; i++) {
                turnOff[i].SetActive(true);
            }
            PathBoi boi = gameObject.GetComponent<PathBoi>();
            if (boi) {
                boi.getLost();
            }
            spawned = true;
        }
        return true;
    }

    float myDumbRound(float f) {
        int sign = (int)Mathf.Sign(f);
        f = Mathf.Abs(f);
        float rem = f - Mathf.Floor(f);
        if (rem > 0.4f) {
            rem = 1;
        } else {
            rem = 0;
        }
        return (Mathf.Floor(f) + rem) * sign;
    }

    bool postPoned = false;
    public void addToWave() {
        postPoned = true;
        for (int i = 0; i < turnOff.Length; i++) {
            turnOff[i].SetActive(false);
        }
    }

    public void readyFromWave() {
        postPoned = false;
        /*
        if (self.skin) {
            self.skin.SetActive(true);
        }
        */
    }


    public override void callAction(Form poo, int state, int x, int y) {
        if (poo.id != self.id) {
            base.callAction(poo, state, x, y);
            if (killTarget && self.dead) {
                master.targetMet(this);
            }
        }
    }

    public virtual void cleanUp() {
        //Debug.Log("cleaning up " + name);
        spawned = false;
        pos = self.centerPoint;
        self.active = false;
        self.removeForm();//die();
        for (int i = 0; i < turnOff.Length; i++) {
            turnOff[i].SetActive(false);
        }
    }

    public virtual void waveComplete() { }

    public virtual void receiveMaster(DungeonRoom n_master) {
        master = n_master;
    }
}
