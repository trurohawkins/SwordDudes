using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public GameObject title;
	public GameObject pp;
	public GameObject charSelect;
	public GameObject mapScreen;
	public GameObject charFunctionsL;
	public GameObject charFunctionsR;
	public Text[] controllerText;

	public Text rt;
	public Text bt;
	public GameObject start;
	public GameObject quit;
	public GameObject startMenu;
	public GameObject selector;
	GridSelector player1;
	GridSelector player2;
	public static MainMenu S;
	public GameObject token;
	int numPlayers = 2;
	GridSelector[] playerControllers;
	int numActivePlayers = 0;
	bool[] activeControlSchemes;
	string []controlTypes;
	charToken[] tokens;
	charFunction[] characterScreens;
	public AILevelBodyUI[] aiLevelButtons;
	List<GridSelector> allControllers;
	List<GameObject> controllerCursors;
	//public startGameBodyUI playButton;
	GameObject prevMenu;
	StartMenu sMenu;
	CharSelectMenu charMenu;

	void Awake () {
		if (!S) {
			S = this;
		} else {
			Destroy (gameObject);
		}
		controlTypes = new string[4];
		controlTypes [0] = "AI";
		controlTypes [1] = "controller";
		controlTypes [2] = "keys";
		controlTypes [3] = "---";
		characterScreens = new charFunction[4];
		controllerText = new Text[4];
		aiLevelButtons = new AILevelBodyUI[8];
		playerTexts = new Text[4];
		tokens = new charToken[4];
		allControllers = new List<GridSelector> ();
		controllerCursors = new List<GameObject>();
		sMenu = startMenu.GetComponent<StartMenu>();
	}


	void Start() {
		if (!GameInfo.S) {
			Debug.LogError ("need game info object");
			numPlayers = 2;
		} else {
			numPlayers = GameInfo.S.numPlayers;
		}
		//playButton.setShowing (false);
		Debug.Log ("start");
		playerControllers = new GridSelector[4];
		activeControlSchemes = new bool[8];//0 - numPlayers: Gamepad, numPlayers - numPlayers  * 2: keyboards
		StartCoroutine ("waitingToPlay");
		if (boomBox.S) {
			boomBox.S.themeMusic (true);
		} else {
			Debug.LogError ("need music");
		}
		/*
		for (int i = 0; i < aiLevelButtons.Length; i++) {
			aiLevelButtons [i].setShowing (false);
		}
		*/
	}

	int backTime = 100;
	int backCounter = -1;
	bool quittingTime = false;

	void Update () {
		if (bPressed()) {//Input.GetKey ("escape") || Input.GetButton ("bButton0") || Input.GetButton ("bButton1")) {
			if (backCounter < backTime) {
				backCounter++;
			} else {
				quittingTime = true;
			}
		} else {
			backCounter = 0;
			quittingTime = false;
		}
		if (quittingTime) {
			quittingTime = false;
			if (boomBox.S) {
				boomBox.S.noPress (-1);
			}
			if (!onTitle && !restarting) {
				Debug.Log ("leaving char select");
				StartCoroutine (restart ());
			} else {
				//StartCoroutine (quitGame ());
			}
		}
		if (starting) {
			if (GameInfo.S.allPlayersChosen()) {
				if (!charMenu.playerPressStart()) {
					if (timer > flashSpeed / (mul + 1)) {
						if (flash == 0) {
							start.SetActive (false);
						} else {
							start.SetActive (true);
						}
						timer = 0;
						flash = (flash + 1) % 2;
						mul = (mul + 1) % 2;
					} else {
						timer++;
					}
				} else {
					StartCoroutine(actuallyPlay());
				}
			} else {
				starting = false;
				start.SetActive (false);
			}
		}
	}

	IEnumerator actuallyPlay() {
		if (boomBox.S) {
			boomBox.S.nextPress (-1);
		}
		yield return new WaitForSeconds (0.1f);
		SceneManager.LoadScene ("arena");
	}
	float flash = 1;
	int timer = 0;

	IEnumerator startingGame() {
		flash = 1;
		start.SetActive (true);
		timer = 0;
		starting = true;
		yield return new WaitForSeconds (0.5f);
		/*
		while (!charMenu.playerPressStart() && GameInfo.S.allPlayersChosen() && starting) {
			if (timer > flashSpeed / (mul + 1)) {
				if (flash == 0) {
					start.SetActive (false);
				} else {
					start.SetActive (true);
				}
				timer = 0;
				flash = (flash + 1) % 2;
				mul = (mul + 1) % 2;
			} else {
				timer++;
			}
			yield return new WaitForEndOfFrame();//WaitForFixedUpdate ();
		}
		if (GameInfo.S.allPlayersChosen () && starting) {
			if (boomBox.S) {
				boomBox.S.nextPress (-1);
			}
			yield return new WaitForSeconds (0.5f);
			SceneManager.LoadScene ("arena");
		} else {
			starting = false;
			start.SetActive (false);
		}
		*/
	}

	bool bPressed() {
		return false;//Input.GetKey ("escape") || Input.GetButton("bButton0") || Input.GetButton("bButton1") || Input.GetButton("bButton2") || Input.GetButton("bButton3");
	}

	public int flashSpeed = 20;
	int mul = 0;
	bool onTitle;

	IEnumerator waitingToPlay() {
		float flash = 0;
		int timer = 0;
		onTitle = true;
		bool quitting = false;
		while (true) {
			if (bPressed()) {//Input.GetKeyDown ("escape") || Input.GetButtonDown("bButton0") || Input.GetButtonDown("bButton1")) {
				StartCoroutine (quitGame ());
				quitting = true;
				break;
			} else if (Input.anyKey && !bPressed()) {//(Input.GetKey("escape") || Input.GetButton("bButton0") || Input.GetButton("bButton1"))) {
				break;

			} else {
				if (timer > flashSpeed / (mul + 1)) {
					/*
					Color tmp = pressPlay.color;
					tmp.a = flash;
					pressPlay.color = tmp;
					*/
					if (flash == 0) {
						pp.SetActive (false);
					} else {
						pp.SetActive (true);
					}
					timer = 0;
					flash = (flash + 1) % 2;
					mul = (mul + 1) % 2;
				} else {
					timer++;
				}
			}
			yield return new WaitForFixedUpdate ();
		}
		if (!quitting) {
			if (boomBox.S) {
				boomBox.S.yesPress (-1);
			}
			pp.gameObject.SetActive (false);
			onTitle = false;
			yield return new WaitForSeconds (0.2f);
			startMenu.SetActive (true);
		} else {
			quitting = false;
		}
	}

	int totalPlayers = -1;
	bool multiPlayer;

	public int getTotalPlayers() {
		return totalPlayers;
	}

	public void mapSelect() {
		title.SetActive(false);
		mapScreen.SetActive(true);
	}

	// int players determines isngle player or multiplayer,
	public void letsPlay(int maxPlayers) {
		title.SetActive (false);

		totalPlayers = maxPlayers;
		multiPlayer = totalPlayers != 1;
		/*
		for (int i = 0; i < multiplayerComponents.Length; i++) {
			multiplayerComponents [i].setShowing (multiPlayer);
		}
		levelText.transform.parent.gameObject.SetActive(multiPlayer);
		*/

		GameInfo.S.multiPlayer = multiPlayer;
		GameInfo.S.enemyLevel = 0;
	//	createCursors ();
		//createTokens();
		if (maxPlayers == 2) {
			GameInfo.S.level = 5;
			//change for cooop
			GameInfo.S.numPlayers = 0;
		}
		if (maxPlayers == 2 && GameInfo.S.progress == 0) {
			Debug.LogError("skipping chr select " + sMenu.controlPresser);
			if (sMenu.controlPresser < 4) {
				newPlayer(sMenu.controlPresser, 1, sMenu.controlPresser);
			} else {
				newPlayer(sMenu.controlPresser, 2, 0);
			}
			StartCoroutine(actuallyPlay());
		} else {
			//Debug.Log("max: " + maxPlayers + " " + totalPlayers);
			charSelect.SetActive (true);
			//only run if we are not on our first char scren
			// or if we were previously on the different screen mode
			if (totalPlayers != -1/* && (totalPlayers != maxPlayers)*/) {
				GameInfo.S.numPlayers = 0;
				for(int i = 0; i < GameInfo.S.songs.Length; i++) {
					GameInfo.S.songs[i] = -1;
				}
				destroyCharacterScreens ();
				destroyTokens ();
				destroyUiControllers ();
			}
			StartCoroutine ("choosingChars");
			StartCoroutine ("waitingForControllers");
		}
	}

	public UIBody[] multiplayerComponents;

	void createTokens() {
		float factor = 1f / (5);//numPlayers + 1);
		float startX = Screen.width * factor;
		for (int i = 0; i < totalPlayers; i++) {
			if (!tokens [i]) {
				Vector3 pos = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width * (0.65f), Screen.height * 0.2f, 10));
				tokens [i] = Instantiate (token, pos, transform.rotation, charSelect.transform).GetComponent<charToken> ();
				tokens [i].setPlayerNumber (i);
				if (playerControllers [i] && GameInfo.S.songs [i] == -1) {
					tokens [i].Action (playerControllers [i]);
					playerControllers [i].addButt (tokens [i]);
					//playerControllers[i].myButt = tokens [i];
				} else {
					if (i > numPlayers) {
						tokens [i].setShowing (false);//gameObject.SetActive(false);
					}
				}
			}
		}
	}

	void destroyTokens() {
		for (int i = 0; i < 4; i++) {
			if (tokens [i]) {
				tokens [i].removeMasterAndDestroy ();
				tokens [i] = null;
			}
		}
	}

	public void dropToken(int pNum) {
		tokens [pNum].Action (null);
		tokens [pNum].Action (playerControllers [pNum]);
		playerControllers [pNum].addButt (tokens [pNum]);
		//playerControllers [pNum].myButt = tokens [pNum];
	}

	void createUiController(int controlType, int controlNum) {
		if (boomBox.S) {
			boomBox.S.yesPress (controlNum);
		}
		Debug.Log("creating ui controller");
		GameObject tmp = Instantiate (selector, new Vector3 (0, 0, 10), transform.rotation, charSelect.transform);
		GridSelector gs = tmp.GetComponent<GridSelector> ();
		allControllers.Add (gs);
		gs.controlType = controlType;
		gs.controlNum = controlNum;
		gs.setColor(GameInfo.S.playerColors[allControllers.Count-1]);
		Transform image = tmp.transform.GetChild (0);
		image.SetParent(transform);
		controllerCursors.Add(image.gameObject);
		//Deubg.Log ("create ui controller" + numActivePlayers + " < " + numPlayers);
		if (numActivePlayers < totalPlayers) {
			/*
			playerControllers [numActivePlayers] = gs;
			gs.playerNum = numActivePlayers;
			controllerText[numActivePlayers].text = controlTypes[controlType];
			Color c = controllerText[numActivePlayers].color;
			c.a = 1;
			controllerText[numActivePlayers].color = c;
			GameInfo.S.controls [numActivePlayers] = controlType;
			GameInfo.S.controlNums [numActivePlayers] = controlNum;
			*/
			charToken ct = tokens[numActivePlayers];
			addCharacterScreen ();
			setPlayer (numActivePlayers-1, gs);
			//pick up char token
			ct.Action (gs);
			gs.addButt (tokens [numActivePlayers - 1]);
			//gs.myButt = tokens [numActivePlayers-1];
		}
	}

	void newPlayer(int scheme, int controlType, int controlNum) {
		Debug.Log("new player " + controlType + " " + controlNum);
		int num = numActivePlayers;
		if (num < totalPlayers) {
			if (charMenu) {
				charMenu.setBlocked(scheme, false);
				charFunction screen = addCharacterScreen();
				screen.type = controlType;
				screen.num = controlNum;
				screen.scheme = scheme;
				charMenu.setPlayer(scheme, num);
			} else {
				// specifically for skippign char screen for saga atm
				GameInfo.S.controlNums[GameInfo.S.numPlayers] = controlNum;
				GameInfo.S.controls[GameInfo.S.numPlayers] = controlType;
				GameInfo.S.numPlayers++;
			}
			//bool gotHuman = checkHumanPlayer(scheme, controlType, controlNum);
			/*
			if (!gotHuman) {
				GameInfo.S.controls[num] = controlType;
				GameInfo.S.controlNums[num] = controlNum;
			} else {
				GameInfo.S.controls[num] = -3;
				GameInfo.S.controlNums[num] = -1;
			}
			*/

			/*
			controllerText[num].text = controlTypes[controlType];
			if (characterScreens[num]) {
				characterScreens [num].setAI (false);
			}
			*/
		}
	}

	// maybe dont need scheme
	public bool checkHumanPlayer(int scheme, int controlType, int controlNum) {
		for (int i = 0; i < 4; i++) {
			if (GameInfo.S.controlNums[i] == controlNum && GameInfo.S.controls[i] == controlType) {
				return true;
			}
		}
		return false;
	}

	void destroyUiControllers() {
		for (int i = 0; i < 4; i++) {
			playerControllers[i] = null;
			activeControlSchemes [i] = false;
			if (controllerCursors.Count > i) {
				Destroy(controllerCursors[i]);
			}
		}
		for (int i = 0; i < allControllers.Count; i++) {
			allControllers [i].destroySelf ();
		}
		allControllers.Clear ();
		controllerCursors.Clear();
	}
		
	public charFunction addCharacterScreen () {
		int i = numActivePlayers;
		GameInfo.S.numPlayers++;
		numActivePlayers++;
		GameObject screen = charFunctionsR;
		if (i % 2 == 0) {
			screen = charFunctionsL;
		}
		GameObject tmp = Instantiate (screen, charSelect.transform.position, transform.rotation, charSelect.transform);
		characterScreens [i] = tmp.GetComponent<charFunction> ();
		characterScreens [i].setPlayerNum (i);
		controllerText [i] = characterScreens [i].controllerText;
		aiLevelButtons [i * 2] = characterScreens [i].aiLevel [0];
		aiLevelButtons [(i * 2) + 1] = characterScreens [i].aiLevel [1];
		playerTexts [i] = characterScreens [i].playerText.GetComponent<Text> ();
		sizeCharScreens();
		//tokens[numActivePlayers-1].myScreen = characterScreens[numActivePlayers-1];
		return characterScreens[numActivePlayers-1];
	}

	public void destroyCharScreen(int pNum, int scheme) {
		if (characterScreens [pNum]) {
			Debug.Log ("destroying char screen " + pNum);
			charFunction screen = characterScreens[pNum];
			screen.removeSelf ();
			for (int i = pNum; i < characterScreens.Length - 1; i++) {
				characterScreens[i] = characterScreens[i+1];
				if (characterScreens[i]) {
					characterScreens[i].unChoose();
					characterScreens[i].pNum = i;
					characterScreens[i].curButton.activate(true, characterScreens[i].pNum);
				}
				GameInfo.S.songs[i] = GameInfo.S.songs[i+1];
				GameInfo.S.controls[i] = GameInfo.S.controls[i+1];
				GameInfo.S.colors[i] = GameInfo.S.colors[i+1];
				GameInfo.S.controlNums[i] = GameInfo.S.controlNums[i+1];
				GameInfo.S.teamNums[i] = GameInfo.S.teamNums[i+1];
			}
			characterScreens [numActivePlayers-1] = null;
			numActivePlayers--;
			GameInfo.S.numPlayers--;
			if (scheme >= 0) {
				activeControlSchemes[scheme] = false;
			}
			/*
			for (int i = 0; i < characterScreens.Length; i++) {
				if (characterScreens[i]) {
					continue;
				} else if (i + 1 < characterScreens.Length) {
					characterScreens[i] = characterScreens[i+1];
					GameInfo.S.songs[i] = GameInfo.S.songs[i+1];
					GameInfo.S.controls[i] = GameInfo.S.controls[i+1];
					GameInfo.S.colors[i] = GameInfo.S.colors[i+1];
					GameInfo.S.controlNums[i] = GameInfo.S.controlNums[i+1];
					GameInfo.S.teamNums[i] = GameInfo.S.teamNums[i+1];
				}
			}
			*/
			sizeCharScreens();
		}
    }

    void sizeCharScreens() {
		float scaleFactor = 1;
		float yPos = 10;
		if (numActivePlayers > 2) {
			scaleFactor = 0.6f;
			yPos = 18;
		}
		GameObject screen = null;
		float xVal= 17.5f;
		float xp;

		for (int i = 0; i <= numActivePlayers && i < totalPlayers; i++) {
			if (i < numActivePlayers) {
				if (i % 2 == 0) {
					screen = charFunctionsL;
					xp = -xVal;
				} else {
					screen = charFunctionsR;
					xp = xVal;
				}
				if (i > 1) {
					yPos = 2;
				}
				if (!characterScreens [i]) {

				} else {
					characterScreens [i].setPos (new Vector3 (xp, yPos, 10));
					characterScreens [i].setScaleFactor (scaleFactor);
				}

			}
			if (tokens [i]) {
				tokens [i].setShowing (true);
			}
		}
		setTeams (teamsOn);
	}
	//called before single player mode is started
	// t oprevent old char screens from being there
	void destroyCharacterScreens() {
		for (int i = 0; i < 4; i++) {
			if (characterScreens [i]) {
				characterScreens [i].removeSelf ();
				characterScreens [i] = null;
			}
		}
		for (int i = 0; i < activeControlSchemes.Length; i++) {
			activeControlSchemes[i] = false;
		}
		numActivePlayers = 0;
	}

	public void charScreenColor(int pNum, int cNum) {
		//Debug.Log ("changing color of " + pNum + " to the color " + cNum);
		if (characterScreens [pNum]) {
			int song = GameInfo.S.songs[pNum];
			//characterScreens[pNum].getDescription(song);
			characterScreens[pNum].getColors(song, cNum);
			/*
			GameObject tmp = Instantiate (GameInfo.S.souls [GameInfo.S.songs[pNum]]);
			SwordSoul s = tmp.GetComponent<SwordSoul> ();

			characterScreens [pNum].setText (s.description);
			Destroy (tmp);
			*/
		}
	}

	// -1 to turn off graphics
	public void setPlayerGraphics(int pNum, int song) {
		if (characterScreens[pNum]) {
			characterScreens[pNum].glanceChar(song, pNum);
		}
	}

	public void giveButton(int pNum, selectButton butt) {
		if (characterScreens[pNum]) {
			characterScreens[pNum].curButton = butt;
			setPlayerGraphics(pNum, butt.soulNumber);
		}
	}

	public charFunction getCharFunction(int playerNum) {
		return characterScreens[playerNum];
	}

	//char token checks when grabbed so we know if we should make a new char screen for AI
	public bool checkCharacterScreen(int num) {
		if (num > -1 && num < 5) {
			return characterScreens [num] != null;
		} else {
			return true;
		}
	}

	public void setAI(int num) {
		if (totalPlayers != 1) {
			Debug.Log("setting AI " + num);
			if (GameInfo.S.controls[num] >= 0) {
				GameInfo.S.controls [num] = -3;
			}
			controllerText[num].text = controlTypes[0];
			if (playerControllers [num] != null) {
				playerControllers [num].playerNum = -1;
				playerControllers [num] = null;
			}
			if (characterScreens[num]) {
				characterScreens [num].setAI (true);
			}
			/*
			num *= 2;
			if (aiLevelButtons [num]) {
				aiLevelButtons [num].setShowing (true);
			}
			if (aiLevelButtons [num+1]) {
				aiLevelButtons [num+1].setShowing (true);
			}
		*/
		}
	}

	public void setPlayer(int num, GridSelector gs) {
		if (playerControllers [num] != null) {
			playerControllers [num].playerNum = -1;
			playerControllers [num] = null;
		}
		playerControllers [num] = gs;
		gs.playerNum = num;
		GameInfo.S.controls [num] = gs.controlType;
		GameInfo.S.controlNums [num] = gs.controlNum;

		controllerText[num].text = controlTypes[gs.controlType];
		if (characterScreens[num]) {
			characterScreens [num].setAI (false);
		}
		/*
		num *= 2;
		if (aiLevelButtons [num]) {
			aiLevelButtons [num].setShowing (false);
		}
		if (aiLevelButtons [num+1]) {
			aiLevelButtons [num+1].setShowing (false);
		}
		*/
		/*
		Color c = controllerText[num].color;
		c.a = 1;
		controllerText[num].color = c;
		*/
		//gs.controlNum = controlNum;
	}

	public void setCharFuntiontoAI(int num, int controller) {
		Debug.Log("setting char screen " + num + " to " + controller);
		if (num >= 0 && num < characterScreens.Length) {
			characterScreens[num].scheme  = controller;
		}
	}

	public bool destroyLastScreen(int controller) {
		for (int i = characterScreens.Length - 1; i >= 0; i--) {
			if (characterScreens[i]) {
				if (characterScreens[i].scheme  == controller) {
					destroyCharScreen(i, -1);
					return true;
				}
			}
		}
		return false;
	}

	public void setNull(int num) {
		if (totalPlayers != 1) {
			GameInfo.S.controls [num] = 0;
			controllerText [num].text = controlTypes [3];
			if (playerControllers [num] != null) {
				playerControllers [num].playerNum = -1;
				playerControllers [num] = null;
			}
			if (characterScreens [num]) {
				characterScreens [num].setAI (false);
			}
			/*
			int np = numActivePlayers;
			destroyCharacterScreens ();
			numActivePlayers = np - 2;//add is going to add character
			addCharacterScreen ();
			*/
		}
	}

	public GridSelector getPlayer(int num) {
		return playerControllers [num];
	}

	void createCursors() {
		if (player1 == null) {
			GameObject tmp = Instantiate (selector, new Vector3 (0, 0, 10), transform.rotation, charSelect.transform);
			player1 = tmp.GetComponent<GridSelector> ();
			Transform image = tmp.transform.GetChild (0);
			image.SetParent(transform);
			image.GetComponent<Image> ().color = Color.red;
		}
		if (player2 == null) {
			GameObject tmp = Instantiate (selector, new Vector3 (0, 0, 10), transform.rotation, charSelect.transform);
			player2 = tmp.GetComponent<GridSelector> ();
			Transform image = tmp.transform.GetChild (0);
			image.SetParent(transform);		
			image.GetComponent<Image> ().color = Color.blue;
		}
	}
	int chosen = 0;

	IEnumerator choosingChars() {
		float flash = 0;
		int timer = 0;
		while (chosen < 2) {
			if (timer > flashSpeed / (mul + 1)) {
				/*
				if (GameInfo.S.p1Pad == 0) {
					Color tmp = rt.color;
					tmp.a = flash;
					rt.color = tmp;
				}
				if (GameInfo.S.p2Pad == 0) {
					Color tmp = bt.color;
					tmp.a = flash;
					bt.color = tmp;
				}
				*/
				timer = 0;
				flash = (flash + 1) % 2;
				mul = (mul + 1) % 2;
			} else {
				timer++;
			}
			yield return new WaitForFixedUpdate ();
		}
	}

	//int p1Pad = 0; // 1 - pad, 2 - keys
	//int p2Pad = 0; // 1- pad 2 - keys
	float minStickVal = 0.1f;
	IEnumerator waitingForControllers() {
		yield return new WaitForSeconds(0.1f);
		Vector3 mp = Input.mousePosition;
		while (true) {
			//if input
			//createUICOntroller
			for (int i = 0; i < 4; i++) {
				char num = (char)(i + 48);
				if (Mathf.Abs(Input.GetAxis ("LeftJoystickX" + num)) > minStickVal || Mathf.Abs(Input.GetAxis ("LeftJoystickY" + num)) > minStickVal) {
					checkAndAddScreen(i, 1, i);
				/*
					if (activeControlSchemes [i] == false) {
						Debug.Log("new controller player " + num);
						//createUiController (1, i);
						newPlayer(i, 1, i);
						activeControlSchemes [i] = true;
					} else {
						charFunction screen = getScreenOfController(i);
						if (screen) {
							Debug.Log("checking screen " + screen.pNum + " " + GameInfo.S.songs[screen.pNum]);
							if (GameInfo.S.songs[screen.pNum] != -1 && !charMenu.checkMenu(i)) {
								newPlayer(i, 1, i);
							}
						}
					}
					*/
				}
			} 
			//if (activeControlSchemes [4] == false) {
				//if (Input.GetKeyDown ("w") || Input.GetKeyDown ("a") || Input.GetKeyDown ("s") || Input.GetKeyDown ("d")) {
			if (GameInfo.S.getControl(0).getMove() || (charMenu.mouseInp && mp != Input.mousePosition)) {
				//newPlayer(4, 2, 0);
				//activeControlSchemes [4] = true;
				checkAndAddScreen(4, 2, 0);
			}
			//}
			if (activeControlSchemes [5] == false) {
				//if (Input.GetKeyDown ("i") || Input.GetKeyDown ("j") || Input.GetKeyDown ("k") || Input.GetKeyDown ("l")) {
				if (GameInfo.S.getControl(1).getMove()) {
					//createUiController (2, 1);
					newPlayer(5, 2, 1);
					activeControlSchemes [5] = true;
				}
			}
			mp = Input.mousePosition;
			//if (chosen >= 2 && starting == false) {
			if (starting == false && GameInfo.S.allPlayersChosen()) {
				//playButton.setShowing (true);
				StartCoroutine ("startingGame");
			}
			yield return new WaitForFixedUpdate ();
		}
	}

	void checkAndAddScreen(int scheme, int type, int num) {
		charFunction screen = getScreenOfController(scheme);
		bool screenReady = false;
		if (screen) {
			//Debug.Log("checking screen " + screen.pNum + " " + GameInfo.S.songs[screen.pNum]);
			if (GameInfo.S.songs[screen.pNum] != -1 && !charMenu.checkMenu(scheme)) {
				screenReady = true;
			}
		} else {
			screenReady = true;
		}
		if (screenReady) {
			newPlayer(scheme, type, num);
		}
		activeControlSchemes[scheme] = true;
	}

	bool starting = false;

	public void goBack() {
		Debug.Log("main menu go back");
		if (mapScreen && mapScreen.activeInHierarchy) {
			Debug.Log("map screen");
			mapScreen.SetActive(false);
			startMenu.SetActive(true);
			title.SetActive(true);
			return;
		}
		if (charMenu) {
			if (charMenu.gameObject.activeInHierarchy) {
				Debug.Log("back from char menu");
				if (multiPlayer) {
					mapScreen.SetActive(true);
				} else {
					startMenu.SetActive(true);
				}
				charMenu.gameObject.SetActive(false);
				starting = false;
				start.SetActive (false);
				StopCoroutine ("waitingForControllers");
				StopCoroutine("choosingCharacters");
			}
		}
    }

    bool restarting = false;

	public IEnumerator restart() {
		restarting = true;
		if (boomBox.S) {
			boomBox.S.noPress (-1);
		}
		/*
		for (int i = 0; i < 4; i++) {
			if (tokens [i]) {
				//playerControllers[i].holding = false;
				//playerControllers[i].myButt = null;
				tokens [i].removeMasterAndDestroy ();
				//Destroy(tokens[i].gameObject);
				tokens [i] = null;
			}
				
		}
		*/
		destroyCharacterScreens ();
		destroyTokens ();
		destroyUiControllers ();
		start.SetActive (false);
		starting = false;
		charSelect.SetActive (false);
		pp.gameObject.SetActive (true);
		title.SetActive (true);
		StopCoroutine("choosingChars");
		StopCoroutine ("waitingForControllers");
		StopCoroutine ("waitingToPlay");
		yield return new WaitForSeconds (0.5f);
		restarting = false;
		//startMenu.SetActive (true);
		StartCoroutine ("waitingToPlay");
	}

	IEnumerator quitGame() {
		quit.SetActive (true);
		pp.SetActive (false);
		/*
		Color tmp = pressPlay.color;
		tmp.a = 0;
		pressPlay.color = tmp;
		*/
		for (int i = 0; i < 200; i++) {
			if (i > 2) {
				if (bPressed()) {
					//Debug.Log ("quit");
					Application.Quit ();
				} else if (Input.anyKey && !bPressed()) {//(Input.GetKey("escape") || Input.GetButton("bButton0") || Input.GetButton("bButton1") || Input.GetButton("bButton2") || Input.GetButton("bButton3"))) {
					break;
				}
			}
			yield return new WaitForSeconds (0.1f);
		}
		quit.SetActive (false);
		yield return new WaitForSeconds (0.3f);
		/*
		tmp.a = 1;
		pressPlay.color = tmp;
		*/
		pp.SetActive (true);
		StartCoroutine ("waitingToPlay");
	}

	public Text[] playerTexts;
	public Text p1Text;
	public Text p2Text;
	public Text levelText;
	public void setSongText(string song, int textNum) {
		if (playerTexts [textNum]) {
			playerTexts [textNum].text = song;
		}
		if (characterScreens[textNum]) {
			if (song == "---") {
				characterScreens [textNum].graphics.setAlpha(0f);
			} else {
				characterScreens [textNum].graphics.setAlpha(1f);
			}
		}
	}

	public void setSong(bool on, int pNum) {
		if (characterScreens[pNum]) {
			if (on) {
				//characterScreens[pNum].graphics.spinSpeed = 50;
				//characterScreens[pNum].graphics.setAlpha(1f);
			} else {
				//characterScreens[pNum].graphics.spinSpeed = 0;
				//characterScreens[pNum].graphics.setAlpha(0.5f);
			}
		}
	}

	public void setAiText(int num, int level) {
		if (characterScreens [num]) {
			characterScreens [num].setAiLevel (level);
		}
	}

	public void setLevelText(string level) {
		levelText.text = level;
	}

	public void setMap(int lvl) {
		GameInfo.S.level = lvl;
		mapScreen.SetActive(false);
		letsPlay(4);
	}

	public bool teamsOn = false;

	public void setTeams(bool val) {
		teamsOn = val;
		GameInfo.S.setTeams (val);
		for (int i = 0; i < 4; i++) {
			if (characterScreens [i]) {
				characterScreens [i].setTeamVal (val);
			}
		}
	}

	public void setTeam(int pNum) {
		if (characterScreens [pNum]) {
			characterScreens[pNum].setTeamText ();
		}
	}

	public void setCharMenu(CharSelectMenu m) {
		charMenu = m;
	}

	public charFunction getScreenOfController(int scheme) {
		for (int i = characterScreens.Length - 1; i >= 0; i--) {
			if (characterScreens[i]) {
				if (characterScreens[i].scheme == scheme) {
					return characterScreens[i];
				}
			}
		}
		return null;
	}

	public bool checkActiveControlScheme(int num) {
		if (num > -1 && num < activeControlSchemes.Length) {
			return activeControlSchemes[num];
		}
		return false;
	}

	// when an ai takes over a control scheme
	public void releaseActveControlScheme(int num) {
		if (num > -1 && num < activeControlSchemes.Length) {
			activeControlSchemes[num] = false;
		}
	}

	public charFunction getCharacterScreen(int num) {
		if (num >= 0 && num < characterScreens.Length) {
			return characterScreens[num];
		}
		return null;
	}
}
