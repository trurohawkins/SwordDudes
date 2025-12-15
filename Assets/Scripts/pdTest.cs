using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pdTest : MonoBehaviour {
	public LibPdInstance pdPatch;
	// Use this for initialization
	void Start () {
		pdPatch.SendBang("p108");
		pdPatch.SendFloat ("p1speed", 100);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
