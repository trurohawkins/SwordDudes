using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;

public class Player : Purpose {
	public Vector3Int startPos;
	public int startAngle;
	public Form body;
	public GameObject soulType;
	public circleAttack myAttack;
	public GameObject blade;
	public GameObject circle;
	//[SyncVar]
	public bool dead;
	int hiSpeed = 0;
	public int loSpeed = 10;
	int maxHyperSpeed = 5;
	int maxExtraSpeed;
	int extraSpeed;
	int slowDown;
	int speedUp;
	int accelSpeed;
	int accelCounter = 0;
	int decelSpeed;
	int decelCounter = 0;
	Vector2 damage;
	Vector2 staggerDamage;
	int staggerTime;
	Vector2 knockBackDamage;
	int attackRange = 5;
	int attackWidth;
	int attackLength;
	int attackWidthSpacing;
	float attackTaper;
	int swordWeight;
	public int speedDiff;
	public float stamina;
	float maxStamina;
	float sRecoverySpeed;
	public float swordCost;
	public float dodgeCost;
	float dodgeLength = 25;
	int dodgeSpeed = 2;

	defense protection;
	float movingPoise;
	float relaxedPoise;
	float readyPoise;
	float attackingPoise;

	Transform staminaBar;
	public bool stagged;
	public bool knocked;
	public bool dodging;
	int dodgeStart = -1;
	int dodgeStartUp;
//	public float moveX;
//	public float moveY;
	public float strikeX;
	public float strikeY;
	public Vector2Int move;
	public bool going;
	public bool moveInput;
	int turnCost = 25;
	public int turnAdjust;

	public GameObject attack;
	public GameObject dodger;
	int deaths = 0;
	public int playerNum;
	Controller ic;
	public bool hasSword = true;
	public int swordStart;

	public FighterAnimator graphics;
	public int colorNum;
	public int team = -1;
	public bool fodder = false;

	public void setSpawnInfo(Vector3Int spawnPos) {
		//Debug.Log ("spawning player: " + teamNum);

		startPos = spawnPos;

		//myColor = pColor;
		//gameObject.GetComponent<Form> ().color = pColor;
	}

	public void setInfo(int pNum, int cNum, Color pColor, int teamNum) {
		initialize ();
		playerNum = pNum;
		colorNum = cNum;
		team = teamNum;
		gameObject.name = "Player" + pNum;
		SwordSoul ss = soulType.GetComponent<SwordSoul>();
		if (protection) {
			//protection.spawnHealthBar (pNum, pColor, ss.mainColor[GameInfo.S.colors[pNum]], ss.subColor[GameInfo.S.colors[pNum]]);
			if (!fodder) {
				protection.spawnHealthIcon(this);
			}
			protection.setTeam (teamNum);
		} else {
			Debug.Log ("no protetction");
		}
		if (!GM.S.drawSprites) {
			gameObject.GetComponent<Form> ().skin = null;
		}
	}

	 protected override void Awake() {
		base.Awake();
		mDest = GM.S.invalidDest;
		dp = new int[0];
		if (!body) {
			initialize ();
		}
		ic = gameObject.GetComponent<Controller> ();
		if (GM.S.drawSprites) {
			graphics = gameObject.transform.GetChild(0).GetComponent<FighterAnimator>();
		} else {
			gameObject.transform.GetChild(0).gameObject.SetActive(false);
		}
	}

	void Start () {
		if (fodder) {
			return;
		} else {
			doStart();
		}

	}

	public void doStart() {
		body.speed = loSpeed + slowDown - speedUp;
		body.direction = 4;
		protection.setPoise (0);

		if (graphics && GM.S.drawSprites) {
			graphics.spawn();
		}

		if (hasSword) {
			setUpSword();
		}
		if (self) {
			self.addAction (this);
		}
	}

	public void turnOffSprites() {
		graphics.gameObject.SetActive(false);
		if (myAttack) {

			myAttack.getSoul().setSprites(false);
		} else {
			//Debug.Log("no anim yet");
		}
	}

	public void turnOnSprites() {
		graphics.gameObject.SetActive(true);
		if (myAttack) {
			myAttack.getSoul().setSprites(true);
		}
	}

	public void setUpSword() {
		myAttack = Instantiate (soulType).GetComponent<circleAttack> ();
		myAttack.gameObject.GetComponent<SwordSoul> ().setColors (colorNum);
		setSwordStart(startPos.z);
		/*
		if (playerNum == 0) {
			myAttack.startPos = GM.S.startPoses [playerNum];GM.S.p1StartPos;
		} else {
			myAttack.startPos = GM.S.p2StartPos;
		}
		*/
		myAttack.meetPlayer (this);
		myAttack.gameObject.name = "Sword" + playerNum;
		myAttack.setTeam (team);
		//GM.S.spawnObj (myAttack.gameObject, startPos.x, startPos.y);
		Map.S.spawnForm (myAttack.gameObject, startPos.x, startPos.y);

		myAttack.spawnSword ();
		startAngle = (int)myAttack.curPos;
        if (GM.S.drawSprites && graphics) {
            if (myAttack) {
                graphics.setColors(myAttack.gameObject.GetComponent<SwordSoul>(), colorNum);
				myAttack.getSoul().setSprites(false);
            }
        } else {
			if (!GM.S.drawSprites)  {
				self.changeColorN(myAttack.gameObject.GetComponent<SwordSoul> ().mainColor [colorNum]);
				gameObject.transform.GetChild (2).gameObject.SetActive (false);

			}
		}
		if (protection && !fodder) {
			protection.setUIColor(colorNum);
		}
    }

    public void setSwordStart(int pos) {
		if (myAttack) {
			myAttack.startPos = pos;
			myAttack.curPos = pos;
		}
	}

	public void setTeam(int t) {
		team = t;
		if (myAttack) {
			myAttack.setTeam (team);
		}
		if (protection) {
			protection.setTeam(team);
		}
	}

	public Vector2Int getCenter() {
		if (myAttack) {
			return myAttack.getCenter ();
		} else {
			return self.centerPoint;
		}
	}

	public Vector2 getPos() {
		return new Vector2(transform.position.x, transform.position.y);
	}

	public void initialize(){
		body = gameObject.GetComponent<Form> ();
		protection = gameObject.GetComponent<defense> ();
	}

//	float lastX = 0;
//	float lastY = 0;
	int lastDir;
	public bool stopped;
	public bool blocked = false;
	bool moving = false;

	public override IEnumerator callAction(int delay) {
		if (dodging) {
			if (dodgeStart == -1) {
				if (dp.Length > 0) {
					// dodge dash
					if (dodgei < dp.Length) {
						//Debug.Log("dodign " + Mathf.Min(dodgeSpeed + chainDodges, maxDodgeSpeed));
						for (int k = 0; k < Mathf.Min(dodgeSpeed + chainDodges, maxDodgeSpeed) && dodgei < dp.Length; k++) {
							body.checkSide(body.centerPoint, dp[dodgei], true, false);
							if (dp.Length - dodgei > body.length || body.curCollided.Count == 0) {//at end of dodge there might be a moobile object that gets in the way
								body.forceMove (dp [dodgei]);
								if (myAttack) {
									myAttack.forceMove ();
								}
								if (Grader.S && body.curCollided.Count > 0) {
									Grader.S.dodgeThruOb(body);
								}
							} else {
								dodgei = dp.Length;
								endDodge ();
							}
							
							if (graphics) {
								graphics.setDir (body.direction);
							}
							dodgei++;
						}
					} else {
						endDodge ();
					}
				} else {//if (dr == -1) {
					// in place dodging
					if (dodgei < dodgeLength) {
						dodgei++;
					} else {
						endDodge ();
					}
				}
			} else {
				dodgeStart--;
				if (dodgeStart == -1) {
					body.etheral = true;
					protection.invulnerable = true;
					if (graphics) {
						graphics.setDodge (2);
					}
					ic.newDest ();
				}
			}
		} else if (!dead && !stopped) {
			if (name == "Player0") {
				//Debug.LogWarning (name + " unit passed " + self.centerPoint);
			}
			if (mDest.x != -1 && mDest.y != -1 && !knocked) {
				if (!moving) {
					moving = true;
					if (myAttack) {
						myAttack.getSoul().playerStartMove();
					}
				}
				if (!myAttack || !myAttack.swinging) {
					protection.setPoise (1); // moving poise
				}
				float spd = body.speed;
				if (myAttack) {
					spd = (body.speed * 1 / myAttack.getFriction ());
				}
				//spd += turnAdjust;
				Vector2Int move = changeDir(mDest);
				turn(move);
				if (speedCounter >= spd) {
					turnAdjust = 0;
					for (int i = 0; i <= body.hyperSpeed; i++) {
						if (playerMove (move, false)) {
							ic.iMoved (0);
						} else {
							ic.iMoved (1);
						}
					}
					lastDir = body.direction;
					speedCounter = 0;
				} else {
					speedCounter++;
				}
				if (Grader.S && amountAccelled < speedDiff) {
					Grader.S.creeping(self);
				}
				//Debug.Log ("I am at " + body.centerPoint);
			} else {
				if (moving) {
					moving = false;
					if (myAttack) {
						myAttack.getSoul().playerStopMove();
					}
				}
				if (!dodging && graphics) {
					//if (graphics.walking != false) {
						graphics.setWalking (false);
					//}
				}
				if (!myAttack || !myAttack.swinging) {
					protection.setPoise (0);
				}
				moveInput = false;
			}
			acceleration ();
			//recovering dodge
			if (dr >= 0) {
				dr--;
				if (dr <= 0) {
					recoverDodge();
				}
			}
		} 
		yield return new WaitForFixedUpdate();
	}

	public Vector2Int mDest;
	int diagMod = 0;
	int desiredDir;

	public void getDest(Vector2Int newDest) {
		if (newDest.x != -1) {
			Vector2Int newD = newDest - body.centerPoint;
			desiredDir =  GM.S.convertVectorToDir (newD);
			/*
			int nd = GM.S.convertVectorToDir (newD);
			int bd = body.direction;
			if (nd != bd) {
				Debug.Log ("old dir " + bd + " new dir " + nd);
				if (nd % 2 == 1) {
					if (diagMod == 0) {
						//hiSpeed++;
						diagMod = 1;
						calcSpeedDiff ();
						if (body.speed == hiSpeed + slowDown) {
							body.speed++;
							amountAccelled--;
						}

					}
				} else {
					if (diagMod == 1) {
						//hiSpeed--;
						diagMod = 0;
						calcSpeedDiff ();
						if (body.speed == (hiSpeed + 1) + slowDown) {
							body.speed--;
							amountAccelled++;
						}
					}

				}
				//if (mDest.x != -1) {//no turn cost when we are standing still, just kidding
					
				if (nd != bd && nd != (bd + 1) % 8 && nd != (bd + 7) % 8) {
					body.speed += turnCost;
					amountAccelled -= turnCost;
				}

				//}
			}
			*/
		} else {
			
		}
		mDest = newDest;
	}

	int preD = 4;

	bool turn (Vector2Int newD) {
		int nd = GM.S.convertVectorToDir (newD);
		int bd = preD;
		preD = body.direction;
		if (nd != bd) {
			//Debug.Log ("old dir " + bd + " new dir " + nd);
			if (nd % 2 == 1) {
				if (diagMod == 0) {
					//hiSpeed++;
					diagMod = 1;
					//setSpeedBoost(-1);
					/*
					calcSpeedDiff ();
					if (body.speed == hiSpeed + slowDown - speedUp) {
						body.speed++;
						setAccel (amountAccelled - 1);
					}
					*/

				}
			} else {
				if (diagMod == 1) {
					//hiSpeed--;
					diagMod = 0;
					//setSpeedBoost(1);
					/*
					calcSpeedDiff ();
					if (body.speed == (hiSpeed + 1 + slowDown - speedUp)) {
						body.speed--;
						setAccel (amountAccelled + 1);
					}
					*/
				}

			}
			//if (mDest.x != -1) {//no turn cost when we are standing still, just kidding
			if (body.speed < loSpeed + slowDown - speedUp + turnCost) {
				if (nd != bd && nd != (bd + 1) % 8 && nd != (bd + 7) % 8) {
					int tc = turnCost;
					if (tc > body.hyperSpeed) {
						tc -= body.hyperSpeed;
						body.hyperSpeed = 0;
					}
					body.speed += tc;
					setAccel (amountAccelled - tc);
					return true;
				}
			}

			//}

		}
		return false;
	}

	public Vector2Int changeDir(Vector2Int d) {
		Vector2Int dir = d - body.centerPoint;
		if (!body.changeDir (d)) {
			ic.newDest ();
			body.changeDir (mDest);
			dir = mDest - body.centerPoint;
		}
		return dir;
	}

	public bool playerMove(Vector2Int dir, bool beingKnocked) {
		if (dir != Vector2Int.zero) {
			if (!beingKnocked && Grader.S) {
				List<Form> forCheck = body.checkForward(true);
				Grader.S.gotBumped(self, forCheck);
			}
			blocked = true;
			if (!body.checkForward (true, false)) {
				if (team != -1) {
					for (int i = 0; i < body.curCollided.Count; i++) {
						Form f = body.curCollided[i];
						//walk thru teammates shade
						if (f.id == 4) {
							//Debug.Log("hit shade");
							attackChunk ac = f.GetComponent<attackChunk>();
							if (ac && ac.getAttack().team == team) {
								//Debug.Log("team shade");
								blocked = false;
							}

						}
					}
				}
				if (blocked) {
					int ogDir = body.direction;
					int negDir = ogDir - 1;
					if (negDir < 0) {
						negDir = 7;
					}
					int posDir = (ogDir + 1) % 8;
					if (body.checkSide (body.centerPoint, negDir, true, true)) {
						//Debug.LogWarning ("dir switch " + negDir);
						body.changeDirection (negDir);
						blocked = false;
					} else if (body.checkSide (body.centerPoint, posDir, true, true)) {
						//Debug.LogWarning ("dir switch " + posDir);
						body.changeDirection (posDir);
						blocked = false;
					}
				}
			} else {
				blocked = false;
			}
			if (!blocked) {//body.checkForward (true, false)) {
				//Debug.Log ("body can move");
				if (!myAttack || myAttack.checkMove (dir, true)) {
					if (beingKnocked) {
						//Debug.Log (name + " can move freely " + dir + " " + myAttack);
					}
					blocked = false;
				} else {
					if (beingKnocked) {
						//Debug.Log (name + " lets adjust the sword too shalll we");
					}
					int result = myAttack.checkPlayerMove (dir, false);
					if (result < 361) {
						float pre = myAttack.curPos;
						//Debug.LogWarning ("lets just move the sword " + result + " times and we are good to move " + GM.S.dirs [body.direction]);
						myAttack.curPos = ic.saneAddition (myAttack.curPos, Mathf.Abs (result), Mathf.Sign (result));
						myAttack.forceMove();
						myAttack.clearPath(1);
						//ic.stopSwingInput();
						//Debug.LogError("adjusted sword " + pre + " -> " + myAttack.curPos);
						blocked = false;
					} else {
						blocked = true;
					}
				}
			} 
			//if (!dodging) {
				if (graphics && !beingKnocked && GM.S.drawSprites) {
					//if (body.direction % 2 == 0 && body.direction != graphics.direction) {
						graphics.setDir (body.direction);
					//}
				}
			//}
		} else {
			blocked = false;
			//Debug.Log ("dir = 0" + self.centerPoint + " " + mDest);
			return blocked;
		}
		if (!blocked) {
			//if (!dodging) {
			if (graphics && !beingKnocked && GM.S.drawSprites) {
				graphics.setWalking (true);
			}
			//}
			//body.forceMove (body.centerPoint + GM.S.dirs [body.direction], false);
			Vector2Int pre = body.centerPoint;
			body.forceMove (body.direction);
			if (myAttack) {
				myAttack.forceMove ();
			}
		}
	
		return blocked;
	}

	public void setStats(int n_hiSpeed, int n_loSpeed, int n_maxHyperSpeed, int n_turnCost, int n_accel, int n_decel, float n_stamina, float n_sRecovery, float n_dodgeCost, float n_dodgeLength, int n_dodgeSpeed,
		int n_dodgeRecover, int n_dodgeStartUp, int n_maxExtraSpeed) {//, int n_swordWeight){
		hiSpeed = n_hiSpeed;
		loSpeed = n_loSpeed;
		maxHyperSpeed = n_maxHyperSpeed;
		turnCost = n_turnCost;
		accelSpeed = n_accel;
		decelSpeed = n_decel;
		//attackRange = n_attackRange;
		//attackWidth = n_attackWidth;
		//attackLength = n_attackLength;
		//attackWidthSpacing = n_attackWidthSpacing;
		//attackTaper = n_attackTaper;
		//damage = n_damage;
		//staggerDamage = n_staggerDamage;
		//staggerTime = n_staggerTime;
		//knockBackDamage = n_knockDamage;
		stamina = n_stamina;
		maxStamina = stamina;
		sRecoverySpeed = 1 / n_sRecovery;
		//swordCost = n_swordCost;
		dodgeCost = n_dodgeCost;
		dodgeLength = n_dodgeLength;
		dodgeSpeed = n_dodgeSpeed;
		maxDodgeSpeed = dodgeSpeed + 20;
		chainWindow = 0;//window;
		//Debug.Log(dodgeSpeed);
		dodgeRecover = n_dodgeRecover;
		dodgeStartUp = n_dodgeStartUp;
		maxExtraSpeed = n_maxExtraSpeed;
		calcSpeedDiff ();
		//swordWeight = n_swordWeight;
	}

	public void setPoise(int poise){
		protection.setPoise (poise);
	}

	public int amountAccelled = 0;
	public float inPercent = 1;

	void acceleration(){
		if (moveInput && !blocked) {
			if (amountAccelled < speedDiff * inPercent) {
				if (accelCounter >= accelSpeed) {
					if (body.speed > (hiSpeed + slowDown - speedUp/* + diagMod*/)) {
						if (!going) {
							going = true;
						}
						body.speed--;
						//Debug.Log(body.speed + " " + inPercent);
						setAccel (amountAccelled + 1);
					} else {
						//Debug.Log (body.hyperSpeed + " " + maxHyperSpeed + " " + extraSpeed);
						if (body.hyperSpeed < maxHyperSpeed + extraSpeed) {
							body.hyperSpeed++;
							setAccel (amountAccelled + 1);
						} else {
							//going = true;
						}
					}
					accelCounter = 0;

				} else {//if (body.hyperSpeed == 1 || body.hyperSpeed < maxHyperSpeed + extraSpeed) { WHY??
					accelCounter++;
				}
			}
		} else {
			if (body.speed < loSpeed + slowDown - speedUp) {
				if (decelSpeed >= 0 && !blocked) {
					if (decelCounter >= decelSpeed) {// || !going) {
						decelCounter = 0;
						if (body.hyperSpeed > 0) {
							body.hyperSpeed--;
						} else if (body.speed < loSpeed + slowDown - speedUp) {
							body.speed++;
						} 
						setAccel (amountAccelled - 1);
					} else if (amountAccelled != 0) {
						//Debug.Log ("poo" + amountAccelled);
						decelCounter++;
					}
				} else {
					//Debug.Log ("got blocked maybe");
					body.speed = loSpeed + slowDown - speedUp;
					body.hyperSpeed = 0;
					setAccel (0);
				}
			} else { //if (going) {
				mDest = GM.S.invalidDest;
				if (!ic) {
					ic = gameObject.GetComponent<Controller>();
				}
				ic.iMoved (0);
				going = false;
				body.speed = loSpeed + slowDown - speedUp;
				body.hyperSpeed = 0;
				setAccel (0);
				if (speedCounter != body.speed) {
					speedCounter = body.speed;
				}
			}
			if (blocked && !moveInput) {
				blocked = false;
			}
		}
	}

	void calcSpeedDiff() {
		speedDiff = (loSpeed + slowDown - speedUp) - (hiSpeed /*+ diagMod*/ + slowDown - speedUp) + (maxHyperSpeed + extraSpeed);
		if (speedDiff == 0) {
			Debug.Log ("0 speedDif");
		}
	}

	public int getAccel() {
		return accelSpeed;
	}

	public int getAc() {
		return accelCounter;
	}

	public int getSc() {
		return speedCounter;
	}

	public int getMaxSpeed() {
		return hiSpeed;
	}

	public int getMaxHSpeed() {
		return maxHyperSpeed + extraSpeed;
	}

	// now works in negative as well
	public void setSpeedBoost(int speedBoost) {
		int dir = (int)Mathf.Sign(speedBoost);
		if (dir > 0) {
			for (int i = 0; i < Mathf.Abs(speedBoost); i++) {
				if (hiSpeed > 0) {
					hiSpeed--;
				} else {
					maxHyperSpeed++;
				}
			}
			calcSpeedDiff ();
		} else {
			for (int i = 0; i < Mathf.Abs(speedBoost); i++) {
				if (maxHyperSpeed > 0) {
					maxHyperSpeed--;
				} else if (hiSpeed < loSpeed) {
					hiSpeed++;
				} else {
					loSpeed++;
				}
			}
			if (body.hyperSpeed > maxHyperSpeed) {
				body.hyperSpeed = maxHyperSpeed;
			}
			if (body.speed < hiSpeed) {
				body.speed = hiSpeed;
			}
			calcSpeedDiff ();
			if (speedDiff < amountAccelled) {
				amountAccelled = speedDiff;
			}
		}
		
					/*
		if (hiSpeed > speedBoost) {
			speedUp = speedBoost;
		} else {
			int hyp = speedBoost - hiSpeed;
			if (hyp > maxExtraSpeed) {
				extraSpeed = maxExtraSpeed;
			} else {
				extraSpeed = hyp;
			}
		}
		body.speed = loSpeed + slowDown - speedUp;
		setAccel (0);

			if (extraSpeed + speedBoost > maxExtraSpeed) {
				extraSpeed = maxExtraSpeed;
				Debug.Log ("extra speed applied");
			} else if (extraSpeed + speedBoost <= 0) {
				extraSpeed = 0;
				body.hyperSpeed = 0;
			} else {
				extraSpeed += speedBoost;
			}

			calcSpeedDiff ();
								*/
	}

	// probably dont need this any more
	//actually seems to cause bug
	public void slowUp(int down) {
		slowDown = down;
		body.speed = loSpeed + slowDown - speedUp;
		setAccel (0);
		calcSpeedDiff ();
	}

	public void getKnocked() {
		knocked = true;
		if (ic) {
			ic.stopMove ();
        }
        if (graphics && GM.S.drawSprites) {
			graphics.knock(true);
		}
     }

    public GameObject[] makeSword(){
		GameObject[] gList = new GameObject[myAttack.bladeMulti * attackRange];
		for (int i = 0; i < myAttack.bladeMulti * attackRange; i++) {
			gList [i] = Instantiate (attack);
		}
		return gList;
	}

	public void meetSword(circleAttack ca){
		//ca.meetPlayer (this);//, swordCost, damage, staggerDamage, staggerTime, knockBackDamage, attackWidth, attackLength, attackWidthSpacing, attackTaper, swordWeight);
		//myAttack = ca;
	}

	public void receiveDodgeStats(int speed, int length, int start, int recover, float window) {
		dodgeSpeed = speed;
		maxDodgeSpeed = dodgeSpeed + 20;
		dodgeLength = length;
		dodgeStartUp = start;
		dodgeRecover = recover;
		chainWindow = window;
	}

	public int maxDodgePower = 25;
	int dodgei = 0;
	int[] dp;
	bool alreadyHidden;
	int dodgeRecover = 10;
	float chainWindow = 0.3f;
	int dr = -1;
	Particle dodgeFX;

	public void dodge(float power) {
		//Debug.LogError("dodge! " + power);
		if (!stopped && !stagged && !knocked && myAttack) {
			//Debug.LogError("dodge starting");
			if ((dr == -1 && !dodging) || dodgeChain()) {
				if (boomBox.S) {
					//boomBox.S.upTempo (0.25f);
				}
				dodging = true;
				moving = true;
				if (myAttack) {
					myAttack.getSoul().playerStartMove();
				}
				//protection.invulnerable = true;
				alreadyHidden = true;
				if (myAttack) {
					alreadyHidden = myAttack.isHidden ();
					myAttack.erasePresence(true);
					if (!alreadyHidden) {
						//myAttack.erasePresence (true);
					} else {
						Debug.LogWarning (name + " attack is already hidden");
					}
					myAttack.getSoul ().dodge ();
				}
				speedCounter = 0;
				dodgei = 0;
				dr = (int)(dodgeRecover * power);//getDodgePercent(power));
				if (mDest != GM.S.invalidDest) {
					if (Vector2Int.Distance (mDest, self.centerPoint) < 2) {
						//Debug.Log ("dodging: " + mDest + " " + self.centerPoint);
						ic.newDest ();
					}
					Vector2Int pos = getDodgePos (power);
					body.erasePresence (false);
					//body.etheral = true;
					float dd = Vector2Int.Distance (self.centerPoint, pos);
					if (dd > 0) {
						dp = dodgePath (pos);
					} else {
						dp = new int[0];
						self.forceMove (self.centerPoint, false);
					}
					if (dodgeFX) {
						dodgeFX.kindle();
					}
				} else {
					body.erasePresence (false);//activates the exit functions. Needed to turn off friction
					//body.etheral = true;
					self.forceMove (self.centerPoint, false);//necesarry for stand still dodges, so player doesn't disappear
					dp = new int[0];
					// ???? dr / = 2;
				}
				if (!chainDodging) {
					dodgeStart = dodgeStartUp;
					if (graphics) {
						graphics.setDodge (1);
					}
				}

			}
		} else {
			Debug.LogWarning ("couldn't dodge stopped? " + stopped + " stagged? " + stagged + ", knocked? " + knocked);
		}
	}

	bool chainDodging;
	int chainDodges = 0;
	int maxDodgeSpeed;

	bool dodgeChain() {
		Debug.Log("DODGE CHAIN!??! " + dr + " > " + ((int)(dodgeRecover * chainWindow)));
		if (!chainDodging && dr >= 0 && dr > (dodgeRecover * chainWindow)) {
			Debug.Log("dodge chain!");
			dr = -1;
			if (graphics && GM.S.drawSprites) {
				graphics.setDodge (2);
			}
			chainDodging = true;
			chainDodges++;
			return true;
		} else {
			return false;
		}
	}

	public Vector2Int getDodgePos(float power) {
		float x = mDest.x - body.centerPoint.x;//(int)(moveX * 1000) - body.centerPoint.x;
		float y = mDest.y - body.centerPoint.y;//(int)(moveY * 1000) - body.centerPoint.y;
		float angle = Mathf.Atan2 (y, x) * Mathf.Rad2Deg;
		//float percent = getDodgePercent(power);
		//Debug.Log("dodging power: " + power + " " + percent);
		int r = (int)(dodgeLength * power);
		Vector2Int pos = calcPos (angle, r);
		//Debug.LogWarning ("dashing " + pos);
		body.etheral = false;
		body.checkBody (pos, false, false);
		while (body.curCollided.Count != 0 && r > 0) {
			r--;
			pos = calcPos (angle, r);
			body.checkBody (pos, false, false);
			//Debug.Log ("D: " + pos + " " + r);
			for (int i = 0; i < body.curCollided.Count; i++) {
				//Debug.Log (body.curCollided [i].name);
			}
		}
		return pos;
	}

	int[] dodgePath(Vector2Int dir){
		int xDiff = dir.x - body.centerPoint.x;
		int xDir = -1;
		if (xDiff > 0) {
			xDir = 6;
		} else if(xDiff < 0){
			xDir = 2;
		}
		int yDiff = dir.y - body.centerPoint.y;
		int yDir = -1;
		if (yDiff > 0) {
			yDir = 0;
		} else if(yDiff < 0){
			yDir = 4;
		}
		int length = Mathf.Abs (xDiff) + Mathf.Abs(yDiff);
		//Debug.Log ("length " + length + "dir: " + xDir + ", " + yDir);
		int[] path = new int[length];

		int[] choices = new int[2];
		choices [0] = xDir;
		choices [1] = yDir;
		int[] diffs = new int[2];
		diffs [0] = xDiff;
		diffs [1] = yDiff;
		int[] counters = new int [2];
		int coinFlip = -1;

		for (int i = 0; i < length; i++) {
			if (coinFlip == -1 || Random.value > 0.75f) {
				coinFlip = Random.Range (0, 2);
			}
			int choice2 = (coinFlip + 1)%2;
			if (choices [coinFlip] != -1 && counters [coinFlip] < Mathf.Abs (diffs [coinFlip])) {
				path [i] = choices [coinFlip];
				counters [coinFlip]++;
			} else if (choices [choice2] != -1 && counters [choice2] < Mathf.Abs (diffs [choice2])) {
				path [i] = choices [choice2];
				counters [choice2]++;
			} else {
				Debug.Log ("poo");
			}
		}
		/*
		path = new int[length];
		for(int i = 0; i <= Mathf.Abs(xDiff); i++){
			path [i] = xDir;
		}
		for(int i = Mathf.Abs(xDiff); i < length; i++){
			path [i] = yDir;
		}
		*/
		return path;
	}

	public void endDodge() {
		moving = false;
		if (myAttack) {
			myAttack.getSoul().playerStopMove();
		}
		mDest = GM.S.invalidDest;
		body.etheral = false;
		if (dp.Length == 0) {
			//if (self.checkBody ()) {
			self.forceMove (self.centerPoint, false);
			//} else {
			//	Debug.LogError ("cannot return from dodge");
			//}
		} else {
			self.checkBody (self.centerPoint, false, false);
			if (self.curCollided.Count > 0) {
				Debug.Log ("dodged into some shit " + self.curCollided [0]);
			} else {
				self.forceMove (self.centerPoint, false);//solidifies self, ie sets current cells' height to 10
			}
		}

		dp = new int[0];
		chainDodging = false;
		protection.invulnerable = false;
		if (!stagged) {
			if (graphics && GM.S.drawSprites) {
				graphics.setDodge(0);
				/*
				graphics.setDodge (1);
				graphics.setMouth (0);
				*/
			}
		}
		dodging = false;
		if (dodgeFX) {
			dodgeFX.smother();
		}
	}

	public void recoverDodge() {
		dodging = false;
		if (myAttack) {
			if (!myAttack.sheathed/*&& !alreadyHidden*/) {
				myAttack.revealSelf ();
			}
			myAttack.getSoul().dodgeOver();
		}
		dr = -1;
		chainDodges = 0;
		if (graphics && GM.S.drawSprites) {
			//graphics.setDodge (0);
		}
	}

	public float getDodgePercent(int power) {
		float perc =  Mathf.Max(0.15f, (float)power / maxDodgePower);
		//Debug.Log(power + " / " + maxDodgePower + " %" + perc);
		return perc;
	}

	public Vector2Int calcPos(float angle, int radius){
		//Debug.Log (radius * Mathf.Cos (angle * Mathf.PI / 180));
		//Debug.Log(radius * Mathf.Sin (angle * Mathf.PI / 180));
		int xp = (int)(body.centerPoint.x + radius * Mathf.Cos (angle * Mathf.PI / 180));
		int yp = (int)(body.centerPoint.y + radius * Mathf.Sin (angle * Mathf.PI / 180));
		return new Vector2Int (xp, yp);
	}

	public IEnumerator die(bool reset, bool burn){
		if (!dead) {
			if (graphics && GM.S.drawSprites) {
				graphics.setAlpha(0);
				if (burn) {
					graphics.setDeath(2);
				} else {
					graphics.setDeath(1);
				}
			}

			removeBody(true);
			if (reset) {
				//called in case we are killed from outside defense, like when we restart, we want to restart the defense system as well
				protection.dying(false);
			}
			deaths++;
			
			yield return new WaitForSeconds (0.1f);// gets rid of dead body glitch
			body.erasePresence(false);

			//Debug.LogError ("player" + playerNum + " is dead");
			//if (ServerMaster.S) {
			if (!reset) {
				GM.S.StartCoroutine ("playerDeath", playerNum);
			}
			//StartCoroutine(GM.S.playerDeath (playerNum, deaths));
			//}

			//yield return new WaitForSeconds (1);
		}
	}

	public void removeBody(bool killed) {
		body.dead = true;
		dead = true;
		knocked = false;
		dodging = false;
		dr = -1;
		chainDodges = 0;
		chainDodging = false;
		mDest = GM.S.invalidDest;
		if (myAttack) {
			if (!killed) {
				myAttack.getSoul().saveState();
			}
			myAttack.die ();
		} else {
			Debug.LogError (name + " no attack to die with");
		}
		ic.die ();
		if (!killed) {
			body.erasePresence(false);
			protection.stop();
		}
		/*
		if (graphics) {
			graphics.setAlpha(0);
		}
		*/
	}

	public void respawn(Vector3Int sp, bool reset, bool fullHealth) {
		body.dead = false;
		moving = false;
        Vector2Int spawnPos = new Vector2Int(sp.x, sp.y);
		body.checkBody(spawnPos, false, false);
		List<Form> inTheWay = body.curCollided;
		if (inTheWay.Count > 0) {
			Debug.Log("player hit something tryign to respawn at " + sp);
			for (int i = 0; i < inTheWay.Count; i++) {
				Debug.Log (inTheWay [i].name);
				if (inTheWay [i] != body && inTheWay[i].id != -1) {
					defense d = inTheWay [i].gameObject.GetComponent<defense> ();
					if (!d) {
						d = inTheWay [i].parent.gameObject.GetComponent<defense> ();
					}
					if (d) {
						Vector2 dir = body.centerPoint - spawnPos;
						d.getKnocked (dir.normalized, 10, 1);
					} else {
						inTheWay [i].die (); // better olution needed
					}
				} else {
					Debug.LogError ("thats me");
				}
			}
			body.forceMove (spawnPos, false);
		} else {
			body.forceMove (spawnPos, false);
		}
		body.transform.position = new Vector3(body.centerPoint.x, body.centerPoint.y, 0);
		if (graphics && GM.S.drawSprites) {
			graphics.setAlpha(1);//Death(true);
			graphics.respawn();
            //graphics.setDirection (body.direction);
        }
		if (myAttack) {
			myAttack.respawn (sp.z);
		} else if (!fodder) {
			Debug.LogError (name + " no attack to respawn with");
		}
		setAccel (0);
		resetSpeed();
		protection.respawn(fullHealth);
		if (boomBox.S && reset) {
			if (GM.S.getNumPlayers() > 1) {
				boomBox.S.respawn (playerNum);
			} else {
				boomBox.S.respawn(0);
				boomBox.S.respawn(1);
			}
		}
		dead = false;
		body.dead = false;
	}

	public void deleteSelf() {
		myAttack.deleteSword();
		protection.deleteUI();
		self.die();
		Destroy(gameObject);
	}

	public void resetDeaths() {
		deaths = 0;
	}

	public int getDeaths(){
		return deaths;
	}

	public void takeHit() {
		if (myAttack) {
			myAttack.getSoul ().takeHit ();
		}
		if (dodging) {
			endDodge();
		}
	}

	public void getStaggered() {
		if (!stagged) {
			stagged = true;
			move = Vector2Int.zero;
			moving = false;
			if (myAttack) {
				if (!myAttack.beingKnocked) {
					myAttack.clearPath (69);
				}
				myAttack.getSoul().playerStopMove();
				myAttack.getSoul().getStaggered();
			}
		}
		if (dodging) {
			endDodge();
		}
		dr = -1;
	}

	public void stopStagger() {
		stagged = false;
		if (myAttack) {
			myAttack.getSoul().staggerOver();
			//myAttack.revealSelf();
		}
	}

	public void setDodgeFX(Particle fx) {
		dodgeFX = fx;
	}

	public Particle getDodgeFX() {
		return dodgeFX;
	}

	public defense getDef() {
		return protection;
	}

	public float getDodgeLength() {
		return dodgeLength;
	}

	public float getDodgeSpeed() {
		return dodgeSpeed;
	}

	public int getMaxDodgeSpeed() {
		return maxDodgeSpeed;
	}

	public int getDI() {
		return dodgei;
	}

	public int getDR() {
		return dr;
	}

	public int[] getDP() {
		return dp;
	}

	public int getAA() {
		return amountAccelled;
	}

	public int getSpeedDiff() {
		return speedDiff;
	}

	public bool isCreeping() {
		return amountAccelled < speedDiff * 0.8f;
	}

	public float getInP() {
		return inPercent;
	}

	public bool getMoving() {
		return moving;
	}

	public void setInP(float inP) {
		if (inP < 1) {
			inP = Mathf.Max(inP - 0.3f, 0.1f);
		}
		if (inP != inPercent) {
			//Debug.Log("move input " + inP + " speed: " + body.speed);
			inPercent = inP;
			if (amountAccelled > speedDiff * inPercent) {
				int extra = amountAccelled - (int)((float)speedDiff * inPercent);
				//Debug.LogError("we should slow down by " + extra + " hyp: " + body.hyperSpeed + " " + body.speed);
				setAccel(amountAccelled - extra);
				if (body.hyperSpeed > 0) {
					extra -= body.hyperSpeed;
					body.hyperSpeed = 0;
				}
				if (extra > 0) {
					body.speed += extra;
				}
				//Debug.LogError("now hyp: " + body.hyperSpeed + " spd:" + body.speed);
			}
			//Debug.Log(speedDiff * 0.5f + " " + amountAccelled);
		}
	}

	public void setAccel(int amnt) {
		if (amnt != amountAccelled) {
			//Debug.Log(amnt + " " + graphics + " " + GM.S.drawSprites + " " + myAttack);
			//Debug.Log("setting accel at " + amountAccelled + " to " + amnt);
			amountAccelled = amnt;
			if (graphics && GM.S.drawSprites) {// && myAttack) {
				float speed = (float)amountAccelled / speedDiff;
				if (myAttack) {
					speed *= myAttack.getFriction();
				}
				graphics.setWalkingSpeed (speed);
			}
		}
	}

	public void resetSpeed() {
		//Debug.Log("reset speed");
		PlayerStats ps = gameObject.GetComponent<PlayerStats>();
		loSpeed = ps.loSpeed;
		hiSpeed = ps.hiSpeed;
		body.speed = loSpeed;// + slowDown - speedUp;
		body.hyperSpeed = 0;
	}

	public int getTeam() {
		return team;
	}

	public bool onTeam(int teamCheck) {
		return (team != -1 && team == teamCheck);
	}
}
