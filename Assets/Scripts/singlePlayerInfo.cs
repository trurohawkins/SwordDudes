using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class singlePlayerInfo : MonoBehaviour {

	//char
	// stage
		//fighters - string
				
				// round
				// aistage
				// sword
				// color
				// team 
					// 0- no team
					// 1 - player team
					// 2 - enemy team
	public string[] char0Stages;
	public string[] char1Stages;
	public string[] char2Stages;
	public string[] char3Stages;
	public string[] char4Stages;
	public string[] char5Stages;
	public string[] char6Stages;
	public string[] char7Stages;
	public string[] char8Stages;
	public string[] char9Stages;
	public string[] trial;
	public bool takeTrial;
	string[][] stages;
	public static singlePlayerInfo S;

	void Awake() {
		if (singlePlayerInfo.S) {
			Destroy (gameObject);
		} else {
			singlePlayerInfo.S = this;
		}

		stages = new string[10][];
		stages [0] = char0Stages;
		stages [1] = char1Stages;
		stages [2] = char2Stages;
		stages [3] = char3Stages;
		stages [4] = char4Stages;
		stages [5] = char5Stages;
		stages [6] = char6Stages;
		stages [7] = char7Stages;
		stages [8] = char8Stages;
		stages [9] = char9Stages;
	}

	void Start() {
		if (GameInfo.S.multiPlayer) {
			Destroy(gameObject);
		}
	}

	public void parseStage(int song, int stage) {
		if (takeTrial) {
			GameInfo.S.maxEnemyLevel = trial.Length - 1;
			parseString(trial[stage]);
		} else {
			Debug.Log ("stage " + stage + " for char " + song);
			GameInfo.S.maxEnemyLevel = stages[song].Length - 1;
			parseString (stages[song] [stage]);
		}
	}

	void parseString(string stage) {
		char[] lvl = stage.ToCharArray ();
		int np = 1;//GameInfo.S.numPlayers;
		GameInfo.S.level = intVal(lvl[0]);
		for (int i = 2; i < lvl.Length; i++) {
			// aistage
			GameInfo.S.controls [np] = -intVal(lvl[i]);
			Debug.Log (GameInfo.S.controls [np]);
			GameInfo.S.controlNums [np] = 0;
			i++;
			int song = intVal(lvl[i]);
			if (GameInfo.S.locked[song]) {
				song = 7;
			}
			GameInfo.S.songs [np] = song;//intVal(lvl[i]);
			i++;
			Debug.Log("player is song: " + GameInfo.S.songs[0] + "and color is " + GameInfo.S.colors[0]);
			int color = intVal(lvl[i]);
			/*
			while (song == GameInfo.S.songs[0] && color == GameInfo.S.colors[0]) {
				color = (color + 1) % 4;
			}
			*/
			GameInfo.S.colors [np] = color;//intVal (lvl [i]);
			i++;
			parseTeam(np, intVal(lvl[i]));
			np++;
			i++;//move passsed space
		}
		GameInfo.S.numPlayers = np;
		GameInfo.S.removeSameColorCombos(true);
	}

	int intVal(char c) {
		return (int)(c - 48);
	}

	public void parseTeam(int player, int team) {
		if (team < 0) {
			/*
			GameInfo.S.teamNums [player] = -1;
			GameInfo.S.teamNums [0] = -1;
			*/
		} else {
			GameInfo.S.teamNums[player] = team;
			Debug.Log("player " + player + " is on team " + team);
			/*
			GameInfo.S.teamNums [0] = 0;
			if (team == 1) {
				GameInfo.S.teamNums [player] = GameInfo.S.teamNums [0];//human is always in player 1
			} else {
				GameInfo.S.teamNums [player] = GameInfo.S.teamNums [0] + 1;
			}
			*/
		}
	}


				
}
