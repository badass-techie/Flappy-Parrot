using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour{
    public Rigidbody rb;
    public Transform player;
    float jumpForce = 200f, jumpCoeff = 10f, fallCoeff = 3.3f;
    float prevHeight, currentHeight;   //used to determine if the parrot is moving up or down
    Quaternion jumpRotation, fallRotation;

    // Start is called before the first frame update
    void Start(){
        jumpRotation = Quaternion.Euler(-45, 270, 0);
        fallRotation = Quaternion.Euler(45, 270, 0);
        currentHeight = player.position.y;
    }
    // Update is called once per frame
    void Update(){
        //updates height for the previous and current frame
        prevHeight = currentHeight;
        currentHeight = player.position.y;
        //makes player jump
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)){    //spacebar for pc, and tap for mobile
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Force);
            if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false)
                FindObjectOfType<AudioManager>().Play("Flap");
        }
        //orients player's rotation to vertical movement
        if (currentHeight > prevHeight) //rising
            player.rotation = Quaternion.Lerp(player.rotation, jumpRotation, jumpCoeff * Time.deltaTime);
        else if (currentHeight < prevHeight)    //falling
            player.rotation = Quaternion.Lerp(player.rotation, fallRotation, fallCoeff * Time.deltaTime);
    }
}