using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class defense : Purpose {

	public float health;
	public float maxHealth;
	public float constitution = 1;
	public float conHits = 5;
	//float conDamage;
	//float maxCon = 15;
	//public int conDefense;
	public float staggerThreshold = -1;
	public float recoverySpeed;
	public float weight = -1;
	public float bodyHeat = 5;
	float recoveryTime;
	public float backStabDefense;
	public bool dodging;
	public float[] poises;
	public float def = 1;
	public float kDef = 1;
	public float sDef = 1;
	public bool dead = false;
	public GameObject hBar;
	//[SyncVar]
	public bool invulnerable;

	float poiseDefense;
	bool activated;
	float curStagger;
	Player human;
	Transform healthBar;
	Image hpSpri;
	Text lives;
	public GameObject damageText;
	public List<Purpose> resumeAction;
	public GameObject blood;
	public GameObject bloodMass;
	protected Form bloodPuddle;
	public GameObject corpse;
	public int corpseHeight;
	//combat fighter;
	FighterAnimator myGraphics;
	int baseHeight;
	protected int team = 1;
	public bool swordStuck = false;
	Brain myBrain;

	 protected override void Awake(){
		base.Awake ();
		GameObject c = GameObject.FindGameObjectWithTag ("canvas");
		if (c) {
			canvas = c.transform;
		}
		lastHits = new List<string> ();
		resumeAction = new List<Purpose> ();
		self.addAction (this);
		baseHeight = self.height;
		active = true;
		//active = false;
	//	myGraphics = gameObject.GetComponentInChildren<Animator> ();

		human = gameObject.GetComponent<Player> ();
		myBrain = gameObject.GetComponent<Brain>();
		if (blood && bloodMass) {
			bloodPuddle = Instantiate(bloodMass, transform).GetComponent<Form>();
		}
		if (!GM.S) {
			Debug.LogWarning(name + " cant find the GM");
		}
		if (GM.S.drawSprites && transform.childCount > 0) {
			myGraphics = gameObject.transform.GetChild (0).GetComponent<FighterAnimator> ();
		}
		if (poises.Length == 0) {
			poiseDefense = 1;
		}
		setStagger(staggerThreshold);
		recoveryTime = 1 / recoverySpeed;
		//conDamage = (maxCon - 1) / conHits;
		maxHealth = health;
	//	activate ();
	}

	public void activate(){
		if (!activated) {
			//Debug.Log ("activating " + name);
			if (!self.active) {
				self.active = true;
				Map.S.forms.Add (self);
				activated = true;
			}
			if (!flying/* && brain*/ && !human) {
				setActions (false);
				//resumeAction = self.activeAction;
				//self.activeAction.Clear ();
			}
			active = true;
		}
	}

	void setActions(bool active) {
		for (int i = 0; i < self.activeAction.Count; i++) {
			if (self.activeAction [i] != this) {
				self.activeAction [i].active = active;
			} else {
				self.activeAction [i].active = !active;
			}
		}
	}

	void deactivate(){
		self.active = false;
		//self.activeAction = resumeAction;
		setActions (true);
		activated = false;
		//Map.S.forms.Remove(self);
	}

	public override IEnumerator callAction(int delay){
		if (!dead) {
			if (flying) {
				knockBack ();
			}
			stagger ();
			if (activated && !flying && !stagged && flushTime == 0 && curInv == 0) {
				//deactivate ();
			}
			flushHits ();
			invulFlash ();
			musicFlick ();
		}
		/*
		if (dead) {
			//dying ();
		} else if (speedCounter >= speed) {
			//Debug.Log ("going");
			if (flying) {
				knockBack ();
			}
			//if (stagged) {
				stagger ();
			//} 
			speedCounter = 0;
			if (activated && !flying && !stagged && flushTime == 0 && curInv == 0) {
				//deactivate ();
			}
			flushHits ();
			invulFlash ();
		} else {
			speedCounter++;
		}
		*/
		yield return new WaitForFixedUpdate ();
	}

	public void setDefenses(float d, float k, float s) {
		def = d;
		kDef = k;
		sDef = s;
	}

	public void setPoise(int poise){
		if (poise < poises.Length) {
			poiseDefense = poises [poise];
		} else {
			Debug.LogError (poise + " is out of range for poises of " + gameObject.name);
		}
	}

	int flushTime = 0;

	void flushHits() {
		if (flushTime > 0) {
			flushTime--;
		} else {
			lastHits.Clear ();
		}
	}

	public float staggerVul = 1;

	protected float calculateDamage(Form enemy){
		Value v = enemy.gameObject.GetComponent<Value> ();
		float dam = (v.getValue("damage") * def);// * poiseDefense) + checkBackStab (enemy);
		if (stagged) {
			dam *= staggerVul;
		}
		return dam;
	}

//	float lastTime = 0;
	List<string> lastHits;

	public override void callAction(Form enemy, int state, int x, int y){
		if (!enemy) {
			Debug.Log(name + " was called by null enemy ");
			return;
		}
		if (!dead && !invulnerable) {
			Value v = enemy.GetComponent<Value> ();
			attackChunk ac = enemy.GetComponent<attackChunk>();
			circleAttack ca = null;
			if (ac) {
				ca = ac.getAttack();
			}
			if (v && !v.onTeam(team)) {
				if (ca) {
					ca.getSoul().hitBody(this);
				}
				float knock = 0;
				Vector2 knockD = Vector2.zero;
				float damage = 0;
				float stagger = 0;
				if (/*Time.time - lastTime < 0.1f && */lastHits.Contains (enemy.name)) {
					//Debug.LogError ("too many hits at once from " + enemy.name + " hp:" + health); // check the way enter actions get called
				} else {
					//Debug.Log (name + " hit " + health);
					//lastTime = Time.time;
					lastHits.Add (enemy.name);
					flushTime = 39;
					//StopCoroutine ("flushHits");
					//StartCoroutine ("flushHits");
				}
				if (boomBox.S) {
					boomBox.S.upTempo (1);
				}
				
				if (weight > -1) {
					//if (v.types [i] == "knockBack") {
					knock = v.getValue ("knockBack");
					if (knock != -1) {
						float knockPower = (knock / weight) * kDef;
						if (knockPower > 0) {
							knockD = self.centerPoint - enemy.centerPoint;
							if (ca) {
								knockD = ca.getVelocity(ac.blade, ac.segment);
							}
							getKnocked (knockD, knockPower, 1);
						} else {
							knockAttack(enemy);
						}
					
					}
				} else {
					knockAttack(enemy);
				}
				if (v.getValue ("damage") != -1 && def > 0) {
					if (enemy.id == 78) {
						Debug.LogError(name + " taking damage from " + enemy.name);
					}
					if (!dodging) {
						attackChunk enAt = enemy.gameObject.GetComponent<attackChunk> ();
						if (enAt) {
							//circleAttack ca = enAt.getAttack ();
							//Vector2Int bodyPos = ca.getBody().centerPoint;
							//Debug.Log ("hit at " + ca.curPos + " their pos is " + bodyPos + " they were swing? " + ca.swinging);// + " " + enemy.name);
							//Debug.Log("our distance: " + Vector2Int.Distance(bodyPos, self.centerPoint) + " the angle " + angleBetween(bodyPos, self.centerPoint));
						}
						//Debug.Log (enemy.name + " " + enemy.centerPo
						damage = calculateDamage(enemy);
						if (!takeDamage (damage, true)) {
							//Debug.Log("die");
						}
					}
				}
				if (staggerThreshold > -1) {
					//float stag = v.getValue ("stagger");
					bool newStag = stagged;
					stagger = v.getValue ("stagger") * sDef;
					takeStagger(stagger, v.getValue ("stagTime"));
					if (Grader.S && !newStag && stagged) {
						Grader.S.stagger(enemy);
					}
				}
				if (damage != 0) {
					damageUI(-damage, -knockD);
				}
				if (stagger != 0 || knock != 0 || damage != 0) {
					if (ca) {
						ca.dealDamage(enemy.centerPoint, transform);
					}
				}
				if (Grader.S) {
					Grader.S.makeHit(enemy, damage, knock);
				}
			}
		}
	}

	float angleBetween(Vector2Int pos1, Vector2Int pos2) {
		Vector2 dir = new Vector2(pos2.x - pos1.x, pos2.y - pos1.y).normalized;
		float x = (int)(dir.x * 1000) - pos1.x;
		float y = (int)(dir.y * 1000) - pos2.y;
		return Mathf.Atan2 (y, x) * Mathf.Rad2Deg;
	}

	public int invulTime = 41;
	int curInv = -1;
	float baseAlpha = 0.2f;

	void invulnerate(bool invul) {
		if (invul) {
			invulnerable = true;
		}
		if (curInv < 0) {
			curInv = invulTime;

		}	
	}

    void invulFlash() {
		if (curInv > 0) {
			if (curInv % 4 == 0) {
				if (self.color.a == baseAlpha) {
					self.color.a = 1;
					if (myGraphics) {
						myGraphics.setAlpha (1);
					}
				} else {
					self.color.a = baseAlpha;
					if (myGraphics) {
						myGraphics.setAlpha (baseAlpha);
					}
				}
				//self.color.a = Mathf.Min(1, baseAlpha + ((self.color.a + 1) % 2));
				if (!GM.S.drawSprites) {
					self.changeColorN (self.color);
				}

			}
			curInv--;
		} else if(curInv == 0) {
			stopInvulnerate ();
		}
	}

	void stopInvulnerate() {
		curInv = -1;
		if (myGraphics) {
			myGraphics.setAlpha (1);
		} else {
			self.color.a = 1;
			self.changeColorN (self.color);
		}
		if (invulnerable) {
			invulnerable = false;
		}
	}

	public int flickTime = 31;
	int curFlickTime;
	public int baseFlickSpeed = 10;
	public int flickDecrement = 2;
	int flickSpeed = 1;
	int curFlick;

	void startFlickering() {
		if (curFlickTime < 0) {
			flicker(0);
			flickSpeed = baseFlickSpeed;
			curFlickTime = flickTime;
		}
		
	}

	void musicFlick() {
        if (curFlickTime > 0) {
            if (curInv % flickSpeed == 0) {
                flicker((curFlick + 1) % 2);
                if (flickSpeed - flickDecrement > 1) {
                    flickSpeed -= flickDecrement;
                } else {
                    flickSpeed = 1;
                }
            }
            curFlickTime--;
        } else if (curFlickTime == 0) {
			stopFlicker();
		}
    }

	void flicker(int value) {
		if (human && boomBox.S) {
			curFlick = value;
			if (GM.S.getNumPlayers() > 1) {
				boomBox.S.setFlicker(human.playerNum, value);
			} else {
				boomBox.S.setFlicker(0, value);
				boomBox.S.setFlicker(1, value);
			}
		}
	}

	void stopFlicker() {
		flicker(1);
		curFlickTime = -1;
	}
	public void takeStagger(float stag, float time) {
		if (stag > 0 && time > 0 && !stagged ) {
		//	Debug.Log ("hit with " + stag);
			if (curStagger > stag) {
				setStagger(curStagger - (int)stag);
			} else {
				setStagger(0);
				stagged = true;
				sTimer = time;//v.getValue ("stagTime");
				if (human) {
					human.getStaggered ();
				}
				//Debug.Log("staggered");
				if (GM.S.drawSprites && myGraphics) {
					myGraphics.stagger(true);
				}
				//Debug.Log (name + " I am staggered");
			}
		}
	}

	void setStagger(float s) {//34 26 36
		curStagger = s;
		float amnt = 1 - (curStagger / staggerThreshold);
		if (hpUI) {
			hpUI.setStag(amnt);
		}
		if (myGraphics) {
			myGraphics.setStaggerColor(amnt);
		}
	}
	
	float sTimer = 0;
	bool stagged = false;

	void stagger(){
		if (stagged) {
			if (sTimer > 0) {
				//Debug.Log ("I am staggered " + sTimer);
				sTimer--;//= recoverySpeed;
			} else {
				stopStagger ();
			}
		} else if (curStagger < staggerThreshold) {
			if (recoverCounter < recoverySpeed) {
				recoverCounter++;
			} else {
				setStagger(curStagger + 1);
				recoverCounter = 0;
			}
		}
	}

	public void stopStagger(){
		stagged = false;
		setStagger(staggerThreshold);
	//	Debug.Log ("DONE STaggered");
		if (human) {
			human.stopStagger ();
		}
		if (GM.S.drawSprites && myGraphics) {
			myGraphics.stagger(false);
		}
	}


	public bool takeDamage(float damage, bool invul) {
		return takeDamage(damage, invul, true);
	}

	public bool takeDamage(float damage, bool invul, bool flickerMusic) {
		return takeDamage(damage, invul, flickerMusic, false);
	}

	public bool takeDamage(float damage, bool invul, bool flickerMusic, bool burn) {
		if (damage > 0) {
			if (bleeding > 0) {
				bleeding += damage;
			} else {
				StartCoroutine(bleed(damage));
			}
			invulnerate(invul);
			if (human && flickerMusic) {
				startFlickering();
			}
			if (!hurting) {
				StartCoroutine (hurtAnim ());
			}
			health -= damage;
			if (hpUI) {
				hpUI.updateHealth(-damage);
			}
			if (!invul) {
                damageUI(-damage, Vector2.down);
			}
            if (health > 0) {
				//Debug.Log ("health = " + health);
				if (damage > 1 && name == "Player0") {
					//Debug.LogError (gameObject.name + "took " + damage + " damage");
				}
				if (human) {
					if (invul) {
						human.takeHit ();
					}
					//updateHealthBar ();
					
				}
				//StartCoroutine (invulnerate (invul));
			} else {
				stopInvulnerate();
				if (human) {
					stopFlicker();
					Debug.Log("player die");
					if (boomBox.S) {
						if (GM.S.getNumPlayers() > 1) {
							boomBox.S.die (human.playerNum);
						} else {
							boomBox.S.die(0);
							boomBox.S.die(1);
						}
					}
				}
				dying(burn);

				flying = false;
				power = 0;
				stagged = false;
				dead = true;
				constitution = 1;
				//Debug.Log ("knock speed " + knockSpeed);
				if (!human) {
					self.alive = false;
					//activate ();
				} else {
					//updateHealthBar ();
					//StopCoroutine("invulnerate");
					//Color col = self.color;
					//col.a = 1;
					//self.changeColorN (col);
					StartCoroutine (human.die (false, burn));
					//human.die();
				}
				return false;
			}
		}
		return true;
	}
	
	public void heal(float amount, bool doUI) {
		//Debug.Log (health + " + " + amount);
		float healAmnt = amount;
		if (health + amount < maxHealth) {
			health += amount;
		} else {
			healAmnt = maxHealth - health;
			health = maxHealth;
		}
		if (doUI) {
			damageUI(healAmnt, Vector2.up);
		}
		if (human && hpUI) {
			//updateHealthBar ();
			hpUI.updateHealth(healAmnt);
		}
	}

	public bool checkMaxHealth() {
		return health == maxHealth;
	}


	float checkBackStab(Form other){
		float backStabDamage = (Random.value * (maxHealth * 0.8f) - backStabDefense);
		Vector2Int pos = self.centerPoint - other.centerPoint;
		if (other.parent) {
			pos = self.centerPoint - other.parent.centerPoint;
		}
		//int aDir;
		if (Mathf.Abs (pos.x) > Mathf.Abs (pos.y)) {
			if (pos.x < 0) {
				//aDir = 2;
				if (self.direction == 6) {
					//Debug.Log ("BACKSTAB 2 - 6");
					return backStabDamage;
				}
			} else {
				//aDir = 6;
				if (self.direction == 2) {
					//Debug.Log ("BACKSTAB 6 - 2");
					return backStabDamage;
				}
			}
		} else /*if (Mathf.Abs (pos.x) < Mathf.Abs (pos.y))*/ {
			if (pos.y < 0) {
				//aDir = 0;
				if (self.direction == 4) {
					//Debug.Log ("BACKSTAB 0 - 4");
					return backStabDamage;
				}
			} else {
				//aDir = 4;
				if (self.direction == 0) {
					//Debug.Log ("BACKSTAB 4 - 0");
					return backStabDamage;
				}
			}
		}

		return 0;


	}

	public void setStats(float n_health, float n_staggerThreshold, float n_recoverySpeed, float n_weight, float relaxedP, float movingP,
		float attackingP, float n_backStabDefense){
		setMaxHealth(n_health);
		setStaggerStats(n_staggerThreshold, n_recoverySpeed);
		weight = n_weight;
		poises = new float[3];
		poises [0] = relaxedP;
		poises [1] = movingP;
		poises [2] = attackingP;
		backStabDefense = n_backStabDefense;
	}

	public void setMaxHealth(float h) {
		health = h;
		maxHealth = health;
		if (hpUI) {
			hpUI.setHealth(health);
		}
	}

	public void setStaggerStats(float threshold, float recover) {
		staggerThreshold = threshold;
		setStagger(threshold);
		recoverySpeed = recover;
		recoveryTime = 1 / recover;
	}

	public void stop() {
		if (flying) {
			power = 0;
			stopKnock ();
		}
		if (stagged) {
			sTimer = 0;
			stopStagger ();
		}
	}

	public Form curCorpse;

	public virtual void dying(bool burn) {
		float diePow = power;
		Vector2 flyDie = flyDirection;
		stop();
		//bleeding = 0;
		if (hpUI) {
			//Debug.Log("UPDATING LIVES UI ");
			// I guess we havent updated the lives yet
			hpUI.updateLives(GM.S.getLives(human) - 1);
		}
		if (self && !human) {
			self.die();
		} else {
			if (!self) {
				Debug.LogError(name + " probably tried to die twice");
			}
		}
		if (corpse) {
			GameObject tmp = Instantiate (corpse, transform.parent);
			Form poo = tmp.GetComponent<Form> ();
			poo.squareBody ();
			bool eth = poo.etheral;
			poo.etheral = true;
			Map.S.spawnForm (tmp, self.centerPoint.x, self.centerPoint.y);
			if (!poo.spawned) {
				//Debug.Log ("no go");
				poo.die ();
			} else {
				poo.etheral = eth;
				curCorpse = poo;
				if (diePow > 0) {
					//Debug.Log("add " + diePow  + "  power to the corpse, in direction: " + flyDie);
					defense d = curCorpse.GetComponent<defense>();
					if (d) {
						d.getKnocked(flyDie, diePow, 1);
					}
				}
				if (human) {
					PlayerCorpse pc = curCorpse.GetComponent<PlayerCorpse>();
					if (pc) {
						if (human && human.myAttack) {
							pc.getInfo(human.myAttack.getSoul(), human.playerNum, self.direction, burn);
						} else {
							pc.getInfo(null, human.playerNum, self.direction, burn);
						}
					}
				}
			}
		}

		if (myBrain) {
			myBrain.die();
		}
	}
	
	public void getKnocked(Vector2 dir, float pow, float percent){ //percent how far we start in speed, 1 - full speed 0 - no speed
		if (human) {
			human.getKnocked ();
		}
		if (power == 0) {
			//self.height = baseHeight + knockHeightMod; //WHY WHY DO I WANT THIS!!!!! We dont want to prevent the sword, from moving through players
		}
		knockSpeed = 0;
		knockHyper = knockMaxHyper;
		power = knockInitialPow = pow;
		power *= percent;
		knockSpeedCounter = knockSpeed;

		flyDirection = dir;
		flying = true;
	}

	Vector2 flyDirection;
	public float power;
	int knockSpeed;
	int knockHyper = 0;
	int knockMinSpeed = 5;
	int knockMaxHyper = 5;
	float knockDecel = 0.5f;
	int knockSpeedCounter = 0;
	float knockInitialPow;
	bool flying = false;

	int knockHeightMod = 11;

	void knockBack() {
		if (power > 0) {
			if (knockSpeedCounter >= knockSpeed) {
				knockSpeedCounter = 0;
				for (int i = 0; i <= knockHyper; i++) {
					Vector2 ogFlyDir = flyDirection;
					Vector2Int hit = knockCheck(self.centerPoint, flyDirection);
					if (hit.x == 1) {
						if (knockHit (ogFlyDir)) {
							flyDirection.x *= -1;
						}
					}
					if (hit.y == 1) {
						if (knockHit (ogFlyDir)) {
							flyDirection.y *= -1;
						}
					}
				//	Debug.Log ("--knock--");
					Vector2Int fly = new Vector2Int ((int)(flyDirection.x * 20), (int)(flyDirection.y * 20));
					//Debug.Log ("fly: " + fly);
					if (human) {
						if (!human.playerMove (human.changeDir(self.centerPoint + fly), true)) {
							//Debug.Log ("being knocked - " + self.centerPoint);
						} else {
							//Debug.Log ("no knock");
						}
					} else {
						Vector2Int fd = new Vector2Int(0 ,0);
						if (Mathf.Abs(flyDirection.x) > 0.5f) {
							fd.x = (int)Mathf.Sign(flyDirection.x);
						}
						if (Mathf.Abs(flyDirection.y) > 0.5f) {
							fd.y = (int)Mathf.Sign(flyDirection.y);
						}
						//Debug.Log(flyDirection + " -> " + fd);
						if (myBrain) {
							myBrain.move(fd);
						} else {
							self.move(self.centerPoint + fd, false);
						}
					}
					power -= knockDecel;
					Vector2Int speeds = calcKnockSpeeds(power);
					knockSpeed = speeds.x;
					knockHyper = speeds.y;
				}
			} else {
				knockSpeedCounter++;
			}
		} else {
			stopKnock ();
		}
	}
	
	public Vector2Int knockCheck(Vector2Int pos, Vector2 dir) { //.x for x side .y for y side
		Vector2Int val = Vector2Int.zero;
		if (dir.x != 0) {
			int xd = 6;
			if (dir.x < 0) {
				xd = 2;
			}
			int ySign = (int)Mathf.Sign(dir.y);
			if (!self.checkSide(pos, xd, true, false) || (dir.y != 0 && self.checkCorner(pos, xd, ySign, 0, ySign))) {// idk why this was needed, th 
				//Debug.Log ("x hit");
				val.x = 1;
			}
		}
		if (dir.y != 0) {
			int yd = 0;
			if (dir.y < 0) {
				yd = 4;
			}
			int xSign = (int)Mathf.Sign (dir.x);
			if (!self.checkSide (pos, yd, true, false) || (dir.x != 0 && self.checkCorner(pos, yd, xSign, xSign, 0))) {
				//Debug.Log ("y hit");
				val.y = 1;
			}
		}
		return val;
	}

	bool knockHit(Vector2 dir) {
		float w = 4;
		bool bounceOff = true;
		if (self.curCollided.Count != 0) {
			for (int i = 0; i < self.curCollided.Count; i++) {
				//Debug.Log (self.curCollided [i].name);
				defense D = self.curCollided [i].gameObject.GetComponent<defense> ();
				if (D && D.weight != -1) {
					D.getKnocked (dir, power / 2, 0.5f);
				} else {
					attackChunk AC = self.curCollided [i].gameObject.GetComponent<attackChunk> ();
					if (AC) {
						//bounceOff = false;
					//	Debug.LogError ("hit sword with body knock");
						circleAttack CA = AC.getAttack ();
						if (CA && !CA.swinging) {
							CA.knockBack (-CA.getDirFromPoint(self.centerPoint), (int)power, self.hyperSpeed, false);
						}
					}
				}
			}
			self.curCollided.Clear ();
			if (power - w > 0) {
				power -= w;
				//Debug.Log ("hit weight : " + w);
			}
		}
		return bounceOff;
	}

	void stopKnock(){
		power = 0;
		flying = false;
		self.height = baseHeight;
		if (human) {
			//Debug.Log ("knocking");
			human.knocked = false;
		}
		if (GM.S.drawSprites && myGraphics) {
			myGraphics.knock(false);
		}
	}

	void knockAttack(Form attack) {
		//Debug.LogError (name + " hit lightly by " + attack.name);
		if (attack.id == 2) {
			// hit by sword
			circleAttack ca = attack.GetComponentInParent<circleAttack>();
			ca.bounce((int)Mathf.Max(weight / 20, 1));
		}
	}

	public int calcKnockSpeed(float pow) {
		return (int)Mathf.SmoothStep (0, knockMinSpeed, 1 - (pow / knockInitialPow));
	}

	public Vector2Int calcKnockSpeeds(float pow) {
		float percent = Mathf.SmoothStep(0, 1, power / knockInitialPow);//pow / knockInitialPow;
		int speedDiff = knockMinSpeed + knockMaxHyper;
		int val = (int)(speedDiff * percent);
		if (val < knockMinSpeed) {
			return new Vector2Int(knockMinSpeed - val, 0);
		} else {
			return new Vector2Int(0, val - knockMinSpeed);
		}
	}

		bool hurting = false;

	IEnumerator hurtAnim(){
		//Debug.Log ("hurting " + hurting);
		if (!hurting) {
			if (myGraphics) {
				hurting = true;

				if (myGraphics) {
					myGraphics.hurt(true);
				}
				yield return new WaitForSeconds (0.5f);
				if (!dead && !stagged) {
					if (myGraphics) {
						myGraphics.hurt(false);//setEyes (2);//eyeState);
					}
				}
				//myGraphics.SetBool ("hurt", false);
				//Debug.Log ("poo");
				hurting = false;
			}
		}
		yield return new WaitForFixedUpdate ();
	}

	public void damageUI(float dam, Vector2 dir) {
		if (damageText) {
			float yv = Random.Range(-1f, 1f);
			Vector3 pos = Camera.main.WorldToScreenPoint(transform.position) + new Vector3(dir.x * 10, dir.y * 10, 0);// +;
			//Vector2 dir =  new Vector3(0, 10);
			DamageUI du;
			if (dam < 0) {
				du = Instantiate(damageText, pos, transform.rotation, canvas).GetComponent<DamageUI>();
			} else {
				du = Instantiate(damageText, pos, transform.rotation, canvas).GetComponent<DamageUI>();
			}
			du.getDamage(dam, dir);//new Vector2(1, yv));
			du.p = transform;
		}
	}
	
	float bleeding = 0;
	IEnumerator bleed(float damage) {
		if (!myGraphics || blood == null) {
			yield return new WaitForFixedUpdate();
		} else {
			bleeding += damage;
			while (bleeding > 0) {
				bloodDrop bd = Instantiate(blood, transform.position, Quaternion.identity, transform).GetComponent<bloodDrop>();
				float before = bleeding;
				bleeding -= bd.getInfo(bloodColor, bleeding, bloodPuddle);//myGraphics.mainColor, bleeding);
				yield return new WaitForSeconds(0.01f);
			}
		}
	}

	public Form linkBloodToRoom(Form puddle, bool first) {
		if (puddle) {
			bloodPuddle = puddle;
		} else {
			if (!first) {
				bloodPuddle = Instantiate(bloodMass, transform).GetComponent<Form>();
				Tiler t = bloodPuddle.GetComponent<Tiler>();
				if (human.myAttack && human.myAttack.getSoul()) {
					bloodColor = human.myAttack.getSoul().powerColor[human.colorNum];
				} else {
					bloodColor = Color.white;
				}
				t.color = bloodColor;
			}
		}
		return bloodPuddle;
	}

	public Vector2 getFlyDirection() {
		return flyDirection;
	}
	
	public float getPower() {
		return power;
	}
	
	public int getKnockCounter() {
		return knockSpeedCounter;
	}
	
	public float getKnockDecel() {
		return knockDecel;
	}
	
	public Vector2Int getKnockSpeeds() {
		return new Vector2Int(knockSpeed, knockHyper);
	}

	bool recovering = false;
	int recoverCounter = 0;

	public GameObject healthIcon;
	HealthIcon hpUI;
	Transform canvas;

	public void spawnHealthIcon(Player p) {
		if (healthIcon) {
			hpUI = Instantiate(healthIcon, transform.position, transform.rotation, canvas).GetComponent<HealthIcon>();
			placeHealthUI(p.playerNum, GameInfo.S.numPlayers);
			hpUI.setHealth(health);
		}
		//hpUI.setSoul(p);//GameInfo.S.pSprites[GameInfo.S.teamNums[pNum]], GameInfo.S.symbols[GameInfo.S.songs[pNum]]);
	}

	public void placeHealthUI(int pNum, int numPlayers) {
		if (hpUI) {
			float buffer = Mathf.Lerp(0.5f, 0.15f, ((float)(numPlayers - 1) / 3));//Screen.width * Mathf.Lerp(0.5f, 0.15f, ((float)(numPlayers - 1) / 3));
			//Debug.Log("BUFFER: " + (buffer/Screen.width));
			float spacePercent = 1f / (Mathf.Max(1, numPlayers - 1));
			Vector3 pos;
			if (GM.S.UiOnLeft) {
				if (numPlayers > 1) {
					buffer *= Screen.height;
					pos = /*Camera.main.ScreenToWorldPoint(*/new Vector3 (Screen.width * 0.063f, Screen.height - buffer - ((Screen.height - (2 * buffer)) * (pNum * spacePercent)), 1);
				} else {
					pos = new Vector3(Screen.width * 0.063f, Screen.height * 0.8f);
				}
			} else {
				buffer *= Screen.width;
				pos = /*Camera.main.ScreenToWorldPoint(*/new Vector3 (buffer + ((Screen.width - (2 * buffer)) * (pNum * spacePercent)), Screen.height * 0.13f, 1);
			}
			hpUI.transform.position = pos;
			hpUI.setPos();
			hpUI.gameObject.SetActive(true);
		}
	}

	public void hideUI() {
		hpUI.gameObject.SetActive(false);
	}

	public void deleteUI() {
		if (hpUI) {
			Destroy(hpUI.gameObject);
		}
	}

	public void spawnHealthBar(int pNum, Color pCol, Color soulColor, Color textColor) {
		GameObject canvas = GameObject.FindGameObjectWithTag ("canvas");
		//Debug.Log ("spawning health bar for player " + pNum);
		//Debug.Log (Screen.width * ((pNum + 1) * 0.33f));
		float buffer = Screen.width * 0.1f;
		float spacePercent = 1f / (Mathf.Max(1, GameInfo.S.numPlayers - 1));
		//Debug.Log (spacePercent);
		Vector3 pos = /*Camera.main.ScreenToWorldPoint(*/new Vector3 (buffer + ((Screen.width - (2 * buffer)) * (pNum * spacePercent)), Screen.height * 0.07f, 1);
		GameObject bar = Instantiate (hBar, pos, transform.rotation, canvas.transform);
		Image bg = bar.transform.GetChild(0).GetComponent<Image>();
		bg.color = soulColor;
		Text sName = bar.transform.GetChild(1).GetComponent<Text>();
		sName.text = GameInfo.S.names[GameInfo.S.songs[pNum]];// "poo stinky";
		sName.color = textColor;
		lives = bar.transform.GetChild(2).GetComponent<Text>();
		lives.text = "x" + GM.S.getLives(human);
		lives.color = textColor;
		//healthBar.transform.position = pos;
		healthBar = bar.transform.GetChild(5);
		hpSpri = healthBar.GetChild (0).GetComponent<Image>();
		hpSpri.color = pCol;
	}

	public void fullHealth(){
		health = maxHealth;
		//updateHealthBar ();
		if (hpUI) {
			hpUI.setHealth(health);
		}
	}

	public void respawn(bool heal) {
		if (heal) {
			fullHealth();
			setStagger(staggerThreshold);
		}
		dead = false;
		if (hpUI) {
			hpUI.updateLives(GM.S.getLives(human));
		}
	}

	public void setLivesUI(int numLives) {
		if (hpUI) {
			hpUI.updateLives(numLives);
		}
	}

	Color bloodColor;

	public void setUIColor(int cNum) {
		if (hpUI && human && !human.fodder) {
			hpUI.setSoul(human);
		}
		if (bloodPuddle) {
			Tiler t = bloodPuddle.GetComponent<Tiler>();
			bloodColor = human.myAttack.getSoul().powerColor[cNum];
			t.color = bloodColor;
		}
	}

	public HealthIcon getUI() { return hpUI; }

	void updateHealthBar(){
		Vector3 hb = healthBar.localScale;
		hb.x = health / maxHealth;
		healthBar.localScale = hb;
	}

	public bool isInvulnerable() {
		return invulnerable;
	}

	public int getCurInvul() {
		return curInv;
	}

	public float getHealth() {
		return health;
	}

	public void setTeam(int newTeam) {
		team = newTeam;
	}

	public bool canMove() {
		return !(flying || stagged);
	}

	public bool staggered() { return stagged; }
}
