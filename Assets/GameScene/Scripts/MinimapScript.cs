using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinimapScript : MonoBehaviour {

    [SerializeField]
    private Transform player;
    private Vector3 __originalPosition;
    public bool __fullMap = false;

    private void Start() {
        __originalPosition = this.transform.position;
    }

    private void LateUpdate() {
        if (Input.GetKeyDown(KeyCode.M)) {
            __fullMap = !__fullMap;
        }
        if (!__fullMap) {
            this.GetComponent<Camera>().orthographicSize = 35.0f;
            if(player == null) {
                return;
            }
            Vector3 newPosition = player.position;
            newPosition.y = transform.position.y;
            transform.position = newPosition;
        }
        else {
            transform.position = __originalPosition;
            this.GetComponent<Camera>().orthographicSize = 230.1f;
        }
    }
}