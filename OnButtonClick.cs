using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OnButtonClick : MonoBehaviour{
    public GameObject endGameUI;
    public Sprite muteIcon, unmuteIcon;
    public Button muteButton;
    public Text buttonText;

    // Start is called before the first frame update
    void Start(){
        if(SceneManager.GetActiveScene().name == "Welcome"){
            if (Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0))){
                muteButton.GetComponent<Image>().sprite = unmuteIcon;
                buttonText.text = "Unmute";
            } else {
                muteButton.GetComponent<Image>().sprite = muteIcon;
                buttonText.text = "Mute";
            }
        }
    }
    public void StartOrRestart(){
        if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false)
            FindObjectOfType<AudioManager>().Play("Tap");
        SceneManager.LoadScene("Play");
    }
    public void MuteOrUnmute(){
        if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false) 
            FindObjectOfType<AudioManager>().Play("Tap");
        if (Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0))){
            muteButton.GetComponent<Image>().sprite = muteIcon;
            buttonText.text = "Mute";
            PlayerPrefs.SetInt("IsMuted", 0);
        } else {
            muteButton.GetComponent<Image>().sprite = unmuteIcon;
            buttonText.text = "Unmute";
            PlayerPrefs.SetInt("IsMuted", 1);
        }
    }
    public void LoadMainMenu(){
        if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false){
            FindObjectOfType<AudioManager>().Play("Tap");
        }
        SceneManager.LoadScene("Welcome");
    }
    public void LoadHelp(){
        if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false){
            FindObjectOfType<AudioManager>().Play("Tap");
        }
        SceneManager.LoadScene("Help");
    }
    public void LoadLeaderboard(bool upload = false){
        if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false)
            FindObjectOfType<AudioManager>().Play("Tap");
        SceneManager.LoadScene("Leaderboard");
        if (upload)
            PlayerPrefs.SetString("UploadOnOpen", "yes");
    }
    public void About(){
        if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false)
            FindObjectOfType<AudioManager>().Play("Tap");
        Application.OpenURL("https://github.com/badass-techie/");
    }
    /*
    public void PlayAd(){
        if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false)
            FindObjectOfType<AudioManager>().Play("Tap");
        if (FindObjectOfType<AdManager>().IsAdLoaded()){
            endGameUI.SetActive(false);
            FindObjectOfType<AdManager>().ShowAd();
        } else {
            if (FindObjectOfType<AdManager>().AdFailedToload()){
                Toast.Show("Check your internet connection", 3f);
                FindObjectOfType<AdManager>().ReloadAd();
            } else{
                Toast.Show("Try again in a few seconds...", 3f);
            }
        }
    }
    */
    public void AddHighScore(){
        if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false)
            FindObjectOfType<AudioManager>().Play("Tap");
        FindObjectOfType<HighScoreManager>().EnterHighScore();
    }
    public void UploadHighScore() {
        if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false)
            FindObjectOfType<AudioManager>().Play("Tap");
        FindObjectOfType<HighScoreManager>().AddHighScore();
    }
    public void CloseHighScore(){
        FindObjectOfType<HighScoreManager>().ClosePanel();
    }
    public void Quit(){
        if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false)
            FindObjectOfType<AudioManager>().Play("Tap");
        Application.Quit();
    }
}
