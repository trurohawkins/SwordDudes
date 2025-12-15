using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class charFunction : MonoBehaviour {
	public Text controllerText;
	public AILevelBodyUI[] aiLevel;
	public ControlBodyUI controls;
	public teamBodyUI team;
	public colorBodyUI[] colors;
	public GameObject playerText;
	public GameObject infoText;
	public GameObject aiText;
	public GameObject controlButton;
	public GameObject teamButton;
	public GameObject[] aiButtons;
	public GameObject[] colorButtons;
	public GameObject sprites;
	public SwordAnimator graphics;
	public Vector2 nameTextAlign;
	public Vector2 infoTextAlign;
	public Vector2 aiNumAlign;
	public Vector2 optionAlign;
	public Vector2 graphicsPos;
	public int pNum = -1;
	public GameObject optionsMenu;
	public selectButton curButton;
	public int type;
	public int num;
	public int scheme = -1;

	void Awake() {
		playerText.transform.SetParent(transform.parent);
		infoText.transform.SetParent (transform.parent);
		aiText.transform.SetParent (transform.parent);
		optionsMenu.transform.SetParent(transform.parent);
		controlButton.transform.SetParent(transform.parent);
		teamButton.transform.SetParent(transform.parent);
		for (int i = 0; i < 2; i++) {
			aiButtons[i].transform.SetParent(transform.parent);
			colorButtons[i].transform.SetParent(transform.parent);
		}
	}

	void Start() {
		setTeamVal (MainMenu.S.teamsOn);
		setTextPos ();
	}

	void OnEnable() {
		Vector3 pos = new Vector3 (graphicsPos.x, graphicsPos.y, transform.position.z);
		/*
		GameObject tmp = Instantiate (sprites, transform.position + pos, transform.rotation, transform);
		graphics = tmp.GetComponent<PlayerAnimator> ();
	//	graphics.setAlphaVal = 0f;

		if (pNum < 0 || GameInfo.S.songs[pNum] == -1) {
			graphics.setAlphaVal = 0f;
		} else {
			graphics.setAlphaVal = 1f;
			graphics.spinSpeed = 50;
			MainMenu.S.charScreenColor (pNum, GameInfo.S.colors [pNum]);
		}
		*/


	}

	public void glanceChar(int song, int player) {
		if (song >= 0 && song < GameInfo.S.souls.Length) {
			SwordSoul s =  Instantiate (GameInfo.S.souls [song]).GetComponent<SwordSoul> ();
			Vector3 pos = new Vector3 (graphicsPos.x, graphicsPos.y, transform.position.z);
			if (s.SwordAnim) {
				graphics = Instantiate(s.SwordAnim, transform.position + pos, transform.rotation, transform).GetComponent<SwordAnimator>();
				//MainMenu.S.setSongText (GameInfo.S.names [song], player);
				//graphics.setAlphaVal = 0.4f;
				//graphics.setAlpha(0.4f);
			}
			getDescription(song);
			getColors(song, GameInfo.S.colors[player]);
			graphics.charSelectGlance();
			//graphics.spinSpeed = 0;
			Destroy(s.gameObject);
		} else {
			//graphics.setAlphaVal = 0;
			//graphics.setAlpha(0);
			if (graphics != null) {
				Destroy(graphics.gameObject);
				graphics = null;
			}
			//MainMenu.S.setSongText ("---", player);
			getDescription(-1);
		}
	}

	public void getDescription(int song) {
		if (song > -1 && song < GameInfo.S.souls.Length) {
			SwordSoul s =  Instantiate (GameInfo.S.souls [song]).GetComponent<SwordSoul> ();
			if (graphics) {
				if (pNum < 0 || GameInfo.S.songs[pNum] == -1) {
					graphics.setAlpha(0f);
				} else {
					graphics.setAlpha(1f);
					//graphics.spinSpeed = 50;
					MainMenu.S.charScreenColor (pNum, GameInfo.S.colors [pNum]);
				}
			}
			//characterScreens [pNum].graphics.setColors (s, cNum);
			setText (s.description);
			Text pt = playerText.GetComponent<Text>();
			pt.text = s.soulName;
			Destroy(s.gameObject);
		} else {
			Text pt = playerText.GetComponent<Text>();
			pt.text = "---";
			setText("---");
		}
	}

	public void getColors(int song, int cNum) {
		if (song >= 0 && song < GameInfo.S.souls.Length) {
			GameObject tmp = Instantiate (GameInfo.S.souls [song]);
			SwordSoul s = tmp.GetComponent<SwordSoul> ();
			s.setColors (graphics, cNum);
			Destroy(tmp);
		}
	}

	void OnDisable() {
		if (graphics) {
			Destroy (graphics.gameObject);
			graphics = null;
		}
	}

	void setTextPos() {
		float scale = transform.localScale.x;
		Vector2 nta = nameTextAlign * scale;
		Vector3 screenPos = Camera.main.WorldToScreenPoint (new Vector3(transform.position.x + nta.x, transform.position.y + nta.y,  transform.position.z));
		RectTransform rt = playerText.GetComponent<RectTransform> ();
		rt.anchoredPosition = new Vector2 (screenPos.x - Screen.width / 2, screenPos.y - Screen.height / 2);//screenPos.y);

		Vector2 ita = infoTextAlign * scale;
		screenPos = Camera.main.WorldToScreenPoint (new Vector3(transform.position.x + ita.x, transform.position.y + ita.y,  transform.position.z));
		rt = infoText.GetComponent<RectTransform> ();
		rt.anchoredPosition = new Vector2 (screenPos.x - Screen.width / 2, screenPos.y - Screen.height / 2);//screenPos.y);

		Vector2 ana = aiNumAlign * scale;
		screenPos = Camera.main.WorldToScreenPoint (new Vector3(transform.position.x + ana.x, transform.position.y + ana.y,  transform.position.z));
		rt = aiText.GetComponent<RectTransform> ();
		rt.anchoredPosition = new Vector2 (screenPos.x - Screen.width / 2, screenPos.y - Screen.height / 2);//screenPos.y);

		Vector2 oa = optionAlign * scale;
		screenPos = Camera.main.WorldToScreenPoint (new Vector3(transform.position.x + oa.x, transform.position.y + oa.y,  transform.position.z));
		rt = optionsMenu.GetComponent<RectTransform>();
		rt.anchoredPosition = new Vector2 (screenPos.x - Screen.width / 2, screenPos.y - Screen.height / 2);//screenPos.y);
	}

	public void setPlayerNum(int n) {
		pNum = n;
		for (int i = 0; i < 2; i++) {
			aiLevel [i].playerNum = n;
			colors [i].playerNum = n;
		}
		controls.playerNumber = n;
		team.playerNumber = n;
		/*
		controllerText.color = GameInfo.S.playerColors [n];
		playerText.GetComponent<Text>().color = GameInfo.S.playerColors [n];
		infoText.GetComponent<Text>().color = GameInfo.S.playerColors [n];
		aiText.GetComponent<Text>().color = GameInfo.S.playerColors [n];
		teamButton.transform.GetChild (0).GetComponent<Text> ().color = GameInfo.S.playerColors [n];
		*/
	}

	public void setScaleFactor(float scale) {
		Vector3 s = new Vector3 (scale, scale, scale);
		transform.localScale = s;
		controlButton.transform.localScale = s;//SetParent(transform.parent);
		teamButton.transform.localScale = s;
		for (int i = 0; i < 2; i++) {
			aiButtons [i].transform.localScale = s;//SetParent(transform.parent);
		}
		//controllerText.transform.localScale = s;
		playerText.GetComponent<Text>().fontSize = (int)(15 * scale);
		playerText.GetComponent<RectTransform> ().sizeDelta = new Vector2(120, 120) * scale;
		infoText.GetComponent<Text>().fontSize = (int)(12 * scale);
		infoText.GetComponent<RectTransform> ().sizeDelta = new Vector2(200, 30) * scale;
		aiText.GetComponent<Text>().fontSize = (int)(20 * scale);
		aiText.GetComponent<RectTransform> ().sizeDelta = new Vector2(100, 100) * scale;
		setTextPos ();
	}

	public void setPos(Vector3 nPos) {
		transform.position = nPos;
		setTextPos ();
	}
	
	public void setAI(bool val) {
		for (int i = 0; i < 2; i++) {
			aiLevel [i].setShowing (val);
		}
		aiText.gameObject.SetActive (val);
		if (val == true) {
			setAiLevel (-GameInfo.S.controls [pNum]);
		}
	}

	public void setAiLevel(int level) {
		aiText.GetComponent<Text> ().text = "" + level;
	}

	public void setTeamVal(bool val) {
		team.setShowing (val);
		if (val) {
			setTeamText ();
		}
	}

	public void setText(string newText) {
		infoText.GetComponent<Text> ().text = newText;
	}

	public void setTeamText() {
		teamButton.GetComponentInChildren<Text> ().text = "" + GameInfo.S.teamNums [pNum];
	}

	public SwordOptionMenu setOptions(bool up) {
		optionsMenu.SetActive(up);
		return optionsMenu.GetComponent<SwordOptionMenu>();
	}

	public void unChoose() {
		if (curButton != null) {
			curButton.unChoose(pNum);
		}
	}

	public void removeSelf() {
		//Debug.Log("removing self " + pNum);
		unChoose();
		Destroy (playerText);
		Destroy (controlButton);
		for (int i = 0; i < 2; i++) {
			Destroy (aiButtons [i]);
			Destroy(colorButtons[i]);
		}
		Destroy (infoText);
		Destroy (aiText);
		Destroy(teamButton);
		Destroy (gameObject);
	}
}
