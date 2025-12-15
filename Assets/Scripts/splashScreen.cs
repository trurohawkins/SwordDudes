using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class splashScreen : MonoBehaviour {
    //public RectTransform background;
    public GameObject playerSprites;
    public GameObject teamSquare;
    public GameObject teamName;
    public GameObject VS;

    void Awake() {
        Debug.Log("width: " + Screen.width);
        //background.anchoredPosition = new Vector2 (Screen.width/4, Screen.height/2);//screenPos.y);
        //background.localScale = new Vector3(Screen.width/2, Screen.height, 0);
    }

    void Start() {
        StartCoroutine("makePlacard");
    }

    public float beat = 0.5f;
    IEnumerator makePlacard() {
        if (GameInfo.S) {
            List<int> teams = new List<int>();
            for (int i = 0; i < GameInfo.S.numPlayers; i++) {
                if (!teams.Contains(GameInfo.S.teamNums[i])) {
                    teams.Add(GameInfo.S.teamNums[i]);
                } 
                //teamCount[GameInfo.S.teamNums[i]]++;
            }
            int[] teamCount = new int[4];
            Vector3 pos = new Vector3(0, Screen.height/2, 0);
            if (teams.Count < 3) {
                //team placards
                for (int i = 0; i < 2; i++) {
                    RectTransform bg = Instantiate(teamSquare, transform).GetComponent<RectTransform>();
                    Vector2 p = new Vector2(Screen.width * 0.25f, 0);//Screen.height/2);
                    if (i == 0) {
                        p.x *= -1;
                    }
                    bg.anchoredPosition = p;
                    Vector2 s = new Vector2(Screen.width/2, Screen.height);
                    bg.sizeDelta = s;
                    Image im = bg.GetComponent<Image>();
                    if (teams.Count > i) {
                        im.color = GameInfo.S.playerColors[teams[i]];
                    } else {
                        im.color = Color.black;
                    }
                    bg = Instantiate(teamName, transform).GetComponent<RectTransform>();
                    p.y = Screen.height * 0.35f;
                    bg.anchoredPosition = p;
                    Text name = bg.GetChild(0).GetComponent<Text>();
                    name.text = "TEAM " + i;
                }
                yield return new WaitForSeconds(beat + 0.2f);
                if (boomBox.S) {
                    boomBox.S.yesPress(-1);
                }
                RectTransform rt = Instantiate(VS, transform).GetComponent<RectTransform>();
                rt.anchoredPosition = Vector2.zero;                //players
                for (int i = 0; i < GameInfo.S.numPlayers; i++) {
                    yield return new WaitForSeconds(beat);
                    if (boomBox.S) {
                        boomBox.S.yesPress(i % 2);
                    }
                    bool flipped = false;
                    if (GameInfo.S.teamNums[i] == teams[0]) {
                        pos.x = Screen.width * (0.1f + (teamCount[0] * 0.15f));
                        teamCount[0]++;
                    } else if (GameInfo.S.teamNums[i] == teams[1]) {
                        pos.x = Screen.width * (0.9f - (teamCount[1] * 0.15f));
                        teamCount[1]++;
                        flipped = true;
                    }
                    spawnPlayerSprite(pos, i, flipped);
                }
            } else {
                //team placards
                for (int i = 0; i < 4; i++) {
                    RectTransform bg = Instantiate(teamSquare, transform).GetComponent<RectTransform>();
                    Vector2 p = new Vector2(Screen.width * 0.25f, 0);//Screen.height/2);
                    if (i%2 == 0) {
                        p.x *= -1;
                    }
                    if (i < 2) {
                        p.y = Screen.height * 0.25f;
                    } else {
                        p.y = Screen.height * -0.25f;
                    }
                    bg.anchoredPosition = p;
                    Vector2 s = new Vector2(Screen.width/2, Screen.height/2);
                    bg.sizeDelta = s;
                    Image im = bg.GetComponent<Image>();
                    if (teams.Contains(i)) {
                        im.color = GameInfo.S.playerColors[i];
                        bg = Instantiate(teamName, transform).GetComponent<RectTransform>();
                        if (i < 2) {
                            p.y = Screen.height * 0.35f;
                        } else {
                            p.y = Screen.height * -0.15f;
                        }
                        bg.anchoredPosition = p;
                        Text name = bg.GetChild(0).GetComponent<Text>();
                        name.text = "TEAM " + i;
                    } else {
                        im.color = Color.black;
                    }
                }
                yield return new WaitForSeconds(beat + 0.2f);
                if (boomBox.S) {
                    boomBox.S.yesPress(-1);
                }
                RectTransform rt = Instantiate(VS, transform).GetComponent<RectTransform>();
                rt.anchoredPosition = Vector2.zero;
                //players
                for (int i = 0; i < GameInfo.S.numPlayers; i++) {
                    yield return new WaitForSeconds(beat);
                    int team = GameInfo.S.teamNums[i];
                    if (boomBox.S) {
                        boomBox.S.yesPress(team);
                    }
                    bool flipped = false;
                    if (team < 2) {
                        pos.y = Screen.height * 0.75f;
                    } else {
                        pos.y = Screen.height * 0.25f;
                    }
                    if (team % 2 == 0) {
                        pos.x = Screen.width * (0.1f + (teamCount[team] * 0.15f));
                        teamCount[team]++;
                    } else {
                        pos.x = Screen.width * (0.9f - (teamCount[team] * 0.15f));
                        teamCount[team]++;
                        flipped = true;
                    }
                    spawnPlayerSprite(pos, i, flipped);
                }
            }

            /*
            float space = 1f / GameInfo.S.numPlayers;
            for (int i = 0; i < GameInfo.S.numPlayers; i++) {
                pos.x = Screen.width * (0.1f + (i * space));
                spawnPlayerSprite(pos, i);
            }
            */
        }
        //yield return new WaitForSeconds(beat);

        //yield return new WaitForSeconds(beat);
        yield return new WaitForEndOfFrame();
    }

    void spawnPlayerSprite(Vector3 pos, int pNum, bool flipped) {
        GameObject tmp = Instantiate(playerSprites, pos, transform.rotation, transform);
        if (flipped) {
            RectTransform rt = tmp.GetComponent<RectTransform>();
            rt.localScale = new Vector3(-rt.localScale.x, rt.localScale.y, rt.localScale.z);
        }
        playerUIAnim anim = tmp.GetComponent<playerUIAnim>();
        SwordSoul s = GameInfo.S.souls[GameInfo.S.songs[pNum]].GetComponent<SwordSoul>();
        int skin = GameInfo.S.colors[pNum];
        anim.setColors(s.mainColor[skin], s.subColor[skin]);
        pos.y -= Screen.height * 0.12f;
        tmp = Instantiate(teamName, pos, transform.rotation, transform);
        Text name = tmp.transform.GetChild(0).GetComponent<Text>();
        name.text = GameInfo.S.names[GameInfo.S.songs[pNum]];
    }
}
