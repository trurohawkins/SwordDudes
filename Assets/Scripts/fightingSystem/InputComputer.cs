using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputComputer : Controller {
	circleAttack targSword;
	SwordSoul targSoul;
	SwordSoul soul;
	SoulStats stats;
	defense def;
	PlayerPathBoi pather;

	public float dodgeCombatChance = 0.2f;
	public float inPlaceDodgeChance = 0f;
	public float dodgePathChance = 0;
	public float dodgeMoveChance = 0;
	public float dodgeMoveCheck = 1;
	public float dodgeCircleChance = 0;
	public float dodgeWallStuck = 0; //0.98f still functional lol
	public int dodgeAngleMiss = 0;// 0 - 90
	public float creepChance = 0f;
	public float burnMisFire = 0f;
	public float burnRunAway = 1;
	public float hitRngAccuracy = 1;//1 - good, > 1 early < 1 late
	public float swingDirAcc = 1;
	public int postSwingInterval = 0;
	public float swingLenAcc = 1;
	public float swingWaitChance = 1;

	public int difficultyLevel = 4;

	public int attackDist = 10;

	int intervalJitter;
	public int attackInterval = 50;
	public int swingLength = 30;


	int postSwingC = 0;
	bool cantSwing = false;

	float hitOffset;
	bool curSwinging = false;
	bool enKnocked = false;
	public float enRange;
	public float scLen;
	public int scUnitDiff;


	int[] adjOrder;
	int[] adjAngs;
	public int enSwordSize;
	//int enRange;
	int mySwordSize;
	public List<int> ignoreListP;
	public List<int> ignoreListS;
	public List<int> ignoreListSP;
	public List<int> ignoreListI;

	int estimate = 3;
	int estCounter = 100;
	public bool dodgeStep;

	int adjAngle = 666;
	int adjSwingD;
	int adjPos;
	int adjDir = 1;
	int swingAng;
	int swingD;
	int swingTime = -1;
	int bladeLook = 40;
	int rngEstimate;
	int rec = -1;
	Vector2Int enDest;
	int enBDir;
	float enInP; //enemy in percent, creeping
	float myInP;
	Vector2Int myDest;
	int myDir;
	int myBDir;
	bool enDodge;
	bool myDodge;
	bool myKnocked;
	float preSwordPos = 666;
	Vector2Int myPos;
	Vector2Int enPos;
	int swingUnitAdj = 3;
	float backAngle = 666;

	public bool burnMe = true;
	float letMeBurn = 0;
	public bool burnNoMatter = false;//doesnt tke into account health when deciding to burn
	public bool ignoreBurn;
	public List<Form> hitList;

	public override void Awake() {
		base.Awake ();
		def = gameObject.GetComponent<defense> ();
		me.inPercent = 1;

		adjOrder = new int[4];
		adjAngs = new int[4];
		for (int i = 0; i < 4; i++) {
			adjOrder [i] = i;
		}
		adjAngs [0] = 80;
		adjAngs [1] = 35;
		adjAngs [2] = 125;

		ignoreListI = new List<int> ();
		ignoreListI.Add (0);
		ignoreListP = new List<int> ();
		ignoreListP.Add (4);//shade block -- destructible
		ignoreListP.Add (1);
		ignoreListP.Add (0);
		ignoreListS = new List<int> ();
		ignoreListS.Add (4);//shade block -- destructible
		ignoreListS.Add (2);
		ignoreListS.Add (0);
		ignoreListSP = new List<int> ();
		ignoreListSP.Add (4);//shade block -- destructible
		ignoreListSP.Add (2);
		ignoreListSP.Add (1);
		ignoreListSP.Add (0);

		hitList = new List<Form> ();
		pather = gameObject.GetComponent<PlayerPathBoi>();
	}

	public void Start() {
		pather = gameObject.GetComponent<PlayerPathBoi>();

	}

	void OnDisable() {
		Debug.Log(name + " is disabled");
		//stopMove();
	}

	public void getPather(int pNum) {
		gameObject.AddComponent<PlayerPathBoi>();
		pather = gameObject.GetComponent<PlayerPathBoi>();
		pather.aiNum = pNum;
	}

    public override void meetSword(circleAttack blade) {
		base.meetSword (blade);
		if (pather) {
			pather.aiNum = me.playerNum;
		}
		if (!attack) {
			attack = me.myAttack;
		}
		soul = attack.gameObject.GetComponent<SwordSoul> ();//attack.getSoul ();
		stats = Instantiate(soul.soulStats, transform).GetComponent<SoulStats>();
		stats.setLevel(difficultyLevel);
		stats.updateAI (this);
		/*
		attackDist = (2 * bodyLen) + attack.attackRange.x - 2;
		scLen = 1.8f;
		scUnitDiff = 3;// based on how fast attack is, the lower the closer a strike is, so lower better for faster swords
		if (attack.maxSpeed > 55) {
			scLen = 21f;
			scUnitDiff = 0;
		} else if (attack.maxSpeed > 41) {
			scLen = 2;
			scUnitDiff = 0;
		} else if (attack.maxSpeed > 25) {
			scUnitDiff = 0;
		}
		//scLen *= swingLenAcc;
		Debug.Log ("AI met sword, got vars " + attackDist);
		distRanges [0] = new Vector3 (minDist, attack.attackRange.x +  bodyLen, -1);
		*/
		updateMySwordInfo ();

		if (burnMe) {
			burnTime (false);
		}
		//moveOK = false; //test purposes
	}

	public void setTarget(circleAttack att, SwordSoul soul) {
		targSword = att;
		targSoul = soul;
	}

	public void updateMySwordInfo(){
		if (attack) {
			attackDist = Mathf.CeilToInt((2 * pather.bodyLen) + attack.attackRange.x - 2);
			scLen = 1.8f;
			scUnitDiff = 3;// based on how fast attack is, the lower the closer a strike is, so lower better for faster swords
			if (attack.maxSpeed > 55) {
				scLen = 21f;
				scUnitDiff = 0;
			} else if (attack.maxSpeed > 41) {
				scLen = 2;
				scUnitDiff = 0;
			} else if (attack.maxSpeed > 25) {
				scUnitDiff = 0;
			}
			//scLen *= swingLenAcc;
			//Debug.Log ("AI met sword, got vars " + attackDist);
			pather.distRanges [0].x = pather.minDist;
			pather.distRanges [0].y = attack.attackRange.x + pather.bodyLen;
			pather.distRanges [0].z = -1;//= new Vector3 (minDist, attack.attackRange.x + bodyLen, -1);
			mySwordSize = attack.getBladeWidth();
			pather.useRangeAttack ();
		}
	}

	public bool swordOK = true;
	public bool dodgePath = true;

	public bool wantToBurn;
	public bool absolutelyDontBurn;
	public bool runAndBurn;
	public bool rangedAttack;
	public float rangeAttack;
	public bool runIfNotFullPowered;
	public bool dontAdjust;
	public bool burnOnHit;

	public override bool think() {
		//not planning to dodge not swing
		if (estCounter > estimate && rec == -1) {
			pather.findTarget ();
		} else {
			//Debug.Log ("ec: " + estCounter + " > " + estimate + " rec: " +rec);
		}
		pather.updateInfo();
		if (!targSword) { // need a full targetting system
			if (pather && pather.targP) {
				targSword = pather.targP.myAttack;
			}
		} else if (!targSoul) {
			if (targSword.getSoul ()) {
				targSoul = targSword.getSoul ();
				if (pather) {
					pather.updateEnemySwordInfo ();
					pather.ready = true;
					pather.lost = true;
				}
			}
		}
		if (targSword) {
			if (enSwordSize != targSword.getBladeWidth() || (int)enRange != (int)(pather.bodyLen + targSword.attackRange.y)) {
				pather.updateEnemySwordInfo ();// for when the enemy sword grows
			}
		}
		if (attack && attack.active) {
			if (mySwordSize != attack.getBladeWidth()) {
				updateMySwordInfo ();
			}
		}
		if (!me.dead && pather.ready) {
			/*
			if (pec == -1) {
				if (((targP.dodging && targP.mDest != GM.S.invalidDest) || !targP.dodging) && (targP.going || targP.knocked)) {
					Vector2Int ppos = findPlayerPos (targP, posEsti);
					Debug.Log ("got esti for " + posEsti + " units: " + ppos);
					pec = 0;
					//Vector2Int mpos = findPlayerPos (me, 10);
				}
			}else {
				if (pec >= posEsti) {
					Debug.Log (target.centerPoint);
					pec = -1;
				} else {
					pec++;
				}
			}
			*/
			//should probly be in sword arts but damn that shit is complicated
			if (pather.blockedByDestructible) {
				if (!attack.swinging) {
					float ab = angleBetween (myPos, enPos);
					int dir = findClosestDir(attack.curPos, ab).x;
					//Debug.Log("destroy " + ab);
					//Debug.Log("closest dir: " + findClosestDir(attack.curPos, ab).x);
					int end = saneAddition(ab, 60, dir);
					//if (soul.energy + getSwingHeat(end, dir) >= soul.burningPoint
					//Debug.Log("heat after " + (soul.energy + getSwingHeat(end, dir)));
					if (soul.energy + getSwingHeat(end, dir) < soul.burningPoint || Random.value < burnMisFire) {
						swingSword(end, dir, true, true);
						//blockedByDestructible = false;
					}
				}
			}
			if (attack.swinging == false && curSwinging == true) {
				//Debug.Log ("after swing state is: " + swordState);
				if (attack.curPos == preSwordPos && swordState == 2) {
					Debug.Log ("swing got blocked probably, but it didn't work");
					pather.getLost ();
				} else {
					if (swordState == 1) {
						if (coolHeatAdj/* && !cantSwing*/) {
							//cantSwing = true;
							//if cooling after adj
							//postSwingC = coolTime - postSwingInterval
							//postSwingC = postSwingInterval - getCoolTime(soul, -1);//getSwingHeat(attack.curPos, attack.cur));
							//Debug.Log(rec + " " + rngEstimate + " " + getCoolTime(soul, -1));
							//rngEstimate += getCoolTime (soul, -1);
						}
					} else if (backAngle != 666) {
						int ba = (int)backAngle;
						backAngle = 666;
						swingSword (ba, -attack.getDir (), false, false);
					}
				}
				swordState = 0;
			}
			curSwinging = attack.swinging;

			if (attack && !attack.swinging) {
				if (postSwingC < postSwingInterval) {
					postSwingC++;
				} else if (cantSwing) {
					cantSwing = false;
				}
			}

			if (pather.running) {
				if (!runAndBurn && !runIfNotFullPowered) {
					if (Mathf.Abs (soul.energy - soul.climate) < Mathf.Abs (soul.energy - soul.burningPoint)) {//are we closer to climate or burning?
						pather.stopRunning ();
					}
				}
			}
		} else {
			//Debug.Log("blcked");
		}
		if (soul && pather.targP && pather.targDef) {
			if (attack.mobileCenter.x != -2 && soul.burning) {
				if (dodgePath) {
					Debug.LogError("turning off dodge path for roar");
					dodgePath = false;
					//stopMove();
					//getLost();
					pather.lost = true;
					pather.closed.Clear();
				}
			} else {
				if (!dodgePath) {
					dodgePath = true;
					pather.lost = true;
					pather.closed.Clear();
				}
			}
			if (burnMe) {
				if (runAndBurn) {
					if (!soul.burning) {
						if (!pather.running) {
							pather.runAway ();
						}
						if (letMeBurn != 1) {
							burnTime (true);
						}
					} else {
						pather.stopRunning ();
						//for roar
						float oppoDist = Vector2Int.Distance (self.centerPoint, pather.target.centerPoint);
						float myDist = Vector2Int.Distance (attack.getCenter (), self.centerPoint);
						if (oppoDist < myDist && oppoDist < enRange + pather.bodyLen) {
							//Debug.LogError ("should stop Burning, oponent can get my body myD: " + myDist + " od: " + oppoDist);
							burnTime (false);
						}
					}
				} else if (runIfNotFullPowered) {
					if (!pather.running && !soul.fullPower) {
						pather.runAway ();
					} else if (pather.running && soul.fullPower) {
						pather.stopRunning ();
					}
				} else {
					if (wantToBurn) {
						if (burnNoMatter || pather.targDef.getHealth () < 75) {
							if (letMeBurn != 1) {
								burnTime (true);
							} 
						} else if (pather.targP.dead || def.getHealth () < 35) {
							if (letMeBurn != 0) {
								burnTime (false);
							}
						}
					}
				}
			}
		}
		// chance of creeping
		if (attack) {
			if (!attack.swinging && swingTime == -1) {//if swinging we might miss because we are slower than anticipated, ensures damage isn't affected
				if (Random.value < creepChance) {
					if (!pather.running) {
						if (me.inPercent == 1) {
							me.inPercent = Mathf.Max (0.1f, Random.value - 0.1f);
							stopMove ();
						}
					}
				} else if (me.inPercent != 1) {
					me.inPercent = 1;
				}
			}
		}
		wait ();
		if (swordOK) {
			swordArts ();
		}
		return true;
	}
		
	int posEsti = 10;
	int pec = -1;

	bool rangeFound = false;
	int shotTime = 0;
	int windBack = 666;
	public bool rangeBlocked = false;

	void swordArts() {
		if (!me.dead && pather.ready /*&& (!running || runAndBurn)*/ && !me.dodging && !pather.blockedByDestructible && (!pather.lost || me.mDest.x >= 0)) {
			//Debug.LogWarning ("sword arts " + adjAngle + " heat:  "+ soul.energy + " esti: " + rec + " < " + rngEstimate + " " + postSwingC);
			//Debug.Log ("wndback: " + windBack);
			bool firstDirBlocked = false;
			bool burnShortened = false;
			bool shortD = false;
			if ((rec != -1 || rangeFound) && ((enBDir != pather.target.direction || enKnocked != pather.targP.knocked || enDodge != pather.targP.dodging || enInP != pather.targP.getInP ()/* || enPos == target.centerPoint*/) || (/*myDest != me.mDestmyDir != self.direction ||*/ myKnocked != me.knocked || myDodge != me.dodging || myInP != me.getInP ()))) {
				//Debug.Log ("reset estimate " + myInP + " != " + me.getInP() + " en: " + enInP + " != " + targP.getInP ());
				//Debug.Log (myDir + " !=  " + self.direction + " en: " + enBDir + " " + target.direction);
				rec = -1;
				swingTime = -1;
				if (rangedAttack) {
					rangeFound = false;
					adjAngle = 666;
					//cantSwing = true;
					//if cooling after adj
					//postSwingC = coolTime - postSwingInterval
					//postSwingC = postSwingInterval - getCoolTime(soul, -1);

				}
			} 
			//no swing loaded, check distances
			// find estimate
			if (rec == -1) {
				int time = pather.howLongUntilPlayerReaches (me, me.mDest, 0);//
				//if (!rangedAttack) {// || true) {
					if (pather.dist2Player > attackDist && !rangedAttack) {
						for (int i = 1; i < bladeLook; i++) {
							if (time > -1 && time < i) {
								myPos = me.mDest;
							} else {
								myPos = pather.findPlayerPos (me, i);
							}

							enPos = pather.findPlayerPos (pather.targP, i);
							float dist = Vector2Int.Distance (enPos, myPos);
							//can shot reach int time
							//swing time + shot time(dist) < i
							if (dist < attackDist * hitRngAccuracy) { 
								//Debug.LogError ("in " + i + " units the enmy shall be at " + enPos + " Ill be at " + myPos);
								/*
								rngEstimate = i;
								rec = 0;
								enBDir = target.direction;
								enKnocked = targP.knocked;
								enDodge = targP.dodging;
								enInP = targP.getInP ();
								myDest = me.mDest;
								myDodge = me.dodging;
								myKnocked = me.knocked;
								myInP = me.getInP ();
								*/
								//Debug.Log (i);
								setEstimate (i);
								break;
							}
						}
					} else {
						//Debug.Log ("time:" + time + " " + me.mDest);
					int rangeTime = 39;
						if (time > -1 && time < rangeTime) {
							myPos = me.mDest;
						} else {
							myPos = pather.findPlayerPos (me, rangeTime);
						}
						enPos = pather.findPlayerPos (pather.targP, rangeTime);
					}
				//} else if (rec == -1) {
					/*
					int swingFrames = 57;//dont hard code//works for full swing(far side) but not for back fourth shot(inside)
					int aheadAdjust = 10;//added to rec so that we are aiming in the future
					int shotSpeed = 3;//1 unit per 3 frames - soft code later
					for (int i = swingFrames; i < swingFrames + bladeLook * shotSpeed * 2; i++){//=(shotSpeed * 2)) {
						if (time > -1 && time < i) {
							myPos = me.mDest;
						} else {
							myPos = findPlayerPos (me, i);
						}

						enPos = findPlayerPos (targP, i);
						float dist = Vector2Int.Distance (enPos, myPos ) - attackDist;// + blade length
						float shotTravelDist = attack.GetComponent<Vault>().calcDistOfShot(i - swingFrames);//(i - swingFrames) * 3f;
						if (shotTravelDist >= dist) {// + aheadAdjust) {
							Debug.Log ("ranged decided " + i + " enPos: " + enPos);
							Debug.Log ("in " + i + " frames enemy will be " + dist + " units away and the shot can travel " + shotTravelDist + " units");
							//rangeFound = true;
							shotTime = i - swingFrames;//(int)shotTravelDist;
							//saveEnemyState ();
							setEstimate (i);
							if (adjAngle != 666) {
								Debug.Log ("old angle still here need a new one " + adjAngle);
								adjAngle = 666;
							}
							//setEstimate (0);
							break;
						}
					}
					*/
				//}
				//Debug.Log (enPos + " " + myPos);
			} else {
				//countdown to loaded swing
				if (rec < rngEstimate || swingTime != -1) {
					//if (swingTime != -1) {
					if (swingTime != -1 && rngEstimate - rec <= swingTime - swingUnitAdj) {
						swingSword (swingAng, swingD, false, false);
						swingTime = -1;
						rec = -1;
						adjAngle = 666;
						//break;
					} else {
						//}
						rec++;
					}
				} else {
					rec = -1;
					return;//resets calculations before checking next section
				}
			}
			if (!attack.swinging && !attack.beingKnocked && !pather.targP.dead) {
				//not doing anything? Lets get to an adjustment angle
				int ap = 666; 
				//calcukate adjustment angle
				if (adjAngle != 666) {
					ap = saneAddition (angleBetween (myPos, enPos), adjAngle, adjDir);
					//Debug.Log ("adjusting to " + angleBetween (myPos, enPos) + " " + adjAngle + " " + adjDir);
				}
				bool letsAttack = false;
				int minAdjDist = 10;
				// if we dont have adjustment angle or we are far enough away that we have time to adjust
				if ((ap == 666 || findShortestDist((int)attack.curPos, ap) > minAdjDist) && !dontAdjust) { 
					switch (adjustToAngle (ap, rngEstimate - rec, 0)) {
					case 1:
						/*if (rangedAttack) {
							newAdjAngleShot ();
						} else {*/
							newAdjAngle ();
						//}
						break;
					case 2:
						letsAttack = true;
						break;
					default:
						
						break;
					}
					//vvDebug.Log ("attack? " + letsAttack);
				} else {
					//Debug.Log ("a=no adjust needed " + letMeBurn);
					//no adjustment, time to figure out our attack
					letsAttack = true;

					if (!ignoreBurn) {
						// we are not burning but we want to burn
						//if not swinging and burst is low, we might lose our heat
						if (!stopBurn () && (!soul.burning || (!curSwinging && soul.bursting == 0 && (soul.energy - soul.burningPoint <= soul.heatSpeed))) && !rangedAttack) {
							//Debug.Log ("lets get burnng");
							int ru = (rngEstimate - rec);
							if (swingTime == -1) {
								if ((rec != -1 || pather.dist2Player > attackDist)) {
									if (rec == -1) {
										ru = (int)pather.dist2Player;
									}
									float heat = getSwingHeat (saneAddition (attack.curPos, 30, 1), 1);
									if (getSoulHeat (soul, 0) + heat < (soul.burningPoint - heatToBurn)) {
										swingSword (saneAddition (attack.curPos, 15, 1), 1, false, true);
										swingSword ((int)attack.curPos, -1, false, true);
									}
								}
							} else {
								//Debug.Log ("we have "  + ru + " until in range");
								float posSwordDist = ((ru - swingUnitAdj) * ((attack.minSpeed + attack.maxSpeed) / 2)) - 4;
								float ab = angleBetween (myPos, enPos);
								//Debug.Log ("myPOs: " + myPos + " en: " + enPos);
								int sd = findShortestDir (attack.curPos, ab);
								int ha = (int)getHitAngle (sd, false, 0, false).x;
								int len = saneDifference ((int)attack.curPos, ha, sd);
								int unitOfSwing = findSwordDist (attack, (int)attack.curPos, ha, sd, len);
								//Debug.LogError ("we have "  + ru + " until in range, psd: " + unitOfSwing);
								//if (ru > unitOfSwing) {
								//if (getSoulHeat (soul, 0) + (posSwordDist * attack.getHeatAmount ()) < (soul.burningPoint - heatToBurn)) {
								//Debug.LogError ("last minute adjust"); this does occur occasionallys
								//swingSword (saneAddition (attack.curPos, 1, 1), 1, false, true);
								//swingSword ((int)attack.curPos, -1, false, true);
								//}
								//} 
							}
						}
					}
				}
				//Debug.Log (cantSwing);
				if (!cantSwing && letsAttack) {
					float ab = angleBetween (myPos, enPos);
					//Debug.Log ("myPOs: " + myPos + " en: " + enPos);
					int sd = findShortestDir (attack.curPos, ab);
					//int sd = adjSwingD;//findShortestDir (attack.curPos, ab);
					//Debug.Log("using dir: " + sd);
					if (Random.value > swingDirAcc) {
						//Debug.Log ("swing dir changed");
						sd *= -1;
					}
					int ha = (int)getHitAngle (sd, false, 0, false).x;
					float ad = findShortestDist (attack.curPos, ha) * scLen;
					int minMove = findSwordDist (attack, (int)attack.curPos, (int)ha, findShortestDir (attack.curPos, ha), (int)ad);
					bool swingWait = Random.value < swingWaitChance;
					if (swingTime == -1 && (!swingWait || (pather.targDef.getCurInvul () == -1 || pather.targDef.getCurInvul () < minMove) && unitsLeftDodging (pather.targP) <= minMove)) {//  && (soul.energy == soul.climate || !rangedAttack)) {
						//Debug.Log ("heat situation, energy: " + soul.energy + " " + soul.climate);
						if (rec != -1 || pather.dist2Player <= attackDist || rangedAttack) {
							int unitCheck = 0;
							if (rec != -1) {
								unitCheck = rngEstimate - rec;
							}
							int finAngle = (int)ab;
							int len = saneDifference ((int)attack.curPos, finAngle, sd);
							int st = findSwordDist (attack, (int)attack.curPos, (int)finAngle, sd, len);
							float heatToImpact = 0;
							bool ra = rangedAttack;
							if (rangedAttack) {
								bool goodHit = true;
								isSwingBlocked ((int)attack.curPos, ha, sd, ignoreListSP);
								if (hitList.Count != 0) {
									goodHit = false;
								} else {
									float heatNeeded = (soul.burningPoint - soul.energy);
									int sLen = (int)((heatNeeded) / attack.getHeatAmount ());
									len = saneDifference ((int)attack.curPos, finAngle, sd);
									Debug.Log ("aiming at " + finAngle);

									if (len < sLen) {
										heatToImpact = len * attack.getHeatAmount ();
										float distBack = (sLen - len) / 2;
										//Debug.Log ("DTF: " + distToFoe + " db: " + distBack);
										isSwingBlocked ((int)attack.curPos, (int)distBack, -sd, ignoreListSP);
										if (hitList.Count == 0) {
											windBack = saneAddition (attack.curPos, distBack, -sd);
											st = findSwordDist (attack, (int)attack.curPos, (int)windBack, sd, len);
											finAngle = saneAddition (finAngle, 2, sd);//make sure we swing enough to burn, at worst we over shoot a little but the shot still comes out when desired
											//curAdj = saneDifference (ab, desiredAngle, offset);
											//ap = saneAddition (ab, curAdj + j, offset);
										} else {
											Debug.LogError ("cant go back " + attack.curPos + " " + distBack + " " + -sd);
											//getLost ();
											rangeBlocked = true;
											goodHit = false;
										}
									} else {
										Debug.Log ("eln too long : " + len);
										goodHit = false;
										windBack = 666;
									}
								}
								if (!goodHit) {
									if (rec != -1 || pather.dist2Player <= attackDist) {
										ra = false;
									} else {
										pather.getLost ();
										windBack = 666;
										adjAngle = 666;
										return;
									}
								}
							} 
							if (!ra) {
								isSwingBlocked ((int)attack.curPos, ha, sd, ignoreListSP);//could come up with simpler function for sword pos specifically
								if (hitList.Count != 0) {
									firstDirBlocked = true;
									sd *= -1;
									isSwingBlocked ((int)attack.curPos, ha, sd, ignoreListSP);
								}
								float hitAngle = getHitAngle (sd, false, unitCheck, false).x;
								len = saneDifference ((int)attack.curPos, hitAngle, sd);
								heatToImpact = len * attack.getHeatAmount ();
								if (!rangedAttack) { //we want ranged attack tto end on the angle between us so we dont over burn
									if (len < 3) {
										len = 3;//= saneAddition(dist, 1, sd);
										shortD = true;
									}
									len = (int)(len * scLen);
									finAngle = saneAddition ((int)attack.curPos, len, sd);
									//Debug.Log ("fa: " + finAngle);
									if (rec == -1 || rngEstimate - rec <= swingTime - swingUnitAdj) {
										finAngle = saneAddition (finAngle, (len / scLen), sd);//were gonna hit early so we need more length
										//Debug.Log ("FA: " + finAngle + " added: " + (len / scLen));
									}
								}
								//check if our swing is blocked
								st = findSwordDist (attack, (int)attack.curPos, (int)hitAngle, sd, len);
								//Debug.Log (attack.curPos + " -- " + sd + " > " + hitAngle + " = " + st);
							} 
							bool swordBlocking = false;
							for (int i = 0; i < hitList.Count; i++) {
								if (hitList [i].id == 2) {
									Vector2 curKnock = attack.getKnockBack ();
									float myPow = findSwordStat (attack, st, curKnock.x, curKnock.y, 0, len, sd);
									curKnock = targSword.getKnockBack ();
									float theirPow = findSwordStat (targSword, st, curKnock.x, curKnock.y);
									//Debug.LogError ("theirs: " + theirPow + " mine: " + myPow);
									if (theirPow > myPow) {
										swordBlocking = true;
										Debug.Log ("sword blocked");
									}
								} else {
									//Debug.Log ("oh fuck what did " + name + " hit? " + hitList [i].name + " at " + self.centerPoint + " angle: " + attack.curPos);
								}
							}
							if (!swordBlocking) {
								if (!ra && !ignoreBurn) {
									//calculate heat from swing
									float he = getSoulHeat (soul, Mathf.Max (0, unitCheck - (st - (swingUnitAdj + 1))));

									float heatIncrease = getSwingHeat (finAngle, sd);//dist * attack.getHeatAmount ();
									if (stopBurn ()) {
										//making sure we dont burn ourselves
										if (he + heatIncrease > soul.burningPoint) {
											//we will burn ourselves
											float extra = (((he + heatIncrease) - soul.burningPoint) / attack.getHeatAmount ()) + 1;
											float oldLen = heatIncrease / attack.getHeatAmount ();
											if (extra < oldLen) {
												// shorten swing
												finAngle = saneAddition (finAngle, extra, -sd);
												heatIncrease = getSwingHeat (finAngle, sd);
												burnShortened = true;
												//Debug.Log ("short swing");
											} else if (Random.value < burnRunAway) {
												//cant shorten swing, do we run away?
												finAngle = (int)attack.curPos;
												if (soul.burning || (soul.burningPoint - 1) - soul.energy < 0) {
													//Debug.Log (soul.energy + " " + soul.burningPoint);
													if (pather.dist2Player < enRange) {
														pather.runAway ();
														return;
													}
												}
											}
										}
									} else if (!soul.burning) {
										//Debug.Log ("lets burn me baby");
										if (he + heatToImpact < soul.burningPoint) {
											float heatTilBurn = soul.burningPoint - (he + heatIncrease);
											float backSwingHA = getHitAngle (-sd, false, unitCheck, false).x;
											float distBackToPlayer = saneDifference (finAngle, backSwingHA, -sd);
											if (distBackToPlayer * attack.getHeatAmount () < heatTilBurn) {
												float extraHeat = heatTilBurn - (distBackToPlayer * attack.getHeatAmount ());
												float extraDist = extraHeat / attack.getHeatAmount ();
												float extraAngle = saneAddition (finAngle, extraDist * 0.55f, sd); // a little more than half, so were sure to burn on impact for the way back
												finAngle = (int)extraAngle;
												distBackToPlayer = saneDifference (finAngle, backSwingHA, -sd);
												if (distBackToPlayer * scLen > 360) {
													distBackToPlayer = 359;
												} else {
													distBackToPlayer *= scLen;
												}
												backAngle = saneAddition (finAngle, distBackToPlayer, -sd);
											}
										} 
									} else {
										//Debug.Log ("we are burning and we want to be");
									}
								}
								if (finAngle != (int)attack.curPos) {
									//Debug.Log ("finANgle: " + finAngle);
									swingAng = finAngle;
									swingD = sd;
									swingTime = st + shotTime;
									//Debug.Log (rngEstimate + " - " + rec + " <= " + swingTime + " - " + swingUnitAdj);
									if (rec == -1 || rngEstimate - rec <= swingTime - swingUnitAdj) {
										if (windBack != 666) {
											swingSword (windBack, -sd, false, false);
											windBack = 666;
										}
										swingSword (finAngle, sd, false, false);// longer strike because we are swinging early and were losing momentum before we hit, still not enough damage at times
										swingTime = -1;
										rec = -1;
										adjAngle = 666;
										rangeFound = false;
									} 
									//Debug.Log ("swing " + attack.curPos + " -> " + finAngle + " " + sd + ", blocked: " + firstDirBlocked + ", burn stopped: " + burnShortened + ", dist short: " + shortD);
									//hit angle, dir, dir blocked, length, length changed
								} else if (ra) {
									//we eneded on the same attack so we angle and so we need t oswitch spots
									//getLost ();
									adjAngle = 666;
								}
							} else {
								pather.getLost ();
							}
						}
					} 
				}///
			}
			if (!me.going && pather.dist == 0 && pather.dist2Player > attackDist && !pather.stayAtTarget) {
				Debug.Log ("I should move if I want to attack " + pather.dist2Player + " > " + attackDist);
				pather.getLost ();
			}
		}
	}

	void setEstimate(int esti) {
		rngEstimate = esti;
		rec = 0;
		saveEnemyState ();
	}

	void saveEnemyState() {
		enBDir = pather.target.direction;
		enKnocked = pather.targP.knocked;
		enDodge = pather.targP.dodging;
		enInP = pather.targP.getInP ();
		myDir = self.direction;
		myDest = me.mDest;
		myDodge = me.dodging;
		myKnocked = me.knocked;
		myInP = me.getInP ();
	}


	int adji = 0;
	int chki = 1;
	int maxAngleChecks = 10;

	void newAdjAngle() {
		//Debug.Log ("new adj angle");
		int anglesChecked = 0;
		adjAngle = 666;
		int ab = (int)angleBetween (myPos, enPos);
		for (int i = adji; i < 4; i++, adji++) {
			// curAdj + ab will be the angle we wish to check
			int curAdj = adjAngs [adjOrder [i]];

			for (int j = chki; j <= 45; j++, chki++) {
				if (anglesChecked <= maxAngleChecks) {
					int[] chks = new int[6];
					chks [0] = chks [3] = 666;
					int ci = 0;
					for (int k = 1; k >= -1; k -= 2) {
						int ap = saneAddition (ab, curAdj + j, k);
						//Debug.Log ("checking: " + ap);
						if (myPos == attack.getCenter()) {//self.centerPoint) {
							ap = (int)attack.curPos;
						}

						//ap is the position we are checking
						int sd = findShortestDir (ap, ab);
						if (Random.value > swingDirAcc) {
							sd *= -1;
						}
						// checking to see if we can swing to desired angle of contact
						int ha = (int)getHitAngle (sd, false, 0, false).x;

						isSwingBlocked (myPos, ap, ha, sd, ignoreListSP);//ignore swords because they are subject to changing position
						if (hitList.Count != 0) {
							isSwingBlocked (myPos, ap, (int)getHitAngle (-sd, false, 0, false).x, -sd, ignoreListSP);
							chks [ci + 2] = -sd;
						} else {
							chks [ci + 2] = sd;
						}
						attack.checkBody (ap);
						if (attack.self.curCollided.Count == 0 && hitList.Count == 0) {// && ) {// doesn't work because we dont know exactly where theyll be standing when 
							chks [ci] = curAdj + j;
							chks [ci + 1] = k;
						}
						ci += 3;
					}
					//Debug.Log("0 - " + chks[0] +
					if (chks [0] != 666 && (chks [3] == 666 || saneDifference (attack.curPos, saneAddition (ab, chks [0], chks [1]), chks [2]) < saneDifference (attack.curPos, saneAddition (ab, chks [3], chks [4]), chks [5]))) {//findShortestDist (attack.curPos, saneAddition (ab, chks [0], chks [1])) < findShortestDist (attack.curPos, saneAddition (ab, chks [3], chks [4])))) {
						adjAngle = chks [0];
						adjDir = chks [1];
						adjSwingD = chks [2];
					} else if (chks [3] != 666) {
						adjAngle = chks [3];
						adjDir = chks [4];
						adjSwingD = chks [5];
					}
					if (adjAngle != 666) {
						break;
					}
					anglesChecked += 2;
				} else {
					break;
				}
			}
			if (adjAngle != 666 || anglesChecked >= maxAngleChecks) {
				break;
			} else {
				chki = 1;
			}
		}
		if (adjAngle == 666) {
			if (anglesChecked < maxAngleChecks) {
				Debug.Log (anglesChecked + " < " + maxAngleChecks + " adj: " + adji + " chk: " + chki);
				pather.getLost ();
				adji = 0;
				chki = 1;
			}
		} else {
			adji = 0;
			chki = 1;
		}
	}

	void newAdjAngleShot() {
		int anglesChecked = 0;
		adjAngle = 666;
		//dist2heat
		int aheadAdjust = 10;//added to rec so that we are aiming in the future
		int shotSpeed = 3;//1 unit per 3 frames - soft code later
		float heatNeeded = (soul.burningPoint - soul.climate) ;
		int sLen = (int) ((heatNeeded) / attack.getHeatAmount());
		int swingFrames = findUnitDist(attack, (int)attack.curPos, saneAddition(attack.curPos, sLen, 1), 1, sLen);//dont hard code//works for full swing(far side) but not for back fourth shot(inside)
		Debug.Log("swingFrames: " + swingFrames);
		swingFrames += postSwingInterval - postSwingC;
		int time = pather.howLongUntilPlayerReaches (me, me.mDest, 0);//
		//int ab = saneAddition(posAngle, 180, 1);//assuming we can keep out current angle, which if we cant we cant predict? Because that means they changed direction
		//int ap = saneAddition (ab, sLen, offset);

		for (int i = swingFrames; i < swingFrames + bladeLook * shotSpeed * 2; i++){//=(shotSpeed * 2)) {
			
			 if (time > -1 && time < i) {
				myPos = me.mDest;
			} else {
				myPos = pather.findPlayerPos (me, i);
			}
			enPos = pather.findPlayerPos (pather.targP, i);
			float dist = Vector2Int.Distance (enPos, myPos ) - attackDist;// + blade length
			float shotTravelDist = attack.GetComponent<Vault>().calcDistOfShot(i - swingFrames);//(i - swingFrames) * 3f;
			if (shotTravelDist >= dist) {// + aheadAdjust) {
				
				Debug.Log ("ranged decided " + i + " enPos: " + enPos);
				//Debug.Log ("in " + i + " frames enemy will be " + dist + " units away and the shot can travel " + shotTravelDist + " units");
				//rangeFound = true;
				shotTime = i - swingFrames;//(int)shotTravelDist;
				//get adjangle
				int ab = (int)angleBetween (myPos, enPos);
				int finAngle = 666;
				for (int j = chki; j <= 20; j++, chki++) {
					if (anglesChecked < maxAngleChecks) {
						int[] chks = new int[8];
						chks [0] = chks [4] = 666;
						int ci = 0;
						int checkBoth = -1;//-1 we will check both, 0, we only do one
						bool farSide = true;
						int k = 1;
						Vector2Int cd = findClosestDir (attack.curPos, ab);
						int sd = 0;
						for (; k >= checkBoth; k -= 2) {
							int offset = k;
							int curAdj = sLen;// - 1;//saneAddition(ab, sLen, i);
							int ap = saneAddition (ab, curAdj + j, offset);
							chks [ci + 3] = 1;
							if (myPos == attack.getCenter ()) {//self.centerPoint) {
								ap = (int)attack.curPos;
							}
							sd = -offset;//findShortestDir (ap, ab);
							if (Random.value > swingDirAcc) {
								sd *= -1;
							}
							// checking to see if we can swing to desired angle of contact
							int ha = (int)getHitAngle (sd, false, 0, false).x;

							isSwingBlocked (myPos, ap, ha, sd, ignoreListSP);//ignore swords because they are subject to changing position
							if (hitList.Count != 0) {
								isSwingBlocked (myPos, ap, (int)getHitAngle (-sd, false, 0, false).x, -sd, ignoreListSP);
								chks [ci + 2] = -sd;
							} else {
								chks [ci + 2] = sd;
							}
							attack.checkBody (ap);
							if (attack.self.curCollided.Count == 0 && hitList.Count == 0) {// && ) {// doesn't work because we dont know exactly where theyll be standing when 
								chks [ci] = curAdj + j;
								chks [ci + 1] = offset;
							}
							ci += 4;
						}
						//Debug.Log("0 - " + chks[0] +
						if (chks [0] != 666 && (chks [4] == 666 || saneDifference (attack.curPos, saneAddition (ab, chks [0], chks [1]), chks [2]) < saneDifference (attack.curPos, saneAddition (ab, chks [4], chks [5]), chks [6]))) {//findShortestDist (attack.curPos, saneAddition (ab, chks [0], chks [1])) < findShortestDist (attack.curPos, saneAddition (ab, chks [3], chks [4])))) {
							adjAngle = chks [0];
							adjDir = chks [1];
							adjSwingD = chks [2];
							if (chks [3] == 0) {
								//coolHeatAdj = false;
							} else {
								//coolHeatAdj = true;
							}
						} else if (chks [4] != 666) {
							adjAngle = chks [4];
							adjDir = chks [5];
							adjSwingD = chks [6];
							if (chks [7] == 0) {
								//coolHeatAdj = false;
							} else {
								//coolHeatAdj = true;
							}
						}
						if (adjAngle != 666) {
							//Debug.Log ("adj angle chosen = " + adjAngle + " dir is " + adjDir + " cool after" + coolHeatAdj);
							finAngle = saneAddition (ab, adjAngle, adjDir);
							//get adjTime
							int adjD = saneDifference ((int)attack.curPos, finAngle, adjSwingD);
							int adjustmentTime = findSwordDist (attack, (int)attack.curPos, finAngle, adjSwingD, adjD);
							//getCoolTime
							int coolTime = getCoolTime (soul, soul.climate + soul.energy + getSwingHeat (finAngle, adjSwingD));
							//Debug.Log ((adjustmentTime + coolTime) + " < " + i);
							Vector2Int ep = pather.findPlayerPos (pather.targP, i + coolTime + adjustmentTime);
							Vector2Int mp = pather.findPlayerPos (me, i + coolTime + adjustmentTime);
							if (time > -1 && time <  i + coolTime + adjustmentTime) {
								mp = me.mDest;
							} 

							float tmpDist = Vector2Int.Distance (ep, mp ) - attackDist;// + blade length
							float tmpShotTravelDist = attack.GetComponent<Vault>().calcDistOfShot((i + coolTime + adjustmentTime) - swingFrames);//(i - swingFrames) * 3f;
							//if (adjustmentTime + coolTime < i) {
							if (tmpShotTravelDist > tmpDist) {
								enPos = ep;
								myPos = mp;
								setEstimate (i + coolTime + adjustmentTime);
								Debug.Log ("estimate: " + rngEstimate + " ep: " + enPos);
								chki = 0;
								return;
							} else {
								Debug.Log ("we need to keep going");
							}
						}
						anglesChecked += 2;
					} else {
						return;
					}
				}
			}
		}
	}

	void newAdjAngleBurn() {
		int anglesChecked = 0;
		adjAngle = 666;
		int ab = (int)angleBetween (myPos, enPos);
		Debug.Log ("enemy actual pos: " + pather.targP.transform.position + " enPos: " + enPos + " " + soul.energy);
		Debug.Log ("MP: " + myPos);
		//for (int i = 1; i >= -1; i-= 2) {
		// curAdj + ab will be the angle we wish to check
		//int curAdj = adjAngs [adjOrder [i]];
		// calculate how far we need to be to burn at hit angle
		float heatNeeded = (soul.burningPoint - soul.climate) ;
		int sLen = (int) ((heatNeeded) / attack.getHeatAmount());
		Debug.Log ("angle between us is " + ab + " len: " + sLen);
		for (int j = chki; j <= 20; j++, chki++) {
			if (anglesChecked < maxAngleChecks) {
				int[] chks = new int[8];
				chks [0] = chks [4] = 666;
				int ci = 0;
				int checkBoth = -1;//-1 we will check both, 0, we only do one
				bool farSide = true;
				int k = 1;
				Vector2Int cd = findClosestDir (attack.curPos, ab);
				//Debug.Log (cd);
				if (cd [1] < sLen - 1 && postSwingC >= postSwingInterval) {//if we cant immediately swing it messes up the heat, so we should just go to the max distance away
					/*
					k = -cd [0];
					checkBoth = k;
					farSide = false;
					*/
				}
				int sd = 0;
				for (; k >= checkBoth; k -= 2) {
					int offset = k;
					int curAdj = sLen;// - 1;//saneAddition(ab, sLen, i);
					int ap = saneAddition (ab, curAdj + j, offset);
					// if we are on the far side of the adjust angle -> cool sword after
					// else we need to just get there and turn around
					//if (findClosestDir (attack.curPos, ab)[0] == findClosestDir (attack.curPos, ap)[0]) {
					if (farSide) {
						//Debug.Log ("on far side");
						chks[ci + 3] = 1;
					} else {
						//Debug.Log ("in close range");
						chks[ci + 3] = 0;
						offset = k;
						float heatToFoe = getSwingHeat (ab, -offset);
						int distToFoe = saneDifference (attack.curPos, ab, -offset);
						float distBack = (curAdj - distToFoe) / 2;
						//Debug.Log ("DTF: " + distToFoe + " db: " + distBack);
						int desiredAngle = saneAddition (attack.curPos, distBack, offset);
						curAdj = saneDifference (ab, desiredAngle, offset);
						ap = saneAddition (ab, curAdj + j, offset);
						//Debug.Log ("ap: " + ap + " da: " + desiredAngle);
					}
					//Debug.Log ("curAdj: " + curAdj + " ap: " + ap);
					if (myPos == attack.getCenter()) {//self.centerPoint) {
						ap = (int)attack.curPos;
					}


					//ap is the position we are checking
					sd = -offset;//findShortestDir (ap, ab);
					if (Random.value > swingDirAcc) {
						sd *= -1;
					}
					// checking to see if we can swing to desired angle of contact
					int ha = (int)getHitAngle (sd, false, 0, false).x;

					isSwingBlocked (myPos, ap, ha, sd, ignoreListSP);//ignore swords because they are subject to changing position
					if (hitList.Count != 0) {
						isSwingBlocked (myPos, ap, (int)getHitAngle (-sd, false, 0, false).x, -sd, ignoreListSP);
						chks [ci + 2] = -sd;
					} else {
						chks [ci + 2] = sd;
					}
					attack.checkBody (ap);
					if (attack.self.curCollided.Count == 0 && hitList.Count == 0) {// && ) {// doesn't work because we dont know exactly where theyll be standing when 
						chks [ci] = curAdj + j;
						chks [ci + 1] = offset;
					}
					ci += 4;
				}
				//Debug.Log("0 - " + chks[0] +
				if (chks [0] != 666 && (chks [4] == 666 || saneDifference(attack.curPos, saneAddition (ab, chks [0], chks [1]), chks[2]) < saneDifference(attack.curPos, saneAddition (ab, chks [4], chks [5]), chks[6]))) {//findShortestDist (attack.curPos, saneAddition (ab, chks [0], chks [1])) < findShortestDist (attack.curPos, saneAddition (ab, chks [3], chks [4])))) {
					adjAngle = chks [0];
					adjDir = chks [1];
					adjSwingD = chks [2];
					if (chks [3] == 0) {
						//coolHeatAdj = false;
					} else {
						//coolHeatAdj = true;
					}
				} else if (chks [4] != 666) {
					adjAngle = chks [4];
					adjDir = chks [5];
					adjSwingD = chks [6];
					if (chks [7] == 0) {
						//coolHeatAdj = false;
					} else {
						//coolHeatAdj = true;
					}
				}
				if (adjAngle != 666) {
					Debug.Log("adj angle chosen = " + adjAngle + " dir is " + adjDir + " cool after" + coolHeatAdj);
					//coolHeatAdj = cha;
					break;
				}
				anglesChecked += 2;
			} else {
				break;
			}
		}
		if (adjAngle != 666 || anglesChecked >= maxAngleChecks) {
			//break;
		} else {
			chki = 1;
		}
		//}
		if (adjAngle == 666) {
			if (anglesChecked < maxAngleChecks) {
				pather.getLost ();
				//adji = 0;
				chki = 1;
			}
		} else {
			//adji = 0;
			chki = 1;
		}
	}

	float heatUnderBurn = 20;
	float heatToBurn = -15;

	//0 - swung, 1 - blocked, 2 - letsAttack = true
	int adjustToAngle(int angle, int minDiff, int dir) {
		if (pather.dist2Player < attackDist && pather.targDef.getCurInvul () == -1) {
			//Debug.Log ("dist close");
			return 2;
		}
		/*List <Form> hitList = */attack.checkBody (angle);
		bool bodyFree = attack.self.curCollided.Count == 0;
		if (!bodyFree) {
			for (int i = 0; i < attack.self.curCollided.Count; i++) {
				if (attack.self.curCollided [i].id == 2 && !targSword.swinging) {
					bodyFree = true;
				}
			}
		}
		bool insidePlayer = false;
		attack.checkBody (attack.curPos);
		for (int i = 0; i < attack.self.curCollided.Count; i++) {
			if (attack.self.curCollided [i].id == 1) {
				insidePlayer = true;
			}
		}
		if (angle == 666/* || !bodyFree*/) {
			return 1;
		}
		int dd = dir;
		if (Mathf.Abs(dd) != 1) {
			dd = findShortestDir (attack.curPos, angle);
			isSwingBlocked ((int)attack.curPos, angle, dd, ignoreListI);
			if (hitList.Count != 0) {
				bool blocked = true;//false;
				for (int i = 0; i < hitList.Count; i++) {
					if (hitList [i].id == 2 && !targSword.swinging) {
						blocked = false;
					}
					if (hitList [i].id == 1) {
						if (!insidePlayer && pather.targDef.getCurInvul () == -1) {
							//Debug.Log ("will hit player, so lets wing early");
							return 2;
						} else {
							blocked = false;
						}
					}
				}
				if (blocked) {
					isSwingBlocked ((int)attack.curPos, angle, -dd, ignoreListI);
					blocked = false;
					for (int i = 0; i < hitList.Count; i++) {
						blocked = true;
						if (hitList [i].id == 2 && !targSword.swinging) {
							blocked = false;
						}
						if (hitList [i].id == 1) {
							if (!insidePlayer && pather.targDef.getCurInvul () == -1) {
								//Debug.Log ("will hit player, so lets wing early");
								return 2;
							} else {
								blocked = false;
							}
						}
					}
					if (blocked) {
						adjAngle = 666;
						return 1;
					} else {
						dd *= -1;
					}
				}
			}
		}
		if (!bodyFree) {
		//	Debug.Log ("not body free");
			return 1;//now we know there is time to find another angle, because we are not too close, and previous section determines that
		}
		int sUnits = -100;
		if (rec != -1) {
			sUnits = findSwordDist (attack, (int)attack.curPos, angle, dd, saneDifference (attack.curPos, angle, dd));
		}
		//Debug.Log ("sUnits: " + sUnits + " minDIff: " + minDiff);
		if (sUnits < minDiff) {//not really important when comparing player movement to sword movement
			if (!ignoreBurn) {
				float heatUnder = heatUnderBurn;
				if (!stopBurn ()) {
					heatUnder = heatToBurn;
				}
				//Debug.Log ("og angle: " + angle);
				float heatIncrease = getSwingHeat (angle, dd);
				//if (!wantToBurn) {
				//Debug.Log ("at " + soul.energy + " going to get " + heatIncrease + " " + (soul.burningPoint - heatUnder));
				if (soul.energy + heatIncrease > (soul.burningPoint - heatUnder)) {
					float extra = (((soul.energy + heatIncrease) - (soul.burningPoint - heatUnder)) / attack.getHeatAmount ()) + 1;
					float oldLen = heatIncrease / attack.getHeatAmount ();
					if (extra < oldLen) {
						angle = saneAddition (angle, extra, -dd);
					} else {
						if ((Mathf.Abs (soul.energy - (soul.burningPoint - heatUnder)) < 1 || soul.energy > soul.burningPoint - heatUnder) && pather.dist2Player < attackDist) {
							//Debug.Log ("lets atatck");
							return 2;//we should attack if we can and run if we cant, check that in the sword arts
						}
						angle = (int)attack.curPos;
					}
						//Debug.Log ("new angle: " + angle);
				}
				//Debug.Log ("new angle: " + angle);
				/*} else {
				if (soul.energy + heatIncrease < soul.burningPoint) {

				}
			}*/
			}
			swingSword (angle, dd, false, true);
			//Debug.Log ("swign " + adjAngle);
			return 0;
		} else {
			//Debug.Log ("sUnits too far");
			return 2;
		}
	}

	public void setBurnAdj(int ba) {
		burningAdj = ba;
	}

	public void setHeatUnderBurn(float hub) {
		heatUnderBurn = hub;
	}

	int burningAdj = 2;
	int noBurnAdj = 1;

	void burnTime(bool letsBurn) {
		//Debug.Log ("burn Time " + letsBurn);
		bool lb = letsBurn;
		if (Random.value < burnMisFire) {
			lb = !lb;
		}
		if (lb) {
			while (adjOrder [0] != burningAdj) {
				cycleAdjUp ();
			}
			letMeBurn = 1;
		} else {
			while (adjOrder [0] != noBurnAdj) {
				cycleAdjUp ();
			}
			letMeBurn = 0;
		}
	}
		
	int unitsLeftDodging(Player p) {
		if (p.dodging && p.getDR() == -1) {
			int[] dp = p.getDP ();
			if (dp.Length == 0) {
				return (int)(p.getDodgeLength () - p.getDI ());
			} else {
				return (int)((dp.Length - p.getDI ()) / p.getDodgeSpeed());
			}
		}
		return 0;
	}

	float curAngle;
	float endAngle;
	float endCheck;
	int enDir;
	int swingLen;
	int unitsTilImpact;
	int waitLook = 3;
	bool timeToDodge = false;
	// not reall for waiting? for dodging
	void wait() {
		if (targSword && targSoul && !pather.targP.dead && !me.dead) {
			if (Random.value < dodgeCombatChance) {
				timeToDodge = true;
			}
			if (timeToDodge) {
				bool canDodge = !me.dodging;
				if (!canDodge) {
					float dVal = me.getDP ().Length;
					if (dVal == 0) {
						dVal = me.getDodgeLength ();

					}
					if (me.getDI () >= dVal - waitLook && me.getDR () <= waitLook) {
						canDodge = true;
					}
				}
				if (canDodge && estCounter > estimate) {
					Vector2Int myPos = pather.findPlayerPos (me, waitLook - 1);
					Vector2Int theirPos = pather.findPlayerPos (pather.targP, waitLook - 1);
					float dp = Vector2Int.Distance (myPos, theirPos);

					if (dp < enRange + 1) {
						if (targSword.anglePath.Count > 0) {
							enDir = targSword.getDir ();
							float swordD = saneDifference (targSword.curPos, getHitAngle (enDir, true, 0, false).x, enDir);
							if (swordD < targSword.getCurrentSpeed ()) {
								estimate = 0;
								estCounter = 0;
								Debug.Log ("quick swing - dodge locked 0");
							} else {
								float hitAngle = getHitAngle (enDir, true, 0, false).x;
								float preSAngle = findSwordPos (targSword, 0);
								float preSD = saneDifference (preSAngle, hitAngle, enDir);
								for (int i = 1; i <= waitLook; i++) {
									float swordAngle = findSwordPos (targSword, i);
									hitAngle = getHitAngle (enDir, true, i, false).x;

									float sDiff = saneDifference (swordAngle, hitAngle, enDir);
									if (Mathf.Abs (sDiff - preSD) > 250 || sDiff <= 7 * enSwordSize || sDiff > 330) {
										estimate = i;
										estCounter = 0; //because generally this happens before sword swing
									//	Debug.Log ("swing - dodge locked " + estimate);
										break;
									}
									preSD = sDiff;
								} 
							}
						} else if (((pather.targP.dodging && pather.targP.mDest != GM.S.invalidDest) || !pather.targP.dodging) && (pather.targP.going || pather.targP.knocked)) {
							enDir = targSword.getDir ();
							Vector2Int ppos = pather.findPlayerPos (pather.targP, 0);
							Vector2Int mpos = pather.findPlayerPos (me, 0);
							for (int i = 0; i <= waitLook; i++) {
								if (i != 0) {
									ppos = pather.findPlayerPos (pather.targP, i);
									mpos = pather.findPlayerPos (me, i);
								}
								float angle = angleBetween (ppos, mpos);
								float sDiff = saneDifference (angle, targSword.curPos, findShortestDir (angle, targSword.curPos));
								float dd = Vector2Int.Distance (ppos, mpos);
								if (dd < enRange + 1) {
									float perc = (dd - 5) / (enRange - 5);
									float minDiff = Mathf.Lerp (50, 20, perc);
									if (sDiff < minDiff) {//sDiff < 5 || sDiff > 340) {
										estimate = i;
										estCounter = 0;
										//Debug.Log ("still attack - dodge locked " + estimate);
										if (estimate == i && estCounter == 0) {
											break;
										}
									}
								}
							}
						}
					}
				}
				if (estCounter <= estimate) {
					if (estCounter >= estimate - 3) {
						//if (Random.value < dodgeCombatChance) {
						if (!me.dodging) {//want to get brfore estimate happens, also there is some discrepancy, earlier is better
							if (Random.value > 0.5f) {
								int awayAngle = (int)angleBetween (pather.target.centerPoint, attack.getCenter ());//self.centerPoint);
								Vector2Int awayDodge = pather.getCirclePos (saneAddition(awayAngle, Random.Range(0, 90), 1), pather.dist2Player);
								me.mDest = attack.getCenter () + awayDodge;//self.centerPoint + awayDodge;
							} else {
								int toAngle = (int)angleBetween (/*self.centerPoint*/attack.getCenter(), pather.target.centerPoint);
								//Debug.Log (toAngle);
								Vector2Int toDodge = pather.getCirclePos (saneAddition(toAngle, Random.Range(0, 45), 1), pather.dist2Player);
								me.mDest = attack.getCenter() + toDodge;//self.centerPoint + toDodge;
							}
							Vector2Int dodgePos = me.getDodgePos (1);//me.maxDodgePower);
							if (Random.value < inPlaceDodgeChance || pather.dist2Player > Vector2Int.Distance (/*self.centerPoint*/attack.getCenter(), dodgePos)) {//if dist2Player is longer, it means the dodge was blovked and will probably not take us where we want
								stopMove ();
							}
							dodge ();
							estCounter = estimate + 2;
						} 
						estCounter = estimate + 2;
						//	}
					}
					estCounter++;
				} else {
					timeToDodge = false;
				}
			}
		}
	}

	int swordState = 0; //0 - still, 1 - adjust, 2 - attack
	bool coolHeatAdj = true;

	void swingSword(int angle, int dir, bool postHalt, bool adjusting) {
		if (attack) {
			//if (adjusting) {
				//Debug.Log (name + " " + attack.curPos + " -> " + angle + ": " + dir + " " + adjusting + " " + soul.energy + " ra: " + rangedAttack);
			//}
			if (adjusting) {
				swordState = 1;
			} else {
				swordState = 2;
			}
			if (!adjusting) {
				if (Random.value > swingLenAcc) {
					float miss = Random.Range (10, 60);//len * 0.3f, len * 0.6f);
					if (Random.value > 0.5f) {
						//Debug.Log ("length shoertened " + miss);
						angle = saneAddition (angle, miss, -dir);
					} else {
						//Debug.Log ("length increase " + miss);
						angle = saneAddition (angle, miss, dir);
					}
				}
			}
			attack.swingPath (angle, dir, false);
			if (!adjusting) {
				attack.weightSwing (dir);
			} else {
				attack.weightSwing (0);
			}
			if (preSwordPos != attack.curPos) {
				preSwordPos = attack.curPos;
			}
			if (!adjusting) {
				if (backAngle == 666) {
					cantSwing = true;
					postSwingC = 0;
				}

			}/* else if (coolHeatAdj) {
				cantSwing = true;
				//if cooling after adj
				//postSwingC = coolTime - postSwingInterval
				postSwingC = postSwingInterval - getCoolTime(soul, soul.climate + getSwingHeat(angle, dir));
			}*/
		} 
	}
		
	public void dodge() {
		int dodgeMiss = Random.Range (0, dodgeAngleMiss);
		if (me.mDest != GM.S.invalidDest && dodgeMiss > 0) {
			int angle = (int)angleBetween (/*self.centerPoint*/attack.getCenter(), me.mDest);
			int dir = 1;
			if (Random.value > 0.5f) {
				dir = -1;
			}
			float distToPos = Vector2Int.Distance (/*self.centerPoint*/attack.getCenter(), me.mDest);
			//Debug.Log ("miss: " + dodgeMiss + "dodge distorted " + me.mDest);
			me.mDest = /*self.centerPoint */attack.getCenter() + pather.getCirclePos(saneAddition(angle, dodgeMiss, dir), distToPos);
			//Debug.Log ("to " + me.mDest);
		}
		me.dodge (1);//me.maxDodgePower);
	}

	public void isSwingBlocked(int pos, int end, int dir, List<int> ignoreList) {
		isSwingBlocked (/*self.centerPoint*/attack.getCenter(), pos, end, dir, ignoreList);
	}

	public bool readCheckSwing = false;

	public void isSwingBlocked(Vector2Int center, int pos, int end, int dir, List<int> ignoreList) {
		if (readCheckSwing) {
			Debug.LogWarning ("checking : " + pos + " + " + dir + " = " + end + " hit other stuff? " + ignoreList.Count );
			Debug.LogWarning ("center: " + center);
		}
		//List<Form> hitList = new List<Form> ();
		hitList.Clear();
		//List<Form> finalList = new List<Form> ();
		int noLoop = 0;
		while (((Mathf.Abs(end) != 180 && pos != end) || (Mathf.Abs(end) == 180 && Mathf.Abs(pos) != Mathf.Abs(end))) && noLoop < 360) {
			attack.checkBody (center, pos);
			if (readCheckSwing) {
				//Debug.Log ("p: " + pos);
			}
			if (attack.self.curCollided.Count == 0) {
				pos = saneAddition (pos, 1, dir);
			} else {
				bool ignoreHit = true;
				for (int i = 0; i < attack.self.curCollided.Count; i++) {
					bool onIgnore = false;
					for (int j = 0; j < ignoreList.Count; j++) {
						if (attack.self.curCollided [i].id == ignoreList [j]) {
							onIgnore = true;
						}
					}
					if (!onIgnore) {
						if (readCheckSwing) {
							Debug.Log ("hit at " + pos);
						}
						if (readCheckSwing) {
							Debug.Log (attack.self.curCollided [i].name + " " + attack.self.curCollided [i].id);
						}
						ignoreHit = false;
						hitList.Add (attack.self.curCollided [i]);
						break;
					} 
				}
				if (!ignoreHit) {
					break;
				} else {
					attack.self.curCollided.Clear ();
					pos = saneAddition (pos, 1, dir);
				}
			}
			noLoop++;
		}
		if (noLoop >= 360) {
			Debug.LogError ("john johjn apoopy");
		}
		//return finalList;
	}

	float findSwordPos(circleAttack sword, int units) {
		curAngle = sword.curPos;
		if (sword.anglePath.Count > 0) {
			endAngle = sword.anglePath [sword.anglePath.Count - 1];// only if direction is the same
			Vector2 speeds = sword.getSpeed ();
			swingLen = sword.getSwingLength ();
			enDir = sword.getDir ();
			int si = (int)sword.getSwingPos ();
			int hs = (int)sword.calcStat (si, swingLen, speeds.x, speeds.y);
			int curInterval = 0;
			for (int i = 0; i < units && si <= swingLen; i++) {
				for (int k = 0; k < hs; k++) {
					curAngle = saneAddition (curAngle, 1, enDir);
					si++;
					hs = (int)targSword.calcStat (si, swingLen, speeds.x, speeds.y);
					if (si > swingLen) {
						if (sword.intervals.Count > curInterval) {
							swingLen = sword.intervals [curInterval].x;
							enDir = sword.intervals [curInterval].y;
							endAngle = sword.anglePath [sword.anglePath.Count - 1];
							si = 1;
							hs = (int)targSword.calcStat (si, swingLen, speeds.x, speeds.y);
							curInterval++;
						} else {
							int lenLeft = (int)(sword.getSwingLength () - sword.getSwingPos ());
							for (int j = 0; j < sword.intervals.Count; j++) {
								lenLeft += sword.intervals [j].x;
							}
							if (sword.anglePath.Count > lenLeft) {
								endAngle = sword.anglePath [sword.anglePath.Count - 1];
								float spd = sword.getHyp ();
								int apCount = sword.anglePath.Count;
								if (!sword.beingKnocked) {
									spd = speeds.x;
								}
								for (int j = 0; j < units - i && curAngle != endAngle; j++) {
									for (int l = 0; l < spd; l++) {
										curAngle = saneAddition (curAngle, 1, sword.curDir);
										if (sword.beingKnocked) {
											apCount--;
											if (apCount <= spd) {
												if (spd > 1) {
													spd--;
												}
											}
										}
										if (curAngle == endAngle) {
											break;
										}
									}
								}
							}
							break;
						}
					} 
				}
			}
		}
		return curAngle;
	}

	int findSwordDist (circleAttack sword, int start, int angle, int dir, int fullSwing) {
		int si = 1;
		if (sword.swinging) {
			curAngle = sword.curPos;
			if (sword.anglePath.Count > 0) {
				endAngle = sword.anglePath [sword.anglePath.Count - 1];// only if direction is the same
			} else {
				endAngle = angle;
			}
			swingLen = sword.getSwingLength ();
			enDir = sword.getDir ();
			si = (int)sword.getSwingPos ();
		} else {
			curAngle = start;
			endAngle = angle;
			swingLen = fullSwing;
			enDir = dir;
		}
		Vector2 speeds = sword.getSpeed ();
		int hs = (int)sword.calcStat (si, swingLen, speeds.x, speeds.y);
		int curInterval = 0;
		int units = 0;
		bool foundIt = false;
		while (curAngle != endAngle && units <= 360 && !foundIt) {
			units++;
			for (int k = 0; k < hs; k++) {
				curAngle = saneAddition (curAngle, 1, enDir);
				if (curAngle == angle) {
					foundIt = true;
				}
				if (curAngle == endAngle) {
					foundIt = true;
				}
				si++;
				hs = (int)targSword.calcStat (si, swingLen, speeds.x, speeds.y);
				if (si > swingLen) {
					if (sword.intervals.Count > curInterval) {
						swingLen = sword.intervals [curInterval].x;
						enDir = sword.intervals [curInterval].y;
						endAngle = sword.anglePath [sword.anglePath.Count - 1];
						si = 1;
						hs = (int)targSword.calcStat (si, swingLen, speeds.x, speeds.y);
						curInterval++;
					} else {
						int lenLeft = (int)(sword.getSwingLength () - sword.getSwingPos());
						for (int j = 0; j < sword.intervals.Count; j++) {
							lenLeft += sword.intervals [j].x;
						}
						if (sword.anglePath.Count > lenLeft) {
							endAngle = sword.anglePath [sword.anglePath.Count - 1];
							float spd = sword.getHyp ();
							int apCount = sword.anglePath.Count;
							if (!sword.beingKnocked) {
								spd = speeds.x;
							}
							if (sword.swinging) {
								enDir = sword.curDir;
							}
							while (curAngle != endAngle && units <= 360) {
								for (int l = 0; l < spd; l++) {
									curAngle = saneAddition (curAngle, 1, enDir);
									units++;
									if (sword.beingKnocked) {
										apCount--;
										if (apCount <= spd) {
											if (spd > 1) {
												spd--;
											}
										}
									}
									if (curAngle == endAngle) {
										foundIt = true;
										break;
									}
								}
							}
						}
						break;
					}
				}
			}
		}
		return units;
	}

	float findSwordStat(circleAttack sword, int units, float min, float max) {
		return findSwordStat (sword, units, min, max, -1, 0, 0);
	}

	float findSwordStat(circleAttack sword, int units, float min, float max, int si, int sl, int d) {
		curAngle = sword.curPos;
		float stat = min;
		if (sword.anglePath.Count > 0 || sl != 0) {
			if (sword.anglePath.Count > 0) {
				endAngle = sword.anglePath [sword.anglePath.Count - 1];// only if direction is the same
			}
			Vector2 speeds = sword.getSpeed ();
			if (sl == 0) {
				swingLen = sword.getSwingLength ();
			}
			if (d == 0) {
				enDir = sword.getDir ();
			}
			if (si < 0) {
				si = (int)sword.getSwingPos ();
			}
			int hs = (int)sword.calcStat (si, swingLen, speeds.x, speeds.y);
			int curInterval = 0;
			for (int i = 0; i < units && si <= swingLen; i++) {
				for (int k = 0; k < hs; k++) {
					curAngle = saneAddition (curAngle, 1, enDir);
					si++;
					hs = (int)targSword.calcStat (si, swingLen, speeds.x, speeds.y);
					if (si > swingLen) {
						if (sword.intervals.Count > curInterval) {
							swingLen = sword.intervals [curInterval].x;
							enDir = sword.intervals [curInterval].y;
							endAngle = sword.anglePath [sword.anglePath.Count - 1];
							si = 1;
							hs = (int)targSword.calcStat (si, swingLen, speeds.x, speeds.y);
							curInterval++;
						} else {
							int lenLeft = (int)(sword.getSwingLength () - sword.getSwingPos ());
							for (int j = 0; j < sword.intervals.Count; j++) {
								lenLeft += sword.intervals [j].x;
							}
							if (sword.anglePath.Count > lenLeft) {
								endAngle = sword.anglePath [sword.anglePath.Count - 1];
								float spd = sword.getHyp ();
								int apCount = sword.anglePath.Count;
								if (!sword.beingKnocked) {
									spd = speeds.x;
								}
								for (int j = 0; j < units - i && curAngle != endAngle; j++) {
									for (int l = 0; l < spd; l++) {
										curAngle = saneAddition (curAngle, 1, sword.curDir);
										if (sword.beingKnocked) {
											apCount--;
											if (apCount <= spd) {
												if (spd > 1) {
													spd--;
												}
											}
										}
										if (curAngle == endAngle) {
											break;
										}
									}
								}
							}
							break;
						}
					} 
				}
			}
			stat = (int)targSword.calcStat (si, swingLen, min, max);
		}
		return stat;
	}

	bool stopBurn() {
		float lmb = letMeBurn;
		if (Random.value < burnMisFire) {
			lmb = (lmb + 1) % 2;
		}
		return lmb == 0;
	}

	float angleBetween(Vector2Int pos1, Vector2Int pos2) {
		//Debug.Log("angle from " + pos1 + " and " + pos2);
		/*
		Vector2 dir = new Vector2(pos2.x - pos1.x, pos2.y - pos1.y).normalized;
		float x = (int)(dir.x * 1000) - pos1.x;
		float y = (int)(dir.y * 1000) - pos2.y;
		*/
		Vector3 D = new Vector3 (pos2.x - pos1.x, pos2.y - pos1.y, 0);
		float A = Mathf.Atan2 (D.y, D.x) * Mathf.Rad2Deg;
		//Debug.Log ("new angle: " + A);
		return A;//Mathf.Atan2 (y, x) * Mathf.Rad2Deg;
	}

	// sort of innaccurate, probably because on just grabbing the 1st item from checklist, and or the length of the sword and which part is hitting it, andor 
	//the size of the sword chunks??? How to fix this
	// also need to figure out which part of the check we are going to hit? how to fix this, left right up down are obvius, but mid angles, how to do without complicated calculations
	// the problem is the hit point is not calculated perfectly, if a sword chunk is thick, the centerpoint is off, and we need to figure out the closest part of the hit check to the hit point (this should be exact)
	Vector2 getHitAngle(int d, bool enemy, int units, bool debug) {
		int point = 8;
		Player s;
		Player t;
		Form tf;
		if (!enemy) {
			s = me;
			t = pather.targP;
			tf = pather.target;
		} else {
			s = pather.targP;
			t = me;
			tf = self;
		}

	//	float dist = Vector2Int.Distance (s.centerPoint, t.centerPoint);
		Vector2Int scp = pather.findPlayerPos (s, units);
		Vector2Int tcp = pather.findPlayerPos (t, units);
		if (debug) {
			Debug.Log ("tar: " + tcp + " self: " + scp);
		}
		int xD = scp.x - tcp.x;
		int yD = scp.y - tcp.y;
		//Debug.Log("xd: " + xD + " yd: " + yD + " d: " + d);
		// more cases where a cardinal direction could be chosen, but brute force(it will include the hit point, but may have to check more points to find it)
		if (Mathf.Abs (yD) < tf.length / 2) {
			if (xD > 0) {
				if (d > 0) {
					point = 0;
				} else {
					point = 4;
				} 
			} else {
				if (d < 0) {
					point = 0;
				} else {
					point = 4;
				} 
			}
		} else if (Mathf.Abs (xD) < tf.width / 2) {
			if (yD > 0) {
				if (d > 0) {
					point = 2;
				} else {
					point = 6;
				} 
			} else {
				if (d < 0) {
					point = 2;
				} else {
					point = 6;
				} 
			}
		} else {
			if (yD > 0 && xD > 0) {
				if (d < 0) {
					point = 5;
				} else {
					point = 1;
				}
				//point = 7;
			}
			if (yD < 0 && xD > 0) {
				if (d < 0) {
					point = 3;
				} else {
					point = 7;
				}
				//point = 5;
			}
			if (yD < 0 && xD < 0) {
				if (d < 0) {
					point = 1;
				} else {
					point = 5;
				}
				//point = 3;
			}
			if (yD > 0 && xD < 0) {
				if (d < 0) {
					point = 7;
				} else {
					point = 3;
				}
				//point = 1;
			}
		}
		if (point == 8) {
			//Debug.Log (xD + " " + yD + "d: " + d);
			return Vector2.zero;
		} else {
			// here we want to figure out which is going to be hit from the check 0 - width/length
			float closest = Mathf.Infinity;
			int ourChunk = 0;
			for (int i = 0; i < tf.getCheck (point).Length; i++) {
				float dist = Vector2Int.Distance (tcp + tf.getCheck (point) [i], scp);//s.centerPoint); // not quite the best check
				//Debug.Log (t.getCheck (point) [i]);
				if (dist < closest) {
					closest = dist;
					ourChunk = i;
				}
			}
			Vector2Int tar = new Vector2Int (tcp.x + tf.getCheck (point) [ourChunk].x + 1, tf.getCheck (point) [ourChunk].y + tcp.y);
			//Debug.Log ("tar x: " + (tcp.x + (tf.getCheck (point) [ourChunk].x + 1)));
		
			if (point == 1) {
				tar = tcp + tf.getCheck (0) [0] - new Vector2Int (0, 1);
			} else if (point == 3) {
				tar = tcp + tf.getCheck (2) [0] + new Vector2Int (1, 0);
			} else if (point == 5) {
				tar = tcp + tf.getCheck (4) [tf.width - 1] + new Vector2Int (0, 1);
			} else if (point == 7) {
				tar = tcp + tf.getCheck (6) [tf.length - 1] - new Vector2Int (1, 0);
			}

			//tar = new Vector2(100, 78);
			//Debug.Log(t.getCheck(point).Length);
			//Debug.Log ("point: " + point + "chunk: " + ourChunk + " tar pos = " + tar);
			// now we need the actual position of the sword chunk which may be different depending on how thick it is
			float x = tar.x - scp.x;//(dir.x * 1000) - scp.x;
			float y = tar.y - scp.y;//(dir.y * 1000) - scp.y;
			float angle = Mathf.Atan2 (y, x) * Mathf.Rad2Deg;
			//Debug.Log("dist: " + Vector2Int.Distance(tar + new Vector2Int(1,0), scp));
			if (debug) {
				Debug.Log ("I will be at " + scp + " they will be at " + tcp + " the angle: " + angle);
			}
			return new Vector2(angle, Vector2Int.Distance(tar + new Vector2Int(1,0), scp));
		}
	}
		
	Vector2Int findClosestDir(float start, float end) {
		int dd = 1;
		int pos = saneDifference ((int)start, (int)end, 1);
		int diff = pos;
		int neg = saneDifference ((int)start, (int)end, -1);
		if (pos > neg) {
			dd = -1;
			diff = neg;
		}
		return new Vector2Int(dd, diff);
	}
		
	int findUnitDist(circleAttack sword, int start, int angle, int dir, int fullSwing) {
		if (fullSwing > 0) {
			int dist = saneDifference ((int)sword.curPos, angle, dir);//(int)Mathf.Abs (attack.curPos - counterAngle);
			float curS = Mathf.Max(1, sword.getSwingPos());
			if (curS > fullSwing) {
				fullSwing = sword.getSwingLength();
			}
			int units = 0;
			float increase = sword.calcStat (curS, fullSwing, sword.minSpeed, sword.maxSpeed);
			for (float i = 0; i >= 0 && i < dist && units < 60 && curS < fullSwing; i += increase) {
				increase = sword.calcStat (curS, fullSwing, sword.minSpeed, sword.maxSpeed);
				curS += increase;
				units++;
			}
			return units;
		} else {
			return -1;
		}
	}

	float addUnitsGetPos(circleAttack sword, int start, int amnt, int dir, int fullSwing) {
		float curS = Mathf.Max(1, sword.getSwingPos());
		if (curS > fullSwing) {
			fullSwing = sword.getSwingLength();
			Debug.Log ("new fullSwing: " + fullSwing);
		}
		float pos = start;//sword.curPos;
		float increase = sword.calcStat(curS, fullSwing, sword.minSpeed, sword.maxSpeed);
		for (int i = 0; i < amnt; i++) {
			curS+=increase;
			increase = sword.calcStat(curS, fullSwing, sword.minSpeed, sword.maxSpeed);//(int)((Mathf.Sin (curS * (Mathf.PI / fullSwing)) * (sword.maxSpeed)) + sword.minSpeed);
			pos = saneAddition ((int)pos, (int)increase, dir);
		}
		return pos;
	}
		
	float getSwingHeat(float angle, int dir) {
		//angle = saneAddition (angle, 40, -dir);
		float heatIncrease = saneDifference (attack.curPos, angle, dir) * attack.getHeatAmount ();
		return heatIncrease;
	}

	float getSoulHeat(SwordSoul so, int units) {
		int burst = so.bursting;
		float en = so.energy;
		for (int i = 0; i < units; i++) {
			if (burst == 0) {
				if (en > so.climate) {
					if (en - so.heatSpeed > so.climate) {
						en -= so.heatSpeed;
					} else {
						en = so.climate;
					}
				} else {
					if (en + so.heatSpeed < so.climate) {
						en += so.heatSpeed;
					} else {
						en = so.climate;
					}
				}
			} else {
				burst--;
			}
			if (burst == 0 && en == so.climate) {
				break;
			}
		}
		return en;
	}
	// return units, if sword stops
	int getCoolTime(SwordSoul so, float startHeat) {
		int burst = so.bursting;
		float en = so.energy;
		if (startHeat != -1) {
			en = startHeat;
			burst = 9;
		}
		int units = 0;
		while ((en != so.climate || burst > 0) && units < 100) {
			//Debug.Log ("en " + en + " b: " + burst);
			if (burst == 0) {
				if (en > so.climate) {
					if (en - so.heatSpeed > so.climate) {
						en -= so.heatSpeed;
					} else {
						en = so.climate;
					}
				}
			} else if (burst > 0) {
				burst--;
			} else {
				burst = 0;
			}
			units++;
		}
		//Debug.Log("cool time == " + units);
		return units;
	}

	void cycleAdjUp() {
		int tmp = adjOrder [0];
		for (int i = 1; i < 4; i++) {
			adjOrder [i - 1] = adjOrder [i];
		}
		adjOrder [3] = tmp;
	}

	void swapAdj(int a, int b) {
		int tmp = adjOrder [a];
		adjOrder [a] = adjOrder [b];
		adjOrder [b] = tmp;
	}

	public void getAdjOrder(int[] order) {
		for (int i = 0; i < order.Length; i++) {
			adjOrder [i] = order [i];
		}
	}

	public void getDistOrder(int[] order) {
		//Debug.Log ("dist order length: " + order.Length + " " + distOrder.Length);
		for (int i = 0; i < order.Length; i++) {
			pather.distOrder [i] = order [i];
		}
	}

	public override Vector2Int calculateDestination() {
		return pather.getTarPos ();
	}

	public override void die() {
		pather.closed.Clear ();
	}

	public void raiseDifficulty(int amnt) {
		difficultyLevel = stats.raiseLevel (amnt);
		stats.updateAI (this);
	}

	public void lowerDifficulty(int amnt) {
		difficultyLevel = stats.lowerLevel (amnt);
		stats.updateAI (this);
	}

	public circleAttack getAttack() {
		return attack;
	}

	public void setSwingInterval(int newInterval) {
		postSwingInterval = newInterval;
		postSwingC = newInterval;
	}

	public void setBurnDesire(int desire) {
		if (desire == -1) {
			wantToBurn = false;
		} else if (desire == 1) {
			wantToBurn = true;
		}
	}

	public void setPathStats(int update, int body, float walkDestructible) {
		if (!pather) {
			Debug.LogError("we need a pather");
		} else {
			pather.updateInterval = update;
			pather.checkBodyInterval = body;
			pather.walkIntoDestructible = walkDestructible;
		}
	}

}
