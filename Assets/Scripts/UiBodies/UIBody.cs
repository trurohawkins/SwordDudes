using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBody : MonoBehaviour {

	public RectTransform visual;
	public int song = -1;
	public int level = 0;
	public bool active;
	Text myText;
	Color baseColor;
	public Color highlight;

	public virtual void Awake() {
		//transform.parent = null;
		if (visual) {
			myText = visual.GetComponentInChildren<Text>();
			if (myText) {
				baseColor = myText.color;
			}
		}
	}

	public virtual void Update () {
		Vector3 screenPos = Camera.main.WorldToScreenPoint (transform.position);
		if (visual) {
			visual.anchoredPosition = new Vector2 (screenPos.x - Screen.width/2, screenPos.y - Screen.height/2);//screenPos.y);
		} else {
			Debug.Log(name + " doesnt have a visual");
		}
	}

	public virtual void Action(GridSelector gs) {}
	
	public virtual void setShowing(bool val) {
		visual.gameObject.SetActive(val);
		active = val;
	}
	
	public bool getShowing() {return true;}

	public virtual void hover(bool val) {
		if (myText) {
			if (val) {
				myText.color = highlight;
			} else {
				myText.color = baseColor;
			}
		}
	}
}
