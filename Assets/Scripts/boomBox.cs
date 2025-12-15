using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boomBox : MonoBehaviour {

	public static boomBox S;
	public LibPdInstance pdPatch;
	bool everythingOpened = false;
	bool shouldIStart = false;
	bool started = false;
	public float tempoBase;
	public int slowestTempo = 1000;
	public int fastestTempo = 300;

	void Awake () {
		if (S) {
			Destroy (gameObject);
		} else {
			//Debug.Log("new boombox");
			S = this;
			DontDestroyOnLoad(gameObject);
			StartCoroutine (openStuff ());
		}
	}

	IEnumerator openStuff() {
		if (pdPatch) {
			pdPatch.SendBang ("openy");
			pdPatch.SendBang ("openn");
			pdPatch.SendBang ("opent");
			pdPatch.SendBang ("opentheme");
			pdPatch.SendBang ("openbeat");
		} else {
			Debug.LogError ("give me the PD patch - love, boomBox");
		}
		yield return new WaitForSeconds (0.3f);
		everythingOpened = true;
	}

	public void yesPress(int num) {
		//Debug.Log("BB yes " + num + " " + everythingOpened);
		if (everythingOpened) {
			if (num < 0 || num > 1) {
				pdPatch.SendBang ("yR");
				pdPatch.SendBang ("yL");
			} else {
				pdPatch.SendBang ("y" + convertPNum (num));
			}
		}
	}

	public void noPress(int num) {
		if (everythingOpened) {
			if (num < 0 || num > 1) {
				pdPatch.SendBang ("nR");
				pdPatch.SendBang ("nL");
			} else {
				pdPatch.SendBang ("n" + convertPNum (num));
			}
		}
	}

	public void nextPress(int num) {
		if (everythingOpened) {
			if (num < 0 || num > 1) {
				pdPatch.SendBang ("tR");
				pdPatch.SendBang ("tL");
			} else {
				pdPatch.SendBang ("t" + convertPNum (num));
			}
		}
	}

	public void themeMusic(bool play) {
		if (play) {
			if (everythingOpened) {
				pdPatch.SendBang ("themeon");
			} else {
				StartCoroutine(waitingToPlay("themeon"));
			}
		} else {
			pdPatch.SendBang ("themeoff");
		}
	}

	IEnumerator waitingToPlay(string bang) {
		while (!everythingOpened) {
			yield return new WaitForEndOfFrame ();
		}
		pdPatch.SendBang (bang);
	}

	void Update() {
		if (Input.GetKeyUp ("h")) {
			upTempo (1);
		}
		if (Input.GetKeyUp ("l")) {
			//tempoBurst = 0;
			//pdPatch.SendFloat("ssswordL", 1);
		}
		if (shouldIStart && !started) {
			if (everythingOpened) {
				beat (1);
				started = true;
			}
		}
	}

	char convertPNum(int pNum) {
		if (pNum == 0) {
			return 'L';
		} else if (pNum == 1) {
			return 'R';
		} else {
			return ' ';
		}
	}

	public void startSwordSongs(bool beatToo) {
		boomBox.S.themeMusic (false);
		if (GM.S.getNumPlayers() > 1) {
			boomBox.S.startMusic ();
		} else {
			boomBox.S.leftSong(GameInfo.S.songs[0]);
			boomBox.S.rightSong(GameInfo.S.songs[0]);
			//boomBox.S.startMusic();
		}
		if (beatToo) {
			boomBox.S.beat(1);
		} else {
			boomBox.S.beat(0);
		}
		boomBox.S.setFlicker(0, 1);
		boomBox.S.setFlicker(1, 1);
	}

	int s1;
	int s2;

	public void startMusic() {
		shouldIStart = true;
		leftSong(GameInfo.S.songs[0]);
		rightSong(GameInfo.S.songs[1]);
	}

	public void leftSong(int song) {
		s1 = (song + 1) % 10;
		pdPatch.SendFloat("whichL", s1);
		pdPatch.SendBang("startL");
	}

	public void rightSong(int song) {
		s2 = (song + 1) % 10;
		pdPatch.SendFloat("whichR", s2);
		pdPatch.SendBang("startR");
	}

	IEnumerator startingGame() {
		s1 = GameInfo.S.p1Song + 1;//Random.Range (1,11);
		pdPatch.SendFloat("whichL", s1);
		pdPatch.SendBang("startL");
		s2 = GameInfo.S.p2Song + 1;//Random.Range (1, 11);
		pdPatch.SendFloat ("whichR", s2);
		pdPatch.SendBang ("startR");
		while (!everythingOpened) {
			Debug.Log ("poo");
			yield return new WaitForFixedUpdate ();
		}
		Debug.LogError ("dog poop");
		beat (1);
	}
		
	public void beat(float play) {
		if (everythingOpened) {
			pdPatch.SendFloat ("beat", play);
		}
	}

	public float curBeat = 1000;

	public void bTempo(float beatsPerMs) {
		curBeat = beatsPerMs;
		pdPatch.SendFloat ("tempo", beatsPerMs);
	}

	public void upTempo(float am) {
		float amount = am * tempoBase;
		if (curBeat - amount > fastestTempo) {
			curBeat -= amount;
		} else {
			curBeat = fastestTempo;
		}
		bTempo (curBeat);
		if (tempoBurst <= 0) {
			StopCoroutine ("tempoDown");
			StartCoroutine ("tempoDown");
		} else {
			tempoBurst = tBurstAmount;
		}
	}

	int tempoBurst = 0;
	public int tBurstAmount = 100;
	public float tempoReturnInterval = 0.5f;

	IEnumerator tempoDown() {
		tempoBurst = tBurstAmount;
		while (tempoBurst > 0) {
			yield return new WaitForFixedUpdate ();
			tempoBurst--;
		}
		while (curBeat < slowestTempo) {
			if (curBeat + 50 < slowestTempo) {
				curBeat += 50;
			} else {
				curBeat = slowestTempo;
			}
			bTempo (curBeat);
			yield return new WaitForSeconds(tempoReturnInterval);
		}
	}

	public void setMusicSpeed(int player, float speed) {
		if (player < 2) {
			//	pdPatch.SendFloat ("p" + pNum + "Speed", speed);
			pdPatch.SendFloat("offset"+convertPNum(player), speed);
		}
	}

	public void setBurst(int player, float b) {
		if (player < 2) {
			//Debug.Log ("burst set");
			effectFading = false;
			StartCoroutine (burstFade (convertPNum(player), b));
		}
	}

	public void resetBurst(int player) {
		effectFading = false;
		pdPatch.SendFloat ("amtmario" + convertPNum(player), 0);
		//Debug.Log("death of player" + player);
		if (convertPNum(player) == 'L') {
			//Debug.Log ("reset" + convertPNum(player));
			p1AMT = 0;
		}
	}

	bool effectFading = false;
	public float p1AMT = 0;

	IEnumerator burstFade(char player, float b) {
		if (!effectFading) {
			effectFading = true;
			float t = 0;
			float start = 0;
			if (b == 0) {
				start = 1;
			}
			float cur = start;
			while (t < 1 && effectFading) {
				t += 0.05f;
				cur = Mathf.Lerp (start, b, t);
				pdPatch.SendFloat ("amtmario" + player, cur);
				if (player == 'L') {
					p1AMT = cur;
				}
				yield return new WaitForFixedUpdate ();
			}
			effectFading = false;
		}
	}

	public void setFlicker(int player, float onOff) {
		if (player < 2) {
			char p = convertPNum (player);
			if (p != ' ') {
				//Debug.Log(onOff);
				pdPatch.SendFloat ("flicker" + p, onOff);
			}
		}
	}

	public void die(int player) {
		if (player > 1) {
			return;
		}
		StopCoroutine ("tempoDown");
		tempoBurst = 0;
		bTempo (1000);
		StopAllCoroutines ();
		pdPatch.SendFloat ("amtmario" + convertPNum(player), 0);
		//Debug.Log("death of player + " + player);
		if (convertPNum(player) == 'L') {
	//		Debug.Log ("reset");
			p1AMT = 0;
		}
		pdPatch.SendBang ("lose" + convertPNum (player)); 
		//setFlicker(player, 0);
	}

	public void respawn(int player) {
	//	setFlicker(player, 1);
		if (player < 2) {
			pdPatch.SendBang("start"+convertPNum(player));
		}
	}

	public void weHaveWinner(int player) {
		Debug.Log("winner song");
		StartCoroutine ("winningPagaent", player);
	}

	IEnumerator winningPagaent(int player) {
		int winSong = s1;
		string winText = "Lwon";
		string loseText = "loseR";
		string firstWin = "winL";
		if (player != 0) {
			winSong = s2;
			winText = "Rwon";
			loseText = "loseL";
			firstWin = "winR";
		}
		//Debug.Log (winSong);
		beat (0);
		pdPatch.SendBang (loseText);
		pdPatch.SendBang (firstWin);
		pdPatch.SendFloat ("songwin", winSong);
		yield return new WaitForSeconds (0.5f);
		pdPatch.SendBang (winText);
	}

	public void restartGame() {
		pdPatch.SendBang ("leavewinscreen");
		curBeat = slowestTempo;
		beat (1);
		pdPatch.SendBang("startL");
		pdPatch.SendBang("startR");
	}
}
