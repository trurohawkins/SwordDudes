using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryPoint : DungeonObject {

    public string[] text;
    public string[] analogText;
    public string[] keyText;
    public string[] afterText;
    public string[] alternateText;
    public bool pauser;

    [HideInInspector]
    public Dialogue dia;
    protected bool stoodOn = false;

    protected override void Awake() {
        base.Awake();
        formText();
    }

    protected void formText() {
        if (GameInfo.S.controls[0] == 1) {
            appendText(analogText);
        } else {
            appendText(keyText);
        }
        appendText(afterText);
    }

    public override void callAction(Form poo, int state, int x, int y) {
        if (poo.id == 1) {
            if (state == 1 && !stoodOn) {
                stoodOn = true;
                startText();
            }
        }
    }

    public void startText() {
        if (pauser) {
            dia.curPause = 1;//pauser;
        } else {
            dia.curPause = -1;
        }
        //dia.pause();
        dia.dumpImage();
        for (int i = 0; i < text.Length; i++) {
            Debug.Log(text[i]);
            dia.getWord(text[i]);
        }
        //StartCoroutine(dia.writeOutWords());
        dia.startWriting();
    }

    public void alterText() {
        text = alternateText;
        formText();
    }


    public void getDialogue(Dialogue d) {
        dia = d;
    }

    public void getText(string[] t) {
        text = t;
    }

    public void appendText(string[] t) {
        if (t.Length == 0) {
            return;
        }
        string[] full = new string[t.Length + text.Length];
        for (int i = 0; i < t.Length + text.Length; i++) {
            if (i < text.Length) {
                full[i] = text[i];
            } else {
                full[i] = t[i-text.Length];
            }
        }
        text = full;
    }

    public void deleteSelf() {
        master.removeDenizen(this);
        self.die();
    }
}
