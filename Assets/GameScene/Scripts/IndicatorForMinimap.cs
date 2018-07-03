using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndicatorForMinimap : MonoBehaviour {

    [SerializeField]
    private Material enemyMat;
    [SerializeField]
    private Material playerMat;
    [SerializeField]
    private Material noMat;
    [SerializeField]
    private MinimapScript minimap;
    [SerializeField]
    private RawImage TabMap;

    private Vector3 normalScale;

    // Use this for initialization
    void Start() {
        if (transform.parent != null && transform.parent.tag == "NPC") {
            this.GetComponent<MeshRenderer>().material = enemyMat;
        }
        else if (transform.parent != null && transform.parent.tag == "Player") {
            this.GetComponent<MeshRenderer>().material = playerMat;
        }
        else {
            this.GetComponent<MeshRenderer>().material = noMat;
        }
        this.normalScale = this.transform.localScale;
    }

    // Update is called once per frame
    void Update() {
        if (minimap.__fullMap || TabMap.enabled == true) {
            this.transform.localScale = new Vector3(10.0f, 1.0f, 10.0f);
        }
        else {
            this.transform.localScale = this.normalScale;
        }
    }
}
