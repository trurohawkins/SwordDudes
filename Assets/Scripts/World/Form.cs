using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;

public class Form : MonoBehaviour {

	public int id;//-1 terrain, 0 floor, 1 player, 2 sword
	//[SyncVar]
	public bool debugDraw = false;
	public Color color;
	public bool spawned;
	public Form parent;
	public bool impenetratable;
	public bool unmovable;
	public bool dontLookAtMe;
	public bool unImportant;
	public bool active;
	public bool alive;

	//unfinsihed
	public bool gaseous; // does not affect movement of others, but can be intereacted with
	
	public bool etheral; // cannot interact with other forms
	// moves without entering anywhere, maybe should change, so its there but invisible? and use with combination of etheral to create true disappearance
	public bool hidden; 
	//when collided with the FOrm is removed, used for Shade Block, as it is one Form in many positions and we want to remvoe its pieces
	public bool removeOnCol = false;//depreceated
	public bool manyBodied = false; //Form does not keep track of its body, and each cell it holds should be treated as a different entity
	public bool invisible = false;
	public bool catchAll = false; //disregards height when checking for collisions, if you put its height as high it will collide with eveyrthhi
	public Vector2Int originPoint;
	public Vector2Int centerPoint;
	public Vector2Int[] body;
	public int direction;
	public int height;
	public int width;
	public int length;
	public Sprite[] sprites;
	public int speed;
	public int hyperSpeed;
	[HideInInspector]
	public int speedCounter = 0;
	public List<Purpose> activeAction;
	public List<Particle> effects;
	//public Inventory holding;
	//public Hands hands;

	public GameObject skin;
	Animator myGraphics;

	Vector2Int[][] checks;
	Vector2Int[] upCheck;
	Vector2Int[] nwCheck;
	Vector2Int[] leftCheck;
	Vector2Int[] wsCheck;
	Vector2Int[] downCheck;
	Vector2Int[] seCheck;
	Vector2Int[] rightCheck;
	Vector2Int[] enCheck;

	//Switches
	public Purpose[] colActions;
	public Purpose[] entActions;
	public Purpose[] exiActions;

	public bool dead;

	void Awake() {
		myGraphics = gameObject.GetComponentInChildren<Animator> ();
		if (gameObject.GetComponentInChildren<SpriteRenderer> ()) { 
			skin = gameObject.GetComponentInChildren<SpriteRenderer> ().gameObject;
		} else {
			if (gameObject.GetComponent<Tiler>()) {
				skin = gameObject;
			} else {
				skin = null;
			}
		}
		if (Map.S) {
			xEdge = Map.S.worldSizeX;
			yEdge = Map.S.worldSizeY;
			curMap = Map.S.world;
		}
		curCollided = new List<Form> ();
		curEntered = new List<Form> ();
		//brain = gameObject.GetComponent<AI> ();
		//hands = gameObject.GetComponent<Hands> ();
		if (hyperSpeed < 1) {
			hyperSpeed = 1;
		}
	}

	void Start() {

	}

	public void die() {
		dead = true;
		Map.S.deadBois.Add(this);
	}

	public void realDeath() {
		removeForm();
		if (Map.S.forms.Contains (this)) {
			Debug.LogError ("failed, " + gameObject.name + " was not desroyed properly or not activated properly " + dead);
		}
		
		for (int i = 0; i < effects.Count; i++) {
			effects[i].removeAll();
		}
		if (gameObject != null) {
			Destroy (this.gameObject);
		}

	}

	int lastDir;

	public void setAnim(string type, bool value){
		myGraphics.SetBool (type, value);
	}

	public void changeDirection(int dir){
		if (myGraphics) {
			//if (direction == 2 || direction == 1 || direction == 3) {
				if (dir == 7 || dir == 6 || dir == 5) {
					//skin.flipX = true;
				}
			//} else if (direction == 7 || direction == 6 || direction == 5) {
				if (dir == 2 || dir == 1 || dir == 3) {
					//skin.flipX = false;
				}
			//}
			//if (direction == 1 || direction == 0 || direction == 7) {
				if(dir == 3 || dir == 4 || dir == 5){
					//myGraphics.SetBool ("backwards", false);
				}
			//} else if (direction == 3 || direction == 4 || direction == 5) {
				if(dir == 1 || dir == 0 || dir == 7){
				//	myGraphics.SetBool ("backwards", true);
				}
			//}
		}
		direction = dir;
	}

	int lastX;
	int lastY;

	public void changeDirection(Vector2Int dir){
		if (dir.x != 0 || dir.y != 0) {
			if (myGraphics) {
				if (lastX != 0 && dir.x != 0 && dir.x != lastX) {
				
					lastX = dir.x;
				}
				if (lastY != 0 && dir.y != 0 && dir.y != lastY) {
					myGraphics.SetBool ("backward", !myGraphics.GetBool ("backward"));
					lastY = dir.y;
				}
			}
				

			if (dir.y > 0) {
				if (dir.x > 0) {
					changeDirection (7);
				} else if (dir.x < 0) {
					changeDirection (1);
				} else {
					changeDirection (0);
				}
			} else if (dir.y < 0) {
				if (dir.x > 0) {
					changeDirection (5);
				} else if (dir.x < 0) {
					changeDirection (3);
				} else {
					changeDirection (4);
				}
			} else {
				if (dir.x > 0) {
					changeDirection (6);
				} else if (dir.x < 0) {
					changeDirection (2);
				}
			}
		} 
	}

	public bool changeDir(Vector2Int dest) {
		if (centerPoint != dest) {
			int chosenD = GM.S.getClosestDir (centerPoint, dest);
			if (chosenD >= 0) {
				changeDirection (chosenD);
			}
			return true;
		} else {
			//Debug.Log ("im here");
			return false;
		}
	}

	int coinFlip = -1;
	public void changeDirectionCardinal(Vector2Int dir){
		if (dir.x != 0 || dir.y != 0) {
			if (myGraphics) {
				if (lastX != 0 && dir.x != 0 && dir.x != lastX) {

					lastX = dir.x;
				}
				if (lastY != 0 && dir.y != 0 && dir.y != lastY) {
					myGraphics.SetBool ("backward", !myGraphics.GetBool ("backward"));
					lastY = dir.y;
				}
			}
			if (coinFlip == -1 || Random.value > 0.75f) {
				coinFlip = Random.Range (0, 2);
			}
			int[] choices = new int[2];
			int[,] dirs = new int[2, 2];
			choices [0] = dir.y;
			dirs [0, 0] = 0;
			dirs [0, 1] = 4;
			choices [1] = dir.x;
			dirs [1, 0] = 6;
			dirs [1, 1] = 2;
			int choice2 = (coinFlip+1) % 2;
			if (choices [coinFlip] > 0) {
				changeDirection (dirs [coinFlip, 0]);
			} else if (choices [coinFlip] < 0) {
				changeDirection (dirs [coinFlip, 1]);
			} else if (choices [choice2] > 0) {
				changeDirection (dirs [choice2, 0]);
			} else if (choices [choice2] < 0) {
				changeDirection (dirs [choice2, 1]);
			}
		}
	}
		
	public bool checkCol(Form other){
		if (!other) {
			return true;
		}
		bool canCollide = false;
		//Debug.Log (name + " - " + other.name);
		if (other != parent && other.parent != this && !other.etheral && other != this && other != this) {
			if (!parent || parent != other.parent) {
				//Debug.Log("can collide");
				canCollide = true;
			}
		}
		return canCollide;
	}

	bool moving = false;
	public int xEdge;
	public int yEdge;
	public Cell[,] curMap;

	public List<Form> move(int dir){
		List<Form> collided = new List<Form>();
		if (!dead) {
			if (dir > -1 && dir < checks.Length) {
				if (!moving) {
					//speedCounter = 0;
					moving = true;
					//Debug.Log (gameObject.name + " moving " + dir);

					for (int i = 0; i < body.Length; i++) {
						int x = body [i].x + centerPoint.x;
						int y = body [i].y + centerPoint.y;
						if (x > -1 && x < Map.S.worldSizeX && y > -1 && y < Map.S.worldSizeY) {
							Map.S.world [centerPoint.x + body [i].x, centerPoint.y + body [i].y].formLeave (this);
						}
					}

					//Debug.Log (Map.S.worldSizeX);

					if (dir != direction) {
						changeDirection (dir);
					}
					bool move = true;
					for (int i = 0; i < checks [dir].Length; i++) {
						int x = centerPoint.x + checks [dir] [i].x;
						int y = centerPoint.y + checks [dir] [i].y;
						if (x > -1 && x < Map.S.worldSizeX && y > -1 && y < Map.S.worldSizeY) {
							if (!etheral && !gaseous) {
								Cell c = Map.S.world [x, y];

								if (Map.S.world [x, y].height < height) {
									//enter checks
									//Debug.Log("there height " + Map.S.world[x,y].height + " is less than mine " + height);
									for (int k = 0; k < c.within.Count; k++) {
										//Debug.Log (c.within [k].name);
										if (c.within [k] != parent && c.within [k].parent != this && !c.within [k].etheral) {
											if (!c.within[k].gaseous) {
												collided.Add (c.within [k]);
											}
											enter (c.within [k], x, y);
											c.within [k].enter (this, x, y);
										}
									}
								} else {
									//Debug.Log (gameObject.name + " collided at " + originPoint);
									//collide checks
									//Debug.Log("looks like we got a collision");
									for (int k = 0; k < c.within.Count; k++) {
										Form co = c.within [k];
										if (co != parent && co.parent != this && !co.etheral && co.height >= height) {
											collide (co, x, y);
											co.collide (this, x, y);
											if (!co.gaseous) {
												collided.Add (co);
											}
										} else {
											//Debug.Log ("sike no collision it was just " + co.name);
										}
									}
									if (collided.Count != 0) {
										move = false;
									}
								}
							} else {
								//Debug.Log ("I am etheral");
							}
						} else {
							move = false;
							//Debug.Log ("out of bounds");
						}
					}
					if (move) {
						switch (dir) {
						case 0:
							originPoint.y += 1;
							centerPoint.y += 1;
							break;
						case 1:
							originPoint.y += 1;
							centerPoint.y += 1;
							originPoint.x -= 1;
							centerPoint.x -= 1;
							break;
						case 2:
							originPoint.x -= 1;
							centerPoint.x -= 1;
							break;
						case 3:
							originPoint.x -= 1;
							centerPoint.x -= 1;
							originPoint.y -= 1;
							centerPoint.y -= 1;
							break;
						case 4:
							originPoint.y -= 1;
							centerPoint.y -= 1;
							break;
						case 5:
							originPoint.y -= 1;
							centerPoint.y -= 1;
							originPoint.x += 1;
							centerPoint.x += 1;
							break;
						case 6:
							originPoint.x += 1;
							centerPoint.x += 1;
							break;
						case 7:
							originPoint.x += 1;
							centerPoint.x += 1;
							originPoint.y += 1;
							centerPoint.y += 1;
							break;
						}
						if (!floatStep) {
							transform.position = new Vector3 (centerPoint.x, centerPoint.y, 0);
						}
						moveEffects (centerPoint);
						/*
						if (hands) {
							for (int i = 0; i < hands.count; i++) {
								if (hands.getInventory () [i]) {
									if (hands.getInventory () [i].move (dir).Count != 0) {
										// drop item?
									}
								}
							}
						}
						*/
					}

					drawBody ();
					//check for touching
					moving = false;
				} else {
					//speedCounter++;
					//Debug.Log ("called extra");
				}

			} else {
				Debug.LogError ("invalid direction " + dir);
			}
		}
		curCollided = collided;
		return collided;
	}

	public void forceMove(int dir) {
		if (!dead) {
			if (dir > -1 && dir < checks.Length) {
				Vector2Int dp = new Vector2Int(0,0);
				switch (dir) {
				case 0:
					dp.y = 1;
					break;
				case 1:
					dp.y = 1;
					dp.x = -1;
					break;
				case 2:
					dp.x = -1;
					break;
				case 3:
					dp.x = -1;
					dp.y = -1;
					break;
				case 4:
					dp.y = -1;
					break;
				case 5:
					dp.y = -1;
					dp.x = 1;
					break;
				case 6:
					dp.x = 1;
					break;
				case 7:
					dp.x = 1;
					dp.y = 1;
					break;
				}
				bool inMap = true;
				for (int i = 0; i < body.Length; i++) {
					int x = body [i].x + centerPoint.x + dp.x;
					int y = body [i].y + centerPoint.y + dp.y;
					if (x < 0 || x >= Map.S.worldSizeX || y < 0 || y >= Map.S.worldSizeY) {
						inMap = false;
						break;
					}
				}
				if (inMap) {
					for (int i = 0; i < body.Length; i++) {
						int x = body [i].x + centerPoint.x;
						int y = body [i].y + centerPoint.y;
						if (x > -1 && x < Map.S.worldSizeX && y > -1 && y < Map.S.worldSizeY) {
							Map.S.world [centerPoint.x + body [i].x, centerPoint.y + body [i].y].formLeave (this);
						}
					}

					if (dir != direction) {
						changeDirection (dir);
					}
					centerPoint += dp;
					moveEffects (centerPoint);
					drawBody ();
				} else {
					Debug.LogError ("trying to move out of bounds " + centerPoint + " -> " + dp);
				}
			}
		}
	}

	void moveEffects(Vector2Int pos) {
		for (int i = 0; i < effects.Count; i++) {
			effects [i].move (pos);
		}
	}

	public List<Form> curCollided;
	public List<Form> curEntered;


	public void move(Vector2Int destination, bool debug){
		curCollided.Clear ();
		if (!dead) {
			if (destination.x > -1 && destination.x < Map.S.worldSizeX && destination.y > -1 && destination.y < Map.S.worldSizeY) {
				bool goodToGo = true;

				for (int i = 0; i < body.Length; i++) {
					int x = body [i].x + centerPoint.x;
					int y = body [i].y + centerPoint.y;
					if (x > -1 && x < Map.S.worldSizeX && y > -1 && y < Map.S.worldSizeY) {// maybe only necesary in force move

						//Debug.Log(centerPoint.x + body [i].x);
						//Debug.Log (centerPoint.y + body [i].y);
						if (!Map.S.world [centerPoint.x + body [i].x, centerPoint.y + body [i].y].formLeave (this)) {
							//goodToGo = false;// why do i need to check if the form is where it thinks it is?
						}
					} else {
						Debug.Log ("this is going to be bad");
						goodToGo = false;
					}
				}

				if (goodToGo) {
					changeDirection (destination - centerPoint);//maybe centerPoint
					if (checkForward (true, false)) {
						Cell c = Map.S.world [destination.x, destination.y];
						for (int k = 0; k < c.within.Count; k++) {
							if (checkCol(c.within [k])) {
								//enter (c.within [k]);
								//c.within [k].enter (this);
							}
						}
						centerPoint = destination;//originPoint + new Vector2Int (width / 2, length / 2);
						moveEffects(centerPoint);
					}
					if (!dead) {
						drawBody ();
					}
					//Debug.Log (originPoint);

				} else {
					erasePresence (false);
					drawBody ();

					Debug.Log ("not good to go");
				}
			}
		} else {
			Debug.Log (gameObject.name + " is already dead dont make it move");
		}
	}

	public void forceMove(Vector2Int destination, bool colliding) {
		if (!dead) {
			//if (destination.x > -1 && destination.x < Map.S.worldSizeX && destination.y > -1 && destination.y < Map.S.worldSizeY) {
			bool insideBounds = true;
			for (int i = 0; i < body.Length; i++) {
				int x = body [i].x + destination.x;
				int y = body [i].y + destination.y;
				if (x < 0 || x >= Map.S.worldSizeX || y < 0 || y >= Map.S.worldSizeY) {
					insideBounds = false;
					break;
				}
			}
			if (insideBounds) {
				for (int i = 0; i < body.Length; i++) {
					int x = body [i].x + centerPoint.x;
					int y = body [i].y + centerPoint.y;
					if (x > -1 && x < Map.S.worldSizeX && y > -1 && y < Map.S.worldSizeY) {
						if (!Map.S.world [centerPoint.x + body [i].x, centerPoint.y + body [i].y].formLeave (this)) {
						}
					}
				}
				changeDirection (destination - centerPoint);
				/*
			for (int i = 0; i < body.Length; i++) {
				int x = body [i].x + destination.x;
				int y = body [i].y + destination.y;
				if (x > -1 && x < Map.S.worldSizeX && y > -1 && y < Map.S.worldSizeY) {
					Cell c = Map.S.world [x, y];
					for (int k = 0; k < c.within.Count; k++) {
						//Debug.Log (c.within [k].name);
						if ((checkCol (c.within [k])) && colliding) {
							//enter (c.within [k]);
							//c.within [k].enter (this);
						}
					}
				} else {
					break;
				}
			}
			*/
				//originPoint = destination;
				centerPoint = destination;//originPoint + new Vector2Int (width / 2, length / 2);
				moveEffects (centerPoint);
				drawBody ();
			}
			//}
		}
	}

	public void erasePresence(bool nHidden){
		hidden = nHidden;
		if (skin != null) {
			//Debug.Log (name + " got skin");
			if (hidden) {
				//Debug.Log ("turn off");
				skin.SetActive (false);
			} else {
				//Debug.Log ("turning it on");
				skin.SetActive (true);
			}
		}
		leaveSpace();
	}

	public void leaveSpace() {
		for (int i = 0; i < body.Length; i++) {
			//Debug.Log("before:" + Map.S.world[originPoint.x + body [i].x, originPoint.y + body [i].y].within.Count);
			int xp = centerPoint.x + body[i].x;
			int yp = centerPoint.y + body[i].y;
			if (xp > -1 && yp > -1 && xp < Map.S.worldSizeX && yp < Map.S.worldSizeY) {
				Map.S.world [centerPoint.x + body [i].x, centerPoint.y + body [i].y].formLeave (this);
			}
			//Debug.Log("after" + Map.S.world[originPoint.x + body [i].x, originPoint.y + body [i].y].within.Count);
		}
	}

	public void goEtheral(bool nEtheral) {
		etheral = nEtheral;
	}
	public bool checkCorner(Vector2Int pos, int dir, int chunk, int xMod, int yMod){
	//	List<Form> hitList = new List<Form> ();
	//	Debug.LogError ("checking corner " + dir);
		bool hitSomething = false;
		if (dir < checks.Length && dir > -1) {
			int c = 0;
			if (chunk > 0) {
				c = checks [dir].Length - 1;
			}
			int x = pos.x + checks [dir] [c].x + xMod;
			int y = pos.y + checks [dir] [c].y + yMod;
		//	Debug.Log ("corner: " + x + ", " + y);
			if (x > -1 && x < Map.S.worldSizeX && y > -1 && y < Map.S.worldSizeY) {
				if (!Map.S.world [x, y].canEnter(this)/*.height >= height*/) {
					for (int k = 0; k < Map.S.world [x, y].within.Count; k++) {
						Form f = Map.S.world [x, y].within [k];
						if (checkCol (f) && f.height >= height) {
							//Debug.Log (f.name + " hit my corner");
							if (!f.gaseous) {
								hitSomething = true;
							}
							if (!curCollided.Contains (f)) {
								curCollided.Add (f);
							}
						}
					}
				}
			} else {
				curCollided.Add(Map.S.terrain);
			}
		}
		/*
		for (int i = 0; i < hitList.Count; i++) {
			if (!curCollided.Contains(hitList[i])) {
				curCollided.Add (hitList[i]);
			}
		}// we only add to curCOllided and dont clear it because this should be able to be used in conjuncion with check side functions - for knockback
		*/
		return hitSomething;
	}

	public bool checkSide(Vector2Int pos, int dir, bool clearCurCol, bool colliding){
		if (clearCurCol) {
			curCollided.Clear ();
		}
		//List<Form> collided = new List<Form> ();
		if (dir < checks.Length && dir > -1) {
			for (int i = 0; i < checks[dir].Length; i++) {
				int x = pos.x + checks[dir] [i].x;
				int y = pos.y + checks[dir] [i].y;
				if (x > -1 && x < Map.S.worldSizeX && y > -1 && y < Map.S.worldSizeY) {
					if (catchAll || !Map.S.world [x, y].canEnter(this)/*.height >= height*/) {
						for (int k = 0; k < Map.S.world [x, y].within.Count; k++) {
							Form f = Map.S.world [x, y].within [k];
							if (checkCol(f) && (catchAll || f.height >= height)) {
								if (!curCollided.Contains (f)) {
									//Debug.Log (name + " hit side w/" + f.name);
									//if (!f.removeOnCol) {//!checkRemove(f, x, y, colliding)) {
									if (!checkRemove(f, x, y) && !f.gaseous) {
										curCollided.Add (f);
									}
									if (colliding) {
										collide (f, x, y);
										f.collide (this, x, y);
									}
									//}
								}
							}
						}
					}
				} else {
					curCollided.Add(Map.S.terrain);
				}
			}
		} else {
			Debug.LogError (gameObject + "invalid direction " + dir);
		}
		return curCollided.Count == 0;
	}

	public bool checkForward(bool colliding, bool debug){
		//List<Form> collided = new List<Form> ();
		curCollided.Clear ();
		if (debug) {
			Debug.Log ("dir " + direction);
		}
		for (int i = 0; i < checks [direction].Length; i++) {
			int x = centerPoint.x + checks [direction] [i].x;
			int y = centerPoint.y + checks [direction] [i].y;
			if (debug) {
				Debug.Log (name + " " + x + ", " + y);
			}
			if (x > -1 && x < Map.S.worldSizeX && y > -1 && y < Map.S.worldSizeY) {
				if (!etheral && !gaseous) {
					Cell c = Map.S.world [x, y];
					if (debug) {
					//	Debug.Log ("checking forward " + c.height);
					}
					if (!c.canEnter(this)/*.height >= height*/) {
						for (int k = 0; k < c.within.Count; k++) {
							if (checkCol (c.within [k]) && c.within [k].height >= height) {
								if (!curCollided.Contains (c.within [k])) {
									if (!c.within[k].gaseous) {
										curCollided.Add (c.within [k]);
									}
									if (colliding) {
										collide (c.within [k], x ,y);
										//Debug.Log (curCollided [i] + " " + name);
										if (c.within[k]) {
											c.within [k].collide (this, x, y);									
										}
									}
								}
							}
						}
					} else if (c.height < height) {
						if (debug) {
							//Debug.Log ("less height");
						}
						for (int k = 0; k < c.within.Count; k++) {
							if (debug) {
								Debug.Log (c.within [k].name);
							}
							if (checkCol (c.within [k])) {
								if (debug) {
									//Debug.Log (name + " entered " + c.within [k].name);
								}
								//enter (c.within [k]);
								//c.within [k].enter (this);
							}
						}
					}
				}
			} else {
				Debug.Log (name + " out side bounds moving " + direction);
				curCollided.Add (Map.S.terrain);
				return false;
			}
		}
		for (int i = 0; i < curCollided.Count; i++) {
			if (colliding) {
				//collide (curCollided [i]);
				//Debug.Log (curCollided [i] + " " + name);
				//curCollided [i].collide (this);
			}
		}
		
		return curCollided.Count == 0;
	}

	public List<Form> checkForward(bool catchAll){
		List<Form> collided = new List<Form> ();
		for (int i = 0; i < checks [direction].Length; i++) {
			int x = centerPoint.x + checks [direction] [i].x;
			int y = centerPoint.y + checks [direction] [i].y;
			if (x > -1 && x < Map.S.worldSizeX && y > -1 && y < Map.S.worldSizeY) {
				if (!etheral && !gaseous) {
					Cell c = Map.S.world [x, y];
						for (int k = 0; k < c.within.Count; k++) {
							if (!c.canEnter(this) || catchAll) {
								if (checkCol (c.within [k]) && (c.within [k].height >= height || catchAll)) {
									if (!collided.Contains (c.within [k])) {
										if (!c.within[k].gaseous) {
											collided.Add (c.within [k]);
										}
									}
								}
							}
						}
				}
			} else {
				collided.Add (Map.S.terrain);
			}
		}
		return collided;
	}
		
	public bool checkBody(){
		//curCollided.Clear ();
		bool freeSpace = true;
		for (int i = 0; i < body.Length; i++) {
			int x = body [i].x + centerPoint.x;
			int y = body [i].y + centerPoint.y;
			if (x > -1 && x < Map.S.worldSizeX && y > -1 && y < Map.S.worldSizeY) {
				if (!etheral && !gaseous) {
					if (!Map.S.world [x, y].canEnter(this)/*.height >= height*/) {
						for (int k = 0; k < Map.S.world [x, y].within.Count; k++) {
							if (checkCol (Map.S.world [x, y].within [k])) {// && !Map.S.world[x,y].within[k].removeOnCol) {
								freeSpace = false;
							}
						}
						//Debug.LogError ("I," + gameObject.name + " shouldnt be here");
					}
				}
			} else {
				return false;
			}
		}
		return freeSpace;
	}

	public bool checkBody(Vector2Int pos, bool colliding, bool catchAll) {//catch all will also store the form to short to collide. Added for AI sword checking
		//List<Form> col = new List<Form> ();
		curCollided.Clear();
		for (int i = 0; i < body.Length; i++) {
			int x = body [i].x + pos.x;
			int y = body [i].y + pos.y;
			if (x > -1 && x < Map.S.worldSizeX && y > -1 && y < Map.S.worldSizeY) {
				if (!etheral && !gaseous) {
					if (!Map.S.world [x, y].canEnter(this)/*height >= height */|| catchAll) {
						for (int k = 0; k < Map.S.world [x, y].within.Count; k++) {
							if (checkCol (Map.S.world [x, y].within [k])) {
								if (!curCollided.Contains (Map.S.world [x, y].within [k])) {
									Form f = Map.S.world [x, y].within [k];
									if (f.height >= height) {
										//Debug.Log (name + " hit with " + f.gameObject.name + " at " + x + ", " + y);
										//if (!f.removeOnCol) {//checkRemove(f, x, y, colliding)) {
											curCollided.Add (f);
										//}
										if (colliding) {
											//checkRemove(f, x, y);
											f.collide (this, x, y);
											collide (f, x, y);
										}
									} else if (catchAll) {
										curCollided.Add (f);
										//	Debug.Log ("cornhee " + f.name);
									}
								}
							}
						}
					} 
				}
			} else {
				//Debug.Log (x + ", " + y + " adding terrain " + Map.S.worldSizeX + ", " + Map.S.worldSizeY);
				curCollided.Add (Map.S.terrain);
			}
		}
		
		//curCollided = col;
		return curCollided.Count == 0;
	}

	bool checkRemove(Form col, int x, int y) {
		if (col.removeOnCol && checkCol(col)) {
			Debug.Log("col removal");
			Cell c = Map.S.world[x,y];
			c.formLeave(col);
			return true;
		}
		return false;
	}

	public void collide(Form col, int x, int y){
		if (checkCol(col)) {
			for (int i = 0; i < colActions.Length; i++) {
				//Debug.Log(name + " collided with " + colActions[i].name);
				colActions [i].callAction (col, 0, x, y);
			}
		}
	}

	public void enter(Form col, int x, int y){
		if (checkCol (col)) {
			for (int i = 0; i < entActions.Length; i++) {
				entActions [i].callAction (col, 1, x, y);
			}
		}
	}

	public void exit(Form col, int x, int y) {
		//Debug.Log ("exit " + col.name);
		if (checkCol (col)) {
			for (int i = 0; i < exiActions.Length; i++) {
				exiActions [i].callAction (col, 2, x, y);
			}
		} else {
			//Debug.Log ("psyche");
		}
	}

	public void removeForm() {
		Brain myBrain = gameObject.GetComponent<Brain>();
		if (myBrain) {
			Map.S.brains.Remove(myBrain);
		}
		SpriteRenderer sr = gameObject.GetComponentInChildren<SpriteRenderer> ();
		if (sr) {
			//Debug.Log ("remove");
			Map.S.allSprites.Remove (sr);
		}
		for (int i = 0; i < body.Length; i++) {
			int x = body [i].x + centerPoint.x;
			int y = body [i].y + centerPoint.y;
			if (x > -1 && x < Map.S.worldSizeX && y > -1 && y < Map.S.worldSizeY) {
				Map.S.world[x,y].formLeave(this);
			}
		}
		Map.S.removeForm(this);
	}

	public bool drawBody(){
		//Debug.Log (gameObject.name + " drawing, length: " + body.Length);
		if (!hidden && !dead) {
			curEntered.Clear ();
			for (int i = 0; i < body.Length; i++) {
				int x = body [i].x + centerPoint.x;
				int y = body [i].y + centerPoint.y;
				if (x > -1 && x < Map.S.worldSizeX && y > -1 && y < Map.S.worldSizeY) {
					Map.S.world [x, y].formEnter (this);
				} else {
					Debug.LogError (gameObject.name + " is trying to draw itself out of bounds at " + x + ", " + y);
					return false;
				}
			}
			if (skin) {
				//skin.sortingOrder = /* height + */Map.S.world [centerPoint.x, centerPoint.y].heightMod;
			}
			for (int i = 0; i < curEntered.Count; i++) {
				//curEntered [i].enter (this);
				//this.enter (curEntered [i]);
			}
		}
		if (!floatStep) {
			transform.position = new Vector3 (centerPoint.x, centerPoint.y, 0);
		} else {

		}
		return true;
	}

	Vector3 vel;
	public bool floatStep = true;
	Vector2Int desiredPos;

	void Update() {
		if (floatStep) {
			if (transform.position.x != (float)centerPoint.x || transform.position.y != (float)centerPoint.y) {
				transform.position = Vector3.SmoothDamp (transform.position, new Vector3 (centerPoint.x, centerPoint.y, 0), ref vel, 0.015f);
			}
		}
		if (Input.GetKeyDown ("g")) { 
			floatStep = !floatStep;
		}
	}

	public bool spawn(int posX, int posY){
		bool freeSpace = false;
		if (posX > -1 && posX < Map.S.worldSizeX && posY > -1 && posY < Map.S.worldSizeY) {
			if (body.Length > 0) {
				Vector2Int p = new Vector2Int (posX, posY);
				//Debug.Log ("spawning at " + p);
				checkBody (p, false, false);
				if (curCollided.Count == 0) {
					freeSpace = true;
					transform.position = new Vector3 (posX, posY, 0);
					//originPoint = p;
					centerPoint = p;//originPoint + new Vector2Int (width / 2, length / 2);
					desiredPos = p;
					moveEffects(centerPoint);
					drawBody ();
				} else {
					Debug.LogError (name + " check body fail at " + posX + ", " + posY);
					for (int i = 0; i < curCollided.Count; i++) {
						if (curCollided[i]) {
							Debug.Log ("hit: " + curCollided [i].name + " " + transform.parent.name + " " + curCollided[i].transform.parent);
						}
					}

				}
			} else {
				//Debug.Log (name + "'s body.Length bad = " + body.Length + " I hope it doesn't need to touch anything");
				freeSpace = true;
			}
		}
		return freeSpace;
	}

	public void squareBody(){
		if (width > 0 && length > 0) {
			//Debug.Log (name + " is being created");
			body = new Vector2Int[width * length];
			int cornLen = width + length + 1;
			int count = 0;
			upCheck = new Vector2Int[width];
			int uc = 0;
			nwCheck = new Vector2Int[cornLen];
			int nc = 0;
			leftCheck = new Vector2Int[length];
			int lc = 0;
			wsCheck = new Vector2Int[cornLen];
			int wc = 0;
			downCheck = new Vector2Int[width];
			int dc = 0;
			seCheck = new Vector2Int[cornLen];
			int sc = 0;
			rightCheck = new Vector2Int[length];
			int rc = 0;
			enCheck = new Vector2Int[cornLen];
			int ec = 0;


			int xPosMod = width % 2;
			int yPosMod = length % 2;
			for (int x = -width/2; x < width/2 + xPosMod; x++) {
				for (int y = -length/2; y < length/2 + yPosMod; y++) {
					body [count] = new Vector2Int (x, y);
					Vector2Int v;
					count++;
					if (y == length/2 + yPosMod - 1) {
						v = new Vector2Int (x, y + 1);
						upCheck [uc] = v;
						uc++;
						nwCheck[nc] = v;
						nc++;
						enCheck[ec] = v;
						ec++;
					}
					if (x == -width/2) {
						v = new Vector2Int (x/* - (1 + width / 2)*/- 1, y/* - (length / 2)*/);
						leftCheck [lc] = v;
						lc++;
						wsCheck [wc] = v;
						wc++;
						nwCheck [nc] = v;
						nc++;
					}
					if (y == -length/2) {
						v = new Vector2Int (x/* - (width / 2)*/, y/* - (1 + length / 2)*/- 1);
						downCheck [dc] = v;
						dc++;
						seCheck [sc] = v;
						sc++;
						wsCheck [wc] = v;
						wc++;
					}
					if (x == width/2 + xPosMod - 1) {
						v = new Vector2Int (x + 1, y/* - (length / 2)*/);
						rightCheck [rc] = v; 
						rc++;
						seCheck [sc] = v;
						sc++;
						enCheck [ec] = v;
						ec++;
					}
				}
			}
			//seCheck [sc] = new Vector2Int (width / 2, -length / 2);

			nwCheck [nc] = new Vector2Int (-(1 + width / 2), yPosMod + length / 2);
			wsCheck [wc] = new Vector2Int (-(1 + width / 2), -(1 + length / 2));
			seCheck [sc] = new Vector2Int (xPosMod + width / 2, -(1 + length / 2));
			enCheck [ec] = new Vector2Int (xPosMod + width / 2, yPosMod + length / 2);

			checks = new Vector2Int[8][];
			checks [0] = upCheck;
			checks [1] = nwCheck;
			checks [2] = leftCheck;
			checks [3] = wsCheck;
			checks [4] = downCheck;
			checks [5] = seCheck;
			checks [6] = rightCheck;
			checks [7] = enCheck;

			if (skin) {
				//if (!etheral) { im not sure about this
					//skin.sortingOrder = Map.S.world [centerPoint.x, centerPoint.y].heightMod;
			/*	} else {
					skin.sortingOrder = 1000;
				}*/
			}
		} else {
			Debug.LogError (gameObject.name + " size invalid");
		}
	}

	public void circleBody(int posX, int posY){
		List<Vector2Int> b = new List<Vector2Int> ();
		float size = width * (Mathf.PI * 2);
		int bc = 0;
		//body = new Vector2Int[size + 20];
		float d = 0f;
		for (float i = 0; i < size; i += 1 ) {

			d += 360 / size;
			int x = (int)(posX + width * Mathf.Cos (d));
			int y = (int)(posY + width * Mathf.Sin (d));

			//body[bc] = new Vector2Int(x,y);
			bool poo = false;
			for(int k = 0; k < b.Count; k++){
				if (b [k] == new Vector2Int (x, y)) {
					//Debug.Log (i + " " + d);
					//Debug.Log (Mathf.Cos (d));
					poo = true;
					//Debug.Log (new Vector2Int (x, y));
				}
			}
			bc++;


			if(!poo){
				Debug.Log (i);
				b.Add(new Vector2Int (x, y));
			}
		}
		//Debug.Log (bc);
		body = b.ToArray ();
	}

	public void boxBody(int width, int length, int posX, int posY){
		centerPoint = new Vector2Int (posX, posY);
		desiredPos = centerPoint;
		moveEffects (centerPoint);
		//centerPoint = new Vector2Int (posX + width / 2, posY + length / 2);
		body = new Vector2Int[width * 2 + length * 2];
		int count = 0;


		for(int x = posX - width/2; x < posX + width/2; x++){
			for (int y = posY - length/2; y < posY + length/2; y++) {
				if (x == posX - width/2 || x == posX + width/2 - 1 || y == posY - length/2 || y == posY + length/2 - 1) {
				//if (x == 0 || x == width - 1 || y == 0 || y == length - 1) {
					body [count] = new Vector2Int (x, y);
					count++;
				}
			}
		}

	}

	public void addAction(Purpose newAct) {
		if (activeAction.Count == 0) {
			activeAction = new List<Purpose> ();
			active = true;
		}
		if (!activeAction.Contains(newAct)) {
			activeAction.Add(newAct);
		}
	}

	public void addParticle(Particle newPa) {
		if (effects.Count == 0) {
			effects = new List<Particle> ();
		}
		if (!effects.Contains (newPa)) {
			effects.Add (newPa);
			addAction (newPa);
		}
	}

	// not tested
	public void changeColorN(Color newColor) {
		color = newColor;
		if(spawned) {
			for (int i = 0; i < body.Length; i++) {
				int x = body [i].x + centerPoint.x;
				int y = body [i].y + centerPoint.y;
				if (x > -1 && x < Map.S.worldSizeX && y > -1 && y < Map.S.worldSizeY) {
					Map.S.world [centerPoint.x + body [i].x, centerPoint.y + body [i].y].setToFormColor(this);
					/*
					if (!.formLeave (this)) {
					}
					*/
				} 
			}
			//drawBody ();
		}
	}
	/*
	[Command]
	void CmdChangeColor(Color newColor, string id){
		RpcChangeColor (newColor, id);
	}

	[ClientRpc]
	void RpcChangeColor(Color newColor, string id){
		if (id != GM.S.myPlayer) {
			color = newColor;
		}
	}
	*/

	public Vector2Int[] getCheck(int dir) {
		return checks [dir];
	}
}
