 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gin : SwordSoul {

	public int minFxRange = 1;
	public int maxFxRange = 6;
	public bool oldSchool;
	public float burnDamage;
	public float basePercent = -0.2f;
	public float igniteDamage;
	Particle bodyFX;
	ginAnim gAnim;

    protected override void Awake() {
        base.Awake();
		if (anim) {
			gAnim = anim.GetComponent<ginAnim>();
		}
    }

    public override void burn() {
		if (!oldSchool && life) {
			life.takeDamage (burnDamage, false);
		}
	}

    public override void heatUp(float heat, float max) {
		if (!oldSchool) {
			if ((!will.swinging || will.beingKnocked) && swingCounter == 0) {
				base.heatUp(heat, max);
				bodyFX.curRange = Mathf.Max(0, maxFxRange * (basePercent + ((energy - baseEnergy) / (climate - baseEnergy))));
			}
		}
    }

    public override void meetBody(GameObject player) {
		base.meetBody(player);
		will.singleStrike = true;
		will.readyToSwing = false;
		for (int k = 0; k < will.bladeMulti; k++) {
			for (int i = 0; i < will.blade [k].Count; i++) {
				Form f = will.blade [k][i];
				for (int j = 0; j < f.effects.Count; j++) {
					//Debug.Log ("changing effect color");
					//f.color = powerColor [pNum];
				}
			}
		}
		if (!oldSchool) {
			bodyFX = Instantiate (fx, wielder.transform).GetComponent<Particle> ();
			bodyFX.setUp(wielder, null);
			bodyFX.kindle();
			bodyFX.curRange = 0;//mabodyFXFbodyFXRange;
			bodyFX.color = powerColor[myPlayer.colorNum];
			bodyFX.sheathed = false;
		} else {
			for (int k = 0; k < will.bladeMulti; k++) {
				for (int i = 0; i < will.blade[k].Count; i++) {
					for (int j = 0; j < will.blade[k][i].effects.Count; j++) {
						will.blade[k][i].effects [j].kindle ();
					}
				}
			}
		}
	}

	public override void getBlade(List<Form>[] newBlade) {
		//base.getBlade(newBlade);
		myBlade = newBlade;
		if (fx) {
			for (int k = 0; k < will.bladeMulti; k++) {
				for (int i = 0; i < will.blade [k].Count; i++) {
					Form f = will.blade [k] [i];
					Particle x = Instantiate (fx, f.transform).GetComponent<Particle> ();
					//Debug.Log("color number: " + cNum);
					x.color = powerColor[myPlayer.colorNum];
					x.endColor = subColor[myPlayer.colorNum];
					x.baseAlpha = 0.5f;
					x.setUp (f, body);
				}
			}
			setParticleBaseRange();
		}
		will.sheathed = true;
		will.erasePresence(false);
		destRoto = will.curPos;
	}

    public override void setColors(int pNum) {
        base.setColors(pNum);
		if (anim) {
			setColors(anim, pNum);
		}
    }

    public override void setColors(SwordAnimator an, int cn) {
        //an.setColor(heatColorA, 0);
		an.setColor(powerColor[cn], 0);
		//an.setColor(soulColorB, 1);
		an.setColor(detColor[cn], 1);
		//an.setColor(heatColorA, 2);
		an.setColor(powerColor[cn], 2);
    }

    public override void startSwingInput(int firstInput) {
		if (will.canSwing()) {
			if (anim) {
				destRoto = firstInput;
				//anim.invisColor(2, false);
				float power = getPower();
				Color cola = Color.Lerp(soulColorA, heatColorA, power);
				anim.setColor(cola, 2);
			}
		}
    }

	float destRoto;
	float curRoto;
	float inputSpeed = 15;

    public override IEnumerator callAction(int delay) {
		if (gAnim && ic) {//fix later if issues, see below error notes, gAnim was in here already
			//if (gAnim.curRoto, destRoto) {
			if (will.isHidden() && !will.beingKnocked) {
				if (destRoto != gAnim.curRoto) {
					if (will.beingKnocked) {
						Debug.Log("knock affecting dest roto");
					}

					if (ic == null) {
						Debug.LogError(name + " no ic ");
					}
					if (gAnim == null) {
						Debug.LogError(name + " no gAnim");
					}
					// null reference in here somewhere
					if (ic.findShortestDist(gAnim.curRoto, destRoto) > inputSpeed) {
						anim.setRotation(ic.saneAddition(gAnim.curRoto, inputSpeed, ic .findShortestDir(gAnim.curRoto, destRoto)));
					} else {
						anim.setRotation(destRoto);
					}
					will.curPos = gAnim.curRoto;
				}
			}
		}
        return base.callAction(delay);
    }

	float getPower() {
		if (curGear == 0) {
			return 0;
		}
		float power = Mathf.Max(0, basePercent + (energy - baseEnergy) / (climate - baseEnergy));
		if (burning) {
			power += basePercent * -2;
		}
		return power;
	}

    //public override bool startBurn()
    public override void swingPauseBegin() {
		if (will.isHidden()) {
			if (will.revealSelf()) {
				//Debug.LogError("start swing");
				//Debug.LogError("reveal " + will.beingKnocked);
				float increase = getPower();
				if (!oldSchool && increase != 0) {
					if (gAnim) {
						gAnim.setSize(increase);
					}
					coolDown(energy - baseEnergy, baseEnergy);
					bodyFX.curRange = 0;
				}
				Color cola = Color.Lerp (soulColorA, heatColorA, increase);
				Color colb = Color.Lerp (soulColorB, heatColorB, increase);
				for (int k = 0; k < will.bladeMulti; k++) {
					for (int i = 0; i < will.blade [k].Count; i++) {
						Form f = will.blade [k] [i];
						if (!oldSchool) {
							Value v = f.GetComponent<Value> ();
							if (!anim) {
								f.transform.GetChild (0).GetChild (0).GetChild (0).GetComponent<SpriteRenderer> ().color = cola;//Color.Lerp (soulColor, heatColor, fade);
							} else {
								anim.setColor(cola, 0);
								anim.setColor(cola, 2);
							}
							//f.transform.GetChild (0).GetChild (0).GetChild (1).GetComponent<SpriteRenderer> ().color = heatColorA;//colb;
							for (int j = 0; j < stats.Length; j++) {
								float newMul = Mathf.Lerp(1, multis[j], increase);
								v.setMulti (stats[j], newMul);
							}
						}
						for (int j = 0; j < f.effects.Count; j++) {
							if (!oldSchool) {// && increase != 0) {
								f.effects[j].color = cola;
								f.effects[j].curRange = Mathf.Lerp (minFxRange, maxFxRange, increase);

							}
							f.effects [j].sheathed = false;
						}
					}
				}
				//Debug.Log(increase + " -> " + will.blade[0][0].GetComponent<Value>().multi[0]);
			}

		}
		//return base.startBurn();
	}

    public override bool canSwing() {
        return swingCounter == 0;
    }

    int swingCounter = 0;

    public override void aliveFrame() {
        if (swingCounter > 0) {
			swingCounter--;
			if (swingCounter == 0) {
				will.erasePresence(false);
				for (int k = 0; k < will.bladeMulti; k++) {
					for (int i = 0; i < will.blade [k].Count; i++) {
						Form f = will.blade [k] [i];
						for (int j = 0; j < f.effects.Count; j++) {
							f.effects [j].erasePresence();//smother ();
							f.effects[j].sheathed = true;
						}
					}
				}
				destRoto = will.curPos;
			}
		}
    }

    //public override bool stopBurn()
    public override void stopSwinging() {
		swingCounter = swingPause;
    }

    public override void knock(bool flaring) {

		//destRoto = will.anglePath[will.anglePath.Count-1];
		Debug.Log(will.anglePath.Count + " " + destRoto);
        //will.clearPath(1);
		will.erasePresence(false);
		if (anim) {
			anim.setColor(soulColorA, 2);
		}
		for (int k = 0; k < will.bladeMulti; k++) {
			for (int i = 0; i < will.blade [k].Count; i++) {
				Form f = will.blade [k] [i];
				for (int j = 0; j < f.effects.Count; j++) {
					f.effects [j].erasePresence();//smother ();
					f.effects[j].sheathed = true;
				}
			}
		}
    }

    public override void erasePresence(bool dodging) {
        if (anim) {
			if (dodging) {
				anim.setAlpha(0);
			} else {
				anim.invisColor(0, true);
				anim.invisColor(2, true);
			}
		}
    }

    public override bool burnAnim() {
		return burning;
    }
    public override void takeHit() {
		//Debug.Log ("took hit");
		if (!oldSchool) {
			return;
		}
		float increase = 0.1f + ((life.maxHealth - life.health) / life.maxHealth);
		Color cola = Color.Lerp (soulColorA, heatColorA, increase);
		Color colb = Color.Lerp (soulColorB, heatColorB, increase);
		if (pAnim) {
			pAnim.lerpHeat(increase);
		}
		for (int k = 0; k < will.bladeMulti; k++) {
			for (int i = 0; i < myBlade [k].Count; i++) {
				Form f = myBlade [k] [i];
				Value v = f.GetComponent<Value> ();
				f.transform.GetChild (0).GetChild (0).GetChild (0).GetComponent<SpriteRenderer> ().color = cola;//Color.Lerp (soulColor, heatColor, fade);
				f.transform.GetChild (0).GetChild (0).GetChild (1).GetComponent<SpriteRenderer> ().color = colb;
				for (int j = 0; j < stats.Length; j++) {
					float newMul = Mathf.Lerp(1, multis[j], increase);
					v.setMulti (stats[j], newMul);
				}
				for (int j = 0; j < f.effects.Count; j++) {
					f.effects[j].curRange = Mathf.Lerp (1, maxFxRange, increase);
				}
			}
		}
		if (boomBox.S) {
			boomBox.S.setBurst (pNum, increase);
		}
	}

    public override void ignite() {
        base.ignite();
		if (life) {
			Debug.Log("ignite damage");
			life.takeDamage(igniteDamage, false);
		}
    }

    public override void reset() {
		bursting = 0;
		will.erasePresence(false);
		StopAllCoroutines ();
		if (boomBox.S) {
			boomBox.S.resetBurst (pNum);
		}
		if (!oldSchool) {
			bodyFX.curRange = 0;
		}
		for (int k = 0; k < will.bladeMulti; k++) {
			for (int i = 0; i < will.blade [k].Count; i++) {
				Form f = will.blade [k][i];
				if (!GM.S.drawSprites) {
					f.transform.GetChild (0).GetChild (0).GetChild (0).GetComponent<SpriteRenderer> ().color = soulColorA;//Color.Lerp (soulColor, heatColor, fade);
					f.transform.GetChild (0).GetChild (0).GetChild (1).GetComponent<SpriteRenderer> ().color = soulColorB;
				}
				for (int j = 0; j < f.effects.Count; j++) {
					f.effects [j].curRange = 0;
				}
			}
		}
	}

	public override void dodgeOver() {
		base.dodgeOver();
		if (anim) {
			anim.invisColor(1, false);
		}
	}

    public override void getStaggered() {
        will.erasePresence(false);
		if (anim) {
			anim.setAlpha(0);
		}
    }

    public override void staggerOver() {
		if (anim) {
			anim.setAlpha(1);
		}
	}

    public override void respawn() {
        will.erasePresence(false);
		coolDown(energy - baseEnergy, baseEnergy);
    }
}
