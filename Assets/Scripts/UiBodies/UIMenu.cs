using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour {

	public Button[] butts;
	public int columns = 1;
	protected int rows;
	protected icon[,] buts;
	protected Text[,] txts;
	Text[] texts;
	public Color[] colors;
	public Font font;
	public string[,] moKeys;
	string[] aKey;
	string[] bKey;

	public int cur = 0;
	protected Vector2Int[] selects;
	Vector2[] moves;
	protected bool[] blocked;
	protected int[] backCount;

	public Controller controller;
	public int controlNum = -1;
	public int controlType;
	bool input;
	public bool mouseInp;
	// only set to true if not all rows are filled in, like for the char select
	public bool ySkip = false;

	public virtual void Awake(){
		setSelects();
		texts = new Text[butts.Length];
		rows = butts.Length / columns;
		txts = new Text[columns, rows];
		buts = new icon[columns, rows];
		int count = 0;
		for (int j = 0; j < columns; j++) {
			for (int i = 0; i < rows; i++) {
				Text t = butts[count].GetComponentInChildren<Text>();
				if (t) {
					texts[i] = t;
					txts[j, i] = t;
					buts[j, i] = butts[count].GetComponent<icon>();
					if (buts[j, i]) {
						//Debug.Log("got sb " + j + " " + i);
					}
					//butts[i].image.color = colors[0];
					if (i != cur) {
						t.color = colors [0];
					} else {
						t.color = colors [1];
					}
					if (font) {
						t.font = font;
					}
				}
				count++;
			}
		}
		input = false;
	}

	protected void setSelects() {
		int num = 6;
		moves = new Vector2[num];
		selects = new Vector2Int[num];
		blocked = new bool[num];
		backCount = new int[num];
		for (int i = 0; i < num; i++) {
			selects[i] = new Vector2Int(0,0);//-1;
			moves[i] = new Vector2(0,0);
			blocked[i] = false;
			backCount[i] = -1;
		}
	}

	int controlLen = 2;

	public virtual void Start() {
		checkControlScheme ();
		mousePos = Input.mousePosition;
	}

	public void checkControlScheme() {
		keyControl[] mine = GameInfo.S.getControls();
		if (mine.Length > 1) {
			controlLen = mine.Length;
			moKeys = new string[mine.Length, 4];
			aKey = new string[mine.Length];
			bKey = new string[1];
			for (int i = 0; i < mine.Length; i++) {
				for (int j = 0; j < 4; j++) {
					moKeys [i, j] = mine [i].getArrow (j);//"w";
				}
				aKey [i] = mine [i].getAccept(i);// "space";
				if (aKey [i] == "") {
					aKey[i] = "space";
				} 
				//bKey[i] = mine[i].getDecline(i);
				//aKey [1] = "return";
			}
			bKey[0] = mine[0].getDecline(0);
		} else {
			moKeys = new string[2, 2];
			moKeys [0, 0] = "w";
			moKeys [0, 1] = "s";
			moKeys [1, 0] = "i";
			moKeys [1, 1] = "k";
			aKey = new string[2];
			aKey [0] = "space";
			//aKey [1] = "return";
		}
	}

	Vector3 mousePos;
	float activeStick = 0.3f;
	//bool[] pressingBack = false;

	protected virtual void Update(){
		//float moveX = Input.GetAxis ("LeftJoystickX"+controlNum);
		bool getSome = false;
		for (int i = 0; i < selects.Length; i++) {
			if (!blocked[i]) {
				moves[i] = getMove(i);//Input.GetAxis ("LeftJoystickY"+controlNum);
				if (moves[i].x != 0 || moves[i].y != 0) {
					getSome = true;
				}
				if (accept(i)) {
					if (boomBox.S) {
						boomBox.S.yesPress (controlNum);
					}
					Debug.Log(name + " but tclick");
					buttClick(i);
				}
			}
			if (decline(i)) {
				if (backCount[i] == -1) {
					pressBack(i);
				}
				if (backCount[i] <= backHold) {
					if (backCount[i] == backHold-1) {
						holdBack(i);
					}
					backCount[i]++;
				} else {
					backCount[i] = 0;
				}
			} else {//if (backCount[i] != 0) { //not sure why I would check, -1 is the state ready for back press, and it seems we should always reset
				backCount[i] = -1;
			}
		}
		if (getSome != input) {
			input = getSome;
			count = moveInterval;
		}
		 if (mouseInp && !blocked[4]) {
			if (mousePos != Input.mousePosition) {
				float closest = Mathf.Infinity;
				Vector2Int chosen = new Vector2Int(-1,-1);
				//for (int j = 0; j < butts.Length; j++) {
				for (int j = 0; j < columns; j++) {
					for (int k = 0; k < rows; k++) {
						if (checkButt(j, k)) {
							float dist = checkDistance(j,k);
							if (dist < closest) {
								chosen = new Vector2Int(j, k);
								closest = dist;
							}
						}
					}
				}
				if (chosen.x > -1) {// && chosen.x < butts.Length) {
					selectButton(4, chosen);
				}
				mousePos = Input.mousePosition;
			}
		}
	}

	public virtual void selectButton(int player, Vector2Int chosen) {
		buttColor(selects[player], false, player);
		selects[player] = chosen;
		buttColor(selects[player], true, player);
	}

	public virtual float checkDistance(int x, int y) {
		if (buts[x,y]) {
			return Vector3.Distance (Input.mousePosition, buts [x, y].transform.position);
		} else {
			return Mathf.Infinity;
		}
	}

	public int moveInterval = 10;
	int count = 0;
	public int backHold = 20;
	//protected int backCount = 0;

	public virtual void LateUpdate() {
		if (inputting && input) {
			if (count >= moveInterval) {
				for (int i = 0; i < moves.Length; i++) {
					if (moves[i] != Vector2.zero) {
						Vector2Int mo = selects[i];
						bool m = false;
						if (Mathf.Abs(moves[i].y) > activeStick) {
							int c = 0;
							do {
								if (moves[i].y > 0) {
									mo.y = goUp (mo.y);
								} else if (moves[i].y < 0) {
									mo.y = goDown (mo.y);
								}
								c++;
								// if we move to a new row, with less buttons, go right until you find one
								for (int j = 0; j < columns && !checkButt(mo.x, mo.y); j++) {
									mo.x = goRight(mo.x);
								}
							} while (!checkButt(mo.x, mo.y) && c < rows);
							m = true;
						} else if (Mathf.Abs(moves[i].x) > activeStick) {
							int c = 0;
							do {
								if (moves[i].x > 0) {
									mo.x = goRight (mo.x);
								} else if (moves[i].x < 0) {
									mo.x = goLeft (mo.x);
								}
								c++;
								if (ySkip) {
									// if we move to a new column, with less buttons, go up until you find one
									for (int j = 0; j < rows && !checkButt(mo.x, mo.y); j++) {
										mo.y = goUp(mo.y);
									}
								}
							} while (!checkButt(mo.x, mo.y) && c < columns);
							m = true;
						}
						if (m) {
							selectButton(i, mo);
						}
					}
				}
				count = 0;
			} else {
				count++;
			}
		}
	}

	public virtual void buttColor(Vector2Int num, bool active, int player) {
		if (txts[num.x, num.y]) {
			if (active) {
				txts[num.x, num.y].color = colors[1];
			} else {
				txts[num.x, num.y].color = colors[0];
			}
		}
	}

	public virtual bool checkButt(int x, int y) {
		//Debug.Log(buts[x, y] + " " + x + " " + y);
		return buts[x, y] != null;
	}

	public virtual void buttClick(int player) {
		icon butt = buts[selects[player].x, selects[player].y];
		if (butt && !butt.isLocked()) {
			butt.GetComponent<Button>().onClick.Invoke ();
		}
	}

	public virtual void pressBack(int player) {}

	public virtual void holdBack(int player) { 
		if (MainMenu.S) {
			MainMenu.S.goBack();
		}
	}

	public void resetCursor() {
		for (int i = 0; i < selects.Length; i++) {
			buttColor(selects[i], false, i);
			selects[i] = new Vector2Int(0,0);
			buttColor(selects[i], true, i);
		}
	}

	public virtual void click() {

	}

	public virtual Vector2 getMove(int player) {
		Vector2 move = new Vector2(0,0);
		if (inputting) {
			if (controlNum >= 0) {
				if (controlType == 1) {
					// gotta be tested, plus add controltype to this equation
					float m = Input.GetAxis ("LeftJoystickY" + controlNum);
					if (Mathf.Abs(m) > activeStick) {
						move.y = m;
					}
					m = Input.GetAxis ("LeftJoystickX" + controlNum);
					if (Mathf.Abs(m) > activeStick) {
						move.x = m;
					}
				} else if (controlType == 2) {
					if (move == Vector2.zero) {
						if (controlNum < 2) {
							if (Input.GetKeyDown (moKeys [controlNum, 0])) {
								move.y = 1;
							} else if (Input.GetKeyDown (moKeys [controlNum, 2])) {
								move.y = -1;
							}
							if (Input.GetKeyDown (moKeys [controlNum, 3])) {
								move.x = 1;
							} else if (Input.GetKeyDown (moKeys [controlNum, 1])) {
								move.x = -1;
							}
						}
					}
				}
			} else {
				if (player < 4) {
					float m = Input.GetAxis ("LeftJoystickY" + player);
					if (Mathf.Abs(m) > activeStick) {
						move.y = m;
					}
					m = Input.GetAxis ("LeftJoystickX" + player);
					if (Mathf.Abs(m) > activeStick) {
						move.x = m;
					}
				} else {
					return new Vector2(GameInfo.S.getControl(player-4).getXmove(), GameInfo.S.getControl(player-4).getYmove());
				}
				/*
				for (int i = 0; i < 4; i++) {
					float move = Input.GetAxis ("LeftJoystickY" + i);
					if (Mathf.Abs(move) > activeStick) {
						return move;
					} else if (i < controlLen) {
						if (Input.GetKeyDown (moKeys [i, 0])) {
							return 1;
						} else if (Input.GetKeyDown (moKeys [i, 1])) {
							return -1;
						}
					}
				}
				*/
			}
		}
		return move;
	}

	bool inputting = true;

	public void setInput(bool val) {
		inputting = val;
	}

	public bool accept(int player) {
		if (inputting) {
			if (mouseInp && Input.GetMouseButtonDown (0)) {
				return true;
			}
			if (controlNum > -1) {
				if (Input.GetButtonDown ("aButton" + controlNum)) {
					return true;
				} else {
					if (controlNum < 2) {
						if (Input.GetKeyDown (aKey [controlNum])) {
							return true;
						}
					}
				}
			} else {
				
				if (player == -1) {
					for (int i = 0; i < 4; i++) {
						//Debug.Log("checking " + i);
						if (Input.GetButtonDown ("aButton" + i)) {
							return true;
						}
					}
					for (int i = 0; i < controlLen; i++) {
						 if (Input.GetKeyDown (aKey [i])) {
							return true;
						}
					}
				} else {
					
					if (player < 4) {
						if (Input.GetButtonDown("aButton" + player)) {
							return true;
						}
					} else {
						if (Input.GetKeyDown(aKey[player-4])) {
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	public bool decline(int player) {
		if (inputting) {
			if (mouseInp && Input.GetMouseButtonDown (1)) {
				return true;
			}
			if (controlNum > -1) {
				if (Input.GetButton ("bButton" + controlNum)) {
					return true;
				} else {
					if (controlNum < 2) {
						if (Input.GetKey (bKey [controlNum])) {
							return true;
						}
					}
				}
			} else {
				if (player == -1) {
					for (int i = 0; i < 4; i++) {
						if (Input.GetButton ("bButton" + i)) {
							return true;
						}
					}
					for (int i = 0; i < controlLen; i++) {
						 if (Input.GetKey (bKey [i])) {
							return true;
						}
					}
				} else {
					if (player < 4) {
						if (Input.GetButton("bButton" + player)) {
							return true;
						}
					} else if (player == 4) {
						if (Input.GetKey(bKey[0])) {
							return true;
						}
					}
				}
			}
		} 
		return false;
	}

	int goDown(int c){
		if (boomBox.S) {
			boomBox.S.nextPress (controlNum);
		}
		if (c < rows-1) {
			c++;
		} else {
			c = 0;
		}
		return c;
	}

	int goUp(int c){
		if (boomBox.S) {
			boomBox.S.nextPress (controlNum);
        }
        if (c > 0) {
            c--;
        } else {
            c = rows - 1;
        }
		return c;
    }

	int goRight(int c){
		if (boomBox.S) {
			boomBox.S.nextPress (controlNum);
		}
		if (c < columns-1) {
			c++;
		} else {
			c = 0;
		}
		return c;
	}

	int goLeft(int c){
		if (boomBox.S) {
			boomBox.S.nextPress (controlNum);
        }
        if (c > 0) {
            c--;
        } else {
            c = columns - 1;
        }
		return c;
    }

    public bool prePause;
	public void getController(Controller ic){
		controller = ic;
		controlNum = ic.controlNum;
		InputController ii = ic.gameObject.GetComponent<InputController> ();
		if (ii) {
			if (ii.gamePad == 2) {
				controlNum = -1; // so that both keyboard players can control it
			}
		}
		prePause = Map.S.flowing;
		//controller.enabled = false;
		GM.S.pausePlayers(true, true);
		if (boomBox.S) {
			boomBox.S.noPress (controlNum);
		}
	}

	void onEnable() {
		checkControlScheme ();
	}
}
