using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageUI : MonoBehaviour {

	public float speed;
	public float chillTime;
	public float lifeTime;
	public Vector2 fontSizes;
	public Vector2 damageRange;
	Vector2 dir;
	public Color hurtColor;
	public Color healColor;
	public Text t;
	public Image i;
	float fullLife;
	public Transform p;
	Vector2 pos;

	void Awake() {
		fullLife = lifeTime;
	}

	public void getDamage(float dam, Vector2 direction) {
		float d = Mathf.Abs(dam);
		int whole = Mathf.FloorToInt(d);
        float f = d - whole;
		if (f > 0.01f) {
			t.text = d.ToString(".0");
		} else {
			t.text = d.ToString("0");
		}
		/*
		if (d - Mathf.FloorToInt(d) > 0.01) {
			t.text = d.ToString(".0");
		} else {
			t.text = d.ToString("0");
		}
		*/
		
		if (dam < 0) {
			t.color = hurtColor;
		} else {
			t.color = healColor;
		}
		float percent = (d - damageRange[0]) / (damageRange[1] - damageRange[0]);
		t.fontSize = (int)Mathf.Lerp(fontSizes[0], fontSizes[1], percent);
		dir = direction.normalized;
	}

	void FixedUpdate () {
		if (chillTime > 0) {
			chillTime--;
		} else if (lifeTime > 0) {
			pos += dir * speed;
			//transform.Translate(dir * speed);
			if (p) {
				transform.position = Camera.main.WorldToScreenPoint(new Vector3(p.position.x + pos.x, p.position.y + pos.y, p.position.z));
			}
			lifeTime--;
			Color cur = t.color;
			float alpha = (lifeTime - 2) / fullLife;
			cur.a = alpha;
			t.color = cur;
			cur = i.color;
			cur.a = alpha;
			i.color = cur;
		} else {
			Destroy(gameObject);
		}
	}
}
