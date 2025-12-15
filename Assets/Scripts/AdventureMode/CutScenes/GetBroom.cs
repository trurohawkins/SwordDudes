using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetBroom : StoryPoint {
    bool hasBroom;
    public void Start() {
        if (GM.S.getNumPlayers() > 0) {
            //if (dia.storybo.getBroom()) {
            if (GM.S.players[0].hasSword) {
                deleteSelf();
                active = false;
            }
        }
    }

    public override IEnumerator callAction(int delay) {
        if (speedCounter > speed) {
            if (GM.S.getNumPlayers() > 0) {
                //if (dia.storybo.getBroom()) {
                if (GM.S.players[0].hasSword) {
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
