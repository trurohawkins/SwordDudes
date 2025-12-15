using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;

public class Purpose : MonoBehaviour {

	[HideInInspector]
	public Form self;
	//[HideInInspector]
	//public AI brain;
	public float range;
	//[HideInInspector]
	//public path legs;
	//[HideInInspector]
	public int speedCounter;
	public int speed;
	//[HideInInspector]
	public bool active = true;

	protected virtual void Awake(){
		self = gameObject.GetComponent<Form> ();
		if (!self) {
			self = gameObject.GetComponentInParent<Form> ();
			if (!self) {
				Debug.LogWarning ("no form attached to this object "  + name);
			}
		}
	}

	public virtual IEnumerator callAction(int delay) {yield return new WaitForFixedUpdate();}

	//state: 0 - collision 1 - enter 2 - exit
	public virtual void callAction(Form poo, int state, int x, int y) {}

	public virtual void updateObj() {/*Debug.Log ("updating " + name);*/}

}
