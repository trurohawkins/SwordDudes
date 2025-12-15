using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTimer : Purpose {

    public int lifeTime;

    protected override void Awake() {
        base.Awake();
        if (self && self.activeAction != null) {
            self.activeAction.Add(this);
        }
    }

    public override IEnumerator callAction(int delay) {
        if (lifeTime > 0) {
            lifeTime--;
        } else {
            //Debug.Log(name + "die");
            self.die();
        }
        yield return new WaitForFixedUpdate();
    }

}
