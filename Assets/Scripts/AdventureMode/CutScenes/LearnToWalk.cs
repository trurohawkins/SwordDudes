using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearnToWalk : StoryPoint {
    public string[] alternate;
    int altI = 0;
    Player myP;
    Vector2Int pStartPos;
    int read = 0;

    public void Start() {
        myP = GM.S.players[0];
        pStartPos = myP.body.centerPoint;
    }

    public override IEnumerator callAction(int delay) {
        if (speedCounter > speed) {
            if (dia && !dia.isWriting()) {
                if (read < 4) {
                    if (pStartPos != myP.body.centerPoint) {
                        text = new string[alternate.Length];
                        text = alternate;//[altI];
                        //pauser = true;
                        //altI++;
                        formText();
                        startText();
                        active = false;
                    } else {
                        read++;
                    }
                } else {
                    startText();
                    active = false;
                }
            }
            speedCounter = 0;
        } else {
            speedCounter++;
        }
        yield return new WaitForFixedUpdate();
    }
}
