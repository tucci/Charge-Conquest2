using UnityEngine;
using UnityEngine.UI;

public class RedScore : MonoBehaviour {

    [SerializeField]
    private TabMenu __tabMenu;

    [SerializeField]
    private Text scoreText;
    // Update is called once per frame
    void Update() {
        scoreText.text = "Red: " + __tabMenu.redCounter.ToString();
        scoreText.color = Color.red;
    }
}
