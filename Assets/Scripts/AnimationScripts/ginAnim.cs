using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ginAnim : SwordAnimator {

    string size;

    public override void setLayer(int l) {
        if (sr != null) {
			for (int i = 0; i < sr.Length; i++) {
                if (i % 2 == 0) {
                    sr[i].sortingOrder = l + 1;
                } else {
                    sr[i].sortingOrder = l;
                }
            }
        }
    }

    public void setSize(float power) {
        if (power < 0.4f) {
            size = "Small";
        } else if (power < 0.9f) {
            size = "Mid";
        } else {
            size = "Big";
        }
        //Debug.Log("new size " + size);
        newAnimation();
    }

    protected override void newAnimation() {
        anims[0].Play(soul+state+size+action);
        anims[1].Play(soul+"Hilt"+action);
    }

    public override void setColor(Color c, int sprite) {
        //Debug.Log("setting color");
        // for hilt deet
    	if (!will || !will.isHidden() || sprite == 2 || sprite == 1) {
			if (sr != null && sprite < sr.Length) {
				sr[sprite].color = c;
			} else {
				Debug.LogError(sprite + " out of boudns " + name + "sr: " + sr);// + sr.Length);
			}
		} else {
			//Debug.Log(name + " couldn;t set the color on " + sprite);
		}
    }

    public float curRoto;
    public override void setRotation(float pos) {
        if (pos != curRoto) {
            base.setRotation(pos);
            curRoto = pos;
        }
    }

    public override void charSelectGlance() {
        setAlpha(1);
    }

    void OnDisable() {
		Debug.LogWarning(name + " disabled");
	}

}
