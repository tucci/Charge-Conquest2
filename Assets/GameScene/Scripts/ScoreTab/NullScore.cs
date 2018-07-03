using UnityEngine;
using UnityEngine.UI;

public class NullScore : MonoBehaviour {

    [SerializeField]
    private TabMenu __tabMenu;

    [SerializeField]
    private Text scoreText;
    // Update is called once per frame
    void Update() {
        scoreText.text = "Not Taken: " + __tabMenu.nullCounter.ToString();

    }
}
