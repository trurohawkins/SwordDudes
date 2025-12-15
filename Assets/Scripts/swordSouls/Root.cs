using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Root : SwordSoul {
	public int maxGrowth = 6;
	int startGrowth;
	int grown = 2;
	int curGI = 0;
	int curDI = 0;
	public int growInterval;
	public int dieInterval;
	public GameObject chunk;
	public Color flowerColor;
	public GameObject flowerFx;
	public GameObject flowerSprite;
	swellPart pollen;
	public int flowerSize = 4;
	public float burnDamage;
	public int stillGrowth = 2;
	public float sizeHeatMin = 0.5f;
	public float sizeHeatMax = 2;
	float curSizeHeat = 1;
	rootAnim rAnim;

	 protected override void Awake() {
		base.Awake ();
		fullPower = false;
		grown =  startGrowth = (int)range;
	}

	public override IEnumerator callAction(int delay) {
		if (speed >= speedCounter) {
			if (!burning && grown < maxGrowth && !myPlayer.dead && !myPlayer.dodging) {
				if (curGI >= growInterval) {
					if (will.range == 0) {
						setGrowth(1);
						will.spawnSword();
						will.calculateSwordRange ();
						
						if (myCPU) {
							myCPU.updateMySwordInfo ();
						}
						if (rAnim) {
							animSwordColor(0);
						}
					} else {
						GameObject tmp = Instantiate (chunk, transform.position, transform.rotation, transform);
						Form f = tmp.GetComponent<Form> ();
						bool flower = false;
						int mod = 0;
						if (grown == maxGrowth - 1) {
							flower = true;
							f.width = flowerSize;
							f.length = flowerSize;
							f.squareBody ();
							f.color = flowerColor;
							pollen = Instantiate (flowerFx, transform.position, transform.rotation).GetComponent<swellPart> ();
							pollen.setUp (f, wielder);
							pollen.setColors(flowerColor,  detColor[myPlayer.colorNum]);
							mod = 1;
							if (GM.S.drawSprites && rAnim) {
								f.skin = rAnim.getFlower().gameObject;
								/*
								f.skin = Instantiate (flowerSprite, transform.position, transform.rotation, f.transform);
								f.skin.GetComponent<SpriteRenderer> ().color = flowerColor;
								*/
							}
						} else {
							f.width = 1;
							f.length = 1;
							f.squareBody ();
						}
						f.parent = wielder;

						if (will.spawnBladeChunk (f, 0, myBlade [0].Count + mod, flower)) {
							if (flower) {
								Value v = f.GetComponent<Value> ();
								for (int j = 0; j < stats.Length; j++) {
									v.setMulti (stats[j], multis[j]);
								}
							
								pollen.kindle ();

								if (boomBox.S) {
									if (GM.S.getNumPlayers() > 1) {
										boomBox.S.setBurst (pNum, 1);
									} else {
										boomBox.S.setBurst(0, 1);
										boomBox.S.setBurst(1, 1);
									}
								}
							}
							setGrowth(grown+1);
							if (rAnim) {
								rAnim.flowerHeat(0, heatColorA);
							}
							if (grown == maxGrowth) {
								fullPower = true;
								if (life) {
									life.def = 1;
								}
							}
							myBlade [0].Add (f);
							if (myBlade [0][0].hidden) {
								f.erasePresence (true);
							}
							will.calculateSwordRange ();
							if (myCPU) {
								myCPU.updateMySwordInfo ();
							}
							will.calcPos();
						}
					}
					curGI = 0;
					//Map.S.spawnForm (tmp, newGrowth.centerPoint.x, newGrowth.centerPoint.y);
				} else if (curGI >= 0) {
					int cg = curGrow;
					if (will.brace != 1) {
						cg += 2;
					}
					curGI += cg;
				}
			}
		}
		StartCoroutine(base.callAction (delay));
		yield return new WaitForFixedUpdate ();

	}

	int curGrow = 1;

	public override bool startBurn() {
		bool alreadyBurning = burning;
		if (!burning) {
			curDI = 0;
			burning = true;
		}
		return alreadyBurning;
	}

	public override void burn() {
		if (myBlade[0].Count > 0) {
			if (curDI == dieInterval) {
				burnPiece();
			} else {
				curDI++;
			}
		}
	}

	void burnPiece() {
		loseSegment();
		if (life) {
			life.takeDamage(burnDamage, false, false, true);
		}
	}

	public override bool stopBurn() {
		bool alreadyBurning = burning;
		if (burning) {
			curGI = 0;
			burning = false;
		}
		return alreadyBurning;
	}

	public override void knock(bool flare) {
		if (myBlade[0].Count > 0) {
			burnPiece();
		}
	}

	void loseSegment() {
		//Debug.Log ("myBlade count " + myBlade [0].Count);
		Form dying = myBlade [0] [myBlade [0].Count - 1];
		myBlade [0].Remove (dying);
		for (int i = 0; i < dying.effects.Count; i++) {
			dying.effects [i].setUp (body, wielder); //puts effects on sword, so it still has connection to map, so it can disperse properly
			dying.effects [i].smother ();
			//dying.effects [i].disperse ();
			dying.effects [i].transform.parent = transform;
		}
		dying.die ();
		setGrowth(grown-1);
		will.calculateSwordRange ();
		if (myCPU) {
			myCPU.updateMySwordInfo ();
		}
		curDI = 0;
		if (fullPower) {
			if (pollen) {
				Destroy(pollen);
				pollen = null;
			}
			fullPower = false;
			if (boomBox.S) {
				if (GM.S.getNumPlayers() > 1) {
					boomBox.S.resetBurst (pNum);
				} else {
					boomBox.S.resetBurst(0);
					boomBox.S.resetBurst(1);
				}
			}
		}
	}

	public override void reset() {
		if (saveGrowth == 420) {
			while (grown > startGrowth) {
				Form dying = myBlade [0] [myBlade [0].Count - 1];
				myBlade [0].Remove (dying);
				for (int i = 0; i < dying.effects.Count; i++) {
					dying.effects [i].setUp (body, wielder); //puts effects on sword, so it still has connection to map, so it can disperse properly
					dying.effects [i].smother ();
					//dying.effects [i].disperse ();
					dying.effects [i].transform.parent = transform;
				}
				dying.die ();
				setGrowth(grown-1);
			}
			will.calculateSwordRange ();
			if (myCPU) {
				myCPU.updateMySwordInfo ();
			}
		}
		if (life) {
			life.def = 1;
		}
		curGrow = 1;
		base.reset ();
	}

	public override void meetBody(GameObject player) {
		base.meetBody (player);
		myPlayer.setSpeedBoost(-speedBoost);//slowUp (speedBoost);
		playerStopMove();
		soulColorA = mainColor[myPlayer.colorNum];
		flowerColor = subColor[myPlayer.colorNum];
		if (anim) {
			rAnim = anim.GetComponent<rootAnim>();
			setGrowth(grown);
		}
		if (pAnim) {
			pAnim.setDodgeColor(subColor[colorNum]);
		}
	}

	protected override void newHeat() {
		fade = (energy - climate) / (burningPoint - climate);
		if (fade >= 0) {
			int flower = 0;
			if (grown == maxGrowth) {
				flower = 1;
			}
			Color cola = Color.Lerp (soulColorA, heatColorA, fade);
			Color colb = Color.Lerp (flowerColor, heatColorA, fade);
			for (int k = 0; k < will.bladeMulti; k++) {
				for (int i = 0; i < will.blade[k].Count/* - flower*/; i++) {
					if (!GM.S.drawSprites) {
						will.blade [k][i].changeColorN (cola);
					}
					if (will.blade[k][i].transform.childCount > 0) {
						if (i < will.blade [k].Count - flower) {
							will.blade [k] [i].transform.GetChild (0).GetChild (0).GetChild (0).GetComponent<SpriteRenderer> ().color = cola;//Color.Lerp (soulColor, heatColor, fade);
							//will.blade [k] [i].transform.GetChild (0).GetChild (0).GetChild (1).GetComponent<SpriteRenderer> ().color = colb;
						} else {
							will.blade[k][i].transform.GetChild(0).GetComponent<SpriteRenderer>().color = colb;
						}
					}
				}
			}
			if (pAnim) {
				pAnim.lerpHeat(fade);
			}
			if (anim) {
				animSwordColor(fade);
			}
			if (ui) {
				float fullFade = Mathf.Clamp((energy - baseEnergy) / (maxHeat - baseEnergy), 0, 1);
				ui.setHeat(fade, fullFade);
			}
		}
	}

    public override void animSwordColor(float fade) {
		if (will.range != 0) {
			base.animSwordColor(fade);
		}
		if (rAnim) {
			rAnim.flowerHeat(fade, heatColorA);
		}
    }

    public override void playerStopMove() {
        base.playerStopMove();
		curGrow = stillGrowth;
		/*
		if (life) {
			if (!fullPower) {
				life.def = 0.5f;
			} else {
				life.def = 1;
			}
		}
		*/
    }

    public override void playerStartMove() {
        base.playerStartMove();
		curGrow = 1;
		/*
		if (life) {
			life.def = 1;
		}
		*/
    }

    public override void dodge() {
        base.dodge();
		if (pollen) {
			pollen.smother();
		}
    }

    public override void dodgeOver() {
        base.dodgeOver();
		if (pollen) {
			pollen.kindle();
		}
    }

	int saveGrowth = 420;
	
    public override void saveState() {
        base.saveState();
		saveGrowth = grown;
    }

    public override void respawn() {
		saveGrowth = 420;
		if (pollen) {
			pollen.kindle();
		}
        base.respawn();
    }

    public void setGrowth(int g) {
		will.range = grown = g;
		float perc = (float)g / (float)maxGrowth;
		Debug.Log("new length at " + perc + "%");
		int min = (int)Mathf.Lerp(attackSpeed[0], chargedSpeed[0], 1.0f - perc);
		int max = (int)Mathf.Lerp(attackSpeed[1], chargedSpeed[1], 1.0f - perc);
		will.baseSpeedChange(min, max);
		curSizeHeat = Mathf.Lerp(sizeHeatMin, sizeHeatMax, perc);
		if (rAnim) {
			//Debug.Log("set growth " + g);
			rAnim.setGrowth(g);
		}
	}

    public override void heatUp(float heat, float max) {
        base.heatUp(heat * curSizeHeat, max);
    }

    public override void setColors(int cNum) {
        base.setColors(cNum);
		if (anim) {
			setColors(anim, cNum);
		}
    }

    public override void setColors(SwordAnimator an, int cn) {
		if (an != null) {
			an.setColor(mainColor[cn], 0);
			an.setColor(powerColor[cn], 1);
			rootAnim ra = an.GetComponent<rootAnim>();
			if (ra) {
				ra.setFlowerColors(cn);
			}
		}
    }


    /*
GameObject tmp = Instantiate (chunk, transform.position, transform.rotation);
Form f = tmp.GetComponent<Form> ();
myBlade [0].Add (f);
f.transform.parent = transform;
f.parent = wielder; //perhaps needed; changed so that when parrying the other blade can get a reference to the playerSword script
f.width = Mathf.Max(1, attackWidth);
f.length = Mathf.Max(1, attackLength);
f.squareBody ();
Vector2Int p = will.calculateSwordPos (0,blade[0].Count - 1, will.curPos);
Map.S.spawnForm (tmp, p.x, p.y);
if (!f.spawned) {
    Debug.Log ("strike blocked at " + i);
    f.die ();
    blade[k].Remove (f);
    //endStrike (0);
    break;
} else {
    f.name = name + "-chunk" + k + "-" + i;
    f.color = soulColor;//normColor;
    if (fx) {
        Particle x = Instantiate (fx, f.transform).GetComponent<Particle> ();
        x.setUp (f, wielder.GetComponent<Form>());
    }
    Value v = f.GetComponent<Value> ();
    v.setValue ("damage", damage.x, damage.y);
    v.setValue ("stagger", stagger.x, stagger.y);
    v.setValue ("stagTime", staggerTime, staggerTime);
    v.setValue ("knockBack", knockBackDamage.x, knockBackDamage.y);
}


Form newGrowth = tmp.GetComponent<Form> ();
newGrowth.squareBody ();
myBlade[0].Add (newGrowth);
will.calcPos ();
curGI = 0;
*/
}
