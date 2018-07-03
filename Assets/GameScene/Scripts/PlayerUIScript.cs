using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIScript : MonoBehaviour
{
    [SerializeField] private List<CapturePoint> capturePoints;

    [SerializeField]
    private List<Text> capturePointTexts;

    [SerializeField]
    GameObject character;

    [SerializeField]
    Text bulletReadyText;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	    for (int i = 0; i < capturePoints.Count; ++i)
	    {
	        capturePointTexts[i].text = capturePoints[i].captureValue.ToString();
	    }
        if (character == null)
            return;
        if(character.GetComponent<CharacterScript>().bulletReady)
        {
            bulletReadyText.text = "Bullet Ready!";
            bulletReadyText.color = Color.green;
        }
        else
        {
            bulletReadyText.text = "Reloading...";
            bulletReadyText.color = Color.red;
        }
	}
}
