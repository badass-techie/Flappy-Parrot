/*
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour{
    static AdManager instance = null;
    string adUnitId;
    RewardedAd ad;
    AdRequest request;
    bool adWatched = false, failedToLoad = false;

    void Awake(){
        if (instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (instance != this){
            Destroy(this.gameObject);
            return;
        }
    }
    // Start is called before the first frame update
    void Start(){
        MobileAds.Initialize(initStatus => { });
        #if UNITY_ANDROID
                adUnitId = "ca-app-pub-3940256099942544/5224354917";
        #elif UNITY_IPHONE
                adUnitId = "ca-app-pub-3940256099942544/1712485313";
        #else
                adUnitId = "unexpected_platform";
        #endif
        CreateAdRequest();
    }
    void LoadMainMenu(){
        SceneManager.LoadScene("Welcome");
    }
    void CreateAdRequest(){
        ad = new RewardedAd(adUnitId);
        // Called when an ad request has successfully loaded.
        //ad.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        ad.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        //ad.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        ad.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        ad.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        ad.OnAdClosed += HandleRewardedAdClosed;
        // Create an empty ad request.
        request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        ad.LoadAd(request);
    }
    public bool IsAdLoaded(){
        return ad.IsLoaded();
    }
    public void ReloadAd(){
        failedToLoad = false;
        ad.LoadAd(request);
    }
    public void ShowAd(){
        ad.Show();
    }
    public bool AdFailedToload(){
        return failedToLoad;
    }
    //ads
    //failed to load
    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args){
        failedToLoad = true;
    }
    //failed to play
    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args){
        Toast.Show("Oops! Something went wrong...");
        Invoke("LoadMainMenu", 2f);
    }
    //ad closed
    public void HandleRewardedAdClosed(object sender, EventArgs args){
        CreateAdRequest(); 
        if (adWatched){
            //reward
            FindObjectOfType<CollisionDetection>().Respawn();
            adWatched = false;
        } else {
            Toast.Show("Canceled...", 1f);
            Invoke("LoadMainMenu", 1f);
        }
    }
    //ad watched
    public void HandleUserEarnedReward(object sender, Reward args){
        adWatched = true;
    }
}
*/