using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debugWindow : MonoBehaviour {
	TextMesh textMesh;

	bool hidden;

	// Use this for initialization
	void Awake ()
	{
		textMesh = gameObject.GetComponentInChildren<TextMesh>();
	}

	void Start(){
		GameObject tmp = GameObject.Find ("playerShip(Clone)");
		if (tmp) {
			transform.parent = tmp.transform;
		}
	}

	void OnEnable()
	{
		Application.logMessageReceived += LogMessage;
	}

	void OnDisable()
	{
		Application.logMessageReceived -= LogMessage;
	}

	public void LogMessage(string message, string stackTrace, LogType type)
	{
		if (textMesh) {
			if (textMesh.text.Length > 300) {
				textMesh.text = message + "\n";
			} else {
				textMesh.text += message + "\n";
			}
		}
	}

	public void switchHidden(){
		Color c = textMesh.color;
		if (hidden) {
			c.a = 1;
			hidden = false;
		} else {
			c.a = 0;
			hidden = true;
		}

		textMesh.color = c;
	}
}
