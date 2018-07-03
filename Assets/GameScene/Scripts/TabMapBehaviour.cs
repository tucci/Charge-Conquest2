using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabMapBehaviour : MonoBehaviour {

    // Use this for initialization
    void Start() {
        this.GetComponent<RawImage>().enabled = false;
    }

    // Update is called once per frame
    void Update() {
        RectTransform rt = GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;

        if (Input.GetKeyDown(KeyCode.Tab)) {
            this.GetComponent<RawImage>().enabled = true;
        }
        if (Input.GetKeyUp(KeyCode.Tab)) {
            this.GetComponent<RawImage>().enabled = false;
        }
    }
}
