using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordDummy : DungeonObject {
    
    circleAttack attack;
    public float hitAmount;

    public Color[] colors;
    public Sprite[] success;
    public SpriteRenderer[] sr;
    Animator[] anims;

    bool beat;

     protected override void Awake() {
        base.Awake();
        attack = gameObject.GetComponent<circleAttack>();
        anims = new Animator[sr.Length];
        for (int i = 0; i < sr.Length; i++) {
            anims[i] = sr[i].GetComponent<Animator>();
        }
    }

    void Start() {
        sr[0].color = colors[0];
        sr[2].color = colors[1];
        sr[3].color = colors[2];
    }

    public void getHit(float power) {
        if (!beat) {
            //Debug.Log("I(" + name + ") was hit with power: " + power);
            if (power > hitAmount) {
                master.targetMet(this);
                beat = true;
            }
            beHappy();
        }
    }

    void beHappy() {
        anims[0].Play("DummyBodyHappyF");
        anims[1].Play("DummyFaceHappyF");
        anims[2].Play("DummyHatHappyF");
        anims[3].Play("DummyDeetHappyF");
    }

    public override void cleanUp() {
        base.cleanUp();
        Debug.Log(name + " cleaning up");
        attack.getSoul().saveState();
        attack.die();
    }

    public override bool spawnIn() {
        if (base.spawnIn()) {
            //int angle = attack.curPos;//saveAngle;
            attack.respawn(attack.curPos);
            if (beat) {
                beHappy();
            }
            return true;
        } else {
            return false;
        }
    }
}
