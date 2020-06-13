using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour{
    public Transform player;
    float offsetX;
    // Start is called before the first frame update
    void Start(){
        offsetX = transform.position.x - player.position.x;
    }

    // Update is called once per frame
    void Update(){
        transform.position = new Vector3(player.position.x + offsetX, transform.position.y, transform.position.z);
    }
}
