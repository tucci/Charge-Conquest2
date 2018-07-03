using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabMenu : MonoBehaviour {


    [SerializeField]
    private CapturePointMap __allCapturePoints;

    public Text scoreText;
    public int redCounter { get; set; }
    public int blueCounter { get; set; }
    public int nullCounter { get; set; }
	
	// Update is called once per frame
	void Update () {
        RectTransform rt = GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.01f, 0.9f);
        rt.anchorMax = new Vector2(0.5f, 1.0f);
        rt.sizeDelta = Vector2.zero;

        UpdateScoreBoard();
        scoreText.text = "Points Captured:";
        Component[] allText = this.GetComponentsInChildren<Text>();
        if (Input.GetKeyDown(KeyCode.Tab)) {
            scoreText.enabled = true;
            foreach (Text txt in allText) {
                txt.enabled = true;
            }
        }
        if (Input.GetKeyUp(KeyCode.Tab)) {
            scoreText.enabled = false;
            foreach (Text txt in allText) {
                txt.enabled = false;
            }
        }
    }

    void UpdateScoreBoard() {
        redCounter = 0;
        blueCounter = 0;
        nullCounter = 0;
        foreach (KeyValuePair<CapturePoint.CapturePointId, CapturePoint> cp in __allCapturePoints.capturePoints) {
            if (cp.Value.belongsToTeam == TeamSide.TeamEnum.BlueTeam) {
                blueCounter++;
            }
            else if (cp.Value.belongsToTeam == TeamSide.TeamEnum.RedTeam) {
                redCounter++;
            }
            else
                nullCounter++;
        } // loop through both
    }

}
