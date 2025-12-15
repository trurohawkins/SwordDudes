using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwordOptionMenu : UIMenu {

    public Button acceptButt;
    public Button[] controllers;
    public Button[] level;
    public Color chosen;
    CharSelectMenu pre;
    icon[] curSelection;
    int humanType;

    public override void Awake() {
        columns = 5;
        rows = 4;
        buts = new icon[columns, rows];
        txts = new Text[columns, rows];
        for (int i = 0; i < 5; i++) {
            buts[i,0] = null;
            txts[i,0] = null;
        }
        fillStuff(acceptButt, 2, 0);
        for (int i = 0; i < 2; i++) {
            fillStuff(butts[i], i, 1);
        }
        buts[2, 1] = null;
        txts[2, 1] = null;
        for (int i = 3; i < 5; i++) {
            fillStuff(butts[i-1], i, 1);
        }

        fillStuff(controllers[0], 0, 2);
        fillStuff(controllers[1], 4, 2);
        for (int i = 1; i < 4; i++) {
            buts[i, 2] = null;
            txts[i,2] = null;
        }
        for (int i = 0; i < 5; i++) {
            fillStuff(level[i], i, 3);
        }
        setSelects();
        curSelection = new icon[3];
        buttColor(new Vector2Int(2,0), true, -1);
    }

    void fillStuff(Button b, int x, int y) {
        buts[x, y] = b.GetComponent<icon>();
        Text t = b.GetComponentInChildren<Text>();
        if (t) {
            txts[x, y] = t;
        }
    }

    int scheme;
    int player;
    int numType;

    public void getInfo(CharSelectMenu menu, int pNum, int sch, int type, int controlNum) {
        pre = menu;
        scheme = sch;
        player = pNum;
        for (int i = 0; i < blocked.Length; i++) {
            if (i != scheme) {
                blocked[i] = true;
            } else {
                blocked[i] = false;
            }
        }
        humanType = type;
        numType = controlNum;
        selects[scheme] = new Vector2Int(2, 0);
        curSelection[0] = butts[GameInfo.S.colors[pNum]].GetComponent<icon>();
        icon humanIcon = controllers[0].GetComponent<icon>();
        if (MainMenu.S.checkHumanPlayer(scheme, humanType, numType)) {
            humanIcon.setLock(true);
            setController(false);
        } else {
            humanIcon.setLock(false);
            setController(true);
        }
        hideAILevels(GameInfo.S.controls[pNum] > 0);
        if (GameInfo.S.controls[pNum] < 0) {
            curSelection[1] = controllers[1].GetComponent<icon>();
            curSelection[2] = level[(-GameInfo.S.controls[pNum])-1].GetComponent<icon>();
        } else {
            curSelection[1] = controllers[0].GetComponent<icon>();
            curSelection[2] = null;
        }
        for (int i = 0; i < curSelection.Length; i++) {
            if (curSelection[i]) {
                curSelection[i].choose(-1);//color = chosen;
            }
        }
    }

    protected override void Update() {
        base.Update();
        if (scheme < 4) {
            if (Input.GetButtonDown("startButton"+scheme)) {
                Accept();
            }
        } else {
            if (Input.GetKeyDown("return")) {
                //Accept();
            }
        }
    }

    public override void buttClick(int player) {
        base.buttClick(player);
        click(selects[player].x, selects[player].y);
    }

    void click(int x, int y) {
        int cur = y - 1;
        if (cur >= 0 && cur < 3) {
            //Text t = txts[x, y];
            icon ic = buts[x, y].GetComponent<icon>();
            if (ic && !ic.isLocked()) {
                if (curSelection[cur]) {
                    curSelection[cur].unChoose(-1);//color = colors[0];
                }
                curSelection[cur] = ic;
                curSelection[cur].choose(-1);
                /*
                t.color = chosen;
                curSelection[cur] = t;
                */
            }
        }
    }
    public override void buttColor(Vector2Int num, bool active, int player) {
        for (int i = 0; i < curSelection.Length; i++) {
            if (txts[num.x, num.y] == curSelection[i]) {
                return;
            }
        }
        base.buttColor(num, active, player);
	}

    public override void pressBack(int player) {
        Accept();
    }

    public void Accept() {
        pre.comeBack(scheme);
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }

    public void setColor(int c) {
        //GameInfo.S.colors[player] = c;
        GameInfo.S.changeColor(c, player);
    }

    public void setController(bool human) {
        if (human) {
            hideAILevels(true);
            GameInfo.S.controls[player] = humanType;
            GameInfo.S.controlNums[player] = numType;
        } else {
            hideAILevels(false);
            setLevel(-3);
            click(2,3);
        }
    }

    public void setLevel(int num) {
        GameInfo.S.controls[player] = num;
        GameInfo.S.controlNums[player] = -1;
    }

    void hideAILevels(bool hide) {
        for (int i = 0; i < level.Length; i++) {
            level[i].gameObject.SetActive(!hide);
        }
        if (hide) {
            rows = 3;
        } else {
            rows = 4;
        }
    }
}
