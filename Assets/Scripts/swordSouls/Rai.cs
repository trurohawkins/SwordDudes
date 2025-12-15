using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rai : SwordSoul {
	public float burnDamage;
	public int zapDamage;
	int curCharge;

	public override bool startBurn() {
		bool ab = false;
		if (!wielder.dead) {
			ab = base.startBurn ();
			if (!ab) {
				//curCharge = chargeTime;
				//will.clearPath (10);
				will.baseSpeedChange(chargedSpeed.x, chargedSpeed.y);
				/*
				will.minSpeed = chargedSpeed.x;// [0];
				will.maxSpeed = chargedSpeed.y;// [1];
				*/
			}
			energy = maxHeat;
			getZapped(zapDamage);
			if (anim) {
				anim.invisColor(3, false);
				anim.playAnim(3, "raiHiltDeet3");
				anim.setColor(heatColorA, 1);
			}
		}
		return ab;
	}

	void getZapped(int zap) {
		if (zap < life.health) {
			life.takeDamage (zap, false, false, true);
		} else {
			flare(0.5f, 2);
			zapDeath = 0;
			zapDeathDamage = zap;
		}
	}

	public override void heatUp(float heat, float max) {
		//if (burning) {
			base.heatUp (heat, max);
		//}
	}

	int zapDeath = -1;
	public int zapDeathTime = 8;
	float zapDeathDamage;

	public override void burn() {
		base.burn ();
		if (life) {
			life.takeDamage (burnDamage, false, false, true);
		}
		float percent = (energy - burningPoint) / (maxHeat - burningPoint);
		//setRange(baseRange * percent);
		if (zapDeath > -1) {
			if (zapDeath < zapDeathTime) {
				//flare(0.1f);
				zapDeath++;
			} else {
				if (zapDeathDamage > 0) {
					life.takeDamage(zapDeathDamage, false, false, true);
				} else {
					//after we get knocked lets turn off the sword and get staggered
					life.takeStagger(100, 20);
					stopBurn();
				}
				zapDeath = -1;
			}
		}
		/*
		if (curCharge > 0) {
			curCharge--;
			energy = burningPoint;
		} else {
			energy = burningPoint - 1;
		}
		*/
	}

	public override bool stopBurn() {
		bool ab = base.stopBurn ();
		if (ab) {
			//life.takeDamage (zapDamage, false);
			will.baseSpeedChange(attackSpeed.x, attackSpeed.y);
			bursting = 0;
			anim.setColor(mainColor[colorNum], 1);
		}
		return ab;
	}

	public override void coolDown(float loss, float min) {
		base.coolDown (loss, min);
	}

	public override void meetBody(GameObject player) {
		base.meetBody (player);

		//normSpeeds = new int[2];
		//normSpeeds [0] = will.minSpeed;
		//normSpeeds [1] = will.maxSpeed;
	}

    public override void getBlade(List<Form>[] newBlade) {
        base.getBlade(newBlade);
		for (int k = 0; k < will.bladeMulti; k++) {
			for (int i = 0; i < will.blade [k].Count; i++) {
				Form f = will.blade [k] [i];
				for (int j = 0; j < f.effects.Count; j++) {
					raiPart rp = (raiPart)f.effects[j];
					rp.color = powerColor[myPlayer.colorNum];
					rp.coreColor = subColor[myPlayer.colorNum];
					rp.tip = detColor[myPlayer.colorNum];
				}
			}
		}
    }

    public override void reset() {
		base.reset ();
		zapDeath = -1;
		for (int k = 0; k < will.bladeMulti; k++) {
			for (int i = 0; i < will.blade [k].Count; i++) {
				Form f = will.blade [k] [i];
				for (int j = 0; j < f.effects.Count; j++) {
					f.effects[j].setRange(baseRange);
				}
			}
		}

	}

    public override void respawn() {
        base.respawn();
		if (anim) {
			animSwordColor(0);
			anim.setColor(mainColor[colorNum], 1);
		}

    }

    public override void ignite() {
		base.ignite();
		flare(0.2f, 5);
		getZapped(zapDamage/2);
    }

    public override void knock(bool flare) {
		base.knock (flare);
		if (burning) {
			//flare (0.5f, 5);
			//reset ();
			//stopBurn();
			getZapped(zapDamage/3);
			zapDeath = 0;
			zapDeathDamage = 0;
			//life.takeStagger(100, 20);
		}
	}

    public override void setColors(int cn) {
		base.setColors(cn);
		colorNum = cn;
		if (anim) {
			for (int i = 0 ; i < 3; i++) {
				anim.setColor(mainColor[colorNum], i);
			}
			anim.setColor(heatColorA, 3);
			animSwordColor(0);
			//anim.setColor(mainColor[cNum], 
		} else {
			Debug.Log("no anim");
		}
    }

    public override void setColors(SwordAnimator an, int cn) {
		anim = an;
        setColors(cn);
    }

    int charge = -1;
    public override void animSwordColor(float fade) {
		if (anim && !burning) {
			int c = (int)(fade * 4);
			if (c != charge) {
				charge = c;
				if (charge == 0) {
					anim.invisColor(3, true);
				} else {
					anim.invisColor(3, false);
					anim.playAnim(3, "raiHiltDeet" + (charge-1));
				}
			}
		}
    }

    public override void revealSelf() {
        if (GM.S.drawSprites && anim) {
			burnAnim();
			for (int i = 0; i < 3; i++) {
				anim.invisColor(i, false);
			}
			newHeat();
		}
    }
}
