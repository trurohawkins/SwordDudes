using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPathBoi : PathBoi {

	InputComputer guy;

     protected override void Awake() {
        base.Awake();
		guy = gameObject.GetComponent<InputComputer>();
    }

    public override void getEnemy(GameObject enemy) {
        base.getEnemy(enemy);
		guy.setTarget(targSword, targSoul);
    }
    
    public override int move() {
        int ret = base.move();
		if (ret == 1) {
			guy.dodgeStep = closed [step].dPar[aiNum];
		} else if (ret == 2) {
			bool dodgeThru = false;
			if (blockedByTarget) {
				if (Random.value < guy.dodgeCircleChance) {
					guy.me.mDest = target.centerPoint;
					Vector2Int dodgePos = guy.me.getDodgePos (1);//guy.me.maxDodgePower);
					self.checkBody (dodgePos, false, false);
					//Debug.LogError ("dodge thru " + dodgePos);
					if (self.curCollided.Count == 0) {
						guy.dodge();
						stopMove ();
						dodgeThru = true;
						blockedByTarget = false;
						getLost ();
					} 
				} else if (!lost && !dodgeThru) {
					circling = true;
					getLost ();
				}
			}
		}
		return 0;
    }

	public override void updateEnemySwordInfo() {
		//Debug.LogError ("updating enemy sword info " + (bodyLen + targSword.attackRange.y) + " != " + enRange);
		//Debug.LogError ("enSwordSize: " + targSword.curHitListkForm ().width + " != " + enSwordSize);
		guy.enRange = bodyLen + targSword.attackRange.y;
		//distRanges [1] = new Vector3 (enRange + targP.getMaxHSpeed () + (bodyLen * 2), (enRange + (bodyLen * 2) + targP.getMaxHSpeed ()) * 3, 1);
		distRanges[1].x = guy.enRange + targP.getMaxHSpeed () + (bodyLen * 2);
		distRanges [1].y = (guy.enRange + (bodyLen * 2) + targP.getMaxHSpeed ()) * 3;
		distRanges [1].z = 1;
		//distRanges [2] = new Vector3 (distRanges [1].y + 1, Mathf.Max (Map.S.worldSizeX, Map.S.worldSizeY) / 2, 1);
		distRanges[1].x = distRanges [1].y + 1;
		distRanges [1].y = Mathf.Max (Map.S.worldSizeX, Map.S.worldSizeY) / 2;
		distRanges [1].z = 1;
		guy.enSwordSize = targSword.getBladeWidth();
	}
	
    public override bool isGoodTarget(int player, Form f) {
        return base.isGoodTarget(player, f) && !GM.S.players [player].onTeam (guy.me.getTeam ());
    }

    public override bool goodToMove() {
        return base.goodToMove() && !guy.me.dead && !guy.me.dodging;
    }

    public override bool amMoving() {
        return (guy.me.mDest.x != -1 || guy.me.mDest.y != -1);
    }

    public override Vector2Int myCenter() {
		if (guy) {
			if (guy.attack) {
				return guy.attack.getCenter();
			} else {
				Debug.Log(name + " no attack");
			}
		} else {
			Debug.Log(name + " no guy ");
		}
		return new Vector2Int((int)transform.position.x, (int)transform.position.y);
	}

	public override void useRangeAttack() {
		if (target) {
			if (Random.value < guy.rangeAttack) {
				bool sightClear = checkSight (myCenter (), target.centerPoint, false, false);
				if (!guy.rangeBlocked && sightClear && guy.attack.range > 3) {
					guy.rangedAttack = true;
					while (distOrder [0] != 2) {
						cycleDistancesUp ();
					}
				} else {
					guy.rangedAttack = false;
					while (distOrder [0] != 0) {
						cycleDistancesDown ();
					}
				}
				guy.rangeBlocked = false;//reset value t ochekc again
			}
		}
	}

	public override bool dodgeThru() {
		return guy.dodgePath && Random.value < guy.dodgePathChance;
	}

    public override void moveTowards(Vector2Int start, Vector2Int destination) {
		if (!guy.me.dodging) {
			//Debug.Log("move towards " + destination + " Step: " + step);
			Vector2Int dest = canIGo (myCenter (), destination, true);//self.centerPoint, destination, false);
			for (int i = 0; i < self.curCollided.Count; i++) {
				if (self.curCollided[i].id == 4 && Random.value > walkIntoDestructible) {
					blockedByDestructible = true;
				} else if (self.curCollided[i].id != 4 && self.curCollided [i] == target || self.curCollided[i].parent == target) {
					blockedByTarget = true;
				}
			}
			self.curCollided.Clear ();
			if (dest.x >= 0 && dest.y >= 0) {
				guy.me.mDest = dest;
				guy.me.moveInput = true;
				if (!guy.attack.swinging && Random.value < guy.dodgeMoveChance) {//can interrupt adjustments and attacks
					float destDist = Vector2Int.Distance (dest, self.centerPoint);
					Vector2Int dodgePos = guy.me.getDodgePos (1);//guy.me.maxDodgePower);
					if (destDist > Vector2Int.Distance (self.centerPoint, dodgePos)) {
						if (Random.value < guy.dodgeMoveCheck && Vector2.Distance (dodgePos, findPlayerPos (targP, 4)) >= guy.attackDist + 1) {
							self.checkBody (dodgePos, false, false);
							if (self.curCollided.Count == 0) {
								guy.dodge ();
							}
						} else {
							//Debug.LogError("cant dodge move " + findPlayerPos(targP, 4));
						}
					}
				}
			} else if (!blockedByTarget) {
				if (guy.dodgeStep && Random.value > guy.dodgeWallStuck) {
					guy.me.mDest = destination;
					Vector2Int dodgePos = guy.me.getDodgePos (1);//guy.me.maxDodgePower);
					self.checkBody (dodgePos, false, false);
					if (self.curCollided.Count == 0) {
						guy.dodge ();
						guy.dodgeStep = false;
					} 
				} else {
					guy.me.moveInput = false;
				}
			}
		}
    }

	public override bool checkSpot (Vector2Int pos, Vector2Int targCenter, bool checkSwing) {
		bool goodSpot = false;
		//Debug.Log ("checking " + pos);
		self.checkBody (pos, false, false);
		if (self.curCollided.Count == 0) {
		//	Debug.Log ("I can fit there");
			goodSpot = true;
			if (!checkSight (pos, targCenter, false, true)) {
				goodSpot = false;
				if (readCheckSight) {
					Debug.Log ("I cant see them");
				}
			} else if (checkSwing) {
				if (!guy.rangedAttack) {
					guy.isSwingBlocked (pos, (int)guy.attack.curPos, (int)getAngle (), 1, guy.ignoreListSP);//not necesarrily accurate because the sword could change pos
					if (guy.hitList.Count != 0) {
						guy.isSwingBlocked (pos, (int)guy.attack.curPos, (int)getAngle (), -1, guy.ignoreListSP);
					}
				} else {
					guy.isSwingBlocked (pos, (int)guy.attack.curPos, Map.S.saneAddition (guy.attack.curPos, 1, -1), 1, guy.ignoreListSP);//do a 360 check
				}
				if (guy.hitList.Count != 0) {
					if (goodSpot && !checkSwing) {
						for (int i = 0; i < guy.hitList.Count; i++) {
							Debug.Log ("chheck spot " + pos + " hit " + guy.hitList [i].name + " would've stopped if we checked swing while checking spots");
						}
					}
					if (checkSwing) {
						if (guy.readCheckSwing) {
							Debug.Log ("bad spot cuz I cant swing");
						}
						goodSpot = false;
					}
				}
			}
		}
		return goodSpot;
	}

}
