using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MessageMaker : NetworkBehaviour {

	public GameObject message;
	//GameObject canvas;
	public static MessageMaker S;

	void Awake () {
		S = this;
		//canvas = GameObject.FindGameObjectWithTag ("canvas");
	}

	public void makeNetworked(string words, int duration){
		CmdMake (words, duration);
	}

	[Command]
	public void CmdMake(string words, int duration){
		RpcMake (words, duration);
	}

	[ClientRpc]
	void RpcMake(string words, int duration){
		make (words, duration);
	}

	public void make(string words, int duration){
		GameObject mess = Instantiate (message, transform.position, transform.rotation, transform);
		mess.GetComponent<RectTransform> ().position = new Vector3 (Screen.width / 2, Screen.height / 2, 0);
		mess.GetComponent<Text> ().text = words;
		//mess.transform.GetChild (0).transform.position = new Vector3 (Screen.width / 2, Screen.height / 2, 0);
		mess.GetComponent<TimedDeath> ().lifeTime = duration;
	}
}
