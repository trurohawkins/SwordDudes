using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class broomAnim : SwordAnimator {

    protected override void newAnimation() {
        anims[0].Play("handle"+action);
        anims[1].Play("bristle"+action);
    }
}
