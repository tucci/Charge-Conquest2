using UnityEngine;
using UnityEngine.UI;

public class BlueScore : MonoBehaviour {

    [SerializeField]
    private TabMenu __tabMenu;

    [SerializeField]
    private Text scoreText;
	// Update is called once per frame
	void Update () {
        scoreText.text = "Blue: " + __tabMenu.blueCounter.ToString();
        scoreText.color = Color.blue;
    }
}
