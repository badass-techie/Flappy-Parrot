using System;
using NumSharp;
using UnityEngine;
using UnityEngine.UI;

public class Species : MonoBehaviour{
    public Transform player;
    public Rigidbody rb;
    public GameObject[] poles;
    public int index;
    const float gap = 0.81f;
    float jumpForce = 200f, jumpCoeff = 10f, fallCoeff = 3.3f;
    float prevHeight, currentHeight;   //used to determine if the parrot is moving up or down
    Quaternion jumpRotation, fallRotation;
    int score = 0;

    // Start is called before the first frame update
    void Start(){
        jumpRotation = Quaternion.Euler(-45, 270, 0);
        fallRotation = Quaternion.Euler(45, 270, 0);
        currentHeight = player.position.y;
        GameObject.Find("Generation").GetComponent<Text>().text = "Generation: " + FindObjectOfType<AI>().generation;
    }

    // Update is called once per frame
    void Update(){
        //updates height for the previous and current frame
        prevHeight = currentHeight;
        currentHeight = player.position.y;
        //orients player's rotation to vertical movement
        if (currentHeight > prevHeight) //rising
            player.rotation = Quaternion.Lerp(player.rotation, jumpRotation, jumpCoeff * Time.deltaTime);
        else if (currentHeight < prevHeight)    //falling
            player.rotation = Quaternion.Lerp(player.rotation, fallRotation, fallCoeff * Time.deltaTime);
    }

    void FixedUpdate(){
        //makes player jump
        double birdHeight = player.position.y, distanceToNextPole = player.position.x - GetNextPole().transform.position.x, lowerGapHeight = GetNextPole().transform.position.y - gap, upperGapHeight = GetNextPole().transform.position.y + gap;
        if (FindObjectOfType<AI>().FeedForward(index, np.array(new double[] { birdHeight, distanceToNextPole, lowerGapHeight, upperGapHeight } ))){
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Force);
        }
    }

    void OnCollisionEnter() {
        //fitness = distance covered by bird - height difference from next gap
        double offset = 7.5;
        double distanceCovered = score * 20.0 + (GetPreviousPole().transform.position.x - player.position.x) - offset;
        double heightDiff = Math.Abs(player.position.y - GetNextPole().transform.position.y);
        FindObjectOfType<AI>().fitnesses[index] = distanceCovered - heightDiff;
        player.gameObject.SetActive(false);
        FindObjectOfType<AI>().speciesAlive--;
        GameObject.Find("BirdsAlive").GetComponent<Text>().text = "Alive: " + FindObjectOfType<AI>().speciesAlive + " / " + FindObjectOfType<AI>().noOfSpecies;
        if (FindObjectOfType<AI>().speciesAlive == 0)
            FindObjectOfType<AI>().Restart();
    }

    void OnTriggerEnter(){
        score++;
        GameObject.Find("Score").GetComponent<Text>().text = "Score: " + score;
        if (score > FindObjectOfType<AI>().highestScore)
            FindObjectOfType<AI>().highestScore = score;
        //trigger.gameObject.SetActive(false);
    }

    GameObject GetNextPole(){
        int nextPoleIndex = 0;
        float minDistance = 4.29497e9f;
        for(int index = 0; index < poles.Length; ++index){
            if (player.position.x > poles[index].transform.position.x && player.position.x - poles[index].transform.position.x < minDistance){
                minDistance = player.position.x - poles[index].transform.position.x;
                nextPoleIndex = index;
            }
        }
        return poles[nextPoleIndex];
    }

    GameObject GetPreviousPole(){
        int prevPoleIndex = 0;
        float minDistance = 4.29497e9f;
        for(int index = 0; index < poles.Length; ++index){
            if (poles[index].transform.position.x > player.position.x && poles[index].transform.position.x - player.position.x < minDistance){
                minDistance = poles[index].transform.position.x - player.position.x;
                prevPoleIndex = index;
            }
        }
        return poles[prevPoleIndex];
    }
}
