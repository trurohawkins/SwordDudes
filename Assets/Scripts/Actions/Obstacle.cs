using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : DungeonObject {
    public GameObject pedastle;
    DungeonObject myPedastle;
    int condition = 1;
    public int timeReset = -1;
    int timer;
    public Color[] typeColors;
    public Sprite hit;
    Sprite neutral;
    public Sprite[] faces;
    SpriteRenderer target;
    SpriteRenderer indicator;
    bool curHit;
    int hitTimer = 20;
    int ht = 0;
    bool badCon;
    int bHeight;

    public override bool spawnIn() {
        if (!base.spawnIn()) {
            return false;
        } else {
            if (!myPedastle) {
                Vector2Int p = pos;
                p.y -= 3;
                DungeonObject obj = master.setUpObj(pedastle, p, false);
                getIndicator(obj.transform.GetChild(0).GetComponent<SpriteRenderer>());
                myPedastle = obj;
            }
            return true;
        }
    }

    protected override void Awake() {
        base.Awake();
        target = transform.GetChild(0).GetComponent<SpriteRenderer>();
        neutral = target.sprite;
    }

    public void Start() {
        bHeight = self.height;
    }

    public override void callAction(Form enemy, int state, int x, int y) {
        bool conditionMet = condition == 0;
        if (!dead && !invulnerable && !self.dead) {
            	Value v = enemy.GetComponent<Value> ();
			    if (v && !v.onTeam(team)) {
            		if (Grader.S) {
					    Grader.S.makeHit(enemy, calculateDamage(enemy), 0);
				    }
                }
			if (!checkCondition(enemy)) {
                weight = -1;//return;
                def = 0;
                sadFace();
            } else {
                if (badCon) {
                    badCon = false;
                    indicator.sprite = faces[0];
                }
                partOfWave = false;
                conditionMet = true;
                curHit = true;
                self.dead = true;
                master.targetMet(this);
                ht = hitTimer;
                target.sprite = hit;
                weight = 0;
                def = 1;//invulnerable = false;
                self.height = 0;
            }
        }
        /*
        base.callAction(enemy, state, x, y);
        if (self.dead && conditionMet) {
            if (curCorpse) {
                beatTarget bt = curCorpse.GetComponent<beatTarget>();
                bt.setConditions(condition, timeReset);
                bt.receiveMaster(master);
            }
            master.targetMet(this);
        }
        */
    }

    bool goodBeat = false;

    public override IEnumerator callAction(int delay) {
        if (curHit) {
            if (ht > 0) {
                ht--;
            } else {
                //base.callAction(curHit, 0, 0, 0);
                if (self.dead) {// && conditionMet) {
                    target.sprite = null;
                    
                    if (timeReset != -1) {
                        timer = timeReset;
                    } else {
                        indicator.sprite = faces[2];
                    }
                }
                curHit = false;
            }
        } else {
            if (badCon) {
                if (ht > 0) {
                    ht--;
                } else {
                    indicator.sprite = faces[0];
                    badCon = false;
                }
            }
        }
        if (timer > 0) {
            if (master.beat) {
                goodBeat = master.beat;
            }
            if (goodBeat) {
                indicator.sprite = faces[2];
                timer = 0;
            } else {
                timer--;
                if (timer == 0) {
                    if (!goodBeat) {
                        master.addTarget(this);
                        target.sprite = neutral;
                        weight = -1;//return;
                        def = 0;
                        self.height = bHeight;
                        self.dead = false;
                        sadFace();
                    }
                } else if (timer < timeReset / 2) {
                    if (timer % 5 == 0) {
                        if (indicator.sprite == faces[0]) {
                            indicator.sprite = faces[1];
                        } else {
                            indicator.sprite = faces[0];
                        }
                    }
                }
            }
        }
            return base.callAction(delay);
    }

    public void setCondition(int con) {
        condition = con;
        if (con < typeColors.Length) {
            target.color = typeColors[con];
        }
    }

    public bool  checkCondition(Form hit) {
        if (hit.id == 2) {
            circleAttack ca = hit.GetComponentInParent<circleAttack>();
            if (condition == 0) {
                return ca.swinging;
            } else if (condition == 1) {
                if (ca.checkSpeed(1)) {
                    return true;
                }
            } else if (condition == 2) {
                SwordSoul ss = ca.getSoul();
                if (ss.burning) {
                    return true;
                }
            }
        }
        return false;
    }

    public override void waveComplete() {
        /*
        if (timer > 0) {
            timer = -1;
        }
        indicator.sprite = faces[2];
        goodBeat = true;
        */
    }

    void sadFace() {
        if (indicator) {
            indicator.sprite = faces[1];
        } else {
            Debug.LogError("no indicator on obstacle on " + name);
        }
        badCon = true;
        ht = hitTimer;
    }

    public void getIndicator(SpriteRenderer sr) {
        indicator = sr;
    }

    public bool isHit() {
        return curHit;
    }
}
