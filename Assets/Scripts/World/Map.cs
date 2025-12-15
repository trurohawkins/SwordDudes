using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;

public class Map : MonoBehaviour {

	public static Map S;
	mapTerrain creator;
	public int seed;
	public bool flowing = false;
	public Cell[,] world;

	public int worldSizeX;
	public int worldSizeY;

	public int distanceDelayMultiplier = 1;
	public int activeZone;
	public List<Form> forms;
	public List<SpriteRenderer> allSprites;
	public List<Brain> brains;
	public List<Form> deadBois;
	public List<Particle> particles;

	public GameObject voidForm;
	[HideInInspector]
	public Form terrain;
	public bool letsGo;
	public bool camFollow;

	void Awake() {
		if (seed == 0) {
			seed = Random.Range (-99999, 99999);
		}
		Random.InitState (seed);
		S = this;
		creator = gameObject.GetComponent<mapTerrain> ();
		GameObject tmp = Instantiate (voidForm);
		terrain = tmp.GetComponent<Form> ();
		forms = new List<Form> ();
		allSprites = new List<SpriteRenderer> ();
		brains = new List<Brain> ();
		deadBois = new List<Form>();
		particles = new List<Particle>();
	}

	void Start(){
		if (!flowing) {
			//createWorld ();
		}
	}

	public void createWorld() {
		/*
		if (GameInfo.S) {
			if (!GameInfo.S.multiPlayer) {
				GameInfo.S.level = Random.Range (0, GameInfo.S.levels.Length);
			}
		}
		*/
		creator.spawnWorld ();
		newWorld();
	}

	public void newWorld() {
		creator.createLevel ();
		if (GM.S.drawSprites) {
			creator.tileWorld ();
		}
	}

	public void newDungeonRoom(int dir) {
		for (int i = 0; i < particles.Count; i++) {
			particles[i].removeAll();
		}
		creator.adventureRoom(dir);
		if (GM.S.drawSprites) {
			creator.tileWorld ();
		}
	}

	public void startWorld() {
		flowing = true;
	}

	void Update(){
		if (GM.S) {
			if (GM.S.created) {
				StartCoroutine (callActions ());
				GM.S.created = false;
			}
		} else if (letsGo) {
			//Random.InitState ((int)Time.time);
			/*
			if (monsterGuy.S) {
				monsterGuy.S.spawnMonsters ();
			}
			*/
			StartCoroutine (callActions ());
			letsGo = false;
		}
		if (Input.GetKeyDown ("space")) {
			//burstPause (2);
		}
		//debugDraw ();
	}

	void debugStep(){
		for (int i = 0; i < forms.Count; i++) {
			/*
			AI brain = forms [i].gameObject.GetComponent<AI> ();
			if (brain) {
				//Debug.Log ("poo brains");
				brain.contemplate (false,0);
			}
			*/
			for (int k = 0; k < forms [i].activeAction.Count; k++) {
				forms [i].activeAction[k].callAction (0);
			}
		}
	}

	public void burstPause(float time) {
		if (flowing) {
			StartCoroutine (pause (time));
		}
	}

	IEnumerator pause(float time) {
		flowing = false;
		yield return new WaitForSecondsRealtime (time);
		flowing = true;
	}

	public void spawnForm(GameObject newForm, int x, int y){
		Form f = newForm.GetComponent<Form>();
		if (f.body.Length == 0) {
			//Debug.Log("no body on " + newForm.name);
			//f.squareBody ();
		}
		if (f.spawn(x, y)) {
			//Debug.Log ("spawned");
			logForm(f);
			//debugDraw ();
			f.spawned =  true;
		} else {
			Debug.LogError ("no go spawning " + f.gameObject.name + " at " + f.centerPoint + " " + x + ", "+ y);
			//newForm.die ();
			f.spawned = false;
		}
	}

	public void searchSpawnForm(GameObject newForm, int x, int y) {
		Form f = newForm.GetComponent<Form>();
		
		if (!f.spawn(x, y)) {
			for (int i = 2; i < 15; i+=3) {
                float d = 0f;
                float size = i * (Mathf.PI * 2);
                for (float j = 0; j <= size + 1; j += 1 ) {
			        d += 360 / size;
			        int xp =  (int)myDumbRound(i * Mathf.Cos (d));
			        int yp = (int)myDumbRound(i * Mathf.Sin (d));
                    Vector2Int pos = new Vector2Int(x + xp, y + yp);
                    if (f.checkBody(pos, false, false)) {
                        //pos = new Vector2Int(x + xp, y + yp);
						if (f.spawn(pos.x, pos.y)) {
							Debug.Log("new pos found: " + pos);
							f.spawned = true;
						} else {
							Debug.LogWarning(f.name + " couldnt be spawned");
							// this probably never happens?
							f.spawned = false;
							return;
						}
                        i = 12;
                        break;
                    }
                }
            }
		} else {
			f.spawned = true;
		}
		logForm(f);
	}

	void logForm(Form f) {
		if (f.active && !forms.Contains(f)) {
			forms.Add (f);
		}
		Brain brain = f.gameObject.GetComponent<Brain>();
		if (brain) {
			if (!brains.Contains (brain)) {
				brains.Add (brain);
			}
		}
		SpriteRenderer sr = f.gameObject.GetComponentInChildren<SpriteRenderer> ();
		if (sr && !allSprites.Contains(sr)) {
			allSprites.Add (sr);
		}
	}

	public void logParticle(Particle p) {
		particles.Add(p);
	}

	public Vector2Int findSpawnPos(Form f, Vector2Int pos) {
		if (!f.checkBody(pos, false, false)) {
			for (int i = 2; i < 15; i+=3) {
                float d = 0f;
                float size = i * (Mathf.PI * 2);
                for (float j = 0; j <= size + 1; j += 1 ) {
			        d += 360 / size;
			        int xp =  (int)myDumbRound(i * Mathf.Cos (d));
			        int yp = (int)myDumbRound(i * Mathf.Sin (d));
                    Vector2Int p = new Vector2Int(pos.x + xp, pos.y + yp);
                    if (f.checkBody(p, false, false)) {
						return p;
                    }
                }
            }
			return new Vector2Int(-1, -1);
		} else {
			return pos;
		}
	}

	public void endWorld() {
		for (int i = 0; i < forms.Count; i++) {
			if (forms[i]) {
				forms[i].die();
			}
		}
		creator.deleteWorld();
	}

	IEnumerator callActions(){
		//yield return new WaitForSeconds (2);
		while (true) {
			if (flowing) {
				//GM.S.doCamera();
				for (int i = 0; i < brains.Count; i++) {
					if (brains [i]) {
						brains[i].think ();
					}
				}
				for (int i = 0; i < forms.Count; i++) {
					if (forms [i] && forms[i].active) {
						Form f = forms[i];
						if (f.activeAction.Count > 0) {
							for (int k = 0; k < f.activeAction.Count; k++) {
								/*yield return*/
								if (f.activeAction [k].active) {
									StartCoroutine (f.activeAction [k].callAction (0));
								}
								if (!f) {
									Debug.Log("form is gone thru actions");
									break;
								}
							}
							//Debug.Log ("actions called ");
						}
					}
				}
			}
			while (deadBois.Count > 0) {
				deadBois[0].realDeath();
				deadBois.RemoveAt(0);
			}
			yield return new WaitForFixedUpdate ();
		}
	}

	public bool checkCell(Vector2Int pos){
		Cell c = world [pos.x, pos.y];
		if (c.height == 0) {
			/*
			for (int i = 0; i < world [pos.x, pos.y].within.Count; i++) {
				Debug.Log (world [pos.x, pos.y].within [i]);
			}
			*/
			return true;
		} else {
			
			return false;
		}
	}

	//int visibleRoom = -1;

	public void debugDraw(int poo){
		Cell[,] cur;
		int xEdge;
		int yEdge;
		//if (visibleRoom < 0) {
			cur = world;
			xEdge = worldSizeX;
			yEdge = worldSizeY;
		//} 

		for (int x = 0; x < xEdge; x++) {
			for (int y = 0; y < yEdge; y++) {
				if (cur [x, y].within.Count > 0) {
					Form draw = cur [x, y].within [cur [x, y].within.Count - 1];
					//if (!draw.skin) {
						cur [x, y].sr.color = draw.color;
					//}
				} else {
					//cur [x, y].sr.color = new Color (0,0,0);
				}
			}
		}
	}

	public IEnumerator pauseWorld(){
		flowing = false;
		yield return new WaitForSeconds (1);
		//removeUnImportant();
	}

	public void removeUnImportant() {
		List<Form> deadList = new List<Form> ();
		for (int i = 0; i < forms.Count; i++) {
			if (forms [i].unImportant) {
				deadList.Add (forms [i]);
			}
		}
		int count = deadList.Count;
		for (int i = 0; i < count; i++) {
			deadList[i].die ();
		}
	}

	public void removeForm(Form f) {
		forms.Remove(f);
	}

	public Vector3Int getSpawn() {
		return creator.getSpawn ();
	}

	public Vector3Int getSpawn(int num) {
		return creator.getSpawn(num);
	}

	public void checkSpawns() {
		creator.checkSpawns ();
	}

		
	public int saneAddition(float start, float increase, float dir) {
		return saneAddition ((int)start, (int)increase, (int)dir);
	}

	public int saneAddition(int start, int increase, int dir) {//so dumb, do better
		if (start == 180 && dir == 1) {
			start = -179;
			increase--;//maybe fix sane difference like this??
		} else if (start == -180 && dir == -1) {
			start = 179;
			increase--;
		}
		bool debug = false;
		if (start == 146) {
			debug = true;
		}
		for (int i = 0; i < increase; i++) {
			if (debug) {
				//Debug.Log (start);
			}
			start += dir;
			if (Mathf.Abs (start) == 180 && i != increase) {
				start *= -1;
			}
		}
		return start;
	}

	public int saneDifference(float start, float end, float dir) {
		return saneDifference ((int)start, (int)end, (int)dir);
	}

	public int saneDifference(int start, int end, int dir) {
		int begin = start;
		int count = 0;
		if (start == 180 && dir == 1) {
			start = -179;
			count++;
		} else if (start == -180 && dir == -1) {
			start = 179;
			count++;
		}
		if (Mathf.Abs (end) > 180) {
			Debug.LogError ("end no good " + end);
			return 666;
		}
		if (dir == 0) {
			Debug.LogError ("direction is zero");
			return 666;
		} else {
			while (start != end) {
				start += dir;
				if (Mathf.Abs (start) == 180 && start != end) {
					start *= -1;
				}
				count++;
				if (count > 360) {
					Debug.LogError ("infinite sane difference " + start + " " + end + " dir: " + dir + ", started at: " + begin );
					break;
				}
			}
		}
		return count;
	}

	float myDumbRound(float f) {
        int sign = (int)Mathf.Sign(f);
        f = Mathf.Abs(f);
        float rem = f - Mathf.Floor(f);
        if (rem > 0.4f) {
            rem = 1;
        } else {
            rem = 0;
        }
        return (Mathf.Floor(f) + rem) * sign;
    }
}
