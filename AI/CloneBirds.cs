using UnityEngine;

public class CloneBirds : MonoBehaviour{
    // Start is called before the first frame update
    void Start(){
        for (int i = 0; i < FindObjectOfType<AI>().noOfSpecies - 1; ++i){
            GameObject clone = Instantiate(GameObject.Find("Parrot"));
            clone.GetComponent<Species>().index = i + 1;
        }
    }
}
