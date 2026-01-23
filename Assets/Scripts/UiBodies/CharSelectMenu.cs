using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharSelectMenu : UIMenu {

    selectButton[] selButts;
    public icon[] bottomRow;
    int[] playerNums;
    public GameObject optionMenu;
    SwordOptionMenu[] menuUp;

    public override void Awake() {
        setSelects();
        menuUp = new SwordOptionMenu[6];
        rows = ((butts.Length+2) / columns) + 1;
        buts = new icon[columns, rows];
        selButts = new selectButton[butts.Length];
        playerNums = new int[selects.Length];
        for (int i = 0; i < playerNums.Length; i++) {
            playerNums[i] = -1;
        }
        int count = 0;
        for (int j = 0; j < columns; j++) {
			for (int i = 0; i < rows-1; i++) {
                if (j == 1) {
                    if (i == 0) {
                        buts[j,i] = butts[0].GetComponent<selectButton>();
                        continue;
                    } else if (i == rows-2) {
                        buts[j,i] = butts[5].GetComponent<selectButton>();
                        continue;
                    }
                }
                selectButton sb = butts[count].GetComponent<selectButton>();
                if (sb) {
                    buts[j,i] = sb;
                }
                count++;
            }
        }
        count = 0;
        for (int i = 0; i < columns; i++) {
            buts[i, rows-1] = bottomRow[count].GetComponent<icon>();
            count++;
        }
    }

    public override void Start() {
        base.Start();
        if (MainMenu.S) {
            MainMenu.S.setCharMenu(this);
        }
        for (int i = 0; i < blocked.Length; i++) {
            blocked[i] = true;
        }
    }

    public void setPlayer(int scheme, int num) {
        playerNums[scheme] = num;
    }

    public override bool checkButt(int x, int y) {
        //return buts[x,y] != null;
        if (buts[x, y] != null) {
            selectButton sb = buts[x,y].GetComponent<selectButton>();
            if (sb) {
                return !sb.locked();
            } else {
                return true;
            }
        } else {
            return false;
        }
	}

    public override void selectButton(int player, Vector2Int chosen) {
        //Debug.Log("select button " + player + " " + playerNums[player]); // line below caused error, cuz playernum is -1 sometimes
        if (playerNums[player] >= 0) {
            if (GameInfo.S.songs[playerNums[player]] < 0) {
                MainMenu.S.setPlayerGraphics(playerNums[player], -1);
            }
            selectButton sb = buts[chosen.x, chosen.y].GetComponent<selectButton>();
            if (sb) {
                MainMenu.S.giveButton(playerNums[player], sb);
            }
            buttColor(selects[player], false, playerNums[player]);
            selects[player] = chosen;
            buttColor(selects[player], true, playerNums[player]);
        }
    }

    public override float checkDistance(int x, int y) {
        if (buts[x,y]) {
		    return Vector3.Distance (Input.mousePosition, buts [x, y].transform.position);
        } else {
            return Mathf.Infinity;
        }
	}

    public override void buttColor(Vector2Int num, bool active, int player) {
        if (buts[num.x, num.y]) {
            selectButton sb = buts[num.x, num.y].GetComponent<selectButton>();
            if (sb) {
                buts[num.x, num.y].activate(active, player);
            } else {
                Text t = buts[num.x, num.y].GetComponentInChildren<Text>();
                if (t) {
                    if (active) {
				        t.color = colors[1];
			        } else {
				        t.color = colors[0];
			        }
                }
            }
        }
    }

    public override void buttClick(int player) {
        if (MainMenu.S.checkActiveControlScheme(player)) {
            selectButton s = buts[selects[player].x, selects[player].y].GetComponent<selectButton>();
            if (s) {
                if (!s.locked()) {
                    if (playerNums[player] >= 0) {
                        s.choose(playerNums[player]);
                        Debug.Log(playerNums[player]);
                        s.selectChar(playerNums[player]);
                        bringUpOption(playerNums[player], player, new Vector3(0,0,0));
                    } else {
                        Debug.Log("removed character press play");
                    }
                }
            } else {
                icon i = buts[selects[player].x, selects[player].y];
                if (i.func == 0) {
                    randomSoul(player);
                } else {
                    base.buttClick(player);
                }
            }
        }
    }

    public void randomSoul(int player) {
        if (MainMenu.S.checkActiveControlScheme(player)) {
            int p = playerNums[player];
            int soul = Random.Range(0, 10);
            while (GameInfo.S.locked[soul]) {
                soul = Random.Range(0, 10);
            }
            Debug.Log("random soul " + soul + " for " + p);
            /*
            GameInfo.S.setSong (p, soul);//myButt.song);
		    GameInfo.S.changeColor (GameInfo.S.colors[p], p);
            */
            buttColor(selects[player], false, player);
            selectButton chosen = moveCursorToSword(player, soul);
            if (chosen) {
                chosen.activate(true, p);
                MainMenu.S.setPlayerGraphics(p, soul);
                chosen.choose(playerNums[player]);

                chosen.selectChar(playerNums[player]);
                bringUpOption(p, player, new Vector3(0,0,0));
            }
        }
    }

    void bringUpOption(int playerNum, int player, Vector3 pos) {
        charFunction cf = MainMenu.S.getCharFunction(playerNum);
        RectTransform rt = cf.infoText.GetComponent<RectTransform>();
        SwordOptionMenu som = cf.setOptions(true);//Instantiate(optionMenu, transform).GetComponent<SwordOptionMenu>();
        charFunction screen = MainMenu.S.getCharacterScreen(playerNum);
        som.getInfo(this, playerNum, player, screen.type, screen.num);
        menuUp[player] = som;
        blocked[player] = true;
    }

    public selectButton moveCursorToSword(int player, int soul) {
        selectButton chosen = null;
        for (int j = 0; j < columns; j++) {
			for (int i = 0; i < rows-1; i++) {
                if (buts[j,i]) {
                    chosen = buts[j, i].GetComponent<selectButton>();
                    if (chosen.soulNumber == soul) {
                        selects[player] = new Vector2Int(j, i);
                        j = columns + 1;
                        break;
                    }
                }
            }
        }
        return chosen;
    }

    public bool playerPressStart() {
        for (int i = 0; i < 4; i++) {
            if (!menuUp[i] && Input.GetButtonDown("startButton"+i)) {
                return true;
            }
        }
        if (!menuUp[4] && Input.GetKeyDown("return")) {
            return true;
        }
        return false;
    }

    public void comeBack(int player) {
        menuUp[player] = null;
    }

    public override void pressBack(int player) {
        //Debug.Log(player + " is pressing back " + playerNums[player]);
        //if (MainMenu.S.checkActiveControlScheme(player)) {
        if (MainMenu.S.getScreenOfController(player)) {
            if (blocked[player]) {
                blocked[player] = false;
            }
            if (GameInfo.S.songs[playerNums[player]] >= 0) {
                GameInfo.S.setSong(playerNums[player], -1);
                GameInfo.S.controls[playerNums[player]] = 0;
                GameInfo.S.controlNums[playerNums[player]] = -1;
                if(buts[selects[player].x, selects[player].y]) {
                    //buts[selects[player].x, selects[player].y].unChoose(playerNums[player]);
                } else {
                    Debug.Log(buts[selects[player].x, selects[player].y]);
                }
            }
        }
    }

    public override void holdBack(int player) {
        Debug.Log("holdign back " + player);
        /*
        if (MainMenu.S.checkActiveControlScheme(player)) {
            Debug.Log("has active control");
            MainMenu.S.destroyCharScreen(playerNums[player], player);
            changePlayerNums(player);
        } else if (MainMenu.S.destroyAIScreen(player)) { // destroys 1st ai screen made by this controller, if there are none it returns false and we go back
            changePlayerNums(player);
        */
        if (MainMenu.S.destroyLastScreen(player)) {
            changePlayerNums(player);
        } else {
            for (int j = 0; j < columns; j++) {
			    for (int i = 0; i < rows-1; i++) {
                    if (buts[j,i]) {
                        buts[j,i].unChoose(-2);
                    }
                }
            }
            base.holdBack(player);
        }
    }

    // used when 1 player is delted, we want to roll back any players after
    void changePlayerNums(int start) {
        int player = playerNums[start];
        //Debug.Log("start: " + start + " " + player);
        charFunction screen = MainMenu.S.getScreenOfController(start);
        if (screen) {
            playerNums[start] = screen.pNum;
            int song = GameInfo.S.songs[screen.pNum];
            // we need to update the cursor
            moveCursorToSword(start, song);
        } else {
            playerNums[start] = -1;
        }
        for (int i = 0; i < playerNums.Length-1; i++) {
            if (i != start && playerNums[i] > player) {
                playerNums[i]--;
                //screen.curButton.activate(true, playerNums[i]);
            }
        }
        for (int i = 0; i < 4; i++) {
            screen = MainMenu.S.getCharacterScreen(i);
            if (screen) {//maybe put back && screen.aiNum == -1) {
                //screen.curButton.activate(true, screen.pNum);
            }
        }
    }

    public void setBlocked(int scheme, bool val) {
        if (scheme >= 0 && scheme <= blocked.Length) {
            blocked[scheme] = val;
        }
    }

    public bool checkMenu(int scheme) {
        if (scheme >= 0 && scheme <= blocked.Length) {
            return menuUp[scheme];
        }
        return true;
    }

}
