using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grader : MonoBehaviour {
    public static Grader S;
    Grade curRoom;
    DungeonMaster dm;
    int[] movement;
        // avoiding obstacles
    public float bumpVal;
        // creeping
    public float creepVal;
        // moving fast
    public float timeVal;
    public int[] mCaps;
    int[] dodge;
        // go thru walls
    public float wallVal;
        // avoid damage
        // in place dodge
        // cooldown/recovery
        // dodge chaining
        // counter attack
    public int[] dCaps;
    int[] sword;
        // aiming
    public float aimVal;
        // knockback
    public float knockVal;
        // stagger
    public float stagVal;
        // momentum
        // heat
        // weight
    public float weightVal;
        // quick strikes
    public int[] sCaps;
    int[][] skills;
    bool[][] skillOut;
    public int roomsBeaten = 0;

    void Awake() {
        if (!S) {
            S = this;
        }
        dm = gameObject.GetComponent<DungeonMaster>();
        movement = new int[mCaps.Length];
        dodge = new int[dCaps.Length];
        sword = new int[sCaps.Length];

        skills = new int[3][];
        skills[0] = movement;
        skills[1] = dodge;
        skills[2] = sword;
        skillOut = new bool[3][];
        skillOut[0] = new bool[mCaps.Length];
        skillOut[1] = new bool[dCaps.Length];
        skillOut[2] = new bool[sCaps.Length];
    }

    public void newRoom(DungeonRoom room) {
        if (!room.beat) {
            curRoom = room.GetComponent<Grade>();
            if (curRoom) {
                curRoom.seconds = Time.time;
                if (!curRoom.init) {
                    curRoom.initBools(movement.Length, dodge.Length, sword.Length);
                    curRoom.judge = this;
                }
            } else {
                Debug.Log("no grade on room");
            }
        }
    }

    public void beatRoom() {
        curRoom.beat = true;
        roomsBeaten++;
        //Debug.Log("beat room with " + curRoom.curBumps + " bumps, at " + Time.time);
        curRoom.gradeBumps(bumpVal);
        if (movement[0] < mCaps[0]) {
            if (curRoom.bumpScore < (mCaps[0] - movement[0])) {
                movement[0]++;
            }
        }
        if (curRoom.curSkills[0][0]) {
           // Debug.Log("bumpy");
            skillOut[0][0] = false;
        }

        curRoom.gradeCreep(creepVal);
        if (movement[1] < mCaps[1]) {
            if (curRoom.creepScore > movement[1]) {
                movement[1]++;
            }
        }
        if (curRoom.curSkills[0][1]) {
           // Debug.Log("smoothy " + " " + curRoom.curCreep * creepVal + " > " + movement[1]);
            skillOut[0][1] = false;
        }

        curRoom.gradeSpeed(timeVal);
        if (movement[2] < mCaps[2]) {
            if (curRoom.speedScore < mCaps[2] - movement[2]) {
                movement[2]++;
            }
        }
        if (curRoom.curSkills[0][2]) {
           // Debug.Log("quickly");
            skillOut[0][2] = false;
        }

        curRoom.gradeThruWalls(wallVal);
        if (dodge[0] < dCaps[0]) {
            if (curRoom.thruWallScore > dodge[0]) {
                dodge[0]++;
            }
        }
        if (curRoom.curSkills[1][0]) {
            //Debug.Log("ghostly");
           skillOut[1][0] = false;
        }

        curRoom.gradeAccuracy(aimVal);
        if (sword[0] < sCaps[0]) {
            if (curRoom.accuracyScore > sword[0]) {
                sword[0]++;
            }
        }
        if (curRoom.curSkills[2][0]) {
           // Debug.Log("accurately");
           skillOut[2][0] = false;
        }

        curRoom.gradeKnockback(knockVal);
        if (sword[1] < sCaps[1]) {
            if (curRoom.knockBackScore > sword[1]) {
                sword[1]++;
            }
        }
        if (curRoom.curSkills[2][1]) {
            //Debug.Log("heartily");
            skillOut[2][1] = false;
        }

        curRoom.gradeStagger(stagVal);
        if (curRoom.curSkills[2][2]) {
            //Debug.Log("nastily");
            if (sword[2] < sCaps[2]) {
                if (curRoom.staggerScore > sword[2]) {
                    sword[2]++;
                }
            }
        }
    }

    public int getAvgLevel() {
        int level = 0;
        int count = 0;
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < skills[i].Length; j++) {
                level += skills[i][j];
                count++;
            }
        }
        return level / count;
    }

    public int getRoomsBeaten() {
        return roomsBeaten;
    }
   
    int getSkillType(int[] category) {
        int min = 9999; // arbitrarily high num
        int chosen = -1;
        for (int i = 0; i < category.Length; i++) {
            if (category[i] < min) {
                chosen = i;
                min = category[i];
            }
        }
        return chosen;
    }
     /*
    public int getMoveSkill() {
        return getSkill(movement);
    }

    public int getDodgeSkill() {
        return getSkill(dodge);
    }

    public int getSwordSkill() {
        return getSkill(sword);
    }
    */
    public Vector2Int getSkill() {
        //return new Vector2Int(0, 1);
        int min = 9999;
        Vector2Int chosen = new Vector2Int(-1, -1);
        int sStart = Random.Range(0, skills.Length);
        for (int i = 0; i < skills.Length; i++) {
            int sk = (sStart + i) % skills.Length;
            int tStart = Random.Range(0, skills[sk].Length);
            for (int j = 0; j < skills[sk].Length; j++) {
                int tk = (tStart + j) % skills[sk].Length;
                if (skills[sk][tk] < min && !skillOut[sk][tk]) {
                    min = skills[sk][tk];
                    chosen = new Vector2Int(sk, tk);
                }
            }
        }
        skillOut[chosen.x][chosen.y] = true;
        return chosen;
    }

    public void releaseSkills(List<Vector2Int> skills) {
        for (int i = 0; i < skills.Count; i++) {
            for(int k = 0; k < skillOut.Length; k++) {
                for (int j = 0; j < skillOut[j].Length; j++) {
                    if (skills[i].x == k && skills[i].y == j) {
                        Debug.Log("releasec skill " + k + ", " + j);
                        skillOut[k][j] = false;
                    }
                }
            }
        }
    }

    public int getSkillType(int type) {
        return getSkillType(skills[type]);
    }
       
    public int getSkillLevel(int skill, int type) {
        return skills[skill][type];
    }

    public float getSkillPercent(int category, int skill) {
        float lvl = skills[category][skill];
        if (category == 0) {
            return lvl / mCaps[skill];
        } else if (category == 1) {
            return lvl / dCaps[skill];
        } else if (category == 2) {
            return lvl / sCaps[skill];
        }
        Debug.LogWarning("no skill for category: " + category + " skill: " + skill);
        return 0;
    }

    bool grading(Form f) {
        switch(f.id) {
            case 2:
                attackChunk ac = f.GetComponent<attackChunk>();
                f = ac.getAttack().getBody();
                break;
            case 3:
                circleAttack ca = f.GetComponent<circleAttack>();
                f = ca.getBody();
                break;
            default:
                break;
        }
        if (f.id == 1) {
            return curRoom && !curRoom.beat;
        } else {
            return false;
        }
    }

    public void gotBumped(Form bumper, List<Form> bumps) {
        if (grading(bumper)) {
            for (int i = 0; i < bumps.Count; i++) {
                Form f = bumps[i];
                //Debug.Log(bumper.name + " bumped " + f.name);
                Value v = f.GetComponent<Value>();
                if (v && (v.getValue("damage") != -1 || v.getValue("stagger") != -1 || v.getValue("knockBack") != -1) || (f.GetComponent<CreepDebris>())) {
                    //Debug.Log(bumper.name + " bumped hurt into " + f.name);
                    curRoom.curBumps++;
                }
                if (f.height >= bumper.height) {
                    //Debug.Log(bumper.name + " bumped into " + f.name);
                    curRoom.curBumps++;
                }
            }
            //curRoom.curBumps++;
        }
    }

    public void creeping(Form creeper) {
        if (grading(creeper)) {
            curRoom.curCreep++;
        }
    }

    public void dodgeThruOb(Form passer) {
        if (grading(passer)) {
            curRoom.thruWalls++;
        }
    }

    int swinging = 0;

    public void swingCommence(Form swinger) {
        //Debug.Log(swinger.name);
        if (grading(swinger)) {
            //Debug.Log("we got a swing");
            swinging = -1;
            curRoom.numSwings++;
        }
    }

    public void swingEnd(Form swinger) {
        if (grading(swinger)) {
            if (swinging < 0) {
                curRoom.numMisses++;
            }
            swinging = 0;
        }
    }

    public void makeHit(Form hitter, float damage, float knockBack) {
        if (grading(hitter)) {
            if (swinging == -1) {
                swinging = 1;
            }
            curRoom.numHits++;
            curRoom.damageDealt += damage;
            curRoom.knockedBack += knockBack;
        }
    }

    public void makeHit(Form hitter) {
        if (grading(hitter)) {
            if (swinging == -1) {
                swinging = 1;
            }
        }
    }

    public void makeHit(Form hitter, float power) {
        if (grading(hitter)) {
            if (swinging == -1) {
                swinging = 1;
            }
            curRoom.swordKnock += power;
        }
    }

    public void stagger(Form staggerer) {
        if (grading(staggerer)) {
            curRoom.curStaggers++;
        }
    }
}
