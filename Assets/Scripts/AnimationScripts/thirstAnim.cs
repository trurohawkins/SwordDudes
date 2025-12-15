using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class thirstAnim : SwordAnimator {

    public Sprite[] bladeSprites;
    public Sprite[] deetSprites;

    List<Animator> bladeAnims;
    List<Animator> deetAnims;

    public override void initiate() {
        base.initiate();
        bladeAnims = new List<Animator>();
        deetAnims = new List<Animator>();
        for (int k = 0; k < will.bladeMulti; k++) {
			for (int i = 0; i < will.blade [k].Count; i++) {
				Form f = will.blade[k][i];
                bladeAnims.Add(f.skin.transform.GetChild(0).GetComponent<Animator>());
                deetAnims.Add(f.skin.transform.GetChild(1).GetComponent<Animator>());
            }
        }
    }

    protected override void newAnimation() {
        for (int i = 0; i < bladeAnims.Count; i++) {
            bladeAnims[i].Play(soul+state+action);
            if (state == "Burn") {
                deetAnims[i].Play("thirstBurnDeetIdle");//soul+state+"Deet"+action);
                //Debug.Log(soul+state+"Deet"+action);
            } else {
                deetAnims[i].Play("thirstBladeDeet");
            }
        }
    }

    public override void setHeatColor(Color c) {
        for (int i = 0; i < deetAnims.Count; i++) {
            SpriteRenderer sr = deetAnims[i].GetComponent<SpriteRenderer>();
            sr.color = c;
        }
    }
}
