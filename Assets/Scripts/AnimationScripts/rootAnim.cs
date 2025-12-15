using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rootAnim : SwordAnimator {

    int growth = 2;
    public Color[] petals;
    public Color[] highlights;
    public Color[] pollen;
    int colorNum = 0;

    protected override void newAnimation() {
        //Debug.Log(soul+"Blade"+growth+action);
        anims[0].Play(soul+"Blade"+growth+action);
        //Debug.Log(soul+"Hilt"+action);
        anims[1].Play(soul+"Hilt"+action);
        if (growth >= 6) {
            string a = action != "Knock" ? action : "Idle";
            anims[2].Play(soul+"FlowerPetal"+a);
            anims[3].Play(soul+"FlowerHighlight"+a);
            anims[4].Play(soul+"FlowerPollen"+a);
        }
    }

    public override void setLayer(int l) {
        sr[0].sortingOrder = l + 1;
        sr[1].sortingOrder = l;
        for (int i = 2; i < 5; i++) {
            sr[i].sortingOrder = l + i;
        }
    }

    public void setGrowth(int grown) {
        if (grown > 0) {
            growth = grown-1;
            invisColor(0, false);
            newAnimation();
            bool flower = growth > 5 ? true : false;
            for (int i = 2; i < 5; i++) {
                invisColor(i, !flower);
            }
        } else {
            Debug.Log("invis color cuz no sword");
            invisColor(0, true);
        }
    }

    public void flowerHeat(float fade, Color heat) {
        if (growth > 5) {
            sr[2].color = Color.Lerp(petals[colorNum], heat, fade);
            sr[3].color = Color.Lerp(highlights[colorNum], heat, fade);
            sr[4].color = Color.Lerp(pollen[colorNum], heat, fade);
        }
    }

    public override void setAlpha(float val) {
        Debug.Log("root set alpha " + val);
		if (sr != null) {
			//Debug.Log("setting alpha " + val);
            int amnt = growth > 5 ? sr.Length : 2;
			for (int i = 0; i < amnt; i++) {
				Color c = sr[i].color;
				c.a = val;
				setColor(c, i);
			}
		} else {
			Debug.LogWarning("no sr on " + name);
		}
	}


    public void setFlowerColors(int cNum) {
        colorNum = cNum;
        if (sr != null) {
            sr[2].color = petals[cNum];
            sr[3].color = highlights[cNum];
            sr[4].color = pollen[cNum];
        }
    }

    public SpriteRenderer getFlower() {
        return sr[2];
    }

    public override void charSelectGlance() {
        invisColor(1, false);
        setGrowth(7);
    }
}
