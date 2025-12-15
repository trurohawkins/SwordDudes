using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class path : Purpose {
	public string walkAnimVar;
	public int wanderLust;
	public int step = 0;
	public List<int> pathway;
	public int turnCost;
	public int inertia;
	public bool scared;
	public Vector2Int[] spaces;
	public Vector2Int destination;
	public Form objOfDesire;
	public int maxPathDistance = 100;

	public float stopDistance = 1.5f;
	public bool randomStep = false;
	public GameObject buffer;
	public bool startPathing = false;
	public bool currentlyPathing = false;

	 protected override void Awake(){
		base.Awake ();
		spaces = new Vector2Int[8];
		spaces [0] = new Vector2Int (0, 1);
		spaces [1] = new Vector2Int (-1, 1);
		spaces [2] = new Vector2Int (-1, 0);
		spaces [3] = new Vector2Int (-1, -1);
		spaces [4] = new Vector2Int (0, -1);
		spaces [5] = new Vector2Int (1, -1);
		spaces [6] = new Vector2Int (1, 0);
		spaces [7] = new Vector2Int (1, 1);

		//destination = new Vector2Int (0, 0);

		probeDirs = new List<int> ();
		probePoses = new List<Vector2Int> ();

		pathway = new List<int> ();
		trueSelf = self;
		if (buffer) {
			self = Instantiate (buffer).GetComponent<Form> ();
			self.centerPoint = trueSelf.centerPoint;
			self.squareBody ();
			self.parent = trueSelf;
		}
	}

	void Start() {
		if (startPathing) {
			pathAround ();
		}
	}

	public Form trueSelf;


	public bool canWalk = true;
	public override IEnumerator callAction(int delay){
		//Debug.Log (gameObject.name + " is moving");
		if (canWalk) {
			if (walkAnimVar != "") {
				trueSelf.setAnim (walkAnimVar, true);
			}
			int turnAdjusted = trueSelf.speed;
			if (step > 0 && step < pathway.Count) {
				if (pathway [step] % 2 == pathway [step - 1] % 2) {
					turnAdjusted += turnCost / 2;
				}
				if (Mathf.Abs (pathway [step] - pathway [step - 1]) == spaces.Length / 2) {
					turnAdjusted += turnCost / 2;
				}
			}

			if (trueSelf.speedCounter > turnAdjusted + delay) {
				for (int i = 0; i < trueSelf.hyperSpeed; i++) {
					int curStep = -1;
					if (pathway.Count > 0 && step > -1 && step < pathway.Count) {
						curStep = pathway [step];
					}
					//self.move (curStep);
					if (curStep > -1 && trueSelf.move(curStep).Count == 0) {
						step++;
					} else {
						updateObj ();
					}
				}
				trueSelf.speedCounter = 0;
			} else {
				trueSelf.speedCounter++;
			}

		}
		yield return new WaitForFixedUpdate();
	}

	public Vector2Int getNextDir() {
		Vector2Int dir = new Vector2Int (0, 0);
		if (pathway.Count != 0 && step < pathway.Count) {
			if (pathway [step] >= 0) {
				dir = spaces [pathway [step]];
			}
			step++;
		}
		return dir;
	}

	public List<int> getPath() {
		return pathway;
	}

	void Update() {
		if (Input.GetKeyDown ("space")) {
		//	updateObj ();
		}
	}

	bool overridden = false;
	public void overrideDestination(Vector2Int dest){
		destination = dest;
		overridden = true;
	}

	public void iAmScared(){
		scared = true;
		inertia = 75;
	}

	int sweep = 0;

	public override void updateObj (){
		if (!overridden) {
			if (objOfDesire) {
				destination = objOfDesire.centerPoint;
			} else {
				/*
				//destination = new Vector2Int (Random.Range (self.originPoint.x - range, self.originPoint.x + range), Random.Range (self.originPoint.y - range, self.originPoint.y + range));
				destination = new Vector2Int (-1, -1);
				int noLoop = 0;
				while (self.checkBody (destination).Count != 0 && noLoop < 25) {
					noLoop++;
					//Debug.Log ("Searching for a place");
					int xVal = Random.Range (5, range);
					int yVal = Random.Range (5, range);
		
					if (Random.value > 0.5f) {
						xVal *= -1;
					}
					if (Random.value > 0.5f) {
						yVal *= -1;
					}

					/*
				switch(sweep) {
				case 0:
					xVal *= -1;
					yVal *= -1;
					break;
				case 1:
					xVal *= -1;
					break;
				case 2:
					yVal *= -1;
					break;
				default:
					break;
				}
			
					sweep = (sweep++) % spaces.Length;
					destination = new Vector2Int (self.centerPoint.x + xVal, self.centerPoint.y + yVal);
				}*/
			}
		}
		//Debug.Log ("updating " + Vector2Int.Distance (self.centerPoint, destination) + " " + stopDistance);
		if (Vector2Int.Distance (self.centerPoint, destination) > stopDistance) {
			//Debug.Log ("new path");
			clearPath();
			pathAround ();
		} /*else {
			//Debug.Log ("closeenoguh");
			brain.contemplate (true,0);
		}*/
	}
	public void clearPath() {
		probePoses.Clear();
		probeDirs.Clear ();
		pathway.Clear ();
		step = 0;
	}
	public Vector2Int pathHead;

	int loopStopper = 0;
	float dist;
	int dir;
	int ndD;
	int rdD;

	void pathAround(){
		currentlyPathing = true;
		pathHead = trueSelf.centerPoint;
		Debug.Log ("starting at: " + pathHead + " destination: " + destination);
		dist = Vector2Int.Distance (pathHead, destination);
		loopStopper = 0;
		Debug.LogError ("path: " + dist);
		while (dist > stopDistance && pathway.Count < maxPathDistance && loopStopper < maxPathDistance) {
			loopStopper++;
			if (Random.Range (0, 101) < inertia && pathway.Count > 0) {
				int inert = pathway [pathway.Count - 1];
				if (self.checkSide (pathHead, inert, true, false) == true) {
					pathway.Add (inert);
					pathHead += spaces [inert];
				}
			} else {
				//1st and 2nd closet directions to travel
				ndD = -1;
				rdD = -1;
				dir = -1;
				if (!scared) {
					getCloser ();//finds the dir closest to destination
				} else {
					goFurther ();
				}
				self.checkSide (pathHead, dir, true, false);
				List<Form> col = self.curCollided;
				if (col.Count == 0) {
					pathHead += spaces [dir];
					pathway.Add (dir);
					dist = Vector2Int.Distance (pathHead, destination);
					Debug.Log ("no hit " + pathHead);
					//try the second closest side
				} else {
					bool notFound = true;
					for (int i = 0; i < col.Count; i++) {
							if ((objOfDesire && col [i] == objOfDesire)) {// || col[i].centerPoint == destination) {
							notFound = false;
							loopStopper = 100;
						}
					}
					if (notFound) {
						//finds the spot exactly across the obstacle in your way in the direction they are trying to go
						Vector2Int dest = probe (dir, pathHead);
						Debug.Log ("probe in dir: " + dir + " produced " + dest);
						if (dest.x < 0) {
								self.checkSide (pathHead, ndD, true, false);
							col = self.curCollided;
							if (col.Count == 0) {
								pathHead += spaces [ndD];
								pathway.Add (ndD);
								dist = Vector2Int.Distance (pathHead, destination);
							} else {
								dest = probe (ndD, pathHead);
								if (dest.x < 0) {
										self.checkSide (pathHead, rdD, true, false);
									col = self.curCollided;
									if (col.Count == 0) {
										pathHead += spaces [rdD];
										pathway.Add (rdD);
										dist = Vector2Int.Distance (pathHead, destination);
									} else {
										dest = probe (rdD, pathHead);
										if (dest.x < 0) {
											if (randomStep) {
											//	Debug.Log ("random");
												for (int i = 0; i < col.Count; i++) {
														if ((objOfDesire && col [i] == objOfDesire)) {// || col[i].centerPoint == destination) {
														loopStopper = 100;
													}
												}
												//Better solution needed
												//Debug.Log ("random direction");
												int newDir = Random.Range (0, spaces.Length);
												int noL = 0;
													while (self.checkSide (pathHead, newDir, true, false) == false && noL < 24) {
													newDir = Random.Range (0, spaces.Length);
													noL++;
												}
												//while(self.c
													if (self.checkSide (pathHead, newDir, true, false) == true) {
													pathHead += spaces [newDir];
													pathway.Add (newDir);
												}
											}
										} else {
											collide (pathHead, rdD, dest);
										}
									}
								} else {
									collide (pathHead, ndD, dest);
								}
							}
						} else {
							collide (pathHead, dir, dest);
						}
					}
				}
			}
			Debug.Log (pathHead + " " + dist);
		}
		//Debug.Log (pathHead);
		//Debug.Log ("Distance: " + dist);
		currentlyPathing = false;
	}

	void getCloser(){
		float lowestD = Mathf.Infinity;
		float ndLowest = Mathf.Infinity;
		float rdLowest = Mathf.Infinity;
		//1st and 2nd closet directions to travel
		for (int i = 0; i < spaces.Length; i++) {
			float di = Vector2Int.Distance (pathHead + spaces [i], destination);
			if (di < lowestD) {
				rdD = ndD;
				rdLowest = ndLowest;
				ndD = dir;
				ndLowest = lowestD;
				lowestD = di;
				dir = i;
			} else if (di < ndLowest) {
				rdD = ndD;
				rdLowest = ndLowest;
				ndLowest = di;
				ndD = i;
			} else if (di < rdLowest) {
				rdLowest = di;
				rdD = i;
			}
		}
	}

	void goFurther(){
		float highestD = Mathf.NegativeInfinity;
		float ndHighest = Mathf.NegativeInfinity;
		float rdHighest = Mathf.NegativeInfinity;
		//1st and 2nd closet directions to travel
		for (int i = 0; i < spaces.Length; i++) {
			float di = Vector2Int.Distance (pathHead + spaces [i], destination);
			if (di > highestD) {
				rdD = ndD;
				rdHighest = ndHighest;
				ndD = dir;
				ndHighest = highestD;
				highestD = di;
				dir = i;
			} else if (di > ndHighest) {
				rdD = ndD;
				rdHighest = ndHighest;
				ndHighest = di;
				ndD = i;
			} else if(di > rdHighest){
				rdD = i;
				rdHighest = di;
			}
		}
	}

	List<int> path1;
	List<int> path2;
	//sends out feelers t ofind the shortest path to circumvent the obstacle
	void collide(Vector2Int pos, int dir, Vector2Int d){
	//	Debug.Log ("C0llision: " + pos + " " + dir + " want to go to " + d);
		List<int> newPath;
		path1 = new List<int> ();
		path2 = new List<int> ();
		if (dir == 0) {
			poo (pos, d, dir, 4, 2, 6);
			//poo (pos, d, dir, 4, 1, 7);
		} else if (dir == 1) {
			poo (pos, d, dir, 5, 3, 7);
			//poo (pos, d, dir, 5, 0, 2);
		} else if (dir == 2) {
			poo (pos, d, dir, 6, 0, 4);
			//poo (pos, d, dir, 6, 1, 3);
		} else if (dir == 3) { 
			poo (pos, d, dir, 7, 1, 5);
			//poo (pos, d, dir, 7, 2, 4);
		} else if (dir == 4) {
			poo (pos, d, dir, 0, 2, 6);
			//poo (pos, d, dir, 0, 4, 5);
		} else if (dir == 5) { 
			poo (pos, d, dir, 1, 3, 7);
			//poo (pos, d, dir, 1, 4, 6);
		} else if (dir == 6) {
			poo (pos, d, dir, 2, 0, 4);
			//poo (pos, d, dir, 2, 5, 7);
		} else if(dir == 7){
			poo(pos, d, dir, 3, 1, 5);
			//poo (pos, d, dir, 3, 0, 6);
		}

		if ((path1.Count < path2.Count || path2.Count == 0) && path1.Count > 0 && Random.Range(0, 101) > wanderLust) {
			newPath = path1; 
		//	Debug.Log ("path1");
		} else if(path2.Count > 0){
			newPath = path2;
			Debug.Log ("path2");
		} else {
			newPath = new List<int> ();
		//	Debug.Log("OH NO, NOWHERE TO GO!!!!");
		}
		for (int i = 0; i < newPath.Count; i++) {
			if (newPath [i] >= 0) {
				pathway.Add (newPath [i]);
				pathHead += spaces [newPath [i]];
			}
		//	Debug.Log ("new path bring me to " + pathHead);
		}
		//Debug.Log (pathHead);
		if(newPath.Count > 0){
			//Debug.Log (newPath [0]);
			//Debug.Log ("new path brought me here " + newPath [newPath.Count - 1]);
		}
	}
	public bool pooDebug = false;
	//two flows are called to go around whatever obstacles are in your way
	void poo(Vector2Int pos, Vector2Int d, int dir, int back, int firstCheck, int secCheck){
		//Vector2Int d = probe (dir, pos);
			if (pooDebug) {	Debug.Log("Path1 first check " + dir);}
			if (self.checkSide (pos, firstCheck, true, false) == true) {
				path1 = flow (pos + spaces [firstCheck], dir, firstCheck, back, secCheck, pooDebug, d, 0);
		}
		if (path1.Count == 0 || path1[path1.Count - 1] == -1) {
				if (pooDebug) {Debug.Log ("path1 2nd check " + firstCheck);}
				path1 = flow (pos + spaces [back], firstCheck, back, secCheck, dir, pooDebug, d, 0);
		} 
		if (path1 [path1.Count - 1] == -1) {
			path1.Clear ();
		}

		//Debug.Log (path1.Count);
			if (pooDebug){Debug.Log ("Path2 first check "+ secCheck);}
			if(self.checkSide(pos, secCheck, true, false) == true){
				path2 = flow (pos + spaces[secCheck], dir, secCheck, back, firstCheck, pooDebug, d, 0);
		}
		if (path2.Count == 0 || path2[path2.Count - 1] == -1) {
			if (pooDebug) {
				Debug.Log ("Path2 2nd check " + secCheck);
			}
				path2 = flow (pos + spaces [back], secCheck, back, firstCheck, dir, pooDebug, d, 0);
		} 
	}

	//int flowMax = maxPathDistance;

	List<int> flow(Vector2Int pos, int inD, int dirD, int outD, int od, bool debug, Vector2Int dest, int c){
		bool finished = false;
		if (debug) {
			Debug.Log ("flow " + pos + " " + inD + " " + c);
		}
		List<int> pathBit = new List<int> ();
		if (c < maxPathDistance) {//flowMax) {
				if (Vector2Int.Distance(pos, dest) < stopDistance  || Vector2Int.Distance(pos, destination) < (stopDistance)) { //cuts out a bunch of loops if you give them one more stepp after reching beng closer
				if (debug) {
					Debug.Log ("we Got closer");
				}
				//c = flowMax - 1;
				c = maxPathDistance - 1;
				//finished = true;
			}
			//dest is the point directly adjacent to the spot at which you collided
				if (pos == dest) {
					Debug.Log (pos);
					finished = true;
				}
			if (!finished) {
				List<int> p = new List<int> ();
					if (self.checkSide (pos, inD, true, false) == true) {
					//Debug.Log ("got through at " + pos + " " + inD);
					p = flow (pos + spaces [inD], od, inD, dirD, outD, debug, dest, c + 1);
					} else if (self.checkSide (pos, dirD, true, false) == true) {
					//Debug.Log("goin around at " + pos + " " + dirD);
					p = flow (pos + spaces [dirD], inD, dirD, outD, od, debug, dest, c + 1);
					} else if (self.checkSide (pos, outD, true, false) == true) {
					//Debug.Log ("outWay " + pos + " " + outD);
					p = flow (pos + spaces [outD], dirD, outD, od, inD, debug, dest, c + 1);
				} else {
					if (debug) {
						Debug.Log ("we have run out options " + pos);
					} 
					pathBit.Add (-1);
					return pathBit;
				}
				pathBit.Add (dirD);
				for (int i = 0; i < p.Count; i++) {
					pathBit.Add (p [i]);
				}
			} else if (debug) {
				Debug.Log (pos);
			}
		} else if (debug) {
			Debug.Log (pos);
			Debug.Log ("100? proabbly a loop!");
		}
		return pathBit;
	}

	List<int> probeDirs;
	List<Vector2Int> probePoses;

	Vector2Int probe(int dir, Vector2Int p){
		Vector2 ph = p;
		Vector2 realDir = destination - p;
		realDir.Normalize ();
//		Debug.Log ("probing at " + p + " in " + dir);
		//here is where you check if you have been to a collision spot before and potentially and eventually gets the Form out of the loop
		for (int i = 0; i < probePoses.Count; i++) {
			if (dir == probeDirs [i] && p == probePoses [i]) {
			//	Debug.LogError ("not here again!?"); 
				return new Vector2Int (-1, -1); // totally necesarry with meandering flow stopper
			}
		}
		probeDirs.Add (dir);
		probePoses.Add (p);
		int noLoop = 0;
		p += spaces[dir];
		ph += (realDir * 2);
		//Debug.Log (ph + " " + realDir);
			self.checkBody(new Vector2Int((int)ph.x, (int)ph.y), false, false);
			while (self.curCollided.Count != 0  && noLoop < 100) {
			ph += realDir;
//			Debug.Log (ph);

			noLoop++;
			if (ph.x < 0 || ph.x >= Map.S.worldSizeX || ph.y < 0 || ph.y >= Map.S.worldSizeY) {
				noLoop = 100;
				//Debug.Log ("returning bad D");
			}
			self.checkBody (new Vector2Int ((int)ph.x, (int)ph.y), false, false);
		}
		if (noLoop >= 100) {
			
			return new Vector2Int (-1, -1);
		}
		//Debug.Log (p);
			//Debug.Log ("d " + dir + " pos " + ph + " -> " + p);

		return new Vector2Int((int)ph.x, (int)ph.y);
	}
		
}
