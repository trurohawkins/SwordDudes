using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shadeAnim : SwordAnimator {

    protected override void newAnimation() {
        anims[0].Play(soul+"Blade"+action);
		anims[1].Play(soul+state+"Deet"+action);
    }
}
