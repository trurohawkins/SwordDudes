using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swell : SwordSoul {

	public int growInterval;
	int curGI = 0;
	public int maxGrowth = 5;
	int growth = 0;
	public int minSize = 1;
	Form chunk;
	public int[] chargedSpeeds;
	public int[] normSpeeds;
	int swellDamage;
	public int crawl;
	public int weightIncrease;
	public int growRange = 8;
	public float taper = 2.5f;
	public int spacing = 15;
	public int grownHeight = 21;
	public int lastSeconds = 10;
	public float knockDamage;
	int lsCounter = -1;
	float lastDamage;
	public int dodgeHeatTimes = 3;

	public override bool startBurn() {
		/*
		if (!burning) {
			if (boomBox.S) {
				//boomBox.S.setBurst (pNum, 1);
			}
			burning = true;
			curGI = growInterval - 1;
			for (int k = 0; k < will.bladeMulti; k++) {
				for (int i = 0; i < will.blade [k].Count; i++) {
					Form f = will.blade [k] [i];
					for (int j = 0; j < f.effects.Count; j++) {
						f.effects [j].kindle ();
					}
				}
			}
			will.setBladeHeight (21);
		}
		//StartCoroutine (growing ());
		return true;
		*/
		//will.setBladeHeight (21);
		return base.startBurn();
	}
	
	bool readyToGrow;
	/*
	public override void burn()
    {
		if (chunk && chunk.width <= maxGrowth) {
			if (curGI == growInterval) {
				// take damage upon reaching interval
				if (!readyToGrow) { 
					if(!life.takeDamage (swellDamage, false)) {
						return; //we dead
					}
					readyToGrow = true;
				}
				//if we grow we reset, otherwise, we wait til next interval to grow, but we dont take damage when it happens
				if (will.growBlade (minSize, maxGrowth)) {
					curGI = 0;
					statGrowth ();
					readyToGrow = false;
					//Debug.Log ("successful growth");
				} else {
					curGI = growInterval - 30;
				}
			} else if (curGI >= 0) {
				curGI++;
			}
		}
	}
			*/
	
	public override IEnumerator callAction(int delay) {
		if (lsCounter > lastSeconds) {
			life.takeDamage(lastDamage, false);
			lsCounter = -1;
		} else if (lsCounter >= 0) {
			lsCounter++;
		} else if (!burning) {
			StartCoroutine(base.callAction (delay));
		} else if (chunk) {// && growth < maxGrowth) {
			if (curGI == growInterval) {
				deathDamage(swellDamage);
				grow(true);
			} else if (curGI >= 0) {
				curGI++;
			}
		}
		yield return new WaitForFixedUpdate ();

	}

	void deathDamage(int sd) {
		if(sd >= life.health) {//!life.takeDamage (swellDamage, false)) {
			Debug.Log("we died");
			//yield return new WaitForFixedUpdate (); //we dead
			lsCounter = 0;
			lastDamage = sd;
		} else {
			life.takeDamage(sd, false);
		}
	}

	void grow(bool resetGI) {
		bool good = true;
		if (growth == 1) {
			while(will.range < growRange) {
				GameObject tmp = Instantiate (will.getChunk(), transform.position, transform.rotation, transform);
				Form f = tmp.GetComponent<Form> ();
				f.width = 1;
				f.length = 1;
				f.squareBody ();
				f.parent = wielder;
				if (will.spawnBladeChunk (f, 0, myBlade [0].Count, false)) {
					myBlade [0].Add (f);
					if (myBlade [0][0].hidden) {
						f.erasePresence (true);
					}
					will.range++;
					will.calculateSwordRange ();
					if (myCPU) {
						myCPU.updateMySwordInfo ();
					}
					f.transform.GetChild (0).GetChild (0).GetChild (0).GetComponent<SpriteRenderer> ().color = heatColorA;
					for (int i = 0; i  < f.effects.Count; i++) {
						f.effects[i].kindle();
					}
				} else {
					good = false;
				}
			}
		} else if (growth == 0) {
			will.setTaperAngle(spacing, taper);
		} else {
			if(!will.growBlade (minSize, maxGrowth)) {
				good = false;
			}
		}
		if (resetGI) {
			if (good) {
				curGI = 0;
				statGrowth ();
				growth++;
				//Debug.Log ("successful growth");
			} else {
				curGI = growInterval - 30;
			}
		} else {
			if (good) {
				growth++;
				statGrowth();
			}
		}
	}

    public override void ignite() {
		if (!burning) {
			base.ignite();
		} else {
			deathDamage((int)(swellDamage * 0.75f));
			grow(false);
		}
    }

    void statGrowth() {
		float sIncrease = (float)chunk.width / maxGrowth;
	//	will.minSpeed = (int)Mathf.Lerp (normSpeeds [0], chargedSpeeds [0], sIncrease);
	//	will.maxSpeed = (int)Mathf.Lerp (normSpeeds [1], chargedSpeeds [1], sIncrease);
		will.minSpeed = (int)Mathf.Lerp (attackSpeed.x, chargedSpeed.x, sIncrease);
		will.maxSpeed = (int)Mathf.Lerp (attackSpeed.x, chargedSpeed.y, sIncrease);
		myPlayer.slowUp ((int)(crawl * sIncrease));

		if (sIncrease > 0.5) {
			//will.bladeSpacing = 2f;
		}
		will.setWeight(will.getWeight() + weightIncrease);
		for (int k = 0; k < will.bladeMulti; k++) {
			for (int i = 0; i < myBlade [k].Count; i++) {
				Form f = myBlade [k] [i];
				Value v = f.GetComponent<Value> ();
				for (int j = 0; j < stats.Length; j++) {
					float newMul = Mathf.Lerp(1, multis[j], sIncrease);
					v.setMulti (stats[j], newMul);
				}
			}
		}
		if (boomBox.S) {
			boomBox.S.setBurst (pNum, sIncrease);
		}
	}

    public override void dodge() {
        base.dodge();
		if (!burning) {
			for (int i = 0; i < dodgeHeatTimes; i++) {
				heatUp(heatSpeed, climate);
			}
		}
    }

    public override void knock(bool flaring) {
        base.knock(flaring);
		//life.takeDamage(knockDamage, true);
    }

    public override void meetBody(GameObject player) {
		base.meetBody (player);
		//normSpeeds = new int[2];
		//normSpeeds [0] = will.minSpeed;
		//normSpeeds [1] = will.maxSpeed;
		swellDamage = (int)(life.maxHealth * (1f / (maxGrowth - 1)));
	}

	public override void getBlade(List<Form>[] newBlade) {
		base.getBlade (newBlade);
		chunk = myBlade[0] [0];
	}

	public override void reset() {
		//will.setBladeHeight (15);
		//will.setGrowth (minSize);
		will.setWeight(swordWeight);
		will.setTaperAngle(0, 0);
		growth = 0;
		readyToGrow = false;
		curGI = 0;

		base.reset ();
		//will.minSpeed = attackSpeed.x;
		//will.maxSpeed = attackSpeed.y;
		//will.bladeSpacing = 1;
		myPlayer.slowUp (0);


	}

    public override void respawn() {
        will.setGrowth (minSize);
		while (will.range > range) {
			Form dying = myBlade [0] [myBlade [0].Count - 1];
			myBlade [0].Remove (dying);
			dying.die ();
			will.range--;
		}
		will.calculateSwordRange ();
		if (myCPU) {
			myCPU.updateMySwordInfo ();
		}
    }
}
