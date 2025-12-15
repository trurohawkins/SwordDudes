using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathBoi : Purpose {
	public Form target;
	public Player targP;
	public defense targDef;
	public circleAttack targSword;
	public SwordSoul targSoul;

	public float walkIntoDestructible = 0;
	public bool stayAtTarget = false;
	public bool blockedByDestructible;
	public bool moveOK = true;
	public bool ready = false;

	public int checkBodyInterval = 2;
	int checkBodyCounter = 0;
	public int updateInterval = 1;
	int updateCounter = 0;
	int moveLookAhead = 2;

	public float bodyLen;
	public float minDist;
	public int[] distOrder;
	public Vector3[] distRanges;

	public bool lost = false;
	public bool orMP;
	public Vector2Int orModPos;
	public float dist;
	public float dist2Player;
	bool clearSight;
	public bool blockedByTarget = false;
	public int aiNum;

	int circleAngle = 720;
	public bool circling = false;
	public int circleMod = 10;

	
	int[] circlePosOrder;
	Vector2Int[] circlePosChecks;

	public List<pathNode> open;
	public List<pathNode> closed;
	public int step = 9999;
	mapTerrain pathMaster;
	Vector2Int modPos;

     protected override void Awake() {
		lostPos = GM.S.invalidDest;
		open = new List<pathNode> ();
		closed = new List<pathNode> ();
		pathMaster = Map.S.gameObject.GetComponent<mapTerrain> ();
		neighbies = new pathNode[8];
		
		circlePosChecks = new Vector2Int[4];
		circlePosChecks [0] = new Vector2Int (180, 1); //back
		circlePosChecks [1] = new Vector2Int (90, 1); //side 1
		circlePosChecks [2] = new Vector2Int (90, -1); // side 2
		circlePosChecks [3] = new Vector2Int (0, 0); //front
		circlePosOrder = new int[4];
		for (int i = 0; i < 4; i++) {
			circlePosOrder[i] = i;
		}
		if (angleOverrides == null) {
			angleOverrides = new List<int>();
		}
        base.Awake();
		minDist = (int)(self.width / 2) + 1;
		bodyLen = Mathf.Sqrt(Mathf.Pow((self.width/2), 2) + Mathf.Pow((self.length/2), 2));
		distOrder = new int[3];
		for (int i = 0; i < 3; i++) {
			distOrder [i] = i;
		}
		distRanges = new Vector3[3];
		distRanges[0] = new Vector3(minDist, minDist * 2, -1);
    }

	public void setRange(int which, int min, int max) {
		if (which >= 0 && which < 3) {
			distRanges[which] = new Vector3(min, max, -1);
		}
	}

	public virtual void getEnemy(GameObject enemy) {
		target = enemy.GetComponent<Form> ();
		minDist = bodyLen * 2;// (int)((Mathf.Max (self.width, self.length) + Mathf.Max (target.width, target.length)) / 2) + 2;
		targP = enemy.GetComponent<Player> ();
		targDef = enemy.GetComponent<defense> ();
		if (targP.myAttack) {
			targSword = targP.myAttack;
			if (targSword.getSoul ()) {
				targSoul = targSword.getSoul ();
				updateEnemySwordInfo ();
			}
		} else {
			targSword = null;
			targSoul = null;
		}
		ready = true;
		lost = true;
	}

	public void loseTarget() {
		ready = false;
		target = null;
	}

	public virtual void updateEnemySwordInfo() {
		//Debug.LogError ("updating enemy sword info " + (bodyLen + targSword.attackRange.y) + " != " + enRange);
		//Debug.LogError ("enSwordSize: " + targSword.curHitListkForm ().width + " != " + enSwordSize);
		//distRanges [2] = new Vector3 (distRanges [1].y + 1, Mathf.Max (Map.S.worldSizeX, Map.S.worldSizeY) / 2, 1);
		distRanges[1].x = distRanges [1].y + 1;
		distRanges [1].y = Mathf.Max (Map.S.worldSizeX, Map.S.worldSizeY) / 2;
		distRanges [1].z = 1;
	}


	public void findTarget() {
		int np = GM.S.getNumPlayers ();
		//Debug.Log("numplayer is 
		if (np >= 2) {
			//Debug.Log("finding target among " + np + " players");
			float lowDist = Mathf.Infinity;
			Form chosen = null;
			for (int i = 0; i < np; i++) {
				if (GM.S.players [i]) {
					Form f = GM.S.players [i].GetComponent<Form> ();
					//Debug.Log ("player " + i + ", is dead? " + GM.S.players [i].dead + " on my team? " + GM.S.players [i].onTeam (me.getTeam ()));
					if (isGoodTarget(i, f)) {
						float d = Vector2Int.Distance (f.centerPoint, self.centerPoint);
						//Debug.Log ("dist: " + d);
						if (d < lowDist) {
							chosen = f;
							lowDist = d;
						}
					}
				}
			}
			if (chosen != target && chosen != null) {
				getEnemy (chosen.gameObject);
			}
		} else {
			GameObject t = GM.S.players[0].gameObject;
			if (t && t != gameObject) {
				getEnemy(t);
			}
		}
	}

	public virtual bool isGoodTarget(int player, Form f) {
		return f != self && !GM.S.players [player].dead;
	}

	public void updateInfo() {
		if (ready) {
			if (target) {
				dist = Vector2Int.Distance (getTarPos (), myCenter ());//self.centerPoint);
				dist2Player = Mathf.Round(Vector2Int.Distance (target.centerPoint, myCenter ()));//self.centerPoint);
				// if the enemy didnt have a sword and now it does
				if (targP) {
					if (!targSword && targP.myAttack) {
						getEnemy(target.gameObject);
					}
				}
			}
			if (lost) {
				if (updateCounter >= updateInterval) {
					updateCounter = 0;
					updatePathing ();
				} else {
					updateCounter++;
				}
			}
			if (goodToMove()) {
				move();
			}
		}
	}

	public virtual bool goodToMove() {
		return moveOK && modPos != Vector2Int.zero;
	}

	public virtual Vector2Int myCenter() {
		return self.centerPoint;
	}

	public bool forceStopMove;

	public bool cantDodgeWalls;
	public virtual int move() {
		if (!targP.dodging) {
			if (checkBodyCounter > checkBodyInterval) {
				if (!checkSpot (getTarPos (), false)) {
					getLost();
				}
				checkBodyCounter = 0;
			} else {
				checkBodyCounter++;
			}
		}
		checkSight (myCenter(), getTarPos (), true, false);
		dist = Vector2Int.Distance (getTarPos (moveLookAhead), myCenter ());// self.centerPoint);//maybe should be checking in the future
		//Debug.Log("target pos current: " + getTarPos(0) + " w/look ahead " + getTarPos(moveLookAhead));
		//Debug.Log("moving " + dist + " " + minDist);
		if (blockedByTarget && !cantDodgeWalls) {
			return 2;
		}
		if (readCheckSight) {
			Debug.Log("not blocked by target, clearSight?" + clearSight + " " + pathMaster.hasPath());
		}
		if (clearSight || !pathMaster.hasPath ()) {
			// at some point we switched to using minDist, this might affect calculations...
			if (dist > minDist) { //0 because we have a specific position we want to be in, that we have calculated exactly
				//Debug.Log("clear sight anf not blocked by target lets move towards " + getTarPos (moveLookAhead));
				moveTowards (myCenter(), getTarPos (moveLookAhead));
				closed.Clear ();
			} else {
				//Debug.Log("shal I move? " + stayAtTarget);
				if (stayAtTarget) {
					if ((!targP.going && !targP.knocked && (!targP.dodging || targP.mDest.x == -1))) {//if target isnt moving
						if (amMoving()) {
							stopMove ();
						}
					}
				} else {
					//Debug.Log("updating pathing");
					updatePathing ();
				}
			}
		} else {
			if (!lost) {
				if (step < closed.Count) {
					//Debug.Log(Vector2Int.Distance(myCenter(), closed[step].getPos()) + " <= " +  minDist);
					if (Vector2Int.Distance(myCenter(), closed[step].getPos()) <= minDist) {///*self.centerPoint*/myCenter() == closed [step].getPos ()) {
						step++;
					} else {
						//Debug.Log(myCenter() + " != " + closed[step].getPos());
						//Debug.Log(dist);
					}
				}
				if (step >= closed.Count) {
					//Debug.Log("path2targ " + step);
					pathToTarget ();
				}
				if (closed.Count > 0) {
					moveTowards (myCenter(), closed [step].getPos ());
					return 1;
						
				}
			}
		}
		return 0;
	}

	public Vector2Int curDest;

	public virtual void moveTowards(Vector2Int start, Vector2Int destination) {
		//Vector2Int dest = canIGo (myCenter (), destination, true);//self.centerPoint, destination, false);
		Vector2Int dest = canIGo (start, destination, true);
		//Debug.Log("detsination: " + destination + " calculated: " + dest);
		for (int i = 0; i < self.curCollided.Count; i++) {
			if (self.curCollided[i].id == 4 && Random.value > walkIntoDestructible) {
				blockedByDestructible = true;
			} else if (self.curCollided[i].id != 4 && self.curCollided [i] == target || self.curCollided[i].parent == target) {
				blockedByTarget = true;
			}
		}
		self.curCollided.Clear ();
		//Debug.Log("moving toward dest " + dest + " dist " + Vector2Int.Distance(self.centerPoint, dest));
		if (Vector2Int.Distance(self.centerPoint, dest) > minDist && dest.x >= 0 && dest.y >= 0) {
			curDest = dest;
			return;
		} else if (!blockedByTarget) {
		}

		stopMove();
	}
	
	public virtual bool amMoving() {
		return true;
	}

	public virtual void stopMove() { 
		if (curDest.x >= 0) {
			curDest = new Vector2Int(-1, -1); 
		}
	}

	public bool running = false;
	
	public void runAway() {
		if (!running) {
			Debug.Log ("run away");
			stopMove ();
			running = true;
			getLost ();
			if (distOrder [0] == 0) {
				Debug.Log("cycling");
				cycleDistancesUp ();
			}
		}
	}

	public void stopRunning() {
		if (running) {
			Debug.Log ("no more running");
			stopMove ();
			running = false;
			getLost ();
			while (distOrder [0] != 0) {
				cycleDistancesDown ();
			}
		}
	}

	int findDir(Vector2Int pos, Vector2Int dest) {
		int chosenD = -1;
		if (pos != dest) {
			float closest = Mathf.Infinity;
			for (int i = 0; i < GM.S.dirs.Length; i++) {
				float dist = Vector2Int.Distance (pos + GM.S.dirs [i], dest);
				if (dist < closest/* && self.checkSide (self.centerPoint, i)*/) {
					closest = dist;
					chosenD = i;
				}
			}
		} 
		return chosenD;
	}

	public Vector2Int getTarPos() {
		return target.centerPoint + modPos;
	}

	Vector2Int getTarPos(int unitsAhead) {
		return findPlayerPos (targP, unitsAhead) + modPos;
	}

	public bool readCheckSight = false;
	public bool checkSight(Vector2Int start, Vector2Int end, bool general, bool ignoreSword) {
		if (readCheckSight/* && general*/) {
			Debug.Log ("checking sight " + " s: " + start + " -> " + end + " " + general);
		}
		bool cs = true;
		int td = findDir (start, end);
		if (td >= 0) {
			Vector2Int dir = GM.S.dirs [td];
			Vector2Int pos = start;
			float range = Mathf.Max (dist, 3);
			if (!general) {
				range = Vector2Int.Distance (start, end);
			}
			for (int i = 0; i < range; i++) {
				Vector2Int check = canIGo (pos, end, true);
				if (check.x > -1 && check.y > -1) {
					int fd = findDir (pos, check);
					if (fd >= 0) {
						dir = GM.S.dirs [fd];
						if (readCheckSight) {
							Debug.Log ("pos: " + pos + " dir: " + dir);
						}
						if (self.curCollided.Count != 0) {
							bool blocked = true;
							for (int j = 0; j < self.curCollided.Count; j++) {
								Form f = self.curCollided[j];
								if (readCheckSight) {
									Debug.Log(f.name);
								}
								if (f.id == 4 && Random.value > walkIntoDestructible) {
									//Value v = f.GetComponent<Value>();
									//if (v.onTeam(me.team)) {
										//blocked = false;
										blockedByDestructible = true;
									//}
								} else {
									if (general && f.id != 4 && (f == target || f.parent == target)) {
										blockedByTarget = true;
										if (readCheckSight) {
											Debug.Log ("target blocked me");
										}
									}
									if (f.parent == self || f.centerPoint == end || (ignoreSword && f.parent == target)) {// && getTarPos () == target.centerPoint) { should it only work if the targetpos is the form's center point, sword has different center poiny, and targpos has mod
										if (readCheckSight) {
											Debug.Log (self.curCollided [j].name); // maybe wont work if sword stacked on something but it seems fine
										}
										blocked = false;
									} else {
										if (readCheckSight/* && general*/) {
											Debug.Log (self.curCollided [j].name + " " + self.curCollided [j].centerPoint);
										}
									}
								}
							}
							if (readCheckSight/* && general*/) {
								Debug.Log (pos);
							}
							cs = !blocked;
							if (!cs) {
								break;
							}
						} else {
							pos += dir;
						}
						if (pos == getTarPos ()) {
							if (readCheckSight/* && general*/) {
								Debug.Log ("reached pos");
							}
							break;
						}
					} else {
						if (readCheckSight/* && general*/) {
							Debug.Log ("we got there");
						}
						cs = true;
						break;
					}
				} else {
					if (readCheckSight /*&& general*/) {
						Debug.Log ("cant go " + pos);
					}
					if (self.curCollided.Count > 0) {
						//Debug.Log ("hit somethng " + general);
						bool blocked = true;
						for (int j = 0; j < self.curCollided.Count; j++) {
							Form f = self.curCollided[j];
							if (readCheckSight) {
								Debug.Log(self.curCollided[j].name);
							}
							if (f.id == 4 && Random.value > walkIntoDestructible) {
								blockedByDestructible = true;
								//blocked = false;
							} else {
								if (general && f.id != 4 &&(f == target || f.parent == target)) {
									blockedByTarget = true;
								}
								if (f.parent == self || f.centerPoint == end || (ignoreSword && f.parent == target) || f == target) {// && getTarPos () == target.centerPoint) { should it only work if the targetpos is the form's center point, sword has different center poiny, and targpos has mod
									if (readCheckSight /*&& general*/) {
										Debug.Log (self.curCollided [j].name); // maybe wont work if sword stacked on something but it seems fine
									}
									blocked = false;
								} else {
									if (readCheckSight/* && general*/) {
										Debug.Log (self.curCollided [j].name + " " + self.curCollided [j].centerPoint);
									}
								}
							}
						}
						cs = !blocked;
						if (!cs) {
							break;
						}
					}
				}
			}
		}

		if (readCheckSight) {
			Debug.Log ("was it clear: " + cs);
		}
		if (general) {
			clearSight = cs;
			if (clearSight == true) {
				//Debug.Log("cuaght THEM!!!");
				blockedByTarget = false;
				blockedByDestructible = false;
			}
		}
		return cs;
	}
	
	public Vector2Int canIGo(Vector2Int start, Vector2Int end, bool clear) {
		Vector2Int dest = end;// new Vector2Int (destination.x, self.centerPoint.y);
		int mDir = findDir (start, end);
		if (mDir >= 0) {
			if (!self.checkSide (start, mDir, clear, false)) {
				/*
				for (int i = 0; i < self.curCollided.Count; i++) {
					if (self.curCollided[i].id == 4) {
						return dest;
					}
				}
				*/
				dest = new Vector2Int (end.x, start.y);
				mDir = findDir (start, dest);
				if (mDir < 0 || !self.checkSide (start, mDir, clear, false)) {
					/*
					for (int i = 0; i < self.curCollided.Count; i++) {
						if (self.curCollided[i].id == 4) {
							return dest;
						}
					}
					*/
					dest = new Vector2Int (start.x, end.y);
					mDir = findDir (start, dest);
					if (mDir < 0 || !self.checkSide (start, mDir, clear, false)) {
						/*
						for (int i = 0; i < self.curCollided.Count; i++) {
							if (self.curCollided[i].id == 4) {
								return dest;
							}
						}
						*/
						dest = new Vector2Int (-1, -1);
					}
				}
			} 
		}
		//Debug.Log ("from " + start + " got dest: " + dest + " towrads end: " + end);
		return dest;
	}
	
	public Vector2Int getCirclePos(int angle, float range) {
		float x = range * Mathf.Cos (angle * Mathf.PI / 180);
		float y = range * Mathf.Sin (angle * Mathf.PI / 180);
		Vector2Int tp = new Vector2Int ((int)x, (int)y);
		return tp;
	}

	public virtual void useRangeAttack() { }

	int rng = 0;//i
	float dst = 0; //j
	int cpc = 0; //k
	int acr = 0; // l
	int crc = 0; //circle k
	int maxChecks = 10;
	int posLookAhead = 10;
	int posAngle; 
	public List<int> angleOverrides;
	public virtual void updatePathing() {
		if (ready) {//if (targSword) {// && targSoul) {
			if (orMP) {
				modPos = orModPos;
				lost = false;
				lostPos = GM.S.invalidDest;
				orMP = false;
				return;
			} 
			stopMove ();//to stop animation
			useRangeAttack ();
			blockedByTarget = false;
			blockedByDestructible = false;
			bool foundPos = false;
			int spotsChecked = 0;
			bool tooManyChecks = false;
			Vector2Int tp = findPlayerPos(targP, posLookAhead);
			for (int i = rng; i < 3; i++, rng++) {
				Vector3 curRange = distRanges [distOrder[i]];
				int dir = (int)curRange.z;
				float start = curRange.x;
				float end = curRange.y;
				if (dir < 0) {
					start = curRange.y;
					end = curRange.x;
				}
				int len = (int)Mathf.Round (Mathf.Abs (start - end));
				for (float j = dst; j <= len; j += bodyLen, dst += bodyLen) { 
					float curDist = start + j * dir;
					//Debug.Log("circling: " + circling);
					if (!circling) {
						for (int k = cpc; k < 4; k++, cpc++) {
							Vector2Int curCheck = circlePosChecks [circlePosOrder [k]];
							int angle = Random.Range(0, 360);
							if (targSword) {
								int aD = 1;
								if (Random.value < 0.5) {
									aD = -1;
								}
								angle = Map.S.saneAddition((int)targSword.curPos, Random.Range(0, 45), aD);
							}
							int curAngle = Map.S.saneAddition (angle, curCheck.x, curCheck.y);
							//Debug.Log("cur angle: " + curAngle + " curCheck: " + curCheck);
							if (angleOverrides.Count == 0) {
								for (int l = acr; l <= 45; l += 5, acr += 5) {
									for (int m = -1; m <= 1; m += 2) {
										int chAng = Map.S.saneAddition (curAngle, l, m);
										Vector2Int spot = getCirclePos (chAng, curDist);
										//Debug.Log ("checking " + spot + " tp: " + tp+ " "  + " from angle: " + chAng + " " + acr);
										if (spot != lostPos) {
											if (checkSpot (tp + spot, tp, true)) {
												modPos = spot;
												//Debug.Log ("founf spot: " + (tp + spot));
												posAngle = chAng;
												foundPos = true;
												break;
											}
											if (spotsChecked < maxChecks) {
												spotsChecked++;
											} else {
												//Debug.Log ("too many checks " + spotsChecked);
												tooManyChecks = true;
												break;
											}
											if (l == 0) {
												break;// lol, so we dont check the orignal angle twice
											}
										} else {
											//Debug.Log ("checking lostPos: " + lostPos);
										}
									}
									if (foundPos || tooManyChecks) {
										break;
									}
								}
							} else {
								for (int l = acr; l < angleOverrides.Count; l++, acr++) {
									Vector2Int spot = getCirclePos (angleOverrides[l], curDist);
									//Debug.Log ("checking " + spot + " tp: " + tp+ " "  + " from angle: " + angleOverrides[l] + " " + curDist);
									if (spot != lostPos) {
										if (checkSpot (tp + spot, tp, true)) {
											modPos = spot;
											//Debug.Log ("founf spot: " + (tp + spot));
											posAngle = angleOverrides[l];
											foundPos = true;
											break;
										}
										if (spotsChecked < maxChecks) {
											spotsChecked++;
										} else {
											//Debug.Log ("too many checks " + spotsChecked);
											tooManyChecks = true;
											break;
										}
										if (l == 0) {
											break;// lol, so we dont check the orignal angle twice
										}
									}
								}
							}
							if (foundPos || tooManyChecks) {
								break;
							} else {
								acr = 0;
							}
						}
						if (foundPos || tooManyChecks) {
							break;
						} else {
							cpc = 0;
						}
					} else {
						if (circleAngle == 720) {
							circleAngle = Map.S.saneAddition ((int)getAngle (), 180, 1);
						}
						for (int k = crc; k < 90; k++, crc++) {
							circleAngle = Map.S.saneAddition (circleAngle, 4, 1);
							Vector2Int spot = getCirclePos (circleAngle, curDist);
							if (spotsChecked < maxChecks) {
								spotsChecked++;
							} else {
								tooManyChecks = true;
								break;
							}
							if (checkSpot (tp + spot, tp, false)) {
								modPos = spot;
								foundPos = true;
								break;
							}
						}
						if (foundPos || tooManyChecks) {
							break;
						} else {
							crc = 0;
						}
					}

				}
				if (foundPos || tooManyChecks) {
					break;
				} else {
					dst = 0;
				}
			}
			if (!foundPos && !tooManyChecks) {
				rng = 0;
			}
			//Debug.Log ("am I Lost: " + foundPos);

			if (foundPos) {
				rng = 0;
				dst = 0;
				cpc = 0;
				acr = 0; 
				crc = 0;
				if (circling) {
					circling = false;
				}
				lost = false;
			} else if (!tooManyChecks) {
				getLost ();//we ended the natural loop rather than by hitting too many checks
			}
		}
	}
    
	bool checkSpot (Vector2Int pos, bool checkSwing) {
		return checkSpot (pos, target.centerPoint, checkSwing);
	}

	public virtual bool checkSpot (Vector2Int pos, Vector2Int targCenter, bool checkSwing) { 
		bool goodSpot = false;
		//Debug.Log ("checking " + pos);
		self.checkBody (pos, false, false);
		if (self.curCollided.Count == 0) {
		//	Debug.Log ("I can fit there");
			goodSpot = true;
			if (!checkSight (pos, targCenter, false, false)) {
				goodSpot = false;
				if (readCheckSight) {
					Debug.Log ("I cant see them");
				}
			}
		}
		return goodSpot;
	}

	Vector2Int lostPos;
	int cpPrio = -1;
	public void getLost() {
		if (!lost) {
			//Debug.LogError (name + " got lost, already lost? " + lostPos);
			lost = true;
			if (lostPos != GM.S.invalidDest && modPos == lostPos) {
				//Debug.LogWarning ("spot: " + getTarPos () + ", " + name + " is stuck in a loop and needs to cycle ");
				//Debug.Log (modPos + " " + lostPos);
				if (cpPrio == -1) {
					cpPrio = circlePosOrder [0];
				} else {
					if (circlePosOrder [1] == cpPrio) {
						Debug.LogWarning ("weve cycled through all of the circlePoses and adjustment angles together");
						cpPrio = -1;
					}
				}
				cycleCirclePosUp ();
			} else {
				lostPos = modPos;
			}
		}
	}

	void pathToTarget() {
		closed.Clear ();
		open.Clear ();
		/*pathNode[] s = */pathMaster.getClosest (myCenter (), getTarPos (), neighbies);//self.centerPoint, getTarPos(), 5);
		pathNode start = null;//s [0];
		for (int i = 0; i < 5; i++) {
			if (neighbies [i] != null) {
				if (start == null && checkSight (myCenter(),/*self.centerPoint,*/ neighbies [i].getPos (), false, false)) {
					start = neighbies [i];
					break;
				}
			}
		}
		if (start == null) {
			getLost();
		} else {
			pathMaster.clearPathScores (aiNum);
			/*pathNode[] e = */pathMaster.getClosest (getTarPos (), myCenter (), neighbies);//self.centerPoint, 5);
			pathNode end = null;// e [0];
			for (int i = 0; i < 5; i++) {
				if (neighbies [i] != null) {
					if (checkSight (neighbies [i].getPos (), getTarPos(), false, true)) {
						end = neighbies [i];
						break;
					}
				}
			}
			if (end) {
				float ds = Vector2Int.Distance (myCenter()/*self.centerPoint*/, start.getPos ());//even if we are near the start/end, we may still need to get closer to it to see the target position
				if (start != end || ds > 5) {
					pathing (start, end, 0);
					int cCount = closed.Count;
					for (int i = 2; i <= cCount - 1; i++) {
						removeLoops (i);//2 - closed.Count -2
					}
					for (int i = 0; i < closed.Count; i++) {
						if (closed [i] == null) {
							closed.RemoveAt (i);
							i--;
						}
					}
					step = 0;
					if (closed.Count == 1) {
						checkSight (/*self.centerPoint*/myCenter(), getTarPos (), true, false);
						if (!clearSight) {
							stopMove ();
						}
					}
				} 
			} else {
				getLost();
			}
		}
	}

	void removeLoops(int checkBegin) {
		int parent = -1;
		bool noConnect = false;
		pathNode checking = closed [closed.Count - (checkBegin -1)];
		if (checking != null) {
			for (int i = closed.Count - checkBegin; i >= 0; i--) {
				if (closed [i] != null) {
					if (closed [i] != checking.getParent (aiNum)) {
						noConnect = true;
					} else {
						if (noConnect) {
							parent = i;
						}
						break;
					}
				}
			}
			if (parent > -1) {
				int extra = closed.Count - parent - checkBegin;
				for (int i = 0; i < extra; i++) {
					closed [closed.Count - checkBegin - i] = null;
				}
			}
		}
	}

	pathNode[] neighbies;

	void pathing(pathNode cur, pathNode destination, int noLoop) {
		if (cur != null) {
			open.Remove (cur);
			closed.Add (cur);
			if (cur.getPos () == destination.getPos()) {
				return;
			} else {
				//bool dodgeMove = dodgeMoveChance > 0;

				/*pathNode[] adj = */cur.getNeighbors (dodgeThru(), neighbies);
				for (int i = 0; i < 8; i++) {
					/*if (adj [i] != null) {
						if (!closed.Contains (adj [i])) {
							if (!adj [i].isDeadEnd (cur, destination)) {
								adj [i].calcScore (aiNum,myCenter(), target.centerPoint, cur);
								if (!open.Contains (adj [i])) {
									open.Add (adj [i]);
								}
							}
						}
					}*/
					if (neighbies [i] != null) {
						if (!closed.Contains (neighbies [i])) {
							if (!neighbies [i].isDeadEnd (cur, destination)) {
								neighbies [i].calcScore (aiNum, /*self.centerPoint*/myCenter(), target.centerPoint, cur);
								if (!open.Contains (neighbies [i])) {
									open.Add (neighbies [i]);
								}
							}
						}
					}
				}
				pathNode next = null;
				float lowestScore = Mathf.Infinity;
				for (int i = 0; i < open.Count; i++) {
					if (open [i].getScore (aiNum) < lowestScore) {
						next = open [i];//if it connects to prev
						lowestScore = open [i].getScore (aiNum);
					}
				}
				pathing (next, destination, noLoop + 1);
			}
		} 
	}

	public virtual bool dodgeThru() { return false; }

	
	public Vector2Int findPlayerPos (Player p, int units) {
		if (!p) {
			Debug.LogError("player is missing"); // probably uncommnent next lines, but need to test, sometimes this could cause a null refrence
			//getLost();
			
			//return new Vector2Int(-1, -1);
		}
		Form pf = p.GetComponent<Form> ();
		Vector2Int pos =  pf.centerPoint;
		 if (p != targP) {
			pos = p.myAttack.getCenter ();// pf.centerPoint;
		}
		if (p.dodging) {
			int[] dp = p.getDP ();
			if (dp.Length > 0) {
				float ds = p.getDodgeSpeed ();
				int di = p.getDI ();
				for (int i = 0; i < units; i++) {
					if (di < dp.Length) {
						for (int k = 0; k < ds && di < dp.Length; k++) {
							pos += GM.S.dirs [dp [di]];
							di++;
						}
					} else {
						break;
					}
				}
			}
			return pos;
		} else if (p.mDest.x > -1 && p.mDest.y > -1 || p.knocked) {
			bool moving = p.mDest.x > -1 && p.mDest.y > -1;
			bool knocked = p.knocked;
			int pSpeed = pf.speed;
			int pHSpeed = pf.hyperSpeed;
			int ac = p.getAccel ();
			int a = p.getAc ();
			int s = p.getSc ();
			int aa = p.getAA ();//amount acelled
			int sd = p.getSpeedDiff();
			float inP = p.getInP ();//inPercent for creeping
			Controller ic = p.gameObject.GetComponent<Controller> ();
			Vector2Int dest = p.mDest;
			defense pDef = p.getDef ();
			Vector2Int kSpd = pDef.getKnockSpeeds ();
			int kc = pDef.getKnockCounter ();
			float pow = pDef.getPower();
			Vector2 flyD = pDef.getFlyDirection ();
			for (int i = 0; i < units; i++) {
				if (moving) {
					if (s >= pSpeed) {
						s = 0;
						for (int j = 0; j < pHSpeed; j++) {
							if (pos == dest) {
								dest = ic.calculateDestination ();
							}
							Vector2Int dir = GM.S.dirs [GM.S.getClosestDir (pos, dest)];//chosenD];
							pos += dir;
						}
					} else {
						s++;
					}
					if (aa < sd * inP) {
						if (a > ac) {
							a = 0;
							if (pSpeed > p.getMaxSpeed ()) {
								pSpeed--;
								aa++;
							} else {
								if (pHSpeed < p.getMaxHSpeed ()) {
									pHSpeed++;
									aa++;
								}
							}
						} else {
							a++;
						}
					}
				}
				if (knocked) {
					if (pow > 0) {
						if (kc >= kSpd.x) {
							kc = 0;
							for (int j = 0; j < kSpd.y; j++) {
								Vector2Int hit = pDef.knockCheck (pos, flyD);
								if (hit.x == 1) {
									flyD.x *= -1;
									if (pow - 4 > 0) {
										pow -= 4;
									}
								}
								if (hit.y == 1) {
									flyD.y *= -1;
									if (pow - 4 > 0) {
										pow -= 4;
									}
								}
								Vector2Int fly = new Vector2Int ((int)(flyD.x * 20), (int)(flyD.y * 20));
								Vector2Int dir = GM.S.dirs [GM.S.getClosestDir (pos, pos + fly)];//chosenD];
								pos += dir;
								pow -= pDef.getKnockDecel();
								kSpd = pDef.calcKnockSpeeds (pow);
							}
						} else {
							kc++;
						}
					}
				}

			}
			return new Vector2Int ((int)pos.x, (int)pos.y);
		} else {
			if (p == targP) {
				return p.GetComponent<Form> ().centerPoint;
			} else {
				return p.myAttack.getCenter ();
			}
		}

	}

	public int howLongUntilPlayerReaches(Player p, Vector2Int destination, int range) {
		if (p == targP) {
			if (p.self.centerPoint == destination) {
				return 0;
			}
		} else {
			if (p.myAttack.getCenter () == destination) {
				return 0;
			}
		}
		int units = 0;
		Form pf = p.GetComponent<Form> ();
		Vector2Int pos = pf.centerPoint;
		if (p != targP) {
			pos = p.myAttack.getCenter ();
		}
		if (p.dodging) {
			int[] dp = p.getDP ();
			if (dp.Length > 0) {
				float ds = p.getDodgeSpeed ();
				int di = p.getDI ();
				while (units < 100 && Vector2Int.Distance (destination, new Vector2Int ((int)pos.x, (int)pos.y)) > range) {
					if (di < dp.Length) {
						for (int k = 0; k < ds && di < dp.Length; k++) {
							pos += GM.S.dirs [dp [di]];
							di++;
						}
					} else {
						break;
					}
					units++;
				}
			}
			return units;
		} else if (p.mDest.x > -1 && p.mDest.y > -1  || p.knocked) {
			bool moving = p.mDest.x > -1 && p.mDest.y > -1;
			bool knocked = p.knocked;
			int pSpeed = pf.speed;
			int pHSpeed = pf.hyperSpeed;
			int ac = p.getAccel ();
			int a = p.getAc ();
			int s = p.getSc ();
			int aa = p.getAA ();//amount acelled
			int sd = p.getSpeedDiff();
			float inP = p.getInP ();//inPercent for creeping
			Controller ic = p.gameObject.GetComponent<Controller> ();
			Vector2Int dest = p.mDest;
			defense pDef = p.getDef ();
			Vector2Int kSpd = pDef.getKnockSpeeds();
			int kc = pDef.getKnockCounter ();
			float pow = pDef.getPower();
			Vector2 flyD = pDef.getFlyDirection ();
			while (units < 100 && Vector2Int.Distance (destination, new Vector2Int ((int)pos.x, (int)pos.y)) > range) {// && (dir.x != 0 || dir.y != 0)) {
				if (moving) {
					if (s >= pSpeed) {
						s = 0;
						for (int j = 0; j < pHSpeed; j++) {
							if (pos == dest) {
								dest = ic.calculateDestination ();
							}
							Vector2Int dir = GM.S.dirs [GM.S.getClosestDir (pos, dest)];//chosenD];
							pos += dir;
						}
					} else {
						s++;
					}
					if (aa < sd * inP) {
						if (a > ac) {
							a = 0;
							if (pSpeed > p.getMaxSpeed ()) {
								pSpeed--;
								aa++;
							} else {
								if (pHSpeed < p.getMaxHSpeed ()) {
									pHSpeed++;
									aa++;
								}
							}
						} else {
							a++;
						}
					}
				}
				if (knocked) {
					if (pow > 0) {
						if (kc >= kSpd.x) {
							kc = 0;
							for (int i = 0; i < kSpd.y; i++) {
								Vector2Int hit = pDef.knockCheck (pos, flyD);
								if (hit.x == 1) {
									flyD.x *= -1;
									if (pow - 4 > 0) {
										pow -= 4;
									}
								}
								if (hit.y == 1) {
									flyD.y *= -1;
									if (pow - 4 > 0) {
										pow -= 4;
									}
								}
								Vector2Int fly = new Vector2Int ((int)(flyD.x * 20), (int)(flyD.y * 20));
								Vector2Int dir = GM.S.dirs [GM.S.getClosestDir (pos, pos + fly)];//chosenD];
								pos += dir;
								pow -= pDef.getKnockDecel();
								kSpd = pDef.calcKnockSpeeds (pow);
							}
						} else {
							kc++;
						}
					}
				}
				units++;
			}
			return units;
		} else {
			return -1;
		}
	}
	
	
	public Vector2 getDir() {
		return new Vector2(target.centerPoint.x - myCenter().x, target.centerPoint.y - /*self.centerPoint.y*/myCenter().y).normalized;//dir;
	}

	public float getAngle() {
		Vector2 dir = getDir ();
		float x = (int)(dir.x * 1000) - myCenter ().x;//self.centerPoint.x;
		float y = (int)(dir.y * 1000) - myCenter().y;//self.centerPoint.y;
		float angle = Mathf.Atan2 (y, x) * Mathf.Rad2Deg;
		return angle;
	}

	public void cycleDistancesUp() {
		int tmp = distOrder [0];
		for (int i = 1; i < 3; i++) {
			distOrder [i - 1] = distOrder [i];
		}
		distOrder [2] = tmp;
		if (!lost) {
			getLost ();
		}
	}

	public void cycleDistancesDown() {
		int tmp = distOrder [2];
		for (int i = 2; i > 0; i--) {
			distOrder [i] = distOrder [i - 1];
		}
		distOrder [0] = tmp;
		if (!lost) {
			getLost ();
		}
	}
		
	public void cycleCirclePosUp() {
		int tmp = circlePosOrder [0];
		for (int i = 1; i < 4; i++) {
			circlePosOrder [i - 1] = circlePosOrder [i];
		}
		circlePosOrder [3] = tmp;
	}

}
