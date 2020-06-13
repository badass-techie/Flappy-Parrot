using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HighScoreManager : MonoBehaviour{
    const string privateCode = "p7JHWblNk0-ssMrz9tz5zgfPFWl7l-BEmMhrYnf8iExQ";
    const string publicCode = "5e7b85fffe232612b8de5251";
    const string webURL = "http://dreamlo.com/lb/";
    public GameObject uploadPanel, errorMessage;
    public Text username;
    public Transform scrollviewContent;

    // Start is called before the first frame update
    void Start(){
        RetrieveHighScores();
        if (PlayerPrefs.GetString("UploadOnOpen") == "yes"){
            PlayerPrefs.DeleteKey("UploadOnOpen");
            EnterHighScore();
        }
    }
    public void EnterHighScore(){
        uploadPanel.SetActive(true);
    }
    public void ClosePanel(){
        uploadPanel.SetActive(false);
    }
    public void AddHighScore() {
        if (username.text != ""){
            StartCoroutine(UploadHighScore(username.text, PlayerPrefs.GetInt("HighScore", 0)));
            Toast.Show("Please wait", 1f, Toast.Position.top, Toast.Theme.light);
            ClosePanel();
        } else {
            Toast.Show("Enter your name first", 2f, Toast.Position.bottom, Toast.Theme.light);
        }
    }
    IEnumerator UploadHighScore(string username, int score){
        UnityWebRequest request = UnityWebRequest.Get(webURL + privateCode + "/add/" + UnityWebRequest.EscapeURL(username) + "/" + score);
        yield return request.SendWebRequest();
        if (request.isNetworkError)
            Toast.Show("Check your internet connection", 2f, Toast.Position.bottom, Toast.Theme.light);
        else if (request.isHttpError)
            Toast.Show("http error", 2f, Toast.Position.bottom, Toast.Theme.light);
        else
            Toast.Show("High score uploaded!", 2f, Toast.Position.bottom, Toast.Theme.light);
    }
    public void RetrieveHighScores(){
        Toast.Show("Fetching high scores...", 1f, Toast.Position.top, Toast.Theme.light);
        StartCoroutine(FetchHighScores());
    }
    IEnumerator FetchHighScores(){
        UnityWebRequest request = UnityWebRequest.Get(webURL + publicCode + "/pipe/");
        yield return request.SendWebRequest();
        if (request.isNetworkError){
            Toast.Show("Failed to retrieve high scores", 3f, Toast.Position.bottom, Toast.Theme.light);
            errorMessage.SetActive(true);
        }
        else if (request.isHttpError){
            Toast.Show("http error", 2f, Toast.Position.bottom, Toast.Theme.light);
            errorMessage.SetActive(true);
        }
        else
            FormatHighScores(request.downloadHandler.text);
    }
    void FormatHighScores(string stream){
        string[] lines = stream.Split(new char[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);
        for(int i = 0; i < lines.Length; ++i){
            string[] tokens = lines[i].Split(new char[] {'|'});
            //add to high score scrollview
            Text usernameText = Resources.Load<Text>("Username"), scoreText = Resources.Load<Text>("Score");
            usernameText.text = tokens[0].Replace('+', ' ');
            scoreText.text = tokens[1];
            Instantiate(usernameText, scrollviewContent);
            Instantiate(scoreText, scrollviewContent);
        }
    }
}
