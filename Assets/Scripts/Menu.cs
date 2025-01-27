using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    private GameObject startMenu;
    private GameObject instructionsMenu;

    void Start() {
        startMenu = GameObject.Find("Start Menu");
        instructionsMenu = GameObject.Find("Instructions Menu");

        instructionsMenu.SetActive(false);
    }

    public void OnPlayButton() {
        SceneManager.LoadScene("Game");
    }

    public void OnInstructionButton() {
        startMenu.SetActive(false);
        instructionsMenu.SetActive(true);
    }

    public void OnCloseButton() {
        startMenu.SetActive(true);
        instructionsMenu.SetActive(false);
    }

    public void OnQuitButton() {
        Application.Quit();
    }
}
