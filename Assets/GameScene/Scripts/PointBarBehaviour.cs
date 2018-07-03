using System;
using UnityEngine;
using UnityEngine.UI;

public class PointBarBehaviour : MonoBehaviour {


    [SerializeField]
    private CapturePoint __point;

    private Image fillableImage;
	// Use this for initialization
	void Start () {
        fillableImage = this.GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
        if (__point.captureValue > 0.0f) {
            fillableImage.color = new Color32(45, 85, 229, 255);
        }
        else if (__point.captureValue < 0.0f) {
            fillableImage.color = new Color32(171, 44, 44, 255);
        }
        else {
            fillableImage.fillAmount = 0.0f;
        }
        fillableImage.fillAmount = Math.Abs(__point.captureValue) / 100.0f ;
	}
}