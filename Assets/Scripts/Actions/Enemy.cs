using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Brain {
    public bool paused;
    public int chillSpeed;
    public int targSpeed;
    public GameObject attack;
    public Vector2Int healthRange;
    public Vector2Int speedRange;
    public Vector2Int attIntervalRange;
    public Vector2Int attRangeRange;
    public Vector2Int attLenRange;
    public Vector2 damageRange;
    public Vector2 KnockRange;
    public Vector2 stagRange;
    public Vector2Int attackSpeedRange;
    protected float damage;
    protected float knockback;
    protected float stagger;
    public bool attackDieOnImpact;
    bool melee;
    protected int team;
    Value myVal;

    protected int attackInterval;
    protected int attackLength = 40;
    public float attackPoint = 0.5f;
    int attackSpeed;
    int attackRange;
    protected Vector2 aDir;
    protected Vector2Int aPos;
    protected int attackTimer;
    protected bool attacking;
    public bool followAttack;
    protected Attack curAttack;

    public int senseRange = 30;

    public bool moveAndShoot;
    public bool waitWhileAttack;
    public bool attackNoTarget = false;
    public int loseEnemy = -1;
    int loseCount;

    bool busy;
    defense def;
    protected PathBoi pather;

    public Form target;
    Vector2Int dest;
    protected FighterAnimator anim;

     protected override void Awake() {
        base.Awake();
        def = gameObject.GetComponent<defense>();
        pather = gameObject.GetComponent<PathBoi>();
        if (!moveAndShoot) {
            pather.stayAtTarget = true;
        }
        anim = gameObject.GetComponentInChildren<FighterAnimator>();
        myVal = gameObject.GetComponent<Value>();
        if (anim) {
            anim.spawn();
            anim.setWalkingSpeed(0.5f);
        }
        attackTimer = 0;
        dest.x = -1;
        setTeam(6);
    }

    void Start() {
        setColors(mainColor, subColor);
    }

    public virtual void getTarget() {
        if (pather.target != target) {
            pather.getEnemy(target.gameObject);
        }
    }

    public override bool think() {
        if (!paused && base.think()) {
            if (target) {
                if (pather) {
                    getTarget();
                    dest = pather.curDest;
                } else {
                    dest = target.centerPoint;
                }
                if (loseEnemy >= 0) {
                    if (!canSense(target)) {
                        if (loseCount > loseEnemy) {
                            pather.loseTarget();
                            target = null;
                            dest.x = -1;
                            loseCount = 0;
                        } else {
                            loseCount++;
                        }
                    }
                }
            } else {
                if (dest.x < 0) {
                    int wanderRange = 20;
                    Vector2Int propo = self.centerPoint + new Vector2Int(Random.Range(-wanderRange, wanderRange), Random.Range(-wanderRange, wanderRange));
                    dest = propo;
                } else {
                    if (Vector2Int.Distance(self.centerPoint, dest) <= pather.minDist || !self.checkBody(dest, false, false)) {
                        dest.x = -1;
                        if (anim) {
                            anim.setWalking(false);
                        }
                    }
                }
                if (pather && pather.curDest != dest) {
                    pather.curDest = dest;
                }
                findTarget();
                if (target) {
                    pather.getLost();
                }
            }
            if (pather) {
                if (!self.checkBody(self.centerPoint, false, true)) {
                    for (int i = 0; i < self.curCollided.Count; i++) {
                        //Debug.Log("am currently colliding with " + self.curCollided[i]);
                    }
                }
                pather.updateInfo();
            }
        }
        return true;
    }

    void findTarget() {
        float closest = 9999999;
        for (int i = 0; i < GM.S.players.Count; i++) {
            //Debug.Log(GM.S.players[i]);
            if (GM.S.players[i] && !GM.S.players[i].dead) {
                if (canSense(GM.S.players[i].self)) {
                    float dist = Vector2Int.Distance(GM.S.players[i].self.centerPoint, self.centerPoint);
                    if (closest > dist) {
                        closest = dist;
                        target = GM.S.players[i].self;
                    }
                }
            }
        }
        if (target) {
            speed = targSpeed;
        } else {
            speed = chillSpeed;
        }
    }

    public bool motionSensitive;

    bool canSense(Form oth) {
        if (motionSensitive) {
            Player p = oth.GetComponent<Player>();
            if (p && p.isCreeping()) {
                return false;
            }
        }
        return Vector2Int.Distance(self.centerPoint, oth.centerPoint) <= senseRange;
    }

    public override IEnumerator callAction(int delay) {
        if (!paused && !def.dead && !self.dead) {// && target) {
            if (anim) {
                if (self.direction != anim.direction) {
                    anim.setDir(self.direction);
                }
            }
            if (def.canMove() && (!attacking || !waitWhileAttack) && speed >= 0) {
                if (speedCounter >= speed) {
                    if (!def.invulnerable) {
                        if (dest.x >= 0 && Vector2Int.Distance(self.centerPoint, dest) > pather.minDist) {
                            Vector2Int dir = GM.S.getClosestVec(self.centerPoint, dest);// new Vector2Int(0, -1);//target.centerPoint - self.centerPoint;
                            move(dir);
                            if (self.curCollided.Count != 0) {
                                if (!target) { // maybe with target too?
                                    dest.x = -1;
                                }
                            } else {
                                //Debug.Log("moved");
                                if (anim) {
                                    anim.setWalking(true);
                                }
                            }
                        } else if (anim) {
                            anim.setWalking(false);
                        }
                    } 
                    speedCounter = 0;
                } else {
                    speedCounter++;
                }
            } else if (anim) {
                anim.setWalking(false);
            }
            if (!def.staggered()) {
                doAttack();
            }
        }
        return base.callAction(delay);
    }

    public override void move(Vector2Int direction) {
        self.move(self.centerPoint + direction, false);
        if (self.curCollided.Count == 0) {
            // move success
            if (followAttack && curAttack) {
                curAttack.self.move(curAttack.self.centerPoint + direction, false);
            }
        } else {
            //Debug.Log("can not move in direction: " + direction);
        }
    }

    public virtual void doAttack() {
        if (!attacking) {
            float dist = 0;
            if (target) {
                dist = Vector2Int.Distance(self.centerPoint, target.centerPoint);
            }
            if ((attackNoTarget || target) && ((pather.curDest.x < 0 && dist <= attackRange) || moveAndShoot)) {
                if (attackTimer >= attackInterval) {
                    if (dist > attackRange && !moveAndShoot) {
                        Debug.Log("I should probly stop attackingand start moving");
                    }
                    attacking = true;
                    Vector2 dir = target.centerPoint - self.centerPoint;
                    dir.Normalize();
                    if (anim) {
                        anim.attack(1);
                        anim.setDir(GM.S.convertVectorToDir(dir));
                        if (waitWhileAttack) {
                            anim.setWalking(false);
                        }
                    }
                    if (melee) {
                        float angle = Map.S.saneAddition(Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg, 90, 1) * Mathf.Deg2Rad;
                        aPos = new Vector2Int((int)(self.centerPoint.x + (Mathf.Cos(angle))), (int)(self.centerPoint.y + (Mathf.Sin(angle))));
                        aDir = (Vector2)target.centerPoint - (Vector2)aPos;
                    } else {
                        aPos = new Vector2Int((int)(self.centerPoint.x + dir.x), (int)(self.centerPoint.y + dir.y));
                        aDir = dir;
                    }
                    attackTimer = 0;
                } else {
                    attackTimer++;
                }
            } else {
                //Debug.Log("cant attack (" + attackNoTarget + " || " + target + ") && (" + pather.curDest.x + " || " + moveAndShoot + ") && " + dist + " <= " + attackRange);
                if (pather.curDest.x < 0 && dist > attackRange) {
                    pather.getLost();//updatePathing();
                }
            }
        } else {
            if (attackTimer == (int)(attackLength * attackPoint)) {
                if (anim) {
                    anim.attack(1);
                }
                GameObject a = Instantiate(attack, transform.parent);//.GetComponent<Form>();
                Form att = a.GetComponent<Form>();
                att.squareBody();
                att.parent = self;
                Knockable kn = att.GetComponent<Knockable>();
                if (kn) {
                    if (!melee) {
                        kn.setStats(attackSpeed, Mathf.Max(1, attackSpeed - 2), attackRange+2, aPos, aDir);
                    } else {
                        kn.setStats(attackSpeed, Mathf.Max(1, attackSpeed - 2), attackRange+2, aPos, aDir.normalized);
                    }
                    kn.setDamages(damage, knockback, stagger);
                    kn.setTeam(team);
                    kn.setFeatures(attackDieOnImpact);
                } else {
                    Attack attack = a.GetComponent<Attack>();
                    attack.setDamages(damage, knockback, stagger);
                    attack.setTeam(team);
                }
                Map.S.spawnForm(a, aPos.x, aPos.y);                    
                if (!att.spawned) {
                    att.die();
                } else {
                    att.etheral = false;
                    curAttack = att.GetComponent<Attack>();
                    curAttack.getCreator(this);
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

    public void releaseAttack() {
        curAttack = null;
    }

    public void setStats(float percent) {
        def.setMaxHealth(Mathf.Lerp(healthRange[0], healthRange[1], percent));//master.getProgress(1));
        def.setStaggerStats(Mathf.Lerp(stagRange[0], stagRange[1], percent), 10);
        attackInterval = (int)Mathf.Lerp(attIntervalRange[0], attIntervalRange[1], percent);
        attackTimer = Random.Range(0, attackInterval);
        attackLength = (int)Mathf.Lerp(attLenRange[0], attLenRange[1], percent);
        targSpeed = (int)Mathf.Lerp(speedRange[0], speedRange[1], percent);
        attackSpeed = (int)Mathf.Lerp(attackSpeedRange[0], attackSpeedRange[1], percent);
        setDamageRange(percent);
        setKnockback(percent);
        setStagger(percent);
        closeRange(percent);
        return;
        /*
        // long range
        if (Random.value > 1) {
            longRange(percent);
        } else { // short range
            closeRange(percent);
        }
        */
    }

    public void setStaggerStats(float threshold, float recover) {
        def.setStaggerStats(threshold, recover);
    }

    public void longRange(float percent) {
        attackRange = (int)Mathf.Lerp(attRangeRange[0], attRangeRange[1], percent);
        pather.setRange(0, (int)(attackRange * 0.75f), attackRange);//attackRange/2, attackRange);
        melee = false;
    }

    public void closeRange(float percent) {
        attackRange = (int)Mathf.Lerp(5, 7, percent);
        //pather.setRange(0, (int)(attackRange * 0.75f), attackRange);
        pather.setRange(0, 0, (int)(attackRange * 0.75));
        melee = true;
    }

    public void setDamageRange(float percent) {
        damage = Mathf.Lerp(damageRange[0], damageRange[1], percent);
    }

    public void setAttackLengthRange(float percent) {
        attackLength = (int)Mathf.Lerp(attLenRange[0], attLenRange[1], percent);
    }

    public void setDamage(float amnt) {
        damage = amnt;
    }

    public void setHealth(float health) {
        def.setMaxHealth(health);
    }

    public void setTeam(int newTeam) {
        team = newTeam;
        def.setTeam(newTeam);
        myVal.setTeam(team);
    }

    public void knockBackEnemy(float percent) {
        Debug.Log("knockback enemy");
        def.setMaxHealth(Mathf.Lerp(35, 65, percent));
        targSpeed = (int)Mathf.Lerp(speedRange[0], speedRange[1], percent);
        def.weight = 1;
        attackInterval = (int)Mathf.Lerp(attIntervalRange[0], attIntervalRange[1], percent);
        attackTimer = Random.Range(0, attackInterval);
        attackRange = 5;
        pather.setRange(0, 0, 1);
        motionSensitive = false;
        loseEnemy = -1;
    }

    public void staggerEnemy(float percent) {
        def.setMaxHealth(Mathf.Lerp(1, 20, percent));
        def.def = 0.01f;
        def.weight = 20;
        def.staggerVul = 100;
        def.setStaggerStats(30, 20);
        attackInterval = (int)Mathf.Lerp(attIntervalRange[0], attIntervalRange[1], percent);
        attackTimer = Random.Range(0, attackInterval);
        attackLength = (int)Mathf.Lerp(attLenRange[0], attLenRange[1], percent);
        setDamageRange(percent);
        setKnockback(percent);
        setStagger(percent);
        closeRange(percent);
        setColors(Color.yellow, Color.blue);
    }

    public void setWeight(int weight) {
        def.weight = weight;
    }

    public void setStaggerDef(float threshold, float recover) {
        def.setStaggerStats(threshold, recover);
    }

    public void setKnockback(float percent) {
        knockback = Mathf.Lerp(KnockRange[0], KnockRange[1], percent);
    }

    public void setStagger(float percent) {
        stagger = Mathf.Lerp(stagRange[0], stagRange[1], percent);
    }

    public override void die() {
        if (def.curCorpse) {
            PlayerAnimator pa = def.curCorpse.GetComponentInChildren<PlayerAnimator>();
            if (pa) {
                pa.setEyes (2);
			    pa.setDir (4);

                if (self.length != 2) {
                    def.curCorpse.length = self.length;
                    def.curCorpse.width = self.width;
                    def.curCorpse.squareBody();
                    def.curCorpse.transform.GetChild(0).localScale = new Vector3(7.5f * self.width, 7.5f * self.length, 1);
                }
            }
            if (anim) {
                PlayerAnimator mine = anim.GetComponent<PlayerAnimator>();
                if (mine) {
                    mine.giveColors(pa);
                }
            }
            //anim.setColors(mySoul, pNum);
            if (pa) {
                pa.setDeath(1);
            }
        }
    }

    public Color mainColor;
    public Color subColor;

    public void setColors(Color main, Color sub) {
        mainColor = main;
        subColor = sub;
        if (anim) {
            PlayerAnimator mine = anim.GetComponent<PlayerAnimator>();
            if (mine) {
                mine.setColors(main, sub, mine.detColor, mine.powerColor);
            }
        }
    }

    public void setSize(int dim) {
        self.length = dim;
        self.width = dim;
        self.squareBody();
        if (anim) {
            anim.transform.localScale = new Vector3(7.5f * dim, 7.5f * dim, 1);
        }
    }
}
