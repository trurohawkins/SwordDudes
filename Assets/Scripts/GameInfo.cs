using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInfo : MonoBehaviour {
	public static GameInfo S;
	public bool multiPlayer;
	public bool preFixed = false;
	public int numPlayers;
	public Color[] playerColors;
	public int[] controls;
	public int[] controlNums;
	public int[] teamNums;
	public int[] songs;
	public int[] colors;
	public bool[] locked;

	public int p1Pad = 0;
	public int p1ControlNum = -1;
	public int p1Song = -1;
	public int p2Pad = 0;
	public int p2ControlNum = -1;
	public int p2Song = -1;
	public int level;
	public string[] names;
	public GameObject[] souls;
	public string[] levels;
	public Sprite[] pSprites;
	public Sprite[] symbols;
	int maxLevel = 5;
	public int enemyLevel = 0;
	public int maxEnemyLevel = 3;
	public GameObject control;
	keyControl[] controllers;
	public int progress = 0;
	public bool hideCursor = true;

	void Awake() {
		if (!S) {
			S = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Debug.Log("I am destroying myself");
			Destroy (gameObject);
			return;
		}
		Cursor.visible = !hideCursor;
		if (!preFixed) {//|| controls.length != numPlayers || controlNums.length != numPlayers || songs.length != numPlayers || colors.length != numPlayers) {
			controls = new int[numPlayers];
			controlNums = new int[numPlayers];
			songs = new int[numPlayers];
			colors = new int[numPlayers];
			for (int i = 0; i < numPlayers; i++) {
				songs [i] = -1;
				controlNums [i] = -1;
				controls [i] = 0;
				colors [i] = i;
			}
		}
		//Debug.Log("Im awake and crating controllers");
		if (controllers == null) {
			controllers = new keyControl[2];
			controllers [0] = Instantiate (control).GetComponent<keyControl>();
			controllers [0].setArrow ("w", 0);
			controllers [0].setArrow ("a", 1);
			controllers [0].setArrow ("s", 2);
			controllers [0].setArrow ("d", 3);
			controllers [0].setSword ("o", 0);
			controllers [0].setSword ("p", 1);
			/*
			controllers [0].setSword (0, 0);
			controllers [0].setSword (0, 1);
			*/
			controllers [0].setDodge ("space");
			controllers [1] = Instantiate (control).GetComponent<keyControl>();
			controllers [1].setArrow ("i", 0);
			controllers [1].setArrow ("j", 1);
			controllers [1].setArrow ("k", 2);
			controllers [1].setArrow ("l", 3);
			controllers [1].setSword ("u", 0);
			controllers [1].setSword ("o", 1);
			controllers [1].setSword (-1, 0);
			controllers [1].setSword (-1, 1);
			controllers [1].setDodge ("right shift");
		}
	}

	public void destroySelf() {
		Destroy(controllers[0].gameObject);
		Destroy(controllers[1].gameObject);
		Destroy(gameObject);
	}

	public void setSong(int pNum, int song) {
		songs [pNum] = song;
		if (song >= 0 && song < names.Length) {
			/*
			if (pNum == 0) {
				p1Song = song;
				MainMenu.S.setP1Text (names [song]);
			} else if (pNum == 1) {
				p2Song = song;
				MainMenu.S.setP2Text (names [song]);
			}
			*/
			//MainMenu.S.setSongText (names [song], pNum);
			MainMenu.S.setSong(true, pNum);
		} else {
			//MainMenu.S.setSongText("---", pNum);
			MainMenu.S.setSong(false, pNum);
		}

	}

	public bool allPlayersChosen() {
		if (numPlayers == 0) {
			return false;
		}
		bool allChosen = true;
		for (int i = 0; i < numPlayers; i++) {
			if (songs [i] == -1) {
				allChosen = false;
				break;
			}
		}
		return allChosen;
	}

	public void incrementLevel (int inc) {
		if (level + inc < 0) {
			level = levels.Length - 1;
		} else {
			level = (level + inc) % levels.Length;
		}
		MainMenu.S.setLevelText (levels [level]);
	}

	public void incrementAILevel(int inc, int player) {
		int curLevel = controls [player];
		if (curLevel <= 0) {
			curLevel *= -1;
			if (curLevel + inc <= maxLevel && curLevel + inc > 0) {
				curLevel += inc;
			}/* else {
				curLevel = (curLevel + inc) % (maxLevel + 1);
			}*/
			controls [player] = -curLevel;
			MainMenu.S.setAiText (player, curLevel);
		}
	}

	public void changeColor(int cNum, int pNum) {
		bool okCombo = checkColorCombo (cNum, pNum);
		if (okCombo) {
			colors [pNum] = cNum;
		} else {
			colors [pNum] = (cNum + 1) % numPlayers;
		}
		MainMenu.S.charScreenColor (pNum, cNum);
	}

	public bool checkColorCombo(int cNum, int pNum) {
		bool okCombo = true;
		int sNum = songs[pNum];
		for (int i = 0; i < numPlayers; i++) {
			if (i != pNum) {
				if (songs [i] == sNum && colors [i] == cNum) {
					okCombo = false;
					break;
				}
			}
		}
		return okCombo;
	}

	public void removeSameColorCombos(bool singlePlayer) {
		int i = 0;
		if (singlePlayer) {
			i = 1;
		}
		for (; i < numPlayers; i++) {
			while (!checkColorCombo(colors[i], i)) {
				colors[i] = (colors[i] + 1) % 4;
			}
		}
	}

	public void setTeams(bool val) {
		if (!val) {
			for (int i = 0; i < 4; i++) {
				teamNums [i] = i;//-1;
			}
		} else {
			for (int i = 0; i < 4; i++) {
				if (numPlayers == 1 || i < numPlayers / 2) {
					teamNums [i] = 0;
				} else {
					teamNums [i] = 1;
				}
			}
		}
	}

	public void cycleTeam(int pNum) {
		int np = numPlayers;
		teamNums [pNum] = (teamNums [pNum] + 1) % np;
		MainMenu.S.setTeam (pNum);
	}

	public keyControl[] getControls() {
		return controllers;
	}

	public keyControl getControl(int i) {
		return controllers[i];
	}
}
