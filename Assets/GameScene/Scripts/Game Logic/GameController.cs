using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    // ------------- Class Variables ------------- //

    // ---- Lane Objects ---- //
    [SerializeField]
    private GameObject leftLane;

    [SerializeField]
    private GameObject midLane;

    [SerializeField]
    private GameObject rightLane;

    // Private Variables 
    private const float waveSpawnTimer = 60.0f;
    private const float moralRespawnTimeMultiplier = 3.0f;
    private int moralValue;

    [SerializeField]
    private float redTimer;

    [SerializeField]
    private float blueTimer;

    [SerializeField]
    private float redWaveRespawnTimerReduction;

    [SerializeField]
    private float blueWaveRespawnTimerReduction;

    [SerializeField]
    private GameObject playerObject;

    [SerializeField]
    private GameObject __endGameUI;

    [SerializeField]
    private GameObject __pauseGameUI;

    [SerializeField]
    private bool togglePause = false;

    // Use this for initialization
    void Awake () {
        Time.timeScale = 1;
        moralValue = 0;

        redTimer  = 40.0f;
        blueTimer = 40.0f;
        redWaveRespawnTimerReduction  = 0.0f;
        blueWaveRespawnTimerReduction = 0.0f;

        leftLane.GetComponent<LaneWave>().laneSide = LaneScript.LaneSide.LeftLane;
        midLane.GetComponent<LaneWave>().laneSide = LaneScript.LaneSide.CenterLane;
        rightLane.GetComponent<LaneWave>().laneSide = LaneScript.LaneSide.RightLane;

    }
	
	// Update is called once per frame
	void Update () {


        // TEST KEY PRESS TO PAUSE RESPAWN WAVE
        if (Input.GetKeyDown(KeyCode.P)) {
            togglePause = !togglePause;
        }

        /***
         *  End Game Conditions 
         *   - Game Over when all capture points are controlled by Red Team
         *   - Game Over when the player dies
         **/

        // Player Dies 
        if (IsEndGame()) {
            Time.timeScale = 0;
            __endGameUI.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if(Time.timeScale != 0) {
                Time.timeScale = 0;
                __pauseGameUI.SetActive(true);
            }
            else {
                Time.timeScale = 1;
                __pauseGameUI.SetActive(false);
            }
        }

        // Compute new Moral Value
        moralValue = calculateMoral();

        // Update the Respawn Timer based on Moral Value
        updateRespawnTimers(moralValue);

        // Blue Spawn Timer 
        if (blueTimer >= (waveSpawnTimer - blueWaveRespawnTimerReduction)) {
            spawnWave(TeamSide.TeamEnum.BlueTeam);
            blueTimer = 0.0f;
        }

        // Red Spawn Timer
        if(redTimer >= (waveSpawnTimer - redWaveRespawnTimerReduction)) {
            spawnWave(TeamSide.TeamEnum.RedTeam);
            redTimer = 0.0f;
        }


        if (!togglePause) {
            blueTimer += Time.deltaTime;
            redTimer += Time.deltaTime;
        }





        // TEST KEY PRESS TO SEE MORAL VALUE PER LANE
        if (Input.GetKeyDown(KeyCode.U)) {

            Debug.Log("LEFT  LANE MORAL VALUE: " + leftLane.GetComponent<LaneWave>().laneMoralValue() + "\n");
            Debug.Log("MID   LANE MORAL VALUE: " + midLane.GetComponent<LaneWave>().laneMoralValue() + "\n");
            Debug.Log("RIGHT LANE MORAL VALUE: " + rightLane.GetComponent<LaneWave>().laneMoralValue() + "\n");
            Debug.Log("TOTAL LANE MORAL VALUE: " + calculateMoral() + "\n");

        }
    }


    // Spawn waves on all three lanes for given side
    void spawnWave(TeamSide.TeamEnum side) {

        if (side == TeamSide.TeamEnum.BlueTeam) {
            leftLane.GetComponent<LaneWave>().spawnBlueWave();
            midLane.GetComponent<LaneWave>().spawnBlueWave();
            rightLane.GetComponent<LaneWave>().spawnBlueWave();
        }

        else {
            leftLane.GetComponent<LaneWave>().spawnRedWave();
            midLane.GetComponent<LaneWave>().spawnRedWave();
            rightLane.GetComponent<LaneWave>().spawnRedWave();
        }

    }

    // Calculates the total Moral Value
    int calculateMoral() {
        return (leftLane.GetComponent<LaneWave>().laneMoralValue() + midLane.GetComponent<LaneWave>().laneMoralValue() + rightLane.GetComponent<LaneWave>().laneMoralValue());
    }


    // Updates the Respawn Timer based on Moral Value
    void updateRespawnTimers(int moralValue) {

        // Blue has Advantage
        if (moralValue > 0) {
            blueWaveRespawnTimerReduction = moralRespawnTimeMultiplier * moralValue;
            redWaveRespawnTimerReduction = 0.0f;
        }

        // Red has Advantage
        else if (moralValue < 0) {
            redWaveRespawnTimerReduction = moralRespawnTimeMultiplier * moralValue * (-1);
            blueWaveRespawnTimerReduction = 0.0f;
        }

        // Additional Check to make sure Moral is set properly set when we're even
        else {
            blueWaveRespawnTimerReduction = 0.0f;
            redWaveRespawnTimerReduction = 0.0f;
        }

        // Debug.Log("Moral Value: " + moralValue  + ", Blue Timer: " + blueWaveRespawnTimerReduction  + ", Red Timer: " + redWaveRespawnTimerReduction);
        
    }


    bool IsEndGame() {
        return (playerObject == null || moralValue == -9 || moralValue == 9);
    }
}
