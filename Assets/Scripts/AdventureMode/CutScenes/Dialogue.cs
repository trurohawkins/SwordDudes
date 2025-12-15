using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : UIMenu {

    public int waitTime = 10;
    public RectTransform bubble;
    public Text words;
    public RectTransform image;
    public Storybo storybo;
    public Text continueText;
    public Color highlight;
    string colorText;
    List<string> wordQueue;
    string accept;

    public override void Awake() {
        base.Awake();
        image.sizeDelta = new Vector2(Screen.width, Screen.height);
        //bubble = gameObject.GetComponentInChildren<RectTransform>();
        bubble.sizeDelta = new Vector2(Screen.width, Screen.height * 0.4f);
        bubble.anchoredPosition = new Vector3(0, -Screen.height * 0.33f);

        //words = GetComponentInChildren<Text>();
        int padding = 100;
        words.rectTransform.sizeDelta = new Vector2(bubble.sizeDelta.x - padding, bubble.sizeDelta.y - padding);;
        wordQueue = new List<string>();
        colorText = ColorUtility.ToHtmlStringRGB(highlight);
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public override void Start() {
        base.Start();
        keyControl mine = GameInfo.S.getControl (0);
        accept = mine.getDodge ();
        if (GameInfo.S.controls[0] == 2) {
            continueText.text = "- PRESS ENTER -";
        } else {
            continueText.text = "- PRESS A -";
        }
        continueText.gameObject.SetActive(false);
    }

    public void getWord(string word) {
        string cur = "";
        for (int i = 0; i < word.Length; i++) {
            char c = word[i];
            // commented out so we can have rich text tags
            if (c == '-') {
                i++;
                string com = "";
                while (i < word.Length && word[i] != '-') {
                    com += word[i];
                    i++;
                }
                cur += parseCommand(com);
            } else  {
                cur += c;
            }
        }
        wordQueue.Add(cur);
    }

    public void dumpWords() {
        wordQueue.Clear();
    }
    // 0 - off, 1 - pause the game, -1 dont pause the game
    public int curPause;
    void prepGameForWriting() {
        transform.SetAsLastSibling();
        transform.GetChild(0).gameObject.SetActive(true);
        pause();
    }

    public void pause() {
        if (curPause > 0 || (curPause == 0 && curShot && curShot.pause)) {
            Map.S.flowing = false;
            GM.S.onlyPausePlayers(true);
            Time.timeScale = 0;
        }
    }

    public bool pauser() {
        return curPause > 0 || (curPause == 0 && curShot && curShot.pause);
    }

    bool coloring = false;
    public bool gamePaused = false;

    IEnumerator writeOutWords() {
        if (storybo.mute) {
            wordQueue.Clear();
            yield return new WaitForEndOfFrame();
        } else {
            if (writing || wordQueue.Count == 0) {
                yield return new WaitForEndOfFrame();
            } else {
                interrupt = false;
                prepGameForWriting();
                writing = true;
                while (wordQueue.Count > 0) {
                    //Debug.Log("writing");
                    string cur = "";
                    string word = wordQueue[0];
                    wordQueue.RemoveAt(0);
                    //foreach(char c in word) {
                    continueText.gameObject.SetActive(false);

                    for (int i = 0; i < word.Length; i++) {
                     while(gamePaused) {
                        //Debug.Log("game paused");
                        yield return new WaitForSecondsRealtime(0.001f);
                    }                       char c = word[i];
                        if (c == '<') {
                            coloring = true;
                        } else if (c == '>') {
                            coloring = false;
                        } else if (c == '(') {
                            i++;
                            string num = "";
                            while (i < word.Length && word[i] != ')') {
                                num += word[i];
                                i++;
                            }
                            int speaker;
                            if (int.TryParse(num, out speaker)) {
                                changeSpeaker(speaker);
                            }
                        } else {
                            if (!coloring) {
                                cur += c;
                            } else {
                                cur += "<color=#"+colorText+">" + c + "</color>";
                            }
                            words.text = cur;
                            if (!interrupt) {
                                yield return new WaitForSecondsRealtime(0.03f);
                            } else {
                                /*
                                words.text = word;
                                interrupt = false;
                    
                                break;
                                */
                            }
                        }
                        

                    }
                    //continueText.gameObject.SetActive(true);
                    interrupt = false;
                    //yield return new WaitForSeconds(3);
                    if (!interrupt) {
                        int waiting = 0;
                        while (!interrupt && (waiting < waitTime || (curPause == 0 && curShot && curShot.pause) || curPause > 1)) {
                            //Debug.Log("waiting");
                            while(gamePaused) {
                                //Debug.Log("pause2");
                                yield return new WaitForSecondsRealtime(0.001f);
                            }
                            yield return new WaitForSecondsRealtime(0.3f);
                            //if (waiting % 3 == 0) {
                                continueText.gameObject.SetActive(waiting % 3 != 0);
                            //}
                            waiting++;
                        }
                    }
                    interrupt = false;
                }
                transform.GetChild(0).gameObject.SetActive(false);
                GM.S.onlyPausePlayers(false);
                Map.S.startWorld();
                Time.timeScale = 1;
                writing = false;
                if (curShot) {
                    curShot.read = true;
                    //storybo.finishShot(-1);
                }
                curPause = 0;
            }
        }
    }

    public void stopWriting() {
        wordQueue.Clear();
        dumpImage();
        writing = false;
        curPause = 0;
        transform.GetChild(0).gameObject.SetActive(false);
        GM.S.onlyPausePlayers(false);
        Map.S.startWorld();
        StopCoroutine("writeOutWords");
    }

    bool writing = false;
    bool interrupt = false;

    protected override void Update() {
        //if (Time.timeScale != 0) {
            if (accept(-1)) {
                if (writing && !gamePaused) {
                    interrupt = true;
                }
            }
        //}
    }

    Shot curShot;

    public void receiveShot(Shot s) {
        s.beforeShot();
        if (!s.read) {
            //Debug.Log("recevied shot " + s.name);
            bool gotSome = shotArt(s, false);
            curShot = s;
            for (int i = 0; i < s.text.Length; i++) {
                gotSome = true;
                getWord(s.text[i]);
            }
            // maybe another way to check
            // but the idea is a shot without a room is meant to be read write here
            if (gotSome && !s.room) {// && s.readyForShot()) {
                //Debug.Log("got some");
                StartCoroutine(s.readyingShot(this));
                //startWriting(3f);
            }
        }
    }

    public bool shotArt(Shot s, bool after) {
        bool gotSome = false;
        Image img = image.GetComponent<Image>();
        if (img) {
            Sprite cur = after ? s.afterImage : s.image;
            if (cur) {
                img.sprite = cur;
                Color c = img.color;
                c.a = 1;
                img.color = c;
                gotSome = true;
            } else {
                Color c = img.color;
                c.a = 0;
                img.color = c;
            }
        }
        changeSpeaker(s.speaker);
        return gotSome;
    }

    public void changeSpeaker(int speaker) {
        Image bub = bubble.GetComponent<Image>();
        bub.color = colors[speaker];
        words.color = colors[speaker+1];
        continueText.color = colors[speaker+1];
    }

    public void readText(string[] text) {
        for (int i = 0; i < text.Length; i++) {
            getWord(text[i]);
        }
        startWriting();
    }

    public void startWriting() {
        StartCoroutine("writeOutWords");
    }

    
    string parseCommand(string com) {
       switch (com) {
            case "move":
                if (GameInfo.S.controls[0] == 2) {
                    com = "WASD";
                } else {
                    com =  "Left Analog Stick";
                }
                break;
            case "swing":
                if (GameInfo.S.controls[0] == 2) {
                    com = "Scroll Wheel";
                } else {
                    com = "Right Analog Stick";
                }
                break;
            case "dodge":
                if (GameInfo.S.controls[0] == 2) {
                    com = "Space Bar";
                } else {
                    com = "Left Trigger";
                }
                break;
            case "ignite":
                if (GameInfo.S.controls[0] == 2) {
                    com = "Right Mouse Button";
                } else {
                    com = "Right Bumper";
                }
                break;
            case "newline":
                com = "" + '\n';
                break;
            default:
                //Debug.Log("default");
                return com;
        }
        //return "<color=red>" + com + "</color>";
        return "<" + com + ">";
    }

    public void dumpShit() {
        curShot = null;
        dumpImage();
        wordQueue.Clear();
    }

    public void dumpImage() {
        Image img = image.GetComponent<Image>();
        if (img) {
            Color c = img.color;
            c.a = 0;
            img.color = c;
        }
    }
    public bool isWriting() { return writing; }
}
