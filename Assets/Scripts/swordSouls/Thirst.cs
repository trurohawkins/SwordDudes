using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thirst : SwordSoul {

	public float swingDamage;
	public float lifeHeal;
	public float curDamage;
	public float healWhither;

	public float igniteDamage = 1.5f;
	float dmgMod = 1;
	public float igniteHeal = 2f;
	float healMod = 1;
	thirstAnim tAnim;

	protected override void Awake() {
        base.Awake();
		if (anim) {
			tAnim = anim.GetComponent<thirstAnim>();
		}
	}

    public override void burn() {
		life.takeDamage (swingDamage * dmgMod, false);
		curDamage += swingDamage * healMod;
	}

    public override void aliveFrame() {
        if (!burning) {
			if (curDamage - healWhither > 0) {
				curDamage -= healWhither;
			} else {
				curDamage = 0;
			}
		}
    }

    public override void reset() {
		base.reset ();
		curDamage = 0;
		hitFlaring = false;
	}
	
	bool hitFlaring = false;

	public override void hitPlayer() {
		if (burning) {
			if (!hitFlaring) {
				StartCoroutine (healHit ());
			}
			life.heal(curDamage * lifeHeal, true);
			curDamage = 0;
		}
	}

    public override void ignite() {
        base.ignite();
		dmgMod = igniteDamage;
		healMod = igniteHeal;
    }

    public override void baseGear() {
        base.baseGear();
		dmgMod = 1;
		healMod = 1;
    }

    public override void brace() {
        base.brace();
		dmgMod = 1;
		healMod = 1;
    }

    IEnumerator healHit() {
		hitFlaring = true;
		for (int k = 0; k < will.bladeMulti; k++) {
			for (int i = 0; i < will.blade [k].Count; i++) {
				Form f = will.blade [k] [i];
				for (int j = 0; j < f.effects.Count; j++) {
					f.effects [j].kindle ();
				}
			}
		}
		yield return new WaitForSeconds (0.5f);
		for (int k = 0; k < will.bladeMulti; k++) {
			for (int i = 0; i < will.blade [k].Count; i++) {
				Form f = will.blade [k] [i];
				for (int j = 0; j < f.effects.Count; j++) {
					f.effects [j].smother ();
				}
			}
		}
		hitFlaring = false;
	}

	public override void setColors(SwordAnimator an, int cn) {
		anim = an;
        setColors(cn);
    }

    public override void setSprites(bool on) {
        base.setSprites(on);
		for (int k = 0; k < will.bladeMulti; k++) {
			for (int i = 0; i < will.blade [k].Count; i++) {
				Transform tmp = will.blade[k][i].transform.GetChild(0);
				if (tmp) {
					tmp.gameObject.SetActive(on);
				}
			}
		}
    }
}
