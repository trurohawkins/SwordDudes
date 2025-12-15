using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockable : Attack {

	public bool dieAtRest = false;
	public bool dieOnImpact = false;
	public bool momentumDamage = true;
	int power;
	int flyAmount;
	int maxSpeed;
	int minSpeed;
	float inertia;
	float hyp;
	float spd;
	Vector2 flyDirection;
	Vector2 spot;
	int distTravelled;
	int framesAlive = 0;
	bool atRest;

	protected override void Awake() {
		base.Awake ();
		self.addAction (this);
		active = true;
		if (!val) {
			Debug.LogError("no value");
		}
		//inertia = (float)(maxSpeed + minSpeed) / flyAmount;
	}
	bool bounced = false;
	// flying used to determine if we hit something by our own power or theirs
	//if our power we want to bounce otherwise, we want to set our flyDIrection
	bool flying = false;

	public override IEnumerator callAction(int delay){
		if (!atRest) {
			framesAlive++;
			//if (power > 0) {
			int fullRange = maxSpeed + minSpeed;
			float curRange = hyp + (minSpeed - spd);
			if (momentumDamage) {
				val.valuePercent("damage", (hyp + (minSpeed - spd)) / (maxSpeed + minSpeed));
			}
			if (speedCounter >= speed) {
				flying = true;
				for (int i = 0; i <= self.hyperSpeed; i++) {
					power -= self.hyperSpeed;
					if (hyp > 0) {
						hyp -= inertia;
						if (hyp >= 0) {
							self.hyperSpeed = (int)hyp;
						}
					} else if (speed < minSpeed) {
						spd += inertia;
						speed = (int)spd;
					} else {
						if (dieAtRest) {
							endAttack();
						} else {
							atRest = true;
						}
					}
					//Vector2Int move = new Vector2Int((int)(self.centerPoint.x + (flyDirection.x * self.hyperSpeed)), (int)(self.centerPoint.y + (flyDirection.y * self.hyperSpeed)));

					distTravelled++;
					Vector2Int move = new Vector2Int ((int)spot.x, (int)spot.y);
					if (move + flyDirection != self.centerPoint) {
						bool moved = false;
						//Debug.Log(move + " + " + flyDirection + " = " + (move + flyDirection));
						move.x += (int)flyDirection.x;
						//Debug.Log(move.x + " " + self.centerPoint.x);
						int dd = 6;
						if (move.x != self.centerPoint.x) {
							if (flyDirection.x < 0) {
								dd = 2;
							}
							if (!self.checkSide (move, dd, true, true)) {
								bool noDestructible = true;
								for (int j = 0; j < self.curCollided.Count; j++) {
									int othID = self.curCollided[j].id;
									//5 for Forms on the floor
									// hitting swords and players handled in other call action
									if (othID == 4 || othID == 5 || othID == 1 || othID == 2) {
										noDestructible = false;
									} else {
										//Debug.Log("flying i hit " + self.curCollided[j].name);
									}
								}
								if (noDestructible) {
									float change = Random.Range(-1.2f, -0.8f);
									if (Mathf.Clamp(flyDirection.x * change, -1, 1) == 0) {
										flyDirection.x *= -1;
									} else {
										flyDirection.x = Mathf.Clamp(flyDirection.x * change, -1, 1);
									}
									move.x = (int)spot.x;
								} else {
									moved = true;
								}
							} else {
								moved = true;
							}
						}
						move.y += (int)flyDirection.y;
						if (move.y != self.centerPoint.y) {
							//Debug.Log("checking y");
							if (flyDirection.y < 0) {
								dd = 4;
							} else {
								dd = 0;
							}
							if (!self.checkSide (move, dd, true, true)) {
								bool noDestructible = true;
								for (int j = 0; j < self.curCollided.Count; j++) {
									int othID = self.curCollided[j].id;
									//5 for Forms on the floor
									// hitting swords and players handled in other call action
									if (othID == 4 || othID == 5 || othID == 1 || othID == 2) {
										noDestructible = false;
									} else {
										//Debug.Log("flying i hit " + self.curCollided[j].name);
									}
								}
								if (noDestructible) {
									float change = Random.Range(-1.2f, -0.8f);
									if (Mathf.Clamp(flyDirection.y * change, -1, 1) == 0) {
										flyDirection.y *= -1;
									} else {
										flyDirection.y = Mathf.Clamp(flyDirection.y * change, -1, 1);
									}
									move.y = (int)spot.y;
								} else {
									moved = true;
								}
							} else {
								moved = true;
							}
						}
						if (self.centerPoint != move) {
							if (moved) {
							//if (self.checkBody (move, true, true)) {
								self.forceMove (move, true);
								spot += flyDirection;
							}
							/*} else {
								Debug.Log(name + " didn't move, spot:" + spot + " move: " + move);
							}*/
						} else {
							spot += flyDirection;
						}
					} else {
						spot += flyDirection;
					}
					if (spot.x < 0 || spot.y < 0 || spot.x >= Map.S.worldSizeX || spot.y >= Map.S.worldSizeY) {
						Debug.LogError("moving out of worlf " + spot);
					}
					//Vector2Int move = new Vector2Int ((int)(spot.x + flyDirection.x), (int)(spot.y + flyDirection.y));
					/*
					if (move != self.centerPoint) {
						if (!self.checkSide (move, 0, true, true) || !self.checkSide (move, 4, true, true)) {
							flyDirection.y *= -1;// Random.Range(-1.2f, -0.8f);
							Debug.Log ("y bounce");
							bounced = true;
						}
						if (!self.checkSide (move, 2, true, true) || !self.checkSide (move, 6, true, true)) {
							flyDirection.x *= -1;//Random.Range(-1.2f, -0.8f);
							Debug.Log ("x bounce");
							bounced = true;
						}
						if (!bounced) {
							self.checkBody (move, true, false);
							if (self.curCollided.Count == 0) {
								self.forceMove (move, true);
								spot += flyDirection;
							} else {
								for (int j = 0; j < self.curCollided.Count; j++) {
									Debug.Log ("hit " + self.curCollided [j].name);
								}
							}
						}
					} else {
						spot += flyDirection;

					}
					*/
				}
				flying = false;
				speedCounter = 0;
			} else {
				speedCounter++;
			}
		//} 
		//Debug.Log("fa: " + framesAlive + " travelled " + distTravelled + " " + spot);
		}
		yield return new WaitForSeconds (0.1f);
	}

	public override void callAction(Form enemy, int state, int x, int y) {
		//Debug.Log("slefid: " + self.id);
		if (flying) {
			//Debug.Log("flying lets ignore");
			//return;
		}
		if (enemy.id != -1 && enemy.id != self.id && enemy.id != 4 && enemy.id != 5) {
			//power = flyAmount;
			atRest = false;
			hyp = self.hyperSpeed = maxSpeed;
			spd = speed = 0;
			flyDirection = self.centerPoint - enemy.centerPoint;
			//Debug.Log ("hit by " + enemy.name + " " + flyDirection + " " + self.centerPoint + " " + enemy.centerPoint);
			flyDirection.Normalize();
			Value v = enemy.GetComponent<Value>();
			if (v) {
				if (val.getTeam() != -1) {
					val.setTeam(v.getTeam());
				}
			}
        }
		if (dieOnImpact) {
			defense o = enemy.GetComponent<defense>();
			endAttack();
		}
	}

	public void knock(Vector2 dir) {
		//power = flyAmount;
		atRest = false;
		flyDirection = dir;
		//Debug.LogError("knocked " + dir);
		hyp = self.hyperSpeed = maxSpeed;
		spd = speed = 0;
	}

	public void setStats(int minSpd, int maxSpd, int flyAmnt, Vector2 n_spot, Vector2 fDir) {//, float n_hyp, int n_pow) {
		hyp = maxSpeed = maxSpd;
		self.hyperSpeed = (int)hyp;
		minSpeed = minSpd;
		flyAmount = flyAmnt;
		inertia = (float)(maxSpeed + minSpeed) / flyAmount;
		flyDirection = fDir;
		//Debug.LogError("fly direction: " + fDir);
		//power = n_pow;
		//hyp = n_hyp;

		speed = 0;
		spd = 0;
		spot = n_spot;
		//Debug.Log ("Starting: " + spot);
	}

	public void setStats(int maxSpd, int distance, Vector2Int n_spot, Vector2 fDir) {
		hyp = maxSpeed = maxSpd;
		self.hyperSpeed = (int)hyp;
		minSpeed = 10;
		speed = 0;
		spd = 0;
		flyDirection = fDir;
		spot = n_spot;
	}

	public void setFeatures(bool impactDeath) {
		dieOnImpact = impactDeath;
	}

	protected override void endAttack() {
		base.endAttack();
		self.die();
	}
}
