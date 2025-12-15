using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class mathTest : MonoBehaviour {
	public float a = 0;
	public float b = 0.01f;

	public float stat;
	float swingPower;
	public float inc = 0.01f;
	public float max;
	public float min;

	public float arc;
	public int point;

	void Awake() {
		swingPower = 0;//Mathf.PI / 2;
		point = 0;
		stat = min;
	}

	void Update () {
		if (Input.GetKeyDown("space")) {
						//Debug.Log (a + b);
			//Debug.Log ((int)Mathf.Round(a + b));
			//a += b;
			//Debug.Log(a + " " + Mathf.Sin(a));
			calcArc();
		}
	}

	void calcArc() {
		for (int i = 0; i < stat; i++) {
			if (point < arc) {
				point++;
				float pre = stat;
				int tilDown = (int)(swingPower / inc);
				if (arc - point > tilDown) {
					if (swingPower + inc < 0.5f) {
						swingPower += inc;
					} else {
						swingPower = 0.5f;
					}
				} else {
					if (swingPower - inc > 0) {
						swingPower -= inc;
					} else {
						swingPower = 0;
					}
				}
				stat = Mathf.Clamp(Mathf.SmoothStep(min, max * 2, swingPower), min, max);
				Debug.Log("stat: " + stat + " diff: " + (stat - pre) + ". tilDown: " + tilDown + ", remaing: " + (arc - point));
			}
		}
		Debug.Log("");
	}
}
