using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkDialogue : Shot {

    public string alternate;

    public override IEnumerator readyingShot(Dialogue d) {
        Player p = GM.S.players[0];
        Vector2Int pos = p.body.centerPoint;
        float frac = 10;
        bool inNeed = true;
        for (int i = 0; i < frac; i++) {
            if (pos != p.body.centerPoint) {
                inNeed = false;
                break;
            }
            yield return new WaitForSeconds(leadUp/frac);
        }
        if (!inNeed) {
            d.dumpWords();
            d.getWord(alternate);
        } 
        //StartCoroutine(d.writeOutWords());
        d.startWriting();
    }
}
