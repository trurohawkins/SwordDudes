using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GM : MonoBehaviour {
	public bool pausedStart;
	public bool doSplash;
	public bool drawSprites;
	public GameObject player;
	public GameObject splashScreen;
	public int[] startPoses;

	public GameObject indicator;
	public GameObject menu;

	public GameObject player1;
	public GameObject playerCpu;
	public List<Player> players;
	public bool[] hasSwords;

	public float camMinDist = 30;
	float camMaxDist;

	public bool created = false;
	public int deathCount;
	public int rounds = 0;

	public static GM S;

	public string myPlayer;
	public Vector2Int[] dirs;
	public Vector2Int invalidDest;

	//MessageMaker mm;

	public GameObject flame;
	int[] pads;
	int[]nums;
	int[] teams;
	GameObject[] souls;
	int[] cols;
	int numPlayers = 2;
	public DungeonMaster curDM;

	void Awake() {
		S = this;
		dirs = new Vector2Int[8];
		dirs [0] = new Vector2Int (0, 1);
		dirs [1] = new Vector2Int (-1, 1);
		dirs [2] = new Vector2Int (-1, 0);
		dirs [3] = new Vector2Int (-1, -1);
		dirs [4] = new Vector2Int (0, -1);
		dirs [5] = new Vector2Int (1, -1);
		dirs [6] = new Vector2Int (1, 0);
		dirs [7] = new Vector2Int (1, 1);
		invalidDest = new Vector2Int (-1, -1);
		players = new List<Player> ();
	}
		
	//public int cpuPlayers = 0;

	void Start() {
		//camMaxDist = (float)Mathf.Sqrt((Map.S.worldSizeX * Map.S.worldSizeX) + (Map.S.worldSizeY * Map.S.worldSizeY));
		//camMaxDist = Camera.main.orthographicSize;
		Map.S.createWorld ();
		if (GameInfo.S) {
			if (!GameInfo.S.multiPlayer) {
				Debug.Log (GameInfo.S.enemyLevel);
				singlePlayerInfo.S.parseStage(GameInfo.S.songs[0], GameInfo.S.enemyLevel);
				/*
				GameInfo.S.controls [1] = -GameInfo.S.enemyLevel;
				GameInfo.S.controlNums [1] = 0;
				GameInfo.S.teamNums[1] = -1;
				GameInfo.S.songs [1] = Random.Range (0, GameInfo.S.souls.Length);
				GameInfo.S.colors [1] = Random.Range (0, 4);
				GameInfo.S.numPlayers = 2;
				*/
			}
			numPlayers = GameInfo.S.numPlayers;
			pads = new int[numPlayers];
			nums = new int[numPlayers];
			teams = new int[numPlayers];
			souls = new GameObject[numPlayers];
			cols = new int[numPlayers];
			for (int i = 0; i < numPlayers; i++) {
				pads [i] = GameInfo.S.controls [i];
				nums [i] =  GameInfo.S.controlNums [i];
				teams [i] = GameInfo.S.teamNums [i];
				//Debug.Log ("i: " + i + " " + souls.Length + " " + GameInfo.S.songs.Length);
				//Debug.Log ("souls: " + GameInfo.S.souls.Length + " " + GameInfo.S.songs [i]);
				if (GameInfo.S.songs[i] >= 0) {
					souls [i] = GameInfo.S.souls[GameInfo.S.songs [i]];
				} else
                {
					souls[i] = GameInfo.S.souls[Random.Range(0, GameInfo.S.souls.Length)];
				}
				cols [i] = GameInfo.S.colors [i];
			}
			/*
			pads [0] = GameInfo.S.controls [0];//GameInfo.S.p1Pad;
			pads [1] = GameInfo.S.controls [1];// GameInfo.S.p2Pad;

			nums [0] =  GameInfo.S.controlNums [0];// GameInfo.S.p1ControlNum;
			nums [1] =  GameInfo.S.controlNums [1];//GameInfo.S.p2ControlNum;

			souls [0] = GameInfo.S.souls[GameInfo.S.p1Song];
			souls [1] = GameInfo.S.souls[GameInfo.S.p2Song];
			*/
		} else {
			pads [0] = 1;
			pads [1] = 1;
			nums [0] = 0;
			nums [1] = 1;
			cols [0] = 0;
			cols [1] = 1;
		}

		for (int i = 0; i < numPlayers; i++) {
			if (pads [i] != 0) {
				GameObject p = Instantiate (player);
				if (pads [i] > 0) {
					p.AddComponent<InputController> ();
					InputController ic = p.GetComponent<InputController> ();
					ic.indicator = indicator;
				} else if (pads [i] < 0) {
					p.AddComponent<InputComputer> ();

					InputComputer ic = p.GetComponent<InputComputer> ();
					ic.getPather(i);
					ic.difficultyLevel = pads [i] * -1;
				} 
				Player pp = p.GetComponent<Player> ();
				pp.soulType = souls [i];
				players.Add (pp);
				pp.setInfo (i, cols [i], GameInfo.S.playerColors [GameInfo.S.teamNums[i]], teams [i]);
				Controller c = p.GetComponent<Controller> ();
				c.menu = menu;
			} else {
				players.Add (null);
			}
		}
		
		for (int i = 0; i < numPlayers; i++) {
			if (players [i]) {
				Vector3Int spawnPoint = Map.S.getSpawn ();
				//Debug.Log(spawnPoint);
				players [i].hasSword = hasSwords [i];
				players [i].setSpawnInfo (spawnPoint);//, i, cols [i], GameInfo.S.playerColors [GameInfo.S.teamNums[i]], teams [i]);
				spawnPlayer (players [i].gameObject, spawnPoint.x, spawnPoint.y);
			}
		}
		for (int i = 0; i < numPlayers; i++) {
			if (players[i]) {
				if (pads [i] > 0) {
					players[i].GetComponent<InputController> ().gamePad = pads [i];// GameInfo.S.p1Pad;
				} else if (pads[i] < 0) {
					InputComputer inp = players[i].GetComponent<InputComputer> ();
					//inp.getEnemy (players[(i+1)%2].gameObject);
					//cpuPlayers++;
				}
				players [i].GetComponent<Controller> ().controlNum = nums [i];//GameInfo.S.p1ControlNum;
			}
		}
		if (GameInfo.S.level == 5) {
			pausedStart = false;
			doSplash = false;
		}
		/*
		if (GameInfo.S.level != 5) {
			fixedCamera = false;
		} else {
			fixedCamera = true;
		}
		*/
		camMaxDist = Mathf.Max(Map.S.worldSizeX, Map.S.worldSizeY)/2;
		if (!fixedCamera) {
			Camera.main.orthographicSize = camMaxDist;
			cameraControl (true);
			Camera.main.transform.position = camDest;
		} else {
			Camera.main.transform.position = new Vector3 (Map.S.worldSizeX / 2, Map.S.worldSizeY / 2, -1);//camMaxDist/2);
			Camera.main.orthographicSize = ((float)Mathf.Max(Map.S.worldSizeX, Map.S.worldSizeY))/2f;// (float)Mathf.Sqrt((Map.S.worldSizeX * Map.S.worldSizeX) + (Map.S.worldSizeY * Map.S.worldSizeY));
		}
		if (numPlayers == 1 && fixedCamera) {
			//Camera.main.transform.parent = players[0].transform;
		}
		//Debug.Log(camDest);
		StartCoroutine (countDown ());
		//Debug.Log ("count down initiated");
		created = true;
	}

	public float camSpeed = 0.2f;
	public float camZoomSpeed = 0.2f;
	public bool fixedCamera;
	float moveTime;
	Vector3 camVelocity;
	float zoomVelocity;
	Vector3 camDest;
	int maxDist;
	public float stepSpeed;
	bool useDamp = false;
	bool everythingSpawned = false;

	void Update() {
		if (Input.GetKeyDown ("r")) {
		}

		if (!fixedCamera) {
			//cameraControl();
			//Camera.main.transform.position = Vector3.SmoothDamp (Camera.main.transform.position, camDest, ref camVelocity, camSpeed);
			if (!everythingSpawned) {
				bool es = true;
				for (int i = 0; i < players.Count; i++) {
					if (players [i]) {
						if (players [i].hasSword && !players [i].myAttack) {
							es = false;
						}
					}
				}
				everythingSpawned = es;
				if (everythingSpawned) {
					//Camera.main.transform.position = camDest;
				}
			}
		}
	}

	public void doCamera() {
		cameraControl(false);
		/*
		float xPos = Mathf.Clamp(players[0].transform.position.x, 46, 59);
		float yPos = Mathf.Clamp(players[0].transform.position.y, 32, 46);
		*/
		if (Camera.main.transform.position != camDest) {
			if (Vector3.Distance(Camera.main.transform.position, camDest) > 2) {
//				Debug.LogError(Camera.main.transform.position + " == " + camDest + " size: " + Camera.main.orthographicSize);
			}
			Camera.main.transform.position = Vector3.SmoothDamp (Camera.main.transform.position, camDest, ref camVelocity, camSpeed);
		}
		/*
		if (Camera.main.orthographicSize != zAmnt) {
			float size = Camera.main.orthographicSize;
			float dir = Mathf.Sign(zAmnt - size);
			Camera.main.orthographicSize = Mathf.Clamp(size + (dir * camZoomSpeed), Mathf.Min(size, zAmnt), Mathf.Max(size, zAmnt));
		}
		*/
	}

	void LateUpdate() {
		if (!fixedCamera) {
			doCamera();
			//Camera.main.transform.position = Vector3.SmoothDamp (Camera.main.transform.position, camDest, ref camVelocity, camSpeed);
		} else {
			//Debug.Log(Camera.main.transform.position);
			//Camera.main.transform.position = new Vector3(Mathf.Clamp(Camera.main.transform.position.x, 46, 59), Camera.main.transform.position.y, Camera.main.transform.position.z);
		}
		//cameraControl();
	}

	public int zoomOut = 20;
	public int bottomBorder;
	public bool UiOnLeft;
	public float zAmnt;
	public int[] zoomDistances;
	public float distanceFactor = 0.33f;

	public void cameraControl(bool snap) {
		if (players.Count > 0) {
			//Debug.Log("doing camera");
			Vector2 camPos = new Vector2 (0, 0);//Camera.main.transform.position;
			float highDist = Mathf.NegativeInfinity;
			int activePlayers = 0;
			float yDist = 0;
			for (int i = 0; i < players.Count; i++) {
				if (players[i]) {
					if (!players[i].dead) {
						Vector2 pos;
						if (players [i].myAttack) {
							pos = players [i].myAttack.getBladePlayerCenter ();//transform.position.x;
						} else {
							pos = players[i].getPos();
						}
					
						//Vector2 pos = new Vector2(players[i].transform.position.x, players[i].transform.position.y);
						camPos += pos;
						// go thru other players and compare the ydistance, get the greatest one
						for (int j = 1; j < players.Count; j++) {
							int c = (i + j) % players.Count;
							if (players[c]) {
								Vector2 p2;
								if (players [c].myAttack) {
									p2 = players [c].myAttack.getBladePlayerCenter ();//transform.position.x;
								} else {
									p2 = players[i].getPos();
								}
								yDist = Mathf.Max(yDist, Mathf.Abs(pos.y - p2.y));
							}
						}

						for (int j = 1; j < players.Count; j++) {
							Player check = players [(i + j) % players.Count];
							if (check) {
								float dist = Vector2.Distance (players [i].getPos (), check.getPos ()) * distanceFactor;
								if (dist > highDist) {
									highDist = dist;
								}
							}
						}
						activePlayers++;
					} else if (players[i].getDeaths() < deathCount && players.Count  > 1) {
						// player is respawning
						highDist = Mathf.Max (Map.S.worldSizeX, Map.S.worldSizeY);
						//camPos += new Vector2(Map.S.worldSizeX/2, Map.S.worldSizeY/2);
					}
				}
			}
			if (activePlayers != 0) {
				camPos.x /= activePlayers;
				camPos.y /= activePlayers;
			} else {
				camPos = new Vector2(Map.S.worldSizeX/2, Map.S.worldSizeY/2);
			}
			if (highDist != Mathf.NegativeInfinity) {
				zAmnt = Mathf.Clamp (highDist + zoomOut, camMinDist, camMaxDist);//camMaxDist/2);
			} else {
				zAmnt = camMinDist + (camMaxDist - camMinDist)/2;
			}
			if (snap) {
				Camera.main.orthographicSize = zAmnt;
			} else if (Camera.main.orthographicSize != zAmnt) {
				float size = Camera.main.orthographicSize;
				float dir = Mathf.Sign(zAmnt - size);
				Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, zAmnt, camZoomSpeed);// Mathf.Clamp(size + (dir * camZoomSpeed), Mathf.Min(size, zAmnt), Mathf.Max(size, zAmnt));
			}
			// when we have a high y distance we want to add the bottom border, to prevent the health icons from covering players
			float buffX = Camera.main.orthographicSize;// * Camera.main.aspect;
			if (Camera.main.orthographicSize < camMaxDist / Camera.main.aspect) {
				buffX *= Camera.main.aspect;
			} else {
				buffX -= 1;
			}
			//bb = Mathf.Lerp(0, bottomBorder, 1 - (Camera.main.orthographicSize-camMinDist) / (camMaxDist));
			float bb = Mathf.Lerp(0, bottomBorder, 1 - (camPos.x / camMaxDist));//(Map.S.worldSizeX-20)));//(Camera.main.orthographicSize-camMinDist) / (camMaxDist));
			float xPos = Mathf.Clamp(camPos.x, buffX, Map.S.worldSizeX - buffX) - 0.5f;
			if (Camera.main.orthographicSize >= camMaxDist / Camera.main.aspect) {
				xPos = Map.S.worldSizeX / 2 - 0.5f;
			}
			float buffY = Camera.main.orthographicSize;
            float yPos = Mathf.Clamp(camPos.y, buffY, Map.S.worldSizeY - buffY) - 0.5f;
            //Debug.Log(camPos.y + " pos: "+ yPos + " Clam(" + buffY + " - " + (Map.S.worldSizeY - buffY) + ")");
			camDest = new Vector3 (xPos - bb, yPos, -1);//zAmnt);
			if (snap) {
				Camera.main.transform.position = camDest;
			}
		}
	}
		
	bool spawned;

	public IEnumerator startGame(){
		//Debug.Log ("starting game");
		yield return new WaitForSeconds (0.5f);
		//if (isServer) {
		/*
			spawned = offline;//false;
			while (!spawned) {
				bool spawnPlayers = true;
				for (int i = 0; i < connections.Count; i++) {
					if (!connections [i].ready) {
						spawnPlayers = false;
					}
				}
				if (spawnPlayers) {

					for (int i = 0; i < connections.Count; i++) {
						Vector2Int spawnPoint = Map.S.getSpawn ();
						connections [i].CmdMapSpawn (spawnPoint.x, spawnPoint.y);
					}
					spawned = true;
				}
				yield return new WaitForFixedUpdate ();
			}
*/
			StartCoroutine (countDown ());
		//}
	}

	IEnumerator countDown(){
		//Debug.Log ("countdown");
		if (doSplash) {
			GameObject s = Instantiate(splashScreen);
			yield return new WaitForSecondsRealtime(5);
			if (boomBox.S) {
				boomBox.S.nextPress(-1);
			}
			Destroy(s);
		}
		Map.S.startWorld();
		if (pausedStart) {
			for (int i = 0; i < players.Count; i++) {
				if (players [i]) {
					players [i].dead = true;
				}
			}

			float pause = 1f;
			int duration = 20;
			if (boomBox.S) {
				//boomBox.S.yesPress (-1);
			}
			yield return new WaitForSecondsRealtime (pause/2);
			if (boomBox.S) {
				boomBox.S.yesPress (0);
			}
			if (MessageMaker.S) {
				MessageMaker.S.make ("READY", duration);
			}
			yield return new WaitForSecondsRealtime (pause);
			if (boomBox.S) {
				boomBox.S.yesPress (1);
			}
			if (MessageMaker.S) {
				MessageMaker.S.make ("SET", duration);
			}
			yield return new WaitForSecondsRealtime (pause);
			if (boomBox.S) {
				boomBox.S.yesPress (-1);
			}
			if (MessageMaker.S) {
				MessageMaker.S.make ("FIGHT!", duration * 2);
			}
		}
		
		for (int i = 0; i < players.Count; i++) {
			if (players [i]) {
				players [i].dead = false;
			}
		}
		if (!musicStarted) {
			if (boomBox.S) {
				if (!curDM) {
					boomBox.S.startSwordSongs(true);
				}
			}
			musicStarted = true;
		}
	}

	bool musicStarted = false;
	
	public void spawnPlayer(GameObject a, int xPos, int yPos) {
		Form f = a.GetComponent<Form> ();
		f.squareBody ();
		Map.S.spawnForm(a, xPos, yPos);
		if(!f.spawned){
			Debug.Log ("form no good " + xPos + players.Count);
		}
		//Player p = a.GetComponent<Player> ();
		//p.getNum(players.Count);
		//players.Add(p);
	}

	// for dungeon mode, spawning enemy players, maybe gonna break something?
	public void addPlayer(Player p) {
		players.Add(p);
		numPlayers++;
	}

	public void removePlayer(Player p) {
		players.Remove(p);
		numPlayers--;
	}

	public void spawnObj(GameObject o, int xPos, int yPos) {
		Form f = o.GetComponent<Form> ();
		f.squareBody ();
		Map.S.spawnForm(o, xPos, yPos);
	}

	public void spawnCircObj(GameObject o, int xPos, int yPos) {
		Form f = o.GetComponent<Form> ();
		f.circleBody (xPos, yPos);
		Map.S.spawnForm(o, xPos, yPos);
	}

	public void pausePlayers(bool pause, bool startMap) {
		if (startMap) {
			for (int i = 0; i < players.Count; i++) {
				if (players [i]) {
					Controller tmp = players [i].GetComponent<Controller> ();
					tmp.enabled = !pause;
				}
			}
		}
		if (pause) {
			//tmp.stopMove ();
			if (curDM) {
				if (curDM.storybo.dia.isWriting()) {
					curDM.storybo.dia.gamePaused = true;
				}
			}
			StartCoroutine (Map.S.pauseWorld ());
			Time.timeScale = 0;
		} else {
			if (curDM) {
				if (curDM.storybo.dia) {
					if (curDM.storybo.dia.isWriting()) {
						//Debug.Log("unpasuing writing");
                        curDM.storybo.dia.gamePaused = false;
						if (!curDM.storybo.dia.pauser()) {
							//Debug.Log("but not resetting time scale");
							Time.timeScale = 1;
						}
					} else {
						Time.timeScale = 1;
					}
                }
            } else {
				Time.timeScale = 1;
			}
			Debug.Log("starting map " + startMap);
			if (startMap) {
				Map.S.startWorld ();
			}
			
		}
	}

	public void onlyPausePlayers(bool pause) {
		for (int i = 0; i < players.Count; i++) {
			if (players [i]) {
				Controller tmp = players [i].GetComponent<Controller> ();
				tmp.enabled = !pause;
			}
		}
	}

	public bool arePlayersPaused() {
		bool allPaused = true;
		for (int i = 0; i < players.Count; i++) {
			if (players[i]) {
				Controller tmp =players[i].GetComponent<Controller>();
                if (tmp.enabled) {
					allPaused = false;
				}
            }
        }
		return allPaused;
	}
	public bool gameOver = false;
	int winner = -1;

	public int getLives(Player p) {
		return deathCount - p.getDeaths();
	}

	public float respawnTime = 3;
	
	public IEnumerator playerDeath (int pNum) {
		Debug.Log ("poo die " + pNum + " rounds: " + rounds);
		if (curDM) {
			bool allDead = true;
			for (int i = 0; i < players.Count; i++) {
				if (players[i].GetComponent<BossEnemy>() == null) {
					if (!players[i].dead) {
						allDead = false;
					}
				}
			}
			yield return new WaitForSeconds(respawnTime);
			if (allDead) {
				curDM.respawning = true;
				destroyWorld(-1);
			}
		} else if (pNum >= 0 && pNum < players.Count) {
		//	pdPatch.SendBang("p" + pNum + "Die");
			if (players [pNum].getDeaths () < deathCount || pNum == winner) {
				yield return new WaitForSeconds (respawnTime);
				if (!curDM) {
					Map.S.checkSpawns ();	
					players [pNum].respawn (Map.S.getSpawn (), true, true); 
				} else {

				}
			} else if (!gameOver) {
				int aliveCount = 0;
				int winTeam = -1;
				int winner = -1;
				//	winner = (pNum + 1) % 2;
				if (rounds == 0) {
					if (players [pNum].getTeam () == -1) {
						for (int i = 0; i < numPlayers; i++) {
							if (i != pNum && players [i]) {
								if (!players [i].dead || players [i].getDeaths () < deathCount) {
									winner = i;
									aliveCount++;
								}
							}
						}
						if (winner != -1 && aliveCount < 2) {
							gameOver = true;
						}
					} else {
						int deadTeam = players [pNum].getTeam ();
						bool[] liveTeams = new bool[] {false, false, false, false };
						gameOver = true;
						bool onlyPlayerDead = !GameInfo.S.multiPlayer && pNum == 0;
						for (int i = 0; i < numPlayers; i++) {
							if (i != pNum && players [i]) {
								if (!players [i].dead || players [i].getDeaths () < deathCount) {
									int t = players[i].getTeam();
									liveTeams[t] = true;
									if (onlyPlayerDead) {
										if (t == deadTeam) {
											onlyPlayerDead = false;
										}
									}
								}

							}
						}
						for (int i = 0; i < 4; i++) {
							if (liveTeams[i]) {
								if (winTeam == -1) {
									winTeam = i;
								} else {
									winTeam = -1;
									break;
								}
							}
						}
						gameOver = winTeam != -1;
						if (onlyPlayerDead) {
							gameOver = true;
							winTeam = 1;
						}
					}
				} else {
					InputComputer ai = players [winner].gameObject.GetComponent<InputComputer> ();
					if (!ai) { //human winner
						if (players [pNum]) {
							ai = players [pNum].gameObject.GetComponent<InputComputer> ();
							if (ai) {
								MessageMaker.S.make ("LEVEL " + ai.difficultyLevel + " COMPLETE", 200);
								ai.raiseDifficulty (1);
								yield return new WaitForSeconds (0.5f);
							} 
						}
					} else {
						MessageMaker.S.make ("YOU LOSE!", 200);
						yield return new WaitForSeconds (1f);
					}

					letsRestart (false);
				}
				yield return new WaitForSeconds (1);
				if (gameOver) {

					if (GameInfo.S.multiPlayer) {
						if (winTeam == -1) {
							if (boomBox.S) {
								boomBox.S.weHaveWinner (winner);
							}
							MessageMaker.S.make ("PLAYER " + winner + " WINS", 200);
						} else {
							if (boomBox.S) {
								boomBox.S.weHaveWinner (winner);
							}
							MessageMaker.S.make ("TEAM " + winTeam + " WINS", 200);
						}
					} else {
						bool playerWins = false;
						if (winTeam == -1) {
							playerWins = winner == 0;//player is always 0
						} else {
							playerWins = winTeam == 0;//player is always team 0
						}

						if (playerWins) {
							//human player wins
							MessageMaker.S.make ("LEVEL " + GameInfo.S.enemyLevel + " COMPLETE", 200);
							yield return new WaitForSeconds (4f);
							if (GameInfo.S.enemyLevel < GameInfo.S.maxEnemyLevel) {
								GameInfo.S.enemyLevel++;
								players [0].resetDeaths ();
								SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
								if (boomBox.S) {
									boomBox.S.beat(0);
									boomBox.S.setFlicker(0, 0);
									boomBox.S.setFlicker(1, 0);
								}
							} else {
								GameInfo.S.enemyLevel = 0;
								MessageMaker.S.make ("YOU'RE A WINNER!", 200);
								yield return new WaitForSeconds (3f);
								returnToMenu();
							}
						} else {
							GameInfo.S.enemyLevel = 0;
							MessageMaker.S.make ("YOU LOSE!", 200);
							yield return new WaitForSeconds (3f);
							returnToMenu();
						}
					}
				}
			}
		}
		yield return new WaitForEndOfFrame();
	}
	
	public void returnToMenu() {
		if (boomBox.S) {
			Destroy (boomBox.S.gameObject);
		}
		GameInfo.S.destroySelf();
		SceneManager.LoadScene ("menu");
	}

	public void letsRestart(bool flow) {
		Map.S.flowing = flow;
		StartCoroutine(restart ());
	}

	public IEnumerator restart() {
		// Adventure mode
		if (curDM) {
			curDM.respawning = true;
			destroyWorld(-1);
			yield return new WaitForEndOfFrame();
		} else if (GameInfo.S.multiPlayer) {
			for (int i = 0; i < players.Count; i++) {
				if (players [i]) {
					StartCoroutine (players [i].die (true, false));
					players [i].resetDeaths ();
				}
			}
			winner = -1;
			yield return new WaitForSeconds (3);
			if (gameOver && boomBox.S) {
				boomBox.S.restartGame ();
			}
			gameOver = false;
			Map.S.checkSpawns ();

			for (int i = 0; i < players.Count; i++) {
				if (players [i]) {
					players [i].respawn (Map.S.getSpawn (), false, true);
				}
			}
			StartCoroutine (countDown ());
		} else {
			GameInfo.S.enemyLevel = 0;
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
			yield return new WaitForSeconds (3);
		}
	}

	public void destroyWorld(int dir) {
		for(int i = 0; i < players.Count;i ++) {
			players[i].removeBody(false);
		}
		Map.S.newDungeonRoom(dir);
		//yield return new WaitForSeconds(0.2f);
		Vector3Int spawnPoint = Map.S.getSpawn((dir+2)%4);
		for (int i = 0; i < numPlayers; i++) {
			if (players[i].GetComponent<BossEnemy>() == null) {
				if (players [i]) {
					//Vector3Int spawnPoint = Map.S.getSpawn ();
					//players [i].setSpawnInfo (spawnPoint, i, cols [i], GameInfo.S.playerColors [GameInfo.S.teamNums[i]], teams [i]);
					players[i].setSwordStart(spawnPoint.z);
					players[i].respawn(spawnPoint, true, dir==-1);
					players[i].setTeam(0);
				}
			}
		}
		cameraControl(true);
		if (Map.S.flowing) {
			Map.S.burstPause(0.1f);
		}
		//StartCoroutine (countDown ());
		//yield return new WaitForSeconds(0.2f);//Map.S.endWorld();
	}

        public int getClosestDir(Vector2Int pos, Vector2Int dest) {
		float closest = Mathf.Infinity;
		int chosenD = -1;
		for (int i = 0; i < dirs.Length; i++) {
			float dist = Vector2Int.Distance (pos + dirs [i], dest);
			if (dist < closest) {
				closest = dist;
				chosenD = i;
			}
		}
		return chosenD;
	}

	public Vector2Int getClosestVec(Vector2Int pos, Vector2Int dest) {
		float closest = Mathf.Infinity;
		Vector2Int chosenD = new Vector2Int(0,0);
		for (int i = 0; i < dirs.Length; i++) {
			float dist = Vector2Int.Distance (pos + dirs [i], dest);
			if (dist < closest) {
				closest = dist;
				chosenD = dirs[i];
			}
		}
		return chosenD;
	}

	public int convertVectorToDir(Vector2 d) {
		return convertVectorToDir(new Vector2Int((int)d.x, (int)d.y));
	}

	public int convertVectorToDir(Vector2Int dir) {
		if (dir.y > 0) {
			if (dir.x > 0) {
				return 7;
			} else if (dir.x < 0) {
				return 1;
			} else {
				return 0;
			}
		} else if (dir.y < 0) {
			if (dir.x > 0) {
				return 5;
			} else if (dir.x < 0) {
				return 3;
			} else {
				return 4;
			}
		} else {
			if (dir.x > 0) {
				return 6;
			} else if (dir.x < 0) {
				return 2;
			}
		}
		return -1;
	}

	public int getNumPlayers() {
		return numPlayers;
	}

	public void setNumPlayers(int np) {
		numPlayers = np;
	}
}
