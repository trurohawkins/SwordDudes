using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerUIAnim : MonoBehaviour {
    public Image[] body;
    public Image[] details;

    public void setColors(Color bodyCol, Color detailColor) {
        for (int i = 0; i < body.Length || i < details.Length; i++) {
            if (i < body.Length) {
                body[i].color = bodyCol;
            }
            if (i < details.Length) {
                details[i].color = detailColor;
            }
        }
    }

    public void flip() {
        for (int i = 0; i < body.Length || i < details.Length; i++) {
            if (i < body.Length) {
               
            }
            if (i < details.Length) {

            }
        }
    }
    
}
