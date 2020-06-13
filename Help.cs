using System;
using UnityEngine;
using UnityEngine.UI;

public class Help : MonoBehaviour{
    public Animator playerAnimation;
    public Rigidbody rb;
    public Transform player;
    public GameObject[] poles;
    public Text scoreText;
    public GameObject sprite;
    float jumpForce = 200f, jumpCoeff = 10f, fallCoeff = 3.3f;
    float prevHeight, currentHeight;   //used to determine if the parrot is moving up or down
    const float gap = 0.95f;
    int score = 0, noOfLayers = 2;
    bool movePlayer = true;
    Quaternion jumpRotation, fallRotation;
    int poleColliderCount = 0, groundColliderCount = 0;
    float force = 120f, torque = 10f;
    Tuple<double[][,], double[][]> neuralNetwork;

    // Start is called before the first frame update
    void Start(){
        jumpRotation = Quaternion.Euler(-45, 270, 0);
        fallRotation = Quaternion.Euler(45, 270, 0);
        currentHeight = player.position.y;
        neuralNetwork = new Tuple<double[][,], double[][]>(new double[noOfLayers][,], new double[noOfLayers][]);
        //hidden layer weights
        neuralNetwork.Item1[0] = new double[ , ] {
            { -0.708279012566562, 0.263913901179989, 0.621663508760586, -0.987565270619264},
            { -0.987565270619264, 0.0875966228021292, 0.978655102652803, 0.263913901179989},
            { -0.987565270619264, -0.00398305943421229, 0.146641035166868, 0.146641035166868},
            { -0.987565270619264, -0.932503552144628, -0.391551457062155, -0.987565270619264},
            { -0.266332696781649, 0.596281915715096, -0.932503552144628, -0.987565270619264},
            { 0.0875966228021292, 0.978655102652803, 0.282843785026504, 0.458546333694154},
            { 0.0875966228021292, -0.515779717599871, -0.932503552144628, -0.515779717599871},
            { 0.697368591882926, -0.984860388555033, 0.395369795800825, -0.987565270619264},
            { -0.987565270619264, 0.0875966228021292, 0.458546333694154, 0.282843785026504},
            { 0.146641035166868, -0.535652304783302, 0.0875966228021292, -0.391551457062155},
            { -0.515779717599871, -0.266332696781649, -0.00398305943421229, 0.596281915715096},
            { -0.535652304783302, 0.146641035166868, 0.742875131658686, 0.282843785026504}
        };
        //output weights
        neuralNetwork.Item1[1] = new double[ , ]{
            { 0.520851920135716, 0.520851920135716, 0.851716758614274, 0.520851920135716, 0.851716758614274, -0.38694483851406, 0.520851920135716, 0.520851920135716, 0.851716758614274, 0.520851920135716, -0.38694483851406, 0.520851920135716}
        };
        //hidden layer biases
        neuralNetwork.Item2[0] = new double[]{
            -0.686702839884303, 0.444728716018018, -0.686702839884303, -0.686702839884303, -0.686702839884303, -0.686702839884303, -0.686702839884303, -0.686702839884303, -0.686702839884303, -0.686702839884303, -0.686702839884303, -0.686702839884303
        };
        //output bias
        neuralNetwork.Item2[1] = new double[] { 0.536809449799736 };
        //double[] values = new double[] { neuralNetwork.Item1[0][0][0], neuralNetwork.Item1[0][11][3], neuralNetwork.Item1[0][5][1] };
        //Debug.Log(values[0] + " , " + values[1] + " , " +values[2]);
    }

    // Update is called once per frame
    void Update(){
        if (movePlayer){
            //updates height for the previous and current frame
            prevHeight = currentHeight;
            currentHeight = player.position.y;
            //orients player's rotation to vertical movement
            if (currentHeight > prevHeight) //rising
                player.rotation = Quaternion.Lerp(player.rotation, jumpRotation, jumpCoeff * Time.deltaTime);
            else if (currentHeight < prevHeight)    //falling
                player.rotation = Quaternion.Lerp(player.rotation, fallRotation, fallCoeff * Time.deltaTime);
        }
    }

    void FixedUpdate(){
        if (movePlayer){
            //makes player jump
            double birdHeight = player.position.y, distanceToNextPole = player.position.x - GetNextPole().transform.position.x, lowerGapHeight = GetNextPole().transform.position.y - gap, upperGapHeight = GetNextPole().transform.position.y + gap;
            if (Flap(new double[] { birdHeight, distanceToNextPole, lowerGapHeight, upperGapHeight })){    //ai decides whether to jump
                rb.velocity = Vector3.zero;
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Force);
                if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false)
                    FindObjectOfType<AudioManager>().Play("Flap");
                GameObject spawnedImage = Instantiate(sprite);
                spawnedImage.SetActive(true);
                spawnedImage.transform.SetParent(GameObject.Find("Canvas").transform, false);
                Destroy(spawnedImage, 0.5f);
            }
        }
    }

    //returns whether to flap or not
    //inputs include the height of the bird, distance to the next pole, and the height to the top and bottom of the gap
    public bool Flap(double[] inputs){
        double[][] activations = new double[noOfLayers + 1][];
        activations[0] = inputs;
        for (int layer = 0; layer < noOfLayers; ++layer){
            activations[layer + 1] = new double[neuralNetwork.Item2[layer].Length];
            for (int row = 0; row < neuralNetwork.Item1[layer].GetLength(0); ++row){
                double weightedSum = 0; //∑ (w * a)
                for (int column = 0; column < neuralNetwork.Item1[layer].GetLength(1); ++column){
                    weightedSum += neuralNetwork.Item1[layer][row, column] * activations[layer][column];
                }
                double z = weightedSum + neuralNetwork.Item2[layer][row];   //z = wa + b
                if (layer == noOfLayers-1)  //if layer is output layer
                    activations[layer + 1][row] = 1 / (1 + Math.Exp(-z));     //sigmoid
                else
                    activations[layer + 1][row] = Math.Max(0.0, z);     //relu
            }
        }
        return activations[noOfLayers][0] > 0.5;
    }

    void OnCollisionEnter(Collision collisionInfo) {
        FindObjectOfType<MoveWorld>().enabled = false;
        //FindObjectOfType<PlayerMovement>().enabled = false;
        movePlayer = false;
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
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                rb.AddForce(new Vector3(0, 1, 0) * 200f, ForceMode.Force);  //makes parrot bounce off the ground
                if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false)
                    FindObjectOfType<AudioManager>().Play("Crash");
            }
        }
    }

    void OnTriggerEnter(Collider trigger){
        score += 1;
        scoreText.text = "Score: " + score.ToString();
        trigger.gameObject.SetActive(false);
        if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false)
            FindObjectOfType<AudioManager>().Play("Score");
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
}
