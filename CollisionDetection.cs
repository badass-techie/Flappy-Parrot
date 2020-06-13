using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollisionDetection : MonoBehaviour{
    public Animator playerAnimation;
    public Transform player;
    public Transform[] poles;
    public Rigidbody rb;
    public Text scoreText, scoreText2, highScoreText;
    public GameObject endGameUI;
    List<float> orgPolePositions = new List<float>();
    int score = 0, highScore;
    bool gameOver = false, endGameCalled = false, freezeRotationOnGroundImpact = true;
    int updateCount = 0, framesElapsed = 0, poleColliderCount = 0, groundColliderCount = 0;
    Vector3 prevTransform, currentTransform;
    float force = 120f, torque = 10f;
    float minGap = 0.81f;

    // Start is called before the first frame update
    void Start(){
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        currentTransform = player.position;
        foreach (Transform pole in poles) orgPolePositions.Add(pole.position.x);
    }
    void FixedUpdate(){
        updateCount++;
        if (updateCount >= 5){
            updateCount = 0;
            //updates transform for the previous and current frame
            prevTransform = currentTransform;
            currentTransform = player.position;
            if (gameOver && !endGameCalled) {
                framesElapsed++;
                //if the parrot stays still or four seconds have elapsed
                if (currentTransform == prevTransform || framesElapsed * Time.fixedDeltaTime * 5 > 4f){
                    framesElapsed = 0;
                    endGameCalled = true;
                    EndGame();
                }
            }
        }
    }
    void OnCollisionEnter(Collision collisionInfo) {
        FindObjectOfType<MoveWorld>().enabled = false;
        FindObjectOfType<PlayerMovement>().enabled = false;
        playerAnimation.enabled = false;
        rb.constraints = RigidbodyConstraints.None;
        if (collisionInfo.collider.tag == "LowerPole"){
            rb.AddForce(new Vector3(1, -1, 0) * force, ForceMode.Force);
            rb.AddTorque(new Vector3(0, 0, -1) * torque, ForceMode.Force);
            poleColliderCount++;
            if (poleColliderCount == 1){
                if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false){
                    FindObjectOfType<AudioManager>().Play("Hit");
                    //Vibration.Vibrate(100);
                }
            }
        }
        else if (collisionInfo.collider.tag == "UpperPole"){
            rb.AddForce(new Vector3(1, -1, 0) * force, ForceMode.Force);
            rb.AddTorque(new Vector3(0, 0, 1) * torque, ForceMode.Force);
            poleColliderCount++;
            if (poleColliderCount == 1){
                if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false){
                    FindObjectOfType<AudioManager>().Play("Hit");
                    //Vibration.Vibrate(100);
                }
            }
        }
        else {
            groundColliderCount++;
            if (groundColliderCount == 1){
                rb.velocity = Vector3.zero;
                if (freezeRotationOnGroundImpact) rb.constraints = RigidbodyConstraints.FreezeRotation;
                rb.AddForce(new Vector3(0, 1, 0) * 200f, ForceMode.Force);  //makes parrot bounce off the ground
                if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false)
                    FindObjectOfType<AudioManager>().Play("Crash");
            }
        }
        gameOver = true;
    }
    void OnTriggerEnter(Collider trigger){
        score += 1;
        scoreText.text = "Score: " + score.ToString();
        trigger.gameObject.SetActive(false);
        if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false)
            FindObjectOfType<AudioManager>().Play("Score");
        //makes the gap between poles smaller
        foreach (Transform pole in poles){
            Transform lowerGap = pole.Find("LowerPole"), higherGap = pole.Find("UpperPole");
            if (lowerGap.localPosition.y < -minGap)
                lowerGap.localPosition += new Vector3(0f, 0.01f, 0f);
            if (higherGap.localPosition.y > minGap)
                higherGap.localPosition -= new Vector3(0f, 0.01f, 0f);
        }

    }
    void EndGame(){
        if (score > highScore){
            PlayerPrefs.SetInt("HighScore", score);
            Toast.Show("New high score :)", 3f, Toast.Position.top);
        }
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        scoreText2.text = "Score: " + score.ToString();
        highScoreText.text = "Best: " + highScore.ToString();
        endGameUI.SetActive(true);
    }
    public void Respawn(){
        gameOver = false;
        endGameCalled = false;
        poleColliderCount = 0;
        groundColliderCount = 0;
        for (int index = 0; index < poles.Length; ++index){
            poles[index].Find("ScoreCollider").gameObject.SetActive(true);
            poles[index].position = new Vector3(orgPolePositions[index], poles[index].position.y, poles[index].position.z);
        }
        //endGameUI.SetActive(false);
        player.position = new Vector3(43.5f, poles[1].position.y > 3f? poles[1].position.y : 3f, 0f);
        player.rotation = Quaternion.Euler(0f, 270f, 0f);
        playerAnimation.enabled = true;
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        FindObjectOfType<MoveWorld>().enabled = true;
        FindObjectOfType<PlayerMovement>().enabled = true;
    }
}
