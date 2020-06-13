using UnityEngine;

public class MoveWorld : MonoBehaviour{
    public Transform[] landscapes;
    public GameObject[] poles;
    public float forwardSpeed, minHeight, maxHeight;

    // Start is called before the first frame update
    void Start(){
        //Toast.Show("Tap anywhere to jump", 2f, Toast.Position.top);
    }
    // Update is called once per frame
    void Update(){
        foreach (Transform landscape in landscapes){
            landscape.position += Vector3.right * forwardSpeed * Time.deltaTime;
            if (landscape.position.x >= 75.914)    //gone out of field of view
                landscape.position -= Vector3.right * landscapes.Length * 100;
        }
        foreach (GameObject pole in poles){
            pole.transform.position += Vector3.right * forwardSpeed * Time.deltaTime;
            if (pole.transform.position.x >= 52){    //gone out of field of view
                pole.transform.position = new Vector3(pole.transform.position.x - poles.Length * 20, Random.Range(minHeight, maxHeight), 0);
                pole.transform.Find("ScoreCollider").gameObject.SetActive(true);
            }
        }
    }
}
