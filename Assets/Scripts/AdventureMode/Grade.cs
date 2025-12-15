using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grade : MonoBehaviour {
    public Grader judge;
    bool[] movement;
    public int curBumps;
    public int curCreep;
    public float seconds;
    bool[] dodge;
    public int thruWalls;
    bool[] sword;
    public int numSwings;
    public float damageDealt;
    public float knockedBack;
    public float swordKnock;
    public int curStaggers;
    public int numHits;
    public int numMisses;
    public bool beat;
    public bool init;
    public bool[][] curSkills;

    //Scores
    public float bumpScore;
    public float creepScore;
    public float speedScore;

    public float thruWallScore;

    public float accuracyScore;
    public float knockBackScore;
    public float staggerScore;

    public void initBools(int move, int dod, int swo) {
        movement = new bool[move];
        dodge = new bool[dod];
        sword = new bool[swo];
        curSkills = new bool[3][];
        curSkills[0] = movement;
        curSkills[1] = dodge;
        curSkills[2] = sword;
        init = true;
    }

    public float gradeBumps(float bumpVal) {
        bumpScore = 1 - (curBumps * bumpVal);
        return bumpScore;
    }

    public float gradeCreep(float creepVal) {
        creepScore = curCreep * creepVal;
        return creepScore;
    }

    public float gradeSpeed(float timeVal) {
        float time = Time.time - seconds;
        return timeVal * time;
    }

    public float gradeThruWalls(float wallVal) {
        thruWallScore = thruWalls * wallVal;
        return thruWallScore;
    }

    public float gradeAccuracy(float aimVal) {
        float total = numHits + numMisses;
        float accuracy = (float)numHits / total;
        float avgDamage = damageDealt / (float)numHits;
        Debug.Log(numHits + " # of hits. " + numMisses + " # of misses. " + (int)(accuracy*100) + "% accuracy. " + avgDamage + " avg dam per hit");
        accuracyScore = aimVal * accuracy;
        Debug.Log("score: " + accuracyScore);
        return accuracyScore;
    }

    public float gradeKnockback(float knockVal) {
        knockBackScore = knockedBack * knockVal;
        return knockBackScore;
    }

    public float gradeStagger(float stagVal) {
        staggerScore = curStaggers * stagVal;
        return staggerScore;
    }
}
