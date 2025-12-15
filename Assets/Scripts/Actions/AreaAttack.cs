using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaAttack : Attack {

    public int lifeTime = 3;
    float skinSize = 1;
    public bool growSprite = false;

    protected override void Awake() {
        base.Awake();
        if (self.skin) {
            skinSize = self.skin.transform.localScale.x;
        }
    }

    public override IEnumerator callAction(int delay) {
        if (lifeTime > 0) {
            if (speedCounter >= speed) {
                self.leaveSpace();
                self.width++;
                //if (lifeTime % 2 == 0) {
                    self.length++;
                //}
                self.squareBody();
                self.drawBody();
                if (self.skin && growSprite) {
                    self.skin.transform.localScale = new Vector3(skinSize * self.width, skinSize * self.length, 1);
                    if (self.width % 2 == 0) {
                        //self.skin.transform.position = new Vector3(transform.position.x - 0.5f, transform.position.y - 0.5f, transform.position.z);
                    } else {
                        //self.skin.transform.position = transform.position;
                    }
                }
                lifeTime--;
                speedCounter = 0;
            } else {
                speedCounter++;
            }
        } else {
            endAttack();
        }
        yield return new WaitForFixedUpdate ();
    }

    protected override void endAttack() {
        base.endAttack();
        self.die();
    }
}
