using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour {

    public AudioMixer audioMixer;

    public void PlayGame() {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public void ResumeGame() {
        Time.timeScale = 1;
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void SetVolume(float volume) {
        audioMixer.SetFloat("MasterVolume",volume);
    }


    public void ReplayGame() {
        SceneManager.LoadScene("StartScreen", LoadSceneMode.Single);
    }
}
