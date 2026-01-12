using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;

public class circleAttack : Purpose {

	public GameObject chunk;
	Player myPlayer;
	FighterAnimator pGraphics;
	Controller ic;
	SwordSoul soul;
	SwordAnimator anim;

	public float curPos;
	public float oldPos;
	public int curDir;
	public List<Vector2Int> intervals;
	public List<int> anglePath;
	Form body;

	public float lifeSpacer;
	float lCounter;
	public bool swinging;

	public List<Form>[] blade;
	public int swordDist;
	float swordCost;
	public Vector2 attackRange;

	public int bladeMulti = 1;
	int bladeWidth = 3;
	int bladeLength;
	//int bladeMid;

	Vector2 damage;
	Vector2 stagger;
	Vector2 knockBackDamage;
	Vector2 mass;

	int staggerTime;
	public int maxSpeed;
	public int minSpeed;
	// based on swords braced and ignited speeds
	int fullMaxSpeed;
	int fullMinSpeed;
	int baseMin;
	int baseMax;
	float modMin = 1;
	float modMax = 1;
	public float brace = 1;
	int maxStrikes;
	int weight = 50;
	public bool windingDown;
	int number;

	public bool singleStrike;
	int swingPause = 25;
	int swingCounter = -1;
	public bool readyToSwing = true;
	public bool beingKnocked;
	float staggered = 0;
	float stagRecover;
	public int startPos;
	public List<Vector2Int> checkPoses;
	public Vector2Int checkCenter;
	public float checkAngle;
	int cNum;
	GameObject chunkSprite;

	 protected override void Awake(){
		anglePath = new List<int> ();
		intervals = new List<Vector2Int> ();
		checkPoses = new List<Vector2Int> ();
		overrideRanges = new float[2];
		//damageMulti = new float[2] {0.5f, 0.5f};
		//damMmax = new float[2] { 2f, 1.5f };
		//knockMulti = new float[2] {0.5f, 0.5f};
		//knockMmax = new float[2] { 3f, 2f };
		base.Awake ();
		self.addAction (this);
		self.floatStep = false;
	}

	public void meetPlayer(Player player) {
		myPlayer = player;
		ic = player.gameObject.GetComponent<Controller> ();;
		number = myPlayer.playerNum;
		cNum = myPlayer.colorNum;
		pGraphics = player.graphics;
		if (!pGraphics) {
			Debug.Log("fuck stick");
		}
		meetBody(player.GetComponent<Form>());
	}

	public void meetBody(Form f) {
		body = f;
		soul = gameObject.GetComponent<SwordSoul> ();
		soul.meetBody (f.gameObject);
		swingPause = soul.swingPause;
		defense def = f.GetComponent<defense>();
		if (def) {
			stagRecover = def.recoverySpeed;
		}
		if (!ic) {
			ic = f.GetComponent<Controller>();
		}
	}

	public void setStats(Vector2 n_damage, Vector2 stag, int stagTime, Vector2 knock, int n_range, int n_width, int n_length, 
		float n_bladeSpacing, int n_bladeMulti, int n_multiAngleSpacing, float n_taper, int n_dist, int n_maxStrikes, int n_weight, 
		int n_minSpeed, int n_maxSpeed, float n_heatAmount, GameObject n_chunk, float swordInc, Vector2 n_mass){
		damage = n_damage;
		stagger = stag;
		staggerTime = stagTime;
		knockBackDamage = knock;
		weight = n_weight;
		range = n_range;
		baseMax = maxSpeed = n_maxSpeed;
		baseMin = minSpeed = n_minSpeed;
		bladeWidth = n_width;
		bladeLength = n_length;
		heatAmount = n_heatAmount;
		powInc = swordInc;
		mass = n_mass;
		//bladeMid = bladeWidth / 2;
		if (n_bladeSpacing < 1) {
			bladeSpacing = Mathf.Max(1, Mathf.Sqrt (Mathf.Pow ((bladeWidth / 2), 2) + Mathf.Pow ((bladeLength / 2), 2)));//bladeSpacing = 1;//Mathf.Max (bladeLength, bladeWidth);
		} else { 
			bladeSpacing = n_bladeSpacing;
		}
		//bladeSpacing = (int)Mathf.Sqrt ((bladeWidth * bladeWidth) + (bladeLength * bladeLength));
		if (n_bladeMulti > 0) {
			bladeMulti = n_bladeMulti;
		} else {
			bladeMulti = 1;
		}

		if (n_dist < 0) {
			swordDist = 3;//Mathf.Max (body.width, body.length);
		} else {
			swordDist = n_dist;
		}


		blade = new List<Form>[bladeMulti];
		for (int i = 0; i < bladeMulti; i++) {
			blade [i] = new List<Form> ();
		}

		multiAngleSpacing = n_multiAngleSpacing;
		taperMulti = n_taper;
		maxStrikes = n_maxStrikes;
		chunkSprite = n_chunk;
	}

	public float bladeSpacing;
	public int multiAngleSpacing = 20;
	public float taperMulti = 3;

	public void spawnSword() {//, GameObject[] sword){
		GameObject[] sword = new GameObject[bladeMulti * (int)range];
		for (int i = 0; i < bladeMulti * range; i++) {
			sword [i] = Instantiate (chunk);
		}
		int s = 0;
		for (int k = 0; k < bladeMulti; k++) {
			for (int i = 0; i < sword.Length / bladeMulti; i++) {
				Form f = sword [s].GetComponent<Form> ();
				blade[k].Add (f);
				blade [k][i].transform.parent = transform;
				blade [k][i].parent = body; //perhaps needed; changed so that when parrying the other blade can get a reference to the playerSword script
				f.width = Mathf.Max (1, bladeWidth/* + (int)(i * taperMulti)*/);
				f.length = Mathf.Max (1, bladeLength/* + (int)(i * taperMulti)*/);
				f.squareBody ();
				//f.floatStep = false;// really only needed for player, because sword id so fast
				s++;
			}
		}
		int noLoop = 0;
		startPos = (int)curPos;
		while (!checkBody (startPos, false) && noLoop <= 360) {
			startPos = Map.S.saneAddition (startPos, 1, 1);
			noLoop++;
			//Debug.Log ("startPos no good " + startPos + " " + noLoop);
		}

		curPos = startPos;
		//pl.GetComponent<Player> ().meetSword (this);
		if (myPlayer) {
			gameObject.name = "Sword" + number;
		}

		s = 0;
		for (int k = 0; k < bladeMulti; k++) {
			for (int i = 0; i < sword.Length / bladeMulti; i++) {
				if (!spawnBladeChunk (sword [s].GetComponent<Form> (), k, i, false)) {
					break;
				}
				s++;
			}
		}
		baseHeight = blade [0] [0].height;
		calculateSwordRange ();
		ic.meetSword (this);
		setGrowth (blade [0] [0].width);
		soul.getBlade (blade);
		Vector2Int speedRange = soul.getSpeedRange();
		//Debug.Log(name + " " + speedRange);
		fullMinSpeed = speedRange.x;
		fullMaxSpeed = speedRange.y;
		if (GM.S.drawSprites && soul.SwordAnim) {
			anim = soul.getAnim();
			//anim.setLayer(10);
			anim.setRotation(curPos);
			anim.initiate();
		}
	}

	public bool spawnBladeChunk(Form f, int k, int i, bool preFixed) {
		GameObject tmp = null;
		if (!preFixed) {
			if (GM.S.drawSprites) {
				if (soul.chunkAnimation) {
					tmp = Instantiate (chunkSprite, f.transform.position, f.transform.rotation);
					tmp.transform.parent = f.transform;

					f.skin = f.transform.GetChild (0).GetChild (0).gameObject;//.GetChild (0).GetComponent<SpriteRenderer> ();
					f.transform.GetChild (0).GetChild (0).GetChild (0).GetComponent<SpriteRenderer> ().color = soul.soulColorA;//Color.Lerp (soulColor, heatColor, fade);
				//	f.transform.GetChild (0).localScale = new Vector3 ((float)(f.width) / 2, (float)(f.length) / 2, 1);
					//sr = 
					f.transform.GetChild (0).GetChild (0).GetChild (1).GetComponent<SpriteRenderer> ().color = soul.soulColorB;
					f.transform.GetChild (0).GetChild (0).transform.localScale = new Vector3 (f.width * 13f, f.length * 13f, 1);
					if (f.width % 2 == 0) {
						f.transform.GetChild (0).GetChild (0).position = f.transform.position + new Vector3 (-0.5f, -0.5f, 0f);
					} else {
						f.transform.GetChild (0).GetChild (0).position = f.transform.position + new Vector3 (0f, 0f, 0f);
					}
				} else {
					Color c = f.color;
					c.a = 0;
					f.color = c;
				}
			} else {
				f.changeColorN(soul.soulColorA);
			}
		}
		Value v = f.gameObject.GetComponent<Value> ();
		v.setTeam (team);
		float spawnPos = curPos;
		//when we spawn we can hit things and so we need our info
		f.name = name + "-chunk" + k + "-" + i;
		v.setValue ("damage", damage.x, damage.y);
		v.setValue ("stagger", stagger.x, stagger.y);
		v.setValue ("stagTime", staggerTime, staggerTime);
		v.setValue ("knockBack", knockBackDamage.x, knockBackDamage.y);
		attackChunk ac = f.GetComponent<attackChunk>();
		ac.setIndex(k, i);
		ac.setAttack(this);
		Vector2Int p = calculateSwordPos (i, k, spawnPos);
		Map.S.spawnForm (f.gameObject, p.x, p.y);
		int noLoop = 1;
		int end = -1;
		//for Root in particular and maybe needs to be tested
		while (!f.spawned && noLoop < 180) {
			end *= -1;
			p = calculateSwordPos(i, k, spawnPos + (noLoop*end));
			if (checkBody(spawnPos + (noLoop*end), false)) {
				Map.S.spawnForm(f.gameObject, p.x, p.y);
			}
			if (end < 0) {
				noLoop++;
			}
		}
		if (!f.spawned) {
			Destroy (tmp);
			f.die ();
			blade[k].Remove (f);
			//endStrike (0);
			return false;
		} else {
			if (noLoop > 1) {
				curPos = spawnPos + (noLoop * end);
				for (int l = 0; l < bladeMulti; l++) {
					for (int m = 0; m < blade [l].Count; m++) {
						p = calculateSwordPos (m, l, curPos);
						blade [l] [m].forceMove (p, false);
					}
				}
			}
			return true;
		}
	}

	public Vector2Int calculateSwordPos(int lenIt, int widIt, float pos){
		//Debug.Log(widIt + ", " + lenIt + " count:" + + blade[0].Count);
		//Debug.Log("segment: " + Mathf.Max(0, Mathf.Min(blade[widIt].Count - 1, lenIt)));
		int segment = Mathf.Max(0, Mathf.Min(blade[widIt].Count - 1, lenIt));

		Form cur = blade [widIt] [segment];
		// for root flower
		int dim = Mathf.Max (cur.width, cur.length);
		if (dim % 2 == 0) {
			dim--;
		}
		int length = (int)(dim * bladeSpacing) / 2;

		for (int i = 0; i < segment; i++) {
			length += Mathf.Max(blade[widIt][i].width, blade[widIt][i].length);
		}
				//Debug.Log("segment: " + segment + " " + length + " " + pos);
		int taper = (int)(multiAngleSpacing - (lenIt * taperMulti));
		if (taper < 0) {
			taper = 0;
		}
		if (multiAngleSpacing > 1 && bladeMulti > 1) {
			widIt -= bladeMulti / 2;
		}
		//Debug.Log(Mathf.Cos ((pos + (widIt * taper)) * Mathf.PI / 180));
		Vector2Int center = getCenter ();

		int xp = (int)Mathf.Round(center.x + (length + swordDist) * Mathf.Cos ((pos + (widIt * taper)) * Mathf.PI / 180));
		//xp = (int)(center.x + ((lenIt * bs) + swordDist + (bs/2)) * Mathf.Cos (pos * Mathf.PI / 180));
		int yp = (int)Mathf.Round(center.y + (length + swordDist) * Mathf.Sin ((pos + (widIt * taper)) * Mathf.PI / 180));
		//yp = (int)(center.y + ((lenIt * bs) + swordDist + (bs/2)) * Mathf.Sin (pos * Mathf.PI / 180));
		//Debug.Log (xp + " " + yp);
		return new Vector2Int (xp, yp);
	}

	public Vector2Int calculateSwordPos(int lenIt, int widIt, float pos, Vector2Int center){
		Form cur = blade [widIt] [Mathf.Max(0, lenIt - 1)];
		int bs = (int)(Mathf.Max (cur.width, cur.length) * bladeSpacing);
		//int bs = Mathf.Max (cur.width, cur.length) * bladeSpacing;
		int taper = (int)(multiAngleSpacing - (lenIt * taperMulti));
		if (taper < 0) {
			taper = 0;
		}
		if (multiAngleSpacing > 1 && bladeMulti > 1) {
			widIt -= bladeMulti / 2;
		}
		int xp = (int)Mathf.Round(center.x + ((lenIt * (bs)) + swordDist + (bs/2)) * Mathf.Cos ((pos + (widIt * taper)) * Mathf.PI / 180));
		int yp = (int)Mathf.Round(center.y + ((lenIt * (bs)) + swordDist + (bs/2)) * Mathf.Sin ((pos + (widIt * taper)) * Mathf.PI / 180));
		//Debug.Log (xp + " " + yp);
		return new Vector2Int (xp, yp);
	}

	public void deleteSword() {
		soul.reset();
		for (int k = 0; k < bladeMulti; k++) {
			for (int i = 0; i < blade [k].Count; i++) {
				blade[k][i].die();
				Destroy(blade[k][i].gameObject);
			}
		}
		if (anim) {
			Destroy(anim.gameObject);
		}
		self.die();
	}

	public override IEnumerator callAction (int delay) {
		//Debug.Log("calling");
		if (active && anglePath.Count > 0) {
			if (!swinging && !beingKnocked) {
				Debug.Log("swinging but not");
			}
			if ((!myPlayer || !myPlayer.stagged) && (!singleStrike || readyToSwing)) {
				if (swingCounter > -1) {
					swingCounter--;
					if (swingCounter == -1) {
						if (anim) {
							anim.setAction(1);
						}
					}
				} else {
					if (speedCounter >= self.speed) {
						//Debug.LogError (curPos + " -> ");
						//Debug.Log("moving at " + self.hyperSpeed + " " + swingi + " " + curSwing);
						for (int i = 0; i <= self.hyperSpeed; i++) {
							if (anglePath.Count > 0) {
								/*
								if (!beingKnocked) {
									setBladeHeight(15);
								} else {
									setBladeHeight(5);
								}
								*/
								curDir = ic.findShortestDir (curPos, anglePath [0]);
								//Debug.Log (curDir);
								if (checkMove (curDir, true, false, getCenter()) < 2) {
									if (GM.S.drawSprites && anim) {
										anim.setFlip(curDir == -1);
									}
									curPos = anglePath [0];
									if (!beingKnocked) {
										//soul.heatUp (heatAmount, soul.maxHeat);
										soul.swing();
										//Debug.LogWarning("cp: " + curPos + " heat " + soul.energy);
									}
									forceMove ();
									if (anglePath.Count > 0) {
										anglePath.RemoveAt (0);
									}
									//Debug.Log ("cp: " + curPos);
								} else {
									//Debug.Log ("sword cant move??");
									if (checkMove(-curDir, true, false, getCenter()) < 2) {
										curPos = ic.saneAddition(curPos, 1, -curDir);
										//Debug.Log("moved back from swing");
									}
									clearChecks ();
								}
								calcSpeed ();
								musicSpeed ();
							} else {
								break;
							}
						
						}
						if (name == "Sword0") {
							//Debug.LogWarning (name + "angle: " + curPos + " curDir: " + curDir + " curSwingD " + curSwingD);
							if (curDir != curSwingD) {
								//Debug.LogWarning ("directions arent' matched");
							}
						}
						//Debug.Log (name + " " + curPos);
						if (name == "Sword0"
							|| name == "Sword1") {
							//Debug.Log (name + " am swinging " + curPos + " " + blade[0][0].GetComponent<Value>().getValue("knockBack") + " i: " + swingi + " cs: " + curSwing);
							if (self.hyperSpeed < minSpeed && !beingKnocked) {
								//Debug.Log ("cs: " + curSwing + " " + swingi + " why so slow " + ((swingi * (Mathf.PI / curSwing))));
								//Debug.Log((int)(Mathf.Sin (swingi * (Mathf.PI / curSwing)) * (maxSpeed * friction)));
							}
						}
						speedCounter = 0;
					} else {
						speedCounter++;
					}
				}
			}
		} else if (swinging || beingKnocked) {
			swinging = false;
			readyToSwing = false;
			calcSpeed ();
			oldDir = 0;
			musicSpeed ();
			if (GM.S.drawSprites) {
				if (pGraphics) {
					if (!myPlayer.stagged) {
						pGraphics.swingOver(ic.getStrikeInput(), getCenter() != body.centerPoint);
					}
				} 
				if (anim) {
					anim.setAction(0);
				}
			}
			//setBladeHeight(5);
			if (beingKnocked) {
				//Debug.Log("knock stop");
				beingKnocked = false;
				knockEtheral (false);
				soul.knockOver();
				if (myPlayer && myPlayer.stagged) {
					erasePresence(myPlayer.dodging);
				}
			} else {
				if (Grader.S) {
					Grader.S.swingEnd(body);
				}
			}
			soul.stopSwinging();
		} 
		if (pGraphics && GM.S.drawSprites) {
			if (droopFailC != -1) {
				droopFailC--;
				if (droopFailC == -1) {
					//pGraphics.setMouth (0);
					pGraphics.cantSwing(false);
				}
			}
		}
		if (staggered > 0) {
			staggered--;
		}
		yield return new WaitForFixedUpdate();
	}

	float[] overrideRanges;

	public void calculateSwordRange() {
		// for root when it loses all segments
		if (range == 0) {
			attackRange.x = 0;
			attackRange.y = 0;
			return;
		}
		if (overrideRanges [0] == 0 && overrideRanges [1] == 0) {
			Vector2Int tipPos = calculateSwordPos ((int)(range - 1), 0, 0);
			Form tip = blade [0] [(int)(range - 1)];
			float disty = Mathf.NegativeInfinity;
			int chosen = -1;
			for (int i = 0; i < tip.body.Length; i++) {
				float ds = Vector2Int.Distance (body.centerPoint, tipPos + tip.body [i]);
				if (ds > disty) {
					disty = ds;
					chosen = i;
				}
			}
			float straightRange = Vector2Int.Distance (body.centerPoint, tipPos + tip.body [chosen]) - 1;
			//attackRange.y = Mathf.Sqrt ((body.width * body.width) + (body.length + body.length)) + (range * Mathf.Sqrt (Mathf.Pow(bladeWidth, 2) + Mathf.Pow(bladeLength, 2)));
			tipPos = calculateSwordPos ((int)(range - 1), 0, -45);
			disty = Mathf.NegativeInfinity;
			chosen = -1;
			for (int i = 0; i < tip.body.Length; i++) {
				float ds = Vector2Int.Distance (body.centerPoint, tipPos + tip.body [i]);
				if (ds > disty) {
					disty = ds;
					chosen = i;
				}
			}
			float diagRange = Vector2Int.Distance (body.centerPoint, tipPos + tip.body [chosen]) - 1;
			attackRange.x = Mathf.Min (straightRange, diagRange);
			attackRange.y = Mathf.Max (straightRange, diagRange);
			//Debug.Log (attackRange.x + " " + attackRange.y);
		} else {
			Debug.Log ("ranges overriden");
			attackRange.x = overrideRanges [0];
			attackRange.y = overrideRanges [1];
		}
	}

	public float calculateSwordRange(float angle) {
		Vector2Int tipPos = calculateSwordPos ((int)(range - 1), 0, angle);
		Form tip = blade [0] [(int)(range - 1)];
		float disty = Mathf.NegativeInfinity;
		int chosen = -1;
		for (int i = 0; i < tip.body.Length; i++) {
			float ds = Vector2Int.Distance (body.centerPoint, tipPos + tip.body [i]);
			if (ds > disty) {
				disty = ds;
				chosen = i;
			}
		}
		Debug.Log (tipPos + tip.body [chosen]);
		return Vector2Int.Distance(body.centerPoint, tipPos + tip.body[chosen]);
	}

	public void setRanges(float[] newRanges) {
		overrideRanges[0] = newRanges[0];
		overrideRanges[1] = newRanges[1];
		attackRange.x = overrideRanges [0];
		attackRange.y = overrideRanges [1];
	}

	public List<Form>[] getBlade() {
		return blade;
	}

	int curSwing = 0;
	float swingi = 0;
	int curSwingD = 0;
	float friction = 1;
	public float frictionValue;
	public float swingPower;
	public float powInc = 0.05f;
	public bool oldSwing = false;

	void calcSpeed() {
		int hyperS = self.hyperSpeed;
		if ((swingi >= curSwing || curSwing == 0) && intervals.Count > 0) {
			curSwing = intervals [0].x;
			//Debug.Log(curSwing + "curSwing from interval");
			curDir = curSwingD = intervals[0].y;
			swingi = 0;
			intervals.RemoveAt (0);
		}
		if (curSwing > 0) {
			calcPower();
			if (swingi < curSwing) {//if (swingi < Mathf.PI){// - (Mathf.PI / curSwing)) {
				//swingi += Mathf.PI / curSwing;

	
				//Debug.Log (" speed: " + intCount * (Mathf.PI / curSwing) + "within: " + curSwing);
				//Debug.Log(swingi + " " + self.hyperSpeed);
				swingi++;
				if (swingi < 1) {
					Debug.Log ("qfjowejf " + swingi);
				}
				self.hyperSpeed = (int)calcStat (minSpeed, (int)(maxSpeed * friction));//(int)(Mathf.Sin (swingi * (Mathf.PI / curSwing)) * (maxSpeed * friction)) + minSpeed;
				if (self.hyperSpeed < minSpeed && !beingKnocked && friction != 1) {
					//Debug.LogError ((int)(Mathf.Sin (swingi * (Mathf.PI / curSwing)) * (maxSpeed * friction)));
					Debug.Log ("swingi " + swingi + " curSwing" + curSwing);
				}
				//Debug.Log("min, max " + minSpeed + ", " + (maxSpeed * friction) + " = " + self.hyperSpeed + " speed");
				if (self.hyperSpeed == minSpeed + maxSpeed) {
					//Debug.Log ("swingi = " + swingi + " curSwing = " + curSwing);
					//Debug.Log ("max speed at " + (swingi / curSwing));
				}
				calcStats ();
			} else {
				//self.hyperSpeed = (int)(Mathf.Sin (swingi) * maxSpeed) + minSpeed;
				if (anglePath.Count > 100 && intervals.Count == 0) {
					Debug.LogWarning (name + " mismatch of speed by " + anglePath.Count + " positions" + "curSwing: " + curSwing);
				}
				if (swingi < 1) {
					Debug.Log ("fucking why " + swingi);
					swingi = 1;
				}
				self.hyperSpeed = (int)calcStat (minSpeed, maxSpeed);//(int)(Mathf.Sin (swingi * (Mathf.PI / curSwing)) * maxSpeed) + minSpeed;
				//Debug.Log("HP: " + self.hyperSpeed);
				calcStats ();
				curSwing = 0;
				//Debug.Log ("reset curSwing");
				swingi = 0;
			}
        }
        if (anglePath.Count == 0 && intervals.Count > 0) {
			self.hyperSpeed = minSpeed;
			resetStats ();
			intervals.Clear ();
		}
		if (hyperS != self.hyperSpeed) {
			//Debug.Log("new speed: " + self.hyperSpeed);
		}
	}
		
	float calcStat(float min, float max) {
		return calcStat (swingi, curSwing, min, max);
	}

	float calcStat(Vector2 stat) {
		return calcStat (swingi, curSwing, stat.x, stat.y);//(int)Mathf.Max((Mathf.Sin (swingi * (Mathf.PI / curSwing)) * stat.y), stat.x);
	}

	public float calcStat(float swingi, int curSwing, float min, float max) {
		float stat = 0;
		if (oldSwing) {
			stat = Mathf.Max(Mathf.Sin (swingi * (Mathf.PI / curSwing)) * max, min);
		} else {
			stat = Mathf.Clamp(Mathf.SmoothStep(min, max * 2, swingPower), min, max);
		}
		/*
		if (Mathf.Sin (swingi * (Mathf.PI / curSwing)) * max < min) {
			Debug.LogError ("calc stat not working right " + " si: " + swingi + " sl: " + curSwing + " = " + (Mathf.Sin (swingi * (Mathf.PI / curSwing)) * max));
			Debug.Log (friction);
		}
		*/
		return stat;
	}

	void calcPower() {
		/*
		if (carefulMove) {
			swingPower = 0;
			return;
		}
		*/
		int tilDown = (int)(swingPower / powInc);
		//Debug.Log((curSwing - swingi) + " > " + tilDown +". " + swingPower);
		if (/*brace == 1 && */curSwing - swingi > tilDown) {
			if (swingPower + powInc < 0.5f) {
				swingPower += powInc;
			} else {
				swingPower = 0.5f;
			}
		} else {
			if (swingPower - powInc > 0) {
				swingPower -= powInc;
			} else {
				swingPower = 0;
			}
		}
	}

	public void speedChange() {
		minSpeed = (int)(baseMin * modMin);
		maxSpeed = (int)(baseMax * modMax);
		musicSpeed();
	}

	public void modSpeedChange(float min, float max) {
		modMin = min;
		modMax = max;
		speedChange();
	}

	public void baseSpeedChange(int min, int max) {
		baseMin = min;
		baseMax = max;
		speedChange();
	}

	void calcStats(){
		float dam = brace != 1 ? damage.x : calcStat (damage);//Mathf.Max((int)(Mathf.Sin (swingi * (Mathf.PI / curSwing)) * damage.y), damage.x);
		//Debug.Log ("dam: " + (int)(Mathf.Sin (swingi * (Mathf.PI / curSwing)) * damage.y));
		float kb = calcStat (knockBackDamage);//Mathf.Max((int)(Mathf.Sin (swingi * (Mathf.PI / curSwing)) * knockBackDamage.y),  knockBackDamage.x);
		//Debug.Log ("knockBack: " + kb);
		float st = calcStat(stagger);//Mathf.Max((int)(Mathf.Sin (swingi * (Mathf.PI / curSwing)) * stagger.y), stagger.x);
		for (int k = 0; k < bladeMulti; k++) {
			for (int i = 0; i < blade [k].Count; i++) {
				Form f = blade [k] [i];
				Value v = f.GetComponent<Value> ();
				v.writeValue ("damage", dam);
				v.writeValue ("knockBack", kb);
				v.writeValue ("stagger", st);
			}
		}
	}

	void resetStats() {
		for (int k = 0; k < bladeMulti; k++) {
			for (int i = 0; i < blade [k].Count; i++) {
				Form f = blade [k] [i];
				Value v = f.GetComponent<Value> ();
				v.lowerValue ("knockBack", 100);
				v.lowerValue ("stagger", 100);
				v.lowerValue ("damage", 100);
			}
		}
	}

	public float maxMuseSpeed = 0.5f;
	public float minMuseSpeed = 0;//-0.5f;
	public float museSpeed;

	public void musicSpeed() {
		if (myPlayer) {
			//museSpeed = ((float)(self.hyperSpeed - minSpeed) / (float)(maxSpeed - minSpeed)) * maxMuseSpeed;
			float fullMuse = maxMuseSpeed + Mathf.Abs(minMuseSpeed);
			museSpeed = (((float)(self.hyperSpeed - fullMinSpeed) / (float)(fullMaxSpeed - fullMinSpeed)) * fullMuse) + minMuseSpeed;
			if (brace != 1) {
				museSpeed -= 0.3f;
			}
			//Debug.Log(swingPower);
			//Debug.Log("ms: " + museSpeed + " hyp: " + self.hyperSpeed + " spdRng: " + fullMinSpeed + " - " + fullMaxSpeed);
			/*
			if (museSpeed < 0) {
				museSpeed = 0;
			}
			if (museSpeed > maxMuseSpeed) {
				museSpeed = maxMuseSpeed;
			}
			if (brace != 1) {
				museSpeed = -0.5f;
			}
			*/
			if (boomBox.S) {
				if (GM.S.getNumPlayers() > 1) {
					boomBox.S.setMusicSpeed (myPlayer.playerNum, museSpeed);
				} else {
					boomBox.S.setMusicSpeed (0, museSpeed);
					boomBox.S.setMusicSpeed (1, museSpeed);
				}
			}
		}
	}

	

	public Vector2Int mobileCenter;

	public Vector2Int getCenter() {
		if (mobileCenter.x < 0) {
			if (body) {
				return body.centerPoint;
			} else {
				return self.centerPoint;
			}
		} else {
			return mobileCenter;
		}
	}

	public void calcPos() {
		for (int k = 0; k < bladeMulti; k++) {
			for (int b = 0; b < blade [k].Count; b++) {
				Vector2Int p = calculateSwordPos (b, k, curPos);
				blade [k] [b].forceMove (p, false);
			}
		}
	}

	float heatAmount = 0.08f;

	public bool canSwing() {
		if (soul.canSwing()) {
			if (active && !beingKnocked && staggered == 0 && !myPlayer.dead && !myPlayer.stagged) {
				if (range == 0) {
					return true;
				}
				if (sheathed || !blade [0] [0].hidden) {
					return true;
				}
			}
		}
		return false;
	}

	int oldDir = 0;
	public int curGoal; // for enemy AI to deflect attacks
	public int droopFail = 20;
	int droopFailC = 0;
	// when blade not heated up or swinging it is in sheathe (only used for Gin thus far)
	public bool sheathed = false;

	public bool swingPath(/*int length*/ int angle, int dir, bool debug){
		if (dir != 0 && canSwing()) {// && (!singleStrike || !readyToSwing)) {
			int cur = (int)curPos;
			if (anglePath.Count > 0) {
				cur = (int)anglePath [anglePath.Count - 1];
			}
			//Debug.Log("swing " + cur + " -> " + angle + " in " + dir);
			int length = Mathf.Abs (angle - cur);
			int meridian = 0;
			if (cur > angle && dir > 0) {
				meridian = 180;
				length = Mathf.Abs (meridian - cur) + Mathf.Abs (-meridian - angle);
			} else if (cur < angle && dir < 0) {
				meridian = -180;
				length = Mathf.Abs (meridian - cur) + Mathf.Abs (-meridian - angle);
			}
			int count = anglePath.Count;
			if (debug) {
				Debug.Log ("lenght: " + length + " + " + count);
			}

			if (length > 210 && name == "Sword0") {
				//if (debug || name == "Sword1") {
					//
				//seems to be cause by a mismatch in the input position and the swords current posiiton
				//return false;
				//}
				Debug.LogError(name + " LONG swing " + length + " " + dir);
				Debug.Log("   swing: " + cur + " -> " + angle + " " + dir + " " + length);
				debug = true;
				return false;
			} else {
				//Debug.Log ("swing: " + cur + " -> " + angle + " " + dir + " " + length);
			}
			if (!swinging || oldDir != dir) {
				if (!singleStrike) {
					if (anim) {
						anim.setAction(1);
					}
				}
				if (Grader.S) {
					Grader.S.swingCommence(self);
				}
			}
			//Debug.Log ("swinging path " + readyToSwing);
			curGoal = angle;
			swinging = true;
			// happens everytime for singlestrikers
			if (anglePath.Count == 0 && singleStrike && sheathed) {
				anglePathAdd(angle);
				myPlayer.setPoise (2);
				return true;
			}


			//Debug.Log("now swinging");
			oldPos = curPos;
			/*
			if (boomBox.S) {
				boomBox.S.upTempo (0.05f);
			}
			*/
			float c = cur * Mathf.Deg2Rad;
			float a = angle * Mathf.Deg2Rad;
			//Debug.Log("dirs: " + c + " -> " + a + " : " + (Mathf.Atan2(c,a) * Mathf.Rad2Deg) + " " + (Mathf.Atan2(a, c) * Mathf.Rad2Deg));
			//length = Mathf.Abs (cur - angle);
			if (count+length > maxStrikes) {
				//Debug.Log("couldn't add");
			}
			if (cur != angle) {// && count + length <= maxStrikes) {
				myPlayer.setPoise (2);
				addPath(cur, dir, length);
				
				oldDir = dir;
				curDir = dir;
				if (name == "Sword0") {
					//Debug.LogWarning(	cur + " -> " + anglePath[anglePath.Count-1]);
				}
				return true;
			}
		} else {
			//Debug.Log("cant swing " + dir);
			cantSwing();
			//Debug.LogError ("no swing kncok - " + beingKnocked + " stag - " + staggered);
		}
		return false;
	}

	void anglePathAdd(int angle) {
		if (angle > 180) {
			Debug.LogError("trying to add bad angle " + angle);
		} else {
			anglePath.Add(angle);
		}
	}

	void cantSwing() {
		if (pGraphics && GM.S.drawSprites) {
			pGraphics.cantSwing(true);//setMouth (2);
			droopFailC = droopFail;
		}
	}

	void findArc(int length, int dir) {
		//Debug.Log ("adding length " + length);
		if (length != 0) {
			if (oldDir != 0 && dir != oldDir) {
				//Debug.Log ("direction change");
			}
			//Debug.Log("od: " + oldDir + ", swing: " + curSwing + ", D " + curSwingD);
			if (oldDir == dir && curSwing != 0 && dir == curSwingD) {
				curSwing += length;
				//Debug.Log (length + " = curswing: " + curSwing);
			} else {
				//Debug.Log("interval: " + intervals.Count);
				if (intervals.Count > 0 && intervals [intervals.Count - 1].y == dir) {
					//Debug.Log (length + " fits together with " + intervals [intervals.Count - 1]);
					Vector2Int tmp = new Vector2Int (intervals [intervals.Count - 1].x + length, dir);
					intervals [intervals.Count - 1] = tmp;
				} else {
					Vector2Int tmp = new Vector2Int (length, dir);
					intervals.Add (tmp);
					//Debug.Log ("newLength: " + length);
				}
				if (curSwing == 0 && intervals.Count > 0) {
					curSwing = intervals [0].x;
					//Debug.Log(curSwing + "curSwing from interval");
					curDir = curSwingD = intervals[0].y;
					swingi = 0;
					intervals.RemoveAt (0);
					calcSpeed ();
					musicSpeed ();
				}
			}
		}
		//Debug.Log ("from length: " + length + "  curSwing " + curSwing);
	}

	void addPath(int angle, int dir, int power) {
		//Debug.Log("adding path, angle: " + angle + " dir: " + dir + " pow: " + power);
		int start = angle;
		if (angle == 180) {
			angle = 179;
		} else if (angle == -180) {
			angle = -179;
		}
		if (oldDir != 0 && dir != oldDir) {
			//Debug.Log("dir change");
			//power += weight;
		}
		findArc(power, dir);
		//angle = saneAddition (angle, 1, dir);
		while (power > 0 && anglePath.Count < maxStrikes) {
			angle = ic.saneAddition (angle, 1, dir);

			if (Mathf.Abs (angle) > 180) {
				Debug.LogError (start + " " + angle);
			} else {
				anglePathAdd (angle);
			}
			/*
			angle += dir;

			if (Mathf.Abs (angle) == 180) {
				angle *= -1;
			}
			*/
			power--;
		}
		if (anglePath.Count > 0) {
			if (singleStrike) {
				//readyToSwing = true;
			}
		}
	}

	public void weightSwing(int dir) {
		if (!swinging || beingKnocked) {
			//Debug.LogError("weight swing from no swing, I think I am Gin and this is a bug, nd also maybe this should be returned here");
			//swinging = true;
			return;
		}
		if (pGraphics && GM.S.drawSprites) {// && singleStrike) {
			pGraphics.swingBegin();//setMouth (3);
			//pGraphics.setEyes (3);
		}
		if (weight > 0) {
			int start = (int)curPos;
			if (anglePath.Count > 0) {
				start = anglePath [anglePath.Count - 1];
			}
			addPath (start, dir, weight);
		}
		if (anglePath.Count > 0) {
			if (singleStrike) {
				//readyToSwing = true;
				if (!canSwing()) {
					Debug.LogError("I am swing when I shouldn't " + myPlayer.dead);
					clearPath(69);
					swinging = false;
				} else {
					readyToSwing = true;
					swingCounter = swingPause;
					soul.swingPauseBegin();
					//Debug.Log("single strike swing " + curPos + " " + anglePath[0]);
				}
			}
		}
	}
		
	public void die(){
		//Debug.Log("dying");
		droopFailC = -1;
		soul.reset ();
		brace = 1;
		erasePresence (false);
		resetStats ();
		curPos = startPos;
		curSwing = 0;
		swingi = 0;
		clearFriction();
		knockEtheral (false);
		beingKnocked = false;
		swinging = false;
		readyToSwing = false;
		clearPath(69);
	}

	public void respawn(float spawnPos) {
		if (blade != null) {//.Length >= bladeMulti) {
			float willPos = soul.getSaveAngle();
			if (willPos != 420) {
				curPos = willPos;
			} else {
				curPos = spawnPos;
			}
			revealSelf ();
			for (int k = 0; k < bladeMulti; k++) {
				for (int i = 0; i < blade[k].Count; i++) {
					blade[k][i].transform.position = new Vector3(blade[k][i].centerPoint.x, blade[k][i].centerPoint.y, 0);
				}
			}
			//called after so that swords can choose tohide themselves immediately
			soul.respawn();
		}
	}

	public void erasePresence(bool dodging){
		if (!singleStrike || (singleStrike && dodging)) {
			clearPath (1);
		}
		soul.erasePresence(dodging);
		removeSelf(true);
	}

	public void removeSelf(bool stayHidden) {
		for (int k = 0; k < bladeMulti; k++) {
			for (int i = 0; i < blade[k].Count; i++) {
				blade [k][i].erasePresence (stayHidden);
			}
		}
	}

	public bool revealSelf(){
		if ((range <= 1 || blade [0] [0].hidden) && (!myPlayer || !myPlayer.dodging)) {
			int dir = 1;
			int noLoop = 0;
			float start = curPos;
			//Debug.Log (name + " revealing at " + curPos);
			while (!checkBody (curPos, false) && noLoop < 360) {
				curPos = ic.saneAddition(curPos, 1, 1);
				//Debug.Log("curPos: " + curPos);
				noLoop++;
			}
			//Debug.Log (name + " " + curPos + " " + noLoop);
			if (noLoop < 360) {
				for (int k = 0; k < bladeMulti; k++) {
					for (int b = 0; b < blade [k].Count; b++) {
						blade [k][b].erasePresence (false);
						Vector2Int p = calculateSwordPos (b, k, curPos);
						//Debug.Log(b + " " + p);
						blade [k] [b].forceMove (p, false);
					}
				}
				soul.revealSelf();
			} else {
				//Debug.LogError ("no good move for " + name + " reappearing");
				StartCoroutine (tryToReappearAgain ());
				return false;
			}
			return true;
		}
		return false;
	}

	IEnumerator tryToReappearAgain() {
		yield return new WaitForSeconds (0.5f);
		revealSelf ();
	}

	public void goEtheral(bool nEtheral) {
		for (int k = 0; k < bladeMulti; k++) {
			for (int i = 0; i < blade[k].Count; i++) {
				blade [k] [i].goEtheral (nEtheral);
			}
		}
	}

	int baseHeight;
	int knockHeightMod = 5;
	public void knockEtheral(bool etheral) {
		for (int k = 0; k < bladeMulti; k++) {
			for (int i = 0; i < blade[k].Count; i++) {
				if (etheral) {
					blade [k] [i].height = baseHeight + knockHeightMod;
				} else {
					blade [k] [i].height = baseHeight;// knockHeightMod;
				}
			}
		}
	}

	Vector2Int tipCheck;
	Vector2Int tip2Check;
	public bool adjustToPlayerMove(Vector2Int move){
		//if (pos < anglePath.Count) {
		if (!active) {
			return true;
		}
		float tmpCur = curPos;
		Vector2Int tmpPo = body.centerPoint;
		//Debug.Log ("checking move");
		if (anglePath.Count != 0) {
			//Debug.Log ("swing while adjusting");
		}
		if (checkMove (move, false)) {//!myPlayer.dodging) || myPlayer.dodging) {
			//	Debug.Log("adjusting sword");
			for (int k = 0; k < bladeMulti; k++) {
				for (int i = 0; i < blade [k].Count; i++) {
					Vector2Int p = calculateSwordPos (i, k, curPos);
					/*
					if (network) {
						blade [k] [i].forceMoveNetwork (p.x, p.y, false);
					} else {*/
					//Debug.Log (p);
					if (i == blade [k].Count - 1) {
						if (p != tipCheck) {
							//Debug.Log (blade[k][i].centerPoint + " " + p + " oh no " + tipCheck + " me - " + body.centerPoint);
						}
					}
					if (i == blade [k].Count - 2) {
						if (p != tip2Check) {
							//Debug.Log (blade[k][i].centerPoint + " " + p + " oh no " + tip2Check + " me - " + body.centerPoint);
						}
					}
					blade [k] [i].forceMove (p, false);
					//		Debug.Log (blade[k][i].name + " adj: " + p);
					if (body.centerPoint != tmpPo || tmpCur != curPos) {
						Debug.LogError ("uh oh looks like this sword may go into a wall");
						Debug.Log (tmpCur + " " + curPos + " / " + tmpPo + body.centerPoint);
					}
				}
			}
			return true;
		} else {
			Debug.Log ("cant adjust");
			return false;
		}
		//}
	}

	void clearChecks() {
		checkPoses.Clear ();
		checkCenter = new Vector2Int (-1, -1);
		checkAngle = 365;
	}

	public void forceMove() {
		//Debug.Log("forcign to move to pos: " + curPos);
		bool goodMove = true;
		for (int k = 0; k < bladeMulti; k++) {
			for (int i = 0; i < blade [k].Count; i++) {
				Vector2Int p = calculateSwordPos (i, k, curPos);
				// idk this causes root anim to malfunction, when the flower is spawned
				if (i < checkPoses.Count && checkPoses [i] != p && bladeMulti < 2) {
					//Debug.LogError (name + " has a discrepancy at " + i + " " + checkPoses [i] + " " + p);
					//goodMove = false;
				}
				blade [k] [i].forceMove (p, false);
			}
		}
		if (!goodMove) {
			//	Debug.Log (checkPoses.Count + " " + checkCenter + " " + checkAngle);
			//	Debug.Log(body.centerPoint + " " + curPos);
		} else {
			if (soul) {
				soul.move();
            }
            if (anim) {
				anim.setRotation(curPos);
			}
        }
        clearChecks();//checkPoses.Clear ();
	}

	public bool checkMove(Vector2Int move, bool colliding){
		if (!active || range <= 1 || blade [0] [0].hidden) {
			return true;
		}
		bool checkGood = true;
		//Debug.Log (move);
		Vector2Int pos = body.centerPoint + GM.S.dirs[body.direction];//new Vector2Int((int)(body.centerPoint.x + Mathf.Sign(move.x)), (int)(body.centerPoint.y + Mathf.Sign(move.y)));
		checkCenter = pos;//body.centerPoint;
		checkAngle = curPos;

			for (int k = 0; k < bladeMulti; k++) {
				for (int i = 0; i < blade [k].Count; i++) {
					Vector2Int p = calculateSwordPos (i, k, curPos, pos);
					blade [k] [i].checkBody (p, colliding, false);
					checkPoses.Add (p);
					if (blade [k] [i].curCollided.Count != 0) {
						checkGood = false;
					}
					if (colliding) {
						hitSomething (k, i/*blade [k] [i].curCollided*/);
					}
				}
			}
		//}
		return checkGood;
	}
	//not used anymore
	public bool checkWithPlayerMove(Vector2Int move, bool colliding, int poo){
		if (!active ||/* (myPlayer.dodging && myPlayer.getDR() == -1)  ||*/ range <= 1 || blade[0][0].hidden) {
			return true;
		}
		int i = 0;
		int dir = findDirection(1, false);
	//	Debug.Log ("direction = " + dir);
		float start = curPos;
		if (dir != 0) {
			while (!checkMove (move, colliding) && i < 180) {
				colliding = false; // to prevent many hits from single stab
				if (checkMove (dir, false, false) < 2) {
					for (int k = 0; k < bladeMulti; k++) {
						for (int b = 0; b < blade [k].Count; b++) {
							Vector2Int p = calculateSwordPos (b, k, curPos + dir);
							blade [k] [b].forceMove (p, false);
						}
					}
					curPos += dir;
					Debug.Log (name + " " + dir);
					if (Mathf.Abs(curPos) > 180) {
						Debug.LogError ("with player move " + start + " " + dir + " " + curPos);
					}
					if (Mathf.Abs (curPos) == 180) {
						curPos *= -1;
					}

				} else {
					i = 180;
				}
				i++;
			}
			//Debug.Log ("hit " + i);
			if (i >= 180) {
				return false;
			} else {
				return true;
			}
		} else {
			Debug.LogError ("0 dir");
			if (checkMove (move, colliding)) {
				//Debug.Log ("I should be able to move chuck!");
				return true;
			}
			return false;
		}
	}

	public int checkPlayerMove(Vector2Int move, bool colliding) {
		int maxCheck = 180;
		if (!active /*|| myPlayer.dodging */ || range <= 1 || blade[0][0].hidden) {
			return 0;
		}
		int i = 1;

		//Vector2Int playerCenter = new Vector2Int ((int)(body.centerPoint.x + Mathf.Sign (move.x)), (int)(body.centerPoint.y + Mathf.Sign (move.y)));
		Vector2Int playerCenter = body.centerPoint + GM.S.dirs [body.direction];
		//Debug.Log(playerCenter);//Debug.Log("player center: " +playerCenter + " vs " + pc);
		//Debug.Log (playerCenter + " " + move + " " + Mathf.Sign(move.x));

		int dir = 0;
		
		while (dir == 0 && i < 45) {
			dir = findDirection (i, playerCenter, false);
			i++;
		}
		if (dir == 0) {
			dir = 1;
		}
		i = 1;
		//Debug.Log (dir + " " + checkMove (i * dir, false, false, playerCenter));
		//dir = 1;
		//checkPoses.Clear ();
		clearChecks();
		while (checkMove(i * dir, false, false, playerCenter) >= 1 && i <= maxCheck) {
			i++;
			//checkPoses.Clear ();
			clearChecks();
		}
		if (i >= maxCheck) {
			Debug.Log("max check reached");
			dir *= -1;
			i = 1;
			checkPoses.Clear ();
			while (checkMove(i * dir, false, false, playerCenter) >= 1 && i <= maxCheck) {
				i++;
				//checkPoses.Clear ();
				clearChecks();
			}
		}
		//Debug.Log("last pos checked: " + playerCenter + " " + (curPos + (i * dir)));
		if (i >= maxCheck) {
			return 365;
		} else {
			return i * dir;
		}
	}
	// moves blade outside bounds sometimes
	// 0 no hits, 1 no motion & 2 hit something
	public int checkMove(int mod, bool colliding, bool debug) {
		int checkGood = -1;
		if (debug) {
			Debug.Log ("checking move " + (curPos + mod));
		}
		if (range > 1 && !blade [0] [0].hidden) {
			for (int k = 0; k < bladeMulti; k++) {
				for (int i = 0; i < blade [k].Count; i++) {
					Vector2Int p = calculateSwordPos (i, k, curPos + mod);
					Vector2Int dir = p - blade [k] [i].centerPoint;
					if (dir.x == 0 && dir.y == 0) {
						if (checkGood != 2 && checkGood != 0) {
							checkGood = 1;
						}
					} else {
						if (debug) {Debug.Log ("changing D " + blade[k][i].centerPoint + " -> " + p + " " + dir);}
						blade [k] [i].checkBody (p, colliding, false);
						if (blade [k] [i].curCollided.Count != 0) {
							checkGood = 2;
							//if (beingKnocked) {
							List<Form> cc = blade [k] [i].curCollided;
							if (debug) {
								Debug.Log (blade [k] [i].name + " collided with ");
								for (int d = 0; d < cc.Count; d++) {
									Debug.Log (cc [d] + " at " + p);
								}
							}
							return checkGood;
						} else if (checkGood != 2) {
							checkGood = 0;
						}
						if (colliding) {
							if (checkGood > 0) {
								// when we are checking the future position of the sword, when that swordcheck fails, we need to bring back the pos, so we do walk our sword into invalid areas
								//curPos += curDir * -1  * mod;
							}
							hitSomething (k, i/*blade [k] [i].curCollided*/);

						}
					}
					if (debug) {
						Debug.Log ("returned: " + checkGood);
					}
				}
			}
		}
		return checkGood;
	}

	// 0 no hits, 1 no motion & 2 hit something
	public int checkMove(int mod, bool colliding, bool debug, Vector2Int newCenter){
		int checkGood = 0;
		if (debug) {
			Debug.Log ("checking move " + (curPos + mod));
		}
		checkCenter = newCenter;
		checkAngle = curPos + mod;
		if (range > 1 && !blade [0] [0].hidden) {
			for (int k = 0; k < bladeMulti; k++) {
				for (int i = 0; i < blade [k].Count; i++) {
					Vector2Int p = calculateSwordPos (i, k, curPos + mod, newCenter);
					if (debug) {Debug.Log("checking " + p); }
					Vector2Int dir = p - blade [k] [i].centerPoint;
					checkPoses.Add (p);
					if (dir.x == 0 && dir.y == 0) {
						if (checkGood != 2) {
							checkGood = 1;
						}
					} else {
						//if (debug) {Debug.Log ("changing D " + blade[k][i].centerPoint + " -> " + p + " " + dir);}
						blade [k] [i].checkBody (p, colliding, false);
						if (blade [k] [i].curCollided.Count != 0) {
							checkGood = 2;
							//if (beingKnocked) {
							List<Form> cc = blade [k] [i].curCollided;
							if (debug) {
								Debug.Log (blade [k] [i].name + " collided with ");
								for (int d = 0; d < cc.Count; d++) {
									Debug.Log (cc [d] + " at " + p);
								}
							}
							//}
						}
						if (colliding) {
							if (checkGood > 0) {
								// when we are checking the future position of the sword, when that swordcheck fails, we need to bring back the pos, so we do walk our sword into invalid areas
								//curPos += curDir * -1  * mod;
							}
							//maybe put back in
							hitSomething (k, i/*blade [k] [i].curCollided*/);
						}
					}

				}
			}
		}
		if (debug) {
			Debug.Log ("returned: " + checkGood);
		}
		return checkGood;
	}

	public bool checkBody(float pos, bool colliding){
		return checkBody (getCenter (), pos, colliding);
	}

	public bool checkBody(Vector2Int center, float pos, bool colliding){
		bool checkGood = true;	
		if (blade.Length >= bladeMulti) {
            for (int k = 0; k < bladeMulti; k++) {
                for (int i = 0; i < blade[k].Count; i++) {
                    Vector2Int p = calculateSwordPos(i, k, pos, center);
                    blade[k][i].checkBody(new Vector2Int(p.x, p.y), colliding, false);
                    if (blade[k][i].curCollided.Count != 0) {
                        checkGood = false;
                        for (int j = 0; j < blade[k][i].curCollided.Count; j++) {
                            //Debug.Log ("body check: " + blade [k] [i].curCollided [j]);
                        }
                    }
                    if (colliding) {
                        hitSomething(k, i/*blade [k] [i].curCollided*/);
                    }
                }
            }
		}
        return checkGood;
	}

	public void /*List<Form>*/checkBody(float pos) {
		/*return*/ checkBody (getCenter (), pos);
	}

	List<Form> hitList;

	public void /*List<Form>*/ checkBody (Vector2Int center, float pos) {
		//Debug.Log ("checking body");
		//List<Form> hitList = new List<Form> ();
		//List<Form> curList = new List<Form> ();
		self.curCollided.Clear();
		Form cur = null;
		Form bChunk = null;
		for (int k = 0; k < bladeMulti; k++) {
			for (int i = 0; i < blade [k].Count; i++) {
				Vector2Int p = calculateSwordPos (i, k, pos, center);
				bChunk = blade [k] [i];
				bChunk.checkBody (p, false, true);
				//Debug.Log ("checking body at " + p + " " + i);
				//curList = blade [k] [i].curCollided;
				for (int j = 0; j < bChunk.curCollided.Count; j++) {
					cur = bChunk.curCollided [j];
					if (!self.curCollided.Contains (cur)) {
						self.curCollided.Add (cur);
						//Debug.Log ("cantcha " + p + " " + curList[j]);
					}
				}
			}
		}
		//return hitList;
	}

	//specifically for things we cant go inside, ie otehr swords not bodies
	void hitSomething(int i1, int i2/*List<Form> hitList*/) {
		//Debug.Log ("did i hit something");
		//anglePath.Clear();
		List<Form> hitList = blade[i1][i2].curCollided;
		if (hitList.Count > 0) {
			/*
			if (Grader.S) {
				if (swinging && !beingKnocked) {
					Grader.S.makeHit(body);
				}
			}
			*/
			for (int k = 0; k < hitList.Count; k++) {
				// perhaps there is a better solution, but when two attacks hit eachother 
				// the other group gets destroyed before this loop is over
				if (hitList [k]) {
					Debug.Log (hitList [k].name);
					//defense d = hitList [k].gameObject.GetComponent<defense> ();
					Value v = hitList [k].gameObject.GetComponent<Value> ();
					Knockable kn = hitList [k].gameObject.GetComponent<Knockable> ();
					if (v) {
						Form otherPlayer = hitList [k].parent;
						SwordDummy sd = null;
						circleAttack ca = null;
						float otherPower = v.getValue ("knockBack");
						// 4 is Shade block id, or perhaps anything created by a sword that we dont want their player to collide with(via player parented over the creation)
						if (otherPlayer && hitList[k].id != 4) {
							Debug.Log ("we got another player " + otherPlayer.name);
							Player op = otherPlayer.GetComponent<Player>();
							if (op) {
								ca = op.myAttack;
							} else {
								ca = otherPlayer.GetComponent<circleAttack>();
								sd = otherPlayer.GetComponent<SwordDummy>();
							}
						}
						if (ca) {
							otherPower = ca.calcStat(ca.getMass());
						}
						
						float ourPower = calcStat(getMass());
						//heat exchange
						if (ca) {
							soul.hitSword(ca);
						}
						Debug.Log("other's power: " + otherPower);
						if (otherPower != -1 && otherPower < ourPower) {
							Debug.Log(name + " we beat them ");
							if (sd) {
								sd.getHit(ourPower);
							}
							if (boomBox.S) {
								boomBox.S.upTempo (1);
							}
							// we beat other sword

							if (ca) {
								if (Grader.S) {
									Grader.S.makeHit(body, ourPower);
								}
								//ca.getStagger(blade [0] [0].gameObject.GetComponent<Value> ().getValue ("stagTime") / 10);
								//do I need to stagger the sword, knocking already locks player control
								ca.knockBack (0, (int)(ourPower - otherPower), self.hyperSpeed, true);// - (otherPower * 0.8f)));
							}
						} else {
							clearPath (2);
						}

					} else {
						// maybe a better check, or make it bounce based on its current speed
						//but if its barely moving we dont want it bouncing i think, so that when we push it against a wall and move back it moves back
						if (self.hyperSpeed > minSpeed) {
							// we killed it presumably
							if (!hitList[k].dead) {// || hitList[k].gameObject == null) {
								bounce(0.5f);
							}
						} else {
							clearPath(9);
						}
					}

					if (kn) {
						Vector2 dir = getVelocity(i1, i2);
						//Debug.Log("velocity: " + dir);
						kn.knock (dir);
					}
				}
			}
		}
		//Debug.Log ("hit over");
	}

	public void bounce(float resistance) {
		beingKnocked = false;
		//removeSelf(false);
		//Debug.Log (hitList[k].name + "got hiy " + curDir + " " + anglePath.Count);
		int pow = (int)(Mathf.Max(1, (anglePath.Count / 4) * resistance));
		clearPath (5);
		if (checkMove(curDir * -1, false, false, body.centerPoint) != 2) {
			//curPos += curDir * -1;
			curPos = ic.saneAddition(curPos, 1, curDir * -1);
			//forceMove ();
			knockBack (curDir * -1, pow, self.hyperSpeed, false);
		}
	}

	public Vector2 getVelocity(int blade, int segment) {
		Vector2 cur = calculateSwordPos (segment, blade, curPos);
		Vector2 old = calculateSwordPos (segment, blade, oldPos);
		//Debug.Log("knock: " + old + " -> " + cur);
		Vector2 dir = (new Vector2 (cur.x - old.x, cur.y - old.y)).normalized;
		if (dir.x == 0 && dir.y == 0) {
			dir = (new Vector2 ((float)Mathf.Cos(curPos * Mathf.Deg2Rad), (float)Mathf.Sin(curPos * Mathf.Deg2Rad))).normalized;
			//Debug.Log("0 velocity became " + dir);
		}
		return dir;
	}

	public void getStagger(float stagAmnt) {
		clearPath (13);
		staggered = stagAmnt / stagRecover;
		Debug.Log (name + " got staggered " + staggered);
	}

	public void knockBack(int direction, int power, int speed, bool flare) {
		//Debug.LogError (name + " got knocked " + direction + " " + power);
		if (!beingKnocked && power > 1) {
			self.hyperSpeed = speed;
			beingKnocked = true;
			int d = direction;
			int dis = 1;
			while (d == 0 && dis < 10) {
				//Debug.Log (dis);
				for (int i = 0; i < 3; i++) {
					int check = checkMove (dis + i, false, false);
					if (check == 0) {
						d = 1;
					} else {
						d = 0;
						if (check == 2) {
							//Debug.Log ("hit something no need to keep gpoing in dir");
							break;
						}
					}
					//Debug.Log (d);
				}
				if (d == 0) {
					for (int i = 0; i < 3; i++) {
						int check = checkMove (-dis - i, false, false);
						if (check == 0) {
							d = -1;
						} else {
							d = 0;
							if (check == 2) {
								//Debug.Log ("hit something no need to keep gpoing in dir");
								break;
							}
						}
					}
				}
				dis += 2;

			}
			if (d != 0) {
				curDir = d;
				curSwingD = d;
				int angle = (int)curPos;
				if (direction == 0) {
					clearPath (6);
				}

				//ic.stopSwingInput();
				//Debug.Log ("hit with : " + direction + " at angle: " + angle);
				addPath(angle, d, power * 10); // increase knockback for all swords? Maybe makes a stat so some go further and other stays where they are? maybe the sword's weight
				knockEtheral(true);
				soul.knock (flare);
				cantSwing();
				//swingPath (angle, d, inertia);
			} else {
				beingKnocked = false;
			}
		} 
	}

	int findDirection(int mod, bool debug){
		int d = 0;
		if (checkMove (mod, false, debug) < 2) {
			d = 1;
		} else if (checkMove (-mod, false, debug) < 2) {
			d = -1;
		}
		if (debug) {
			Debug.Log (name + " " + d);
		}
		return d;
	}

	int findDirection(int mod, Vector2Int center, bool debug){
		int d = 0;
		if (checkMove (mod, false, debug, center) < 2) {
			d = 1;
		} else if (checkMove (-mod, false, debug, center) < 2) {
			d = -1;
		}
		if (debug) {
			Debug.Log (name + " " + d);
		}
		return d;
	}

	public int getDirFromPoint(Vector2Int point) {
		//Debug.Log ("point: " + point);
		int modCheck = 5;
		float pDist = 0;
		float nDist = 0;
		for (int k = 0; k < bladeMulti; k++) {
			for (int i = 0; i < blade [k].Count; i++) {
				Vector2Int p = calculateSwordPos (i, k, curPos + modCheck);

				Vector2Int n = calculateSwordPos (i, k, curPos - modCheck);
				//Debug.Log ("p pos = " + p + " n pos = " + n);
				pDist += Vector2Int.Distance(p, point);
				nDist += Vector2Int.Distance(n, point);
			}
		}
		//Debug.Log ("pDist: " + pDist + " nDist: " + nDist);
		if (pDist < nDist) {
			return 1;
		} else {
			return -1;
		}
	}

	public bool inside;

	public void applyFriction(float newFric) {
		//if (self.hyperSpeed < (maxSpeed - minSpeed) / 4) {
		if (swingi > curSwing * 0.9f) {
			//clearPath (9);
		}
		if (newFric < friction) {
			friction = newFric;
		}
		if (myPlayer) {
			// to apply firction to walking animation
			myPlayer.setAccel(myPlayer.getAA());
		}
	}

	public void clearFriction() {
		friction = 1;
		if (myPlayer) {
			myPlayer.setAccel(myPlayer.getAA());
		}
	}

	public float getFriction() {
		return friction;
	}

	public void clearPath(int num) {
		//Debug.Log ("clear" + num);
		//StopAllCoroutines ();
		clearChecks();
		clearFriction();
		intervals.Clear();
		curSwing = 0;
		swingi = 0;
		self.hyperSpeed = minSpeed;
		anglePath.Clear ();
		resetStats ();
	}
		
	public bool growBlade(int minSize, int maxGrowth) {
		//Debug.Log("growing now to " + (blade [0] [0].width + 1));
		// checking blade growth
		if (body.etheral) {
			// when etheral we can push ourselves int oa wall, so its easier if we just wait
			return false;
		}
		int goodGrow = 1;
		for (int k = 0; k < bladeMulti; k++) {
			for (int i = 0; i < blade [k].Count; i++) {
				if (blade [k] [i].width - minSize < maxGrowth) {
					blade [k] [i].width++;
					blade [k] [i].length++;
				} else {
					goodGrow = 0;
					break;
				}
			}
			if (goodGrow == 0) {
				break;
			}
		}
		if (goodGrow != 0) {
			for (int k = 0; k < bladeMulti; k++) {
				for (int i = 0; i < blade [k].Count; i++) {
					blade [k] [i].squareBody ();
					Vector2Int pos = calculateSwordPos (i, k, curPos);
					blade [k] [i].checkBody (pos, false, false);
					if (blade[k][i].curCollided.Count != 0) {
						int clamp = 3;
						Vector2Int dir = (body.centerPoint - pos);
						dir.Clamp(new Vector2Int(-clamp,-clamp), new Vector2Int(clamp,clamp));
						Vector2Int pushPlayer = body.centerPoint + dir;
						body.checkBody (pushPlayer, false, false);
						if (body.curCollided.Count == 0) {
							body.forceMove (pushPlayer, true);
						} else {
							goodGrow = -1;
							break;
						}
					}
				}
				if (goodGrow == -1) {
					break;
				}
			}
			//if (goodGrow != -1) {
			for (int k = 0; k < bladeMulti; k++) {
				for (int i = 0; i < blade [k].Count; i++) {
					if (goodGrow == -1) {
						blade [k] [i].width--;// = ow;
						blade [k] [i].length--;
						blade [k] [i].squareBody ();
					} else {
						Form f = blade [k] [i];
						f.transform.GetChild (0).GetChild (0).transform.localScale = new Vector3 (f.width * 13f, f.length * 13f, 1);
						if (f.width % 2 == 0) {
							f.transform.GetChild (0).GetChild (0).position = f.transform.position + new Vector3 (-0.5f, -0.5f, 0f);
						} else {
							f.transform.GetChild (0).GetChild (0).position = f.transform.position + new Vector3 (0f, 0f, 0f);
						}
					}
					Vector2Int pos = calculateSwordPos (i, k, curPos);
					blade [k] [i].forceMove (pos, false);
				}
			}
		} 
		if (goodGrow > 0) {
			calculateSwordRange ();
			return true;
		} else {
			return false;
		}
	}

	public void setGrowth(int size) {
		//Debug.Log ("setting growth to " + size);
		erasePresence (false);
		for (int k = 0; k < bladeMulti; k++) {
			for (int i = 0; i < blade [k].Count; i++) {
				blade [k] [i].width = size;// = ow;
				blade [k] [i].length = size;
				Form f = blade [k] [i];
				if (f.transform.childCount > 0 && f.transform.GetChild(0).childCount > 0) {
					f.transform.GetChild (0).GetChild (0).transform.localScale = new Vector3 (f.width * 13f, f.length * 13f, 1);
					if (f.width % 2 == 0) {
						f.transform.GetChild (0).GetChild (0).position = f.transform.position + new Vector3 (-0.5f, -0.5f, 0f);
					} else {
						f.transform.GetChild (0).GetChild (0).position = f.transform.position + new Vector3 (0f, 0f, 0f);
					}
				}
				blade [k] [i].squareBody ();
				Vector2Int pos = calculateSwordPos (i, k, curPos);
				blade [k] [i].forceMove (pos, false);
			}
		}
		calculateSwordRange ();
		revealSelf ();
	}

	public void setBladeHeight(int newHeight) {
		//Debug.Log ("new height: " + newHeight);
		baseHeight = newHeight;
		for (int k = 0; k < bladeMulti; k++) {
			for (int i = 0; i < blade [k].Count; i++) {
				blade [k] [i].height = newHeight;
			}
		}
	}

	public Form getBody() {
		return body;
	}

	public SwordSoul getSoul() {
		return soul;
	}

	public bool isHidden() {
		if (range > 1) {
			return blade [0] [0].hidden;
		} else {
			return false;
		}
	}

	public void hitPlayer() {
		//Debug.LogError ("hit player");
		soul.hitPlayer ();
	}

	public void dealDamage(Vector2Int damagePos, Transform receiver) {
		soul.dealDamage(damagePos, receiver);
	}

	public int getSwingLength() {
		return curSwing;
	}

	public float getSwingPos() {
		return swingi;
	}

	public Vector2 getSpeed() {
		return new Vector2 (minSpeed, maxSpeed);
	}

	public int getCurrentSpeed() {
		return self.hyperSpeed;
	}

	public Vector2 getKnockBack() {
		return knockBackDamage;
	}

	public Form getChunkForm() {
		if (range > 1) {
			return blade [0] [0];
		} else {
			return null;
		}
	}

	public int getBladeWidth() {
		if (range > 1) {
			return blade [0] [0].width;
		} else {
			return 1;
		}
	}

	public void setTaperAngle(int multiSpacing, float taper) {
		multiAngleSpacing = multiSpacing;
		taperMulti = taper;
	}

	public void setPos(int pos) {
		curPos = pos;
	}
	public GameObject getChunk() {
		return chunk;
	}
	public int getWeight() {
		return weight;
	}

	public Vector2 getMass() {
		return mass * brace;
	}

	public void setWeight(int n_weight) {
		weight = n_weight;
	}
	public int getDir() {
		return curDir;//curSwingD;
	}

	public int getHyp() {
		return self.hyperSpeed;
	}

	public float getHeatAmount() {
		return heatAmount;
	}

	public Player getPlayer() {
		return myPlayer;
	}

	public bool checkSpeed(int diff) {
		//Debug.Log("current speed: " +self.hyperSpeed + " diff: " + Mathf.Abs(self.hyperSpeed - maxSpeed));
		return Mathf.Abs(self.hyperSpeed - maxSpeed) <= diff;
	}

	public Vector2 getBladePlayerCenter() {
		/*
		int midPos = 0;
		if (bladeMulti != 1) {
			midPos = bladeMulti / 2;
		}
		Vector2Int center = getCenter ();
		if (range > 1 && !blade [0] [0].hidden) {
			center += calculateSwordPos ((int)(range - 1), midPos, curPos);
			center.x /= 2;
			center.y /= 2;

		}
		*/
		if (mobileCenter.x < 0) {
			return new Vector2(body.transform.position.x, body.transform.position.y);
		} else {
			return new Vector2(mobileCenter.x, mobileCenter.y);
		}
		//return center;
	}

	public void startSwingInput(int angle) {
		soul.startSwingInput(angle);
		if (pGraphics && GM.S.drawSprites) {
			if (!myPlayer.stagged && !myPlayer.dead && getCenter() == body.centerPoint) {
				pGraphics.swingInput(true);
			}
		}
	}

	public void stopSwinging() {
		//Debug.Log ("stopping swing anim");
		if (pGraphics && GM.S.drawSprites) {
			if (anglePath.Count == 0) {
				pGraphics.swingInput(false);
			}
		}
	}

	public int team = -1;
	// called before spawning, then every subsequent blade chunk has their team updated at spawning
	public void setTeam(int newTeam) {
		//Debug.Log ("sword team: " + team);
		team = newTeam;
		if (blade != null) {// && blade[0][0]) {
			for (int k = 0; k < bladeMulti; k++) {
				for (int i = 0; i < blade [k].Count; i++) {
					Value v = blade[k][i].GetComponent<Value>();
					v.setTeam(team);
				}
			}
		}
	}
}
