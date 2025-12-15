using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthIcon : MonoBehaviour {

    public int lives;
    public float health;
    public GameObject damage;
    public Vector3 damSpawnMod;

    public Text lives_t;
    public Text wholeHP;
    public Text fracHP;
    public Image token;
    public Image symbol;

    public Slider heatWheel;
    public Image heatSprite;
    public Slider healthWheel;

    public Image bg;
    Vector3 fracPose;

    void Awake() {
        RectTransform self = gameObject.GetComponent<RectTransform>();
        //bg = gameObject.GetComponent<Image>();
        List<Image> alphaSet = new List<Image>();
        alphaSet.Add(bg);
        alphaSet.Add(token);
        alphaSet.Add(symbol);
        alphaSet.Add(heatSprite);
    }

    public void setPos() {
        fracPose = wholeHP.rectTransform.position;
    }

    public void setHealth(float hp) {
        health = hp;
        healthUI();
    }

    void Update() {
        /*
        if (Input.GetKeyDown("space")) {
            updateHealth(Random.Range(-15, -1));
        }
        if (Input.GetKeyDown("p")) {
            updateHealth(Random.Range(-15f, -1f));
        }
        */
    }
    
    void healthUI() {
        int whole = Mathf.FloorToInt(health);
        wholeHP.text = whole.ToString();
        float f = health - whole;
        if (f > 0.01) {
            string frac = f.ToString("0.00").Split('.')[1].Insert(0, ".");
            fracHP.text = frac;
            wholeHP.rectTransform.position = fracPose;
        } else {
            Vector3 pos = wholeHP.rectTransform.position;
            pos.x = gameObject.GetComponent<RectTransform>().position.x;
            wholeHP.rectTransform.position = pos;
            fracHP.text = "";
        }
        healthWheel.value = health / 100;
    }

    public void updateHealth(float dam) {
        health += dam;
        healthUI();
        /*
        float yv = Random.Range(-1f, 1f);
        if (dam < 0) {
            DamageUI du = Instantiate(damage, transform.position - damSpawnMod, transform.rotation, transform).GetComponent<DamageUI>();
            du.getDamage(dam, new Vector2(-1, yv));
        } else {
            DamageUI du = Instantiate(damage, transform.position + damSpawnMod, transform.rotation, transform).GetComponent<DamageUI>();
            du.getDamage(dam, new Vector2(1, yv));
        }
        */
    }

    public void updateLives(int l) {
        //Debug.Log("lives: " + l);
        lives_t.text = " x" + l.ToString();
    }

    public float tokenAlpha = 0.75f;
    List<Image> alphaSet;

    public void setSoul(Player p) {
        int pNum = p.playerNum;
        PlayerAnimator pa = p.GetComponentInChildren<PlayerAnimator>();
        if (pa) {
            token.sprite = GameInfo.S.pSprites[GameInfo.S.teamNums[pNum]];

            bg.color = pa.mainColor;
            token.color = pa.subColor;
            setAlpha();
            if (p.myAttack && p.myAttack.getSoul().symbol) {
                symbol.sprite = p.myAttack.getSoul().symbol;
                symbol.color = pa.powerColor;
            } else {
                symbol.sprite = null;
                Color c = symbol.color;
                c.a = 0;
                symbol.color = c;
            }
            lives_t.color = pa.detColor;
            fracHP.color = pa.detColor;
            wholeHP.color = pa.detColor;
        } else {
            token.sprite = GameInfo.S.pSprites[GameInfo.S.teamNums[pNum]];
            //GameObject tmp = Instantiate(GameInfo.S.souls[GameInfo.S.songs[pNum]]);
            SwordSoul ss = p.myAttack.getSoul();//tmp.GetComponent<SwordSoul>();
            symbol.sprite = ss.symbol;
            int c = GameInfo.S.colors[pNum];

            bg.color = ss.mainColor[c];
            token.color = ss.subColor[c];
            symbol.color = ss.powerColor[c];
            heatSprite.color = ss.subColor[c];
            norm = heatSprite.color;
            heat = ss.powerColor[c];
            setAlpha();
                        
            noStag = ss.detColor[c];

            lives_t.color = noStag;//ss.detColor[c];
            fracHP.color = noStag;//ss.detColor[c];
            wholeHP.color = noStag;//ss.detColor[c];
            if (ss.stagUIColor.Length > 0) {
                stag = ss.stagUIColor[c];
            }
            //Destroy(tmp);
        }
    }

    public void setAlpha() {
        if (alphaSet == null) {
            alphaSet = new List<Image>();
            alphaSet.Add(bg);
            alphaSet.Add(symbol);
            alphaSet.Add(token);
            alphaSet.Add(heatSprite);
        }
        for (int i = 0; i < alphaSet.Count; i++) {
            Color co = alphaSet[i].color;
            co.a = tokenAlpha;
            alphaSet[i].color = co;
        }
    }

    Color norm;
    Color heat;

    public void setHeat(float col, float bar) {
        heatWheel.value = bar;
        heatSprite.color = Color.Lerp(norm, heat, col);
    }

    Color noStag;
    Color stag;

    public void setStag(float perc) {
        Color c = Color.Lerp(noStag, stag, perc);
        //lives_t.color = c;
        fracHP.color = c;
        wholeHP.color = c;
    }

}
