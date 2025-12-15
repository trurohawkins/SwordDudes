using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Value : MonoBehaviour {

	//change to map
	public string[] types;
	public float[] minValues;
	public float[] maxValues;
	public float[] values;
	public float[] multi;
	int team = -1;

	void Awake() {
		if (minValues.Length == types.Length) {
			multi = new float[minValues.Length];
			for (int i = 0; i < types.Length; i++) {
				values [i] = minValues [i];
				multi [i] = 1;
			}
		}
	}

	public void setValue(string t, float min, float max) {
		for (int i = 0; i < types.Length; i++) {
			if (types [i] == t) {
				if (i < values.Length) {
					values[i] = minValues [i] = min;
					maxValues [i] = max;
				} else {
					Debug.LogError("Values on " + gameObject.name + " are not set up right");
				}
			}
		}
	}

	public float getValue(string t) {
		for (int i = 0; i < types.Length; i++) {
			if (types [i] == t) {
				return values [i] * multi[i];
			}
		}
		return -1;
	}

	public float getMin(string t) {
		for (int i = 0; i < types.Length; i++) {
			if (types [i] == t) {
				return minValues [i];
			}
		}
		return -1;
	}
	public float getMax(string t) {
		for (int i = 0; i < types.Length; i++) {
			if (types [i] == t) {
				return maxValues [i];
			}
		}
		return -1;
	}


	public void lowerValue(string t, float percent) {
		for (int i = 0; i < types.Length; i++) {
			if (types [i] == t) {
				float amount = (maxValues[i] - minValues[i]) * percent;
				if (values [i] - amount < minValues [i]) {
					values [i] = minValues [i];
				} else {
					values [i] -= amount;
				}
			}
		}
	}

	public void raiseValue(string t, float percent) {
		for (int i = 0; i < types.Length; i++) {
			if (types [i] == t) {
				float amount = (maxValues[i] - minValues[i]) * percent;
				if (values [i] + amount > maxValues [i]) {
					values [i] = maxValues [i];
				} else {
					values [i] += amount;
				}
			}
		}
	}

	public void writeValue(string t, float value) {
		for (int i = 0; i < types.Length; i++) {
			if (types [i] == t) {
				if (value >= minValues [i] && value <= maxValues [i]) {
					values [i] = value;
				}
			}
		}
	}

	public void valuePercent(string t, float percent) {
		percent = Mathf.Clamp(percent, 0, 1);
		for (int i = 0; i < types.Length; i++) {
			if (types [i] == t) {
				values[i] = minValues[i] + ((maxValues[i] - minValues[i]) * percent);
			}
		}
	}

	public void setMulti(string t, float m) {
		for (int i = 0; i < types.Length; i++) {
			if (types [i] == t) {
				multi [i] = m;
			}
		}
	}

	public void setTeam(int newTeam) {
		team = newTeam;
	}

	public bool onTeam(int teamCheck) {
		return (team != -1 && team == teamCheck);
	}

	public int getTeam() {
		return team;
	}
}
