using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : Controller {
	public int gamePad;
	public bool dodgeLeftTrigger = true;
	public float moveX;
	public float moveY;

	public float strikeX;
	public float strikeY;

	public bool swinging;

	int attackWindow;
	Vector2Int nullMove;
	public GameObject indicator;
	Transform sIn;
	SpriteRenderer[] sInsr;
	Color[] activeCol;
	Color inactiveCol;

	public override void Awake() {
		base.Awake ();
		nullMove = new Vector2Int (-1, -1);
		/*
		int ps = 2; //player sizes
		arrowUp = new string[ps];
		arrowUp[0] = "w";
		arrowUp[1] = "i";
		arrowLeft = new string[ps];
		arrowLeft[0] = "a";
		arrowLeft[1] = "j";
		arrowDown = new string[ps];
		arrowDown[0] = "s";
		arrowDown [1] = "k";
		arrowRight = new string[ps];
		arrowRight [0] = "d";
		arrowRight [1] = "l";
		attackPosButton = new string[ps];
		attackPosButton [0] = "q";
		attackPosButton [1] = "u";
		attackNegButton = new string[ps];
		attackNegButton [0] = "e";
		attackNegButton [1] = "o";
		dodgeButton = new string[ps];
		dodgeButton [0] = "space";
		dodgeButton [1] = "return";
		*/
	}

	void Start() {
		if (gamePad != 1) {
			me.setInP (1);// inPercent = 1;
			//sInsr.color = inactiveCol;
			keyControl mine = GameInfo.S.getControl (controlNum);
			arrowUp = mine.getArrow (0);
			arrowLeft = mine.getArrow (1);
			arrowDown = mine.getArrow (2);
			arrowRight = mine.getArrow (3);
			attackPosButton = mine.getSword (0);
			if (attackPosButton == "") {
				swordPosButt = mine.getSwordM (0);
			}
			attackNegButton = mine.getSword (1);
			if (attackNegButton == "") {
				swordNegButt = mine.getSwordM (1);
			}
			dodgeButton = mine.getDodge ();
			if (dodgeButton == "") {
				dodgeButt = mine.getDodgeM ();
			}
		}

		//me.moveInput = true; moveX = 1; newDest (); 
	}

	void Update () {
		if (gamePad == 1) {
			gamepadControls (Map.S.flowing);
			//swordIndicator ();
		} else {
			//keyControls ();
			cabinetControls (Map.S.flowing);
		}
	}

	public float constraint;

	int finD = 0;
	// 0 - scroll, 1 - mouse buttons, 2 - keys
	int keyAttack = 0;//true;
	int scrollTimer = 0;
	public int scrollInterval = 1;

	void cabinetControls(bool flow) {
		if (!me.dead && flow) {
			me.move.x = (int)moveX;
			me.move.y = (int)moveY;
			if (Input.GetKey (arrowUp)) {
				//me.body.changeDirection (0);
				me.move.y = 1;
				moveY = 1;
			} else if (Input.GetKey (arrowDown)) {
				//me.body.changeDirection (4);
				moveY = -1;
			} else if (moveX != 0 || !me.going) {
				moveY = 0;
			}

			if (Input.GetKey (arrowLeft)) {
				//me.body.changeDirection (2);
				moveX = -1;
			} else if (Input.GetKey (arrowRight)) {
				//me.body.changeDirection (6);
				moveX = 1;
			} else if (moveY != 0 || !me.going) {
				moveX = 0;
			}

			if (!Input.GetKey (arrowUp) && !Input.GetKey (arrowLeft) && !Input.GetKey (arrowDown) && !Input.GetKey (arrowRight)) {
				me.moveInput = false;
			} else {
				me.moveInput = true;
				if (moveY != prevMY || moveX != prevMX || me.mDest.x == -1) {
					newDest ();
				}
			}
			prevMX = moveX;
			prevMY = moveY;
			/*
			if ((dodgeButt < 0 && Input.GetKeyDown (dodgeButton)) || (dodgeButt > -1 && Input.GetMouseButtonDown(dodgeButt))) {
				me.dodge ();
			}
			*/
			if (!dodgeHold && holdingDodge() != 0) {
				StartCoroutine(dodging());
			}
			if (attack) {
				int ksv = keySwordValue();
				if (ksv != 0) {
					strikeStarted = true;
					finD = ksv;//(int)Input.mouseScrollDelta.y;
					scrollTimer = 0;
				} else {
					if (scrollTimer < scrollInterval) {
						scrollTimer++;
					} else if (strikeStarted) {
						//Debug.LogWarning("scroll over");
						attack.weightSwing(finD);
						finD = 0;
						strikeStarted = false;
					}
				}
				if (strikeStarted) {
					//int dir = keySwordValue();//(int)(Mathf.Sign(Input.mouseScrollDelta.y));// * constraint);

					float pos = attack.curPos;
					if (attack.anglePath.Count > 0) {
						pos = attack.anglePath [attack.anglePath.Count - 1];
					}
					attack.swingPath (saneAddition(pos, constraint, finD), finD, false);
				}
				if (attack.getSoul().curGear == 1) {
					if (Input.GetMouseButton(1)) {
						//ignition
						attack.getSoul().gearShift(2);//speedChange(2, 10);
					}
					if (Input.GetMouseButton(0)) {
						//brace
						attack.getSoul().gearShift(0); //speedChange(1, 5);
					}
				} else {//if ( &&) { // if (Input.GetMouseButtonUp(1)) {
					if (attack.getSoul().curGear == 2 &&  !Input.GetMouseButton(1)) {
						attack.getSoul().gearShift(1);
					}
					if (attack.getSoul().curGear == 0 && !Input.GetMouseButton(0)) {
						attack.getSoul().gearShift(1);
					}
				}
			}
		}
		if (Input.GetKeyDown ("escape")) {
			pause ();
		}
	}


	int keySwordValue() {
		switch (keyAttack) {
			case 0:
				//Debug.Log(Mathf.Sign(Input.mouseScrollDelta.y) + " " + Input.mouseScrollDelta.y);
				if (Input.mouseScrollDelta.y == 0) {
					//Debug.Log("zero");
					return 0;
				} else {
					return (int)(Mathf.Sign(Input.mouseScrollDelta.y));
				}
			case 1:
				if (Input.GetMouseButtonDown(0)) {
					return 1;
				} else if (Input.GetMouseButtonDown(1)) {
					return -1;
				} else {
					return 0;
				}
			default:
				if (Input.GetKey(attackPosButton)) {
					return 1;
				} else if (Input.GetKey(attackNegButton)) {
					return -1;
				} else {
					return 0;
				}
		}
	}

	float holdingDodge() {
		if (gamePad == 1) {
			float dodgeInput = Input.GetAxis ("Controller_LT"+controlNum);
			//Debug.Log(dodgeInput);
			if (dodgeInput != 0) {
				if ((dodgeLeftTrigger && dodgeInput > 0) || (!dodgeLeftTrigger && dodgeInput < 0)) {
					return dodgeInput;
				}
			}
		} else if ((dodgeButt < 0 && Input.GetKey (dodgeButton)) || (dodgeButt > -1 && Input.GetMouseButton(dodgeButt))) {
			return 1;
		}
		return 0;
	}

	bool dodgeHold = false;

	IEnumerator dodging() {
		dodgeHold = true;
		int dPower = 0;
		float power = 0;
		while (holdingDodge() != 0 && (dPower < me.maxDodgePower || attack.swinging)) {
			if (attack && !attack.swinging) {
				dPower++;
				power = Mathf.Max(holdingDodge(), power);
				//Debug.Log(dPower + " " + power);
			}
			yield return new WaitForSecondsRealtime(0.001f);
		}
		//Debug.Log("dodge! " + me.getDodgePercent(dPower));
		if (gamePad == 1) {
			me.dodge(me.getDodgePercent(dPower));
			//me.dodge(power);
		} else {
			me.dodge(me.getDodgePercent(dPower));
		}
		while (holdingDodge() != 0) {
			yield return new WaitForSecondsRealtime(0.00001f);
		}
		dodgeHold = false;
	}

	bool dashCounting;

	IEnumerator dashCounter(float x, float y) {
		int pause = 12;
		if(!dashCounting) {
			//Debug.Log ("dash counting begin");
			dashCounting = true;
			int pauseCounter = 0;
			while (pauseCounter < pause) {
				yield return new WaitForFixedUpdate ();
				if (Mathf.Abs (moveX) != 1 && Mathf.Abs (moveY) != 1) {
					break;
				}
				pauseCounter++;
			}
			//Debug.Log ("release " + pauseCounter);
			if (pauseCounter < pause) {
				pauseCounter = 0;
				while (pauseCounter < pause) {
					if ((moveX != 0 && moveX == x) || (moveY != 0 && moveY == y)) {
						me.dodge (me.maxDodgePower);
						break;
					}
					yield return new WaitForFixedUpdate ();
					pauseCounter++;
				}
			}
			dashCounting = false;
		}
	}

	public float deadZone = 0.2f;
	bool gettingLT = false;

	void gamepadControls(bool flowing) {
		if (!me.dead && flowing) {
			float mX = Input.GetAxis ("LeftJoystickX"+controlNum);
			float mY = Input.GetAxis ("LeftJoystickY"+controlNum);
			/*
			if (Mathf.Abs(moveX) == 1 || Mathf.Abs(moveY) == 1) {
				StartCoroutine (dashCounter (moveX, moveY));
			}
			*/
			if (!me.dodging) {
				if (mX != prevMX || mY != prevMY || me.mDest == nullMove) {
					if (mX == 0 && mY == 0) {
						me.moveInput = false;
						if (!me.going) {
							moveX = mX;
							moveY = mY;
						}
						//	me.mDest = new Vector2Int (-1, -1);
					} else {
						moveX = mX;
						moveY = mY;
						newDest ();
						me.moveInput = true;
					}

					if (me.moveInput) {
						me.setInP(Mathf.Max(Mathf.Abs(moveX), Mathf.Abs(moveY)));
						//me.setInP(Mathf.Clamp (0.1f + Mathf.Max(Mathf.Abs(moveX), Mathf.Abs (moveY)), 0, 1));
						// allows for creeping
						//me.curHiSpeed = me.loSpeed - (int)(me.speedDiff * highInput);
					} else {
						me.setInP(1);
					}
				}
			} else {
				moveX = mX;
				moveY = mY;
				if (moveX == 0 && moveY == 0) {
					me.moveInput = false;
				}
				newDest ();
			}
			prevMX = mX;
			prevMY = mY;
			//Debug.Log ("RT " + Input.GetAxis ("Controller_RT"));
			//Debug.Log ("LT " + Input.GetAxis ("Controller_LT"));
			
			if (Input.GetAxis ("Controller_LT"+controlNum) != 0 && !gettingLT) {
				gettingLT = true;
				//me.dodge (me.maxDodgePower);
				if (!dodgeHold && holdingDodge() != 0) {
					StartCoroutine(dodging());
				}
			}
			if (Input.GetAxis ("Controller_LT"+controlNum) == 0) {
				gettingLT = false;
			}
			

			/*
			if (Input.GetButtonDown("rightShoulder" + controlNum)) {
				attack.getSoul().ignite();
			}
			if (Input.GetButtonDown("leftShoulder" + controlNum)) {
				attack.getSoul().careful(true);
			} else if (Input.GetButtonUp("leftShoulder" + controlNum)) {
				attack.getSoul().careful(false);
			}
			*/
			if (attack && attack.getSoul()) {
				if (attack.getSoul().curGear == 1) {
					if (Input.GetButton("rightShoulder" + controlNum)) {
						//Debug.Log("ignition");
						attack.getSoul().gearShift(2);
					}
					if (Input.GetButton("leftShoulder" + controlNum)) {
						//Debug.Log("brace");
						attack.getSoul().gearShift(0); 
					}
				} else {
					if (attack.getSoul().curGear == 2 && !Input.GetButton("rightShoulder" + controlNum)) {
						//attack.getSoul().careful(false);
						attack.getSoul().gearShift(1);
					}
					if (attack.getSoul().curGear == 0 && !Input.GetButton("leftShoulder" + controlNum)) {
						attack.getSoul().gearShift(1);
					}
				}
			}
			/*
			prevX = strikeX;
			prevY = strikeY;
				
			strikeX = Input.GetAxis ("RightJoystickX" + controlNum);
			if (Mathf.Abs(strikeX) < deadZone) {
				strikeX = 0;
			}
			strikeY = Input.GetAxis ("RightJoystickY" + controlNum);
			if (Mathf.Abs(strikeY) < deadZone) {
				strikeY = 0;
			}
			*/
			//Debug.Log ("strike Y = " + Input.GetAxis ("RightJoystickY" + controlNum));
			//strikeX = Input.GetAxis ("tigreJoystickX"+controlNum);
			//strikeY = Input.GetAxis ("tigreJoystickY"+controlNum);
			float swingX = 0;
			float swingY = 0;
			if (!swinging) {
				stickToStrike();
				swingX = Mathf.Abs(strikeX - prevX);
				swingY = Mathf.Abs(strikeY - prevY);

			}
			if (me.hasSword && !me.dodging) {
				//if (Mathf.Abs (Input.GetAxis ("RightJoystickX" + controlNum)) + Mathf.Abs (Input.GetAxis ("RightJoystickY" + controlNum)) > 0) { //strikeX != 0 || strikeY != 0) {
				if (swingX > moveDiff || swingY > moveDiff) {
					if (!swinging && (!attack.singleStrike || !attack.readyToSwing)) {// && !attack.swinging) {
						// STRIKE!
						StartCoroutine ("swordStrike");
						Vector2Int sc = me.body.centerPoint;
						float x = (int)(strikeX * 1000) - sc.x;
						float y = (int)(strikeY * 1000) - sc.y;
						//Debug.Log ("BEGIN: strike: " + strikeX + ", " + strikeY + " -> floats: " + x + ", " + y);
						int angle = (int)(Mathf.Atan2 (y, x) * Mathf.Rad2Deg);	
						attack.startSwingInput (angle);
					}
				}
			}
			if (Mathf.Abs(strikeX) < deadZone && Mathf.Abs(strikeY) < deadZone) {
				if (strikeStarted) {
					//sword.strikeOver ();
					strikeStarted = false;
					attack.stopSwinging ();
				}
			}
			if (!swinging) {
				prevX = strikeX;
				prevY = strikeY;
			}

			//if (Input.GetButtonDown ("aButton" + controlNum)) {
			//StartCoroutine (me.dodge ());
		//}
		}
		if (Input.GetButtonDown ("startButton" + controlNum)) {
			//Debug.Log ("pause");
			pause();
		}

	}

	void stickToStrike() {
		strikeX = Input.GetAxis ("RightJoystickX" + controlNum);
		if (Mathf.Abs(strikeX) < deadZone) {
			strikeX = 0;
		}
		strikeY = Input.GetAxis ("RightJoystickY" + controlNum);
		if (Mathf.Abs(strikeY) < deadZone) {
			strikeY = 0;
		}
	}

	public override void newDest() {
		//Debug.Log (moveX + " " + moveY);
		if (moveX != 0 || moveY != 0) {
			me.getDest( calculateDestination ());
		} else {
			//Debug.Log ("choochy");
			me.getDest (new Vector2Int (-1, -1));
		}
	}

	public override Vector2Int calculateDestination() {
		if (Mathf.Abs (moveY) < 0.1f && moveY != 0) {
			//Debug.Log ("moveY too low");
			moveY = 0.1f * Mathf.Sign (moveY);
		}
		if (Mathf.Abs (moveX) < 0.1f && moveX != 0) {
			//Debug.Log ("moveX too low");
			moveX = 0.1f * Mathf.Sign (moveX);
		}
		Vector2Int cen = me.getCenter ();
		int x = (int)Mathf.Max (/*me.body.centerPoint.x*/cen.x + (moveX * 30), 0);
		int y = (int)Mathf.Max (/*me.body.centerPoint.y*/cen.y + (moveY * 30), 0);
		return new Vector2Int (x, y);
	}

	float prevMX;
	float prevMY;
	float prevX;
	float prevY;

	string arrowUp;
	public string arrowLeft;
	public string arrowDown;
	public string arrowRight;
	public string attackPosButton;
	public string attackNegButton;
	public string dodgeButton;
	int swordPosButt = -1;
	int swordNegButt = -1;
	int dodgeButt = -1;
	
	List<int> anglePath;
	float moveDiff = 0.005f;
	int aCounter;
	public float strikePoint = 0.5f;
	public bool strikeStarted;
	public bool singleStrike;

	public IEnumerator swordStrike() {
		//Debug.Log("sword strike " + (int)attack.curPos);
		swinging = true;
		Vector2Int sc = me.body.centerPoint;
		int prev = -999;
		int dir = 0;
		int finD = 0;
		int still = 0;
		while (Mathf.Abs (strikeX) > 0 || Mathf.Abs (strikeY) > 0) {
			stickToStrike();
			strikeStarted = true;
			if ((strikeX != prevX || strikeY != prevY) && (Mathf.Abs (strikeX) > 0|| Mathf.Abs (strikeY) > 0)) {
				prevX = strikeX;
				prevY = strikeY;
				float x = (int)(strikeX * 1000) - sc.x;
				float y = (int)(strikeY * 1000) - sc.y;
				//Debug.Log ("   INPUT strike: " + strikeX + ", " + strikeY + " -> floats: " + x + ", " + y);
				int angle = (int)(Mathf.Atan2 (y, x) * Mathf.Rad2Deg);	
				int pos = (int)attack.curPos;
				swordIndicator(angle);
				dir = 0;
				if (attack.anglePath.Count > 0) {
					pos = attack.anglePath [attack.anglePath.Count - 1];
				} else {
					pos = (int)attack.curPos;
				}
				//Debug.Log("   INPUT pos: " + pos + " -> angle: " + angle);
				if (pos != angle) {
					if (prev != -999) {
						dir = findShortestDir(pos, angle);
					} else {
						if (attack.anglePath.Count > 0) {
							//pos = attack.anglePath [attack.anglePath.Count - 1];
						}
						dir = findShortestDir (pos, angle);//closestDir (pos, angle);
					}
					if (dir != 0) {
						finD = dir;
					}
					if (prev != -999) {
						//Debug.Log("strike happening " + angle + ", dir: " + dir);
						//Debug.Log(" calc w/pos("+pos+"): " + findShortestDir (pos, angle) + " calc w/prev("+prev+"): " + findShortestDir(prev, angle));
					}
					if (!attack.swingPath (angle, dir, false)) {
						//break;
					}
					//Debug.Log ("swing: " + angle + " " + dir);
				} else if (still < 5) {
					still++;
				} else {
					//break;
				}
				prev = angle;
			}
			yield return new WaitForEndOfFrame();
			//yield return new WaitForFixedUpdate ();
		}
		//Debug.Log("end strike");
		endSwordIndicator();
		attack.weightSwing (finD);
		swinging = false;
		yield return new WaitForEndOfFrame();
	}

	public override void stopSwingInput() {
		Debug.Log("no swing input");
		swinging = false;
		StopCoroutine("swordStrike");
	}

	int closestDir(int pos, int angle) {
		int dir = 0;
		if ((pos < 0 && angle < 0) || (pos > 0 && angle > 0)) {
			if (pos > angle) {
				dir = -1;
			} else {
				dir = 1;
			}
		} else {
			float posDist, negDist;
			if (pos > 0) {
				posDist = (180 - pos) + (180 - Mathf.Abs (angle));
				negDist = pos + Mathf.Abs (angle);
			} else {
				posDist = Mathf.Abs (pos) + angle;
				negDist = (180 - Mathf.Abs (pos)) + (180 - angle);
			}
			if (posDist < negDist) {
				dir = 1;
			} else {
				dir = -1;
			}
		}
		return dir;
	}

	public IEnumerator swordStrike1(){
		Debug.Log ("Begin sword input");
		if (!swinging) {
			me.setPoise (3); // attacking poise
			swinging = true;

			List<float> strikes = new List<float> ();
			List<int> directions = new List<int> ();

			Vector2Int sc = me.body.centerPoint;
			//Debug.Log ("starting " + prevX);
			int pauseCount = 0;
			//StartCoroutine (sword.lerpColor (swordChargeColor));
			//float prevX = 2;
			//float prevY = 2;
			int strikeProgress = 0;
			strikeStarted = false;

			while (/*aCounter < attackWindow && */Mathf.Abs (strikeX) + Mathf.Abs (strikeY) >= deadZone /*&& 
				(controller || Mathf.Abs(strikeX - prevX) > moveDiff || Mathf.Abs(strikeY - prevY) > moveDiff)*/) {
				if (aCounter != 0) {
					prevX = strikeX;
					prevY = strikeY;
				}
				
				strikeX = Input.GetAxis ("RightJoystickX"+controlNum);
				strikeY = Input.GetAxis ("RightJoystickY"+controlNum);
				//Debug.Log ("x: " + prevX + " " + strikeX + " y: " + prevY + " " + strikeY);
				float x = (int)(strikeX * 1000) - sc.x;
				float y = (int)(strikeY * 1000) - sc.y;
				float angle = Mathf.Atan2 (y, x) * Mathf.Rad2Deg;
				
				if (Mathf.Abs (strikeX) + Mathf.Abs (strikeY) >= deadZone && pauseCount < attackWindow) {
					swordIndicator(angle);
					int dir = 0;
					float prev = -999;
					//find direction of strike compared to previous
					if (strikes.Count > 0) {
						prev = strikes [strikes.Count - 1];
						if (prev != angle) {
							//Debug.Log (prev + " " + angle);
							if ((prev > 0 && angle > 0) || (prev < 0 && angle < 0)) {
								if (prev > angle) {
									dir = -1;
								} else if (prev < angle) {
									dir = 1;
								}
							} else if (prev != angle) {
								if (prev > 0) {
									if (prev > 90) {
										dir = 1;
									} else {
										dir = -1;
									}
								} else {
									if (prev > -90) {
										dir = 1;
									} else {
										dir = -1;
									}
								}
							}
							directions.Add (dir);
						} 
					}
					//Debug.Log ("prev: " + prev + " angle: " + angle);
					if (prev != angle) {
						strikes.Add (angle);
						attack.swingPath ((int)Mathf.Abs(attack.curPos - angle), dir, false);
						if (strikes.Count > 1) {
							if (strikeProgress + 1 < strikes.Count) {
								//attack.swingPath (Mathf.Abs((int)strikes [strikeProgress] - (int)strikes [strikeProgress + 1]), (int)directions [strikeProgress]);
								strikeProgress++;
							}
							if (!strikeStarted) {
								//sword.getStrike ();
								strikeStarted = true;
							}
						}
					} else {
					//Debug.Log ("not adding strike " + prev + " " + angle);
					}

				} else {

					//Debug.Log("early end " + strikeX + " " + prevX);
					break;
				}
				//Debug.Log (Mathf.Abs (strikeX - prevX) + " " + Mathf.Abs(strikeY = prevY));
				if (/*!gamePad && */Mathf.Abs (strikeX - prevX) < moveDiff && Mathf.Abs (strikeY - prevY) < moveDiff) {
					pauseCount++;
					//Debug.Log (" pause " + pauseCount);
				} else {
					pauseCount = 0;
				}
				aCounter++;
				yield return new WaitForFixedUpdate (); // probably need to change; need to test on faster computer
				//yield return new WaitForEndOfFrame ();
				//Debug.Log ("new read");
			}
			if(!strikeStarted) {
				// for single stab
				if (directions.Count == 0) {
					if (strikes.Count > 0) {
						//attack.swingPath (Mathf.Abs((int)strikes [0] - (int)strikes [0] + 1), 1);
					} else {
						Debug.Log ("no input made");
					}
				}
				//sword.getStrike ();
				strikeStarted = true;
			}
			swinging = false;
			//sword.strikeOver ();
		} 
	}

	void swordIndicator(float angle) {
		if (!me.dead) {// && (Mathf.Abs (strikeX) > deadZone || Mathf.Abs (strikeY) > deadZone)) {
			if (sInsr[0].color != activeCol[0]) {
				sInsr[0].color = activeCol[0];
				sInsr[1].color = activeCol[1];
			}
			/*
			float x = (int)(strikeX * 1000) - me.body.centerPoint.x;
			float y = (int)(strikeY * 1000) - me.body.centerPoint.y;

			float angle = Mathf.Atan2 (y, x) * Mathf.Rad2Deg;
			*/
			float xp = me.body.centerPoint.x + 5 * Mathf.Cos (angle * Mathf.PI / 180);
			float yp = me.body.centerPoint.y + 5 * Mathf.Sin (angle * Mathf.PI / 180);

			sIn.position = new Vector3 (xp, yp, 0);
		}/* else {
			sInsr.color = inactiveCol;
			//sIn.position = new Vector3 (10000, 10900, 0);
		}*/
	}

	public void setIndicatorColors(Color a, Color b) {
		if (sIn) {
			Destroy(sIn.gameObject);
		}
		sIn = Instantiate (indicator, transform.position, transform.rotation, transform).transform;
		sInsr = new SpriteRenderer[2];
		activeCol = new Color[2];
		sInsr[0] = sIn.GetComponent<SpriteRenderer> ();
		sInsr[1] = sIn.GetChild(0).GetComponent<SpriteRenderer>();

		//sInsr.color = activeCol = me.body.color;
		inactiveCol = activeCol[0];
		inactiveCol.a = 0;
		sInsr[0].color = inactiveCol;
		sInsr[1].color = inactiveCol;
		activeCol[0]= a;
		activeCol[1] = b;
	}

	void endSwordIndicator() {
		sInsr[0].color = inactiveCol;
		sInsr[1].color = inactiveCol;
	}

	public void delteIndicator() {

	}

	public void setStats (int n_attackWindow){
		attackWindow = n_attackWindow;
	}

	public override void die() {
		//Debug.Log ("preM: " + prevMX + ", " + prevMY);
		//Debug.Log ("preS: " + prevX + ", " + prevY);
		//Debug.Log ("mDest: " + me.mDest + " inp: " + me.moveInput);
		//Debug.Log("x pre: " + prevX + " cur: " + strikeX);
	//Debug.Log("y pre: " + prevY + " cur: " + strikeY);
		prevX = 0;//
		prevY = 0;
		strikeX = 0;
		strikeY = 0;
		prevMX = 0;
		prevMY = 0;
		scrollTimer = scrollInterval + 1;
		strikeStarted = false;
	}

	public override void iMoved(int state) {
		if (state == 1) {
			//Debug.Log (name + " moved to " + self.centerPoint);
		}
	}

	public void setConstraint(int cons) {
		constraint = cons;
	}

	public override bool getStrikeInput() {
		return strikeX != 0 || strikeY != 0;
	}
}
